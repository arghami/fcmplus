using bridge.model;
using fcm.calcolatore;
using fcm.dao;
using fcm.exception;
using fcm.model;
using net.downloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using utils.logging;

namespace plus.enhancer
{
    public class Enhancer
    {
        public static void enhance(String filename, bool tutteGiornate, ProgressBar bar)
        {
            //devio la console su file
            using (FileStream fs = new FileStream("log.txt", FileMode.Append))
            {
                StreamWriter sw = new StreamWriter(fs);
                Console.SetOut(sw);

                using (FcmDao dao = new FcmDao(filename))
                {
                    string[] competizioni = dao.getListaCompetizioni();
                    foreach (var comp in competizioni)
                    {
                        string idCompetizione = comp.Split('-')[0].Trim();
                        int firstGio = 1;
                        int lastGio = getUltimaOrMassima(dao, idCompetizione);
                        if (!tutteGiornate)
                        {
                            firstGio = lastGio;
                        }
                        int numGio = lastGio - firstGio + 1;
                        Logger.log("Intervallo di giornate selezionato: da " + firstGio + " a " + lastGio);
                        //scarico i file dinamici richiesti
                        Logger.log("Inizio download script remote");
                        Downloader dwn = new Downloader();
                        dwn.cleanDataDir();
                        dwn.getUrls();
                        for (int gior = firstGio; gior <= lastGio; gior++)
                        {
                            dwn.download(gior);
                        }

                        //carico le regole della competizione
                        Logger.log("Caricamento regole e fasce");
                        Regole regole = dao.getRegoleCompetizione(idCompetizione);
                        List<Fascia> fasceModDifesa = dao.getFasceModificatoreDifesa(idCompetizione);
                        List<Fascia> fasceNumDifensori = dao.getContributoNumeroDifensoriModificatoreDifesa(idCompetizione);
                        List<Fascia> fasceModCentrocampo = dao.getFasceModificatoreCentrocampo(idCompetizione);
                        List<Fascia> fasceGol = dao.getFasceConversioneGol(idCompetizione);

                        //estraggo la lista dei gironi della competizione
                        Logger.log("Caricamento gironi");
                        string[] listaGir = dao.getListaGironi(idCompetizione);

                        //reimposto la barra di avanzamento
                        bar.Maximum = numGio * listaGir.Length;
                        bar.Value = 0;

                        //per ogni girone...
                        foreach (var itemGir in listaGir)
                        {
                            string idGirone = itemGir.Split('-')[0].Trim();
                            //per ogni giornata di A...
                            for (int gior = firstGio; gior <= lastGio; gior++)
                            {
                                bar.Increment(1);
                                List<Incontro> incontri = dao.getIncontri(idGirone, gior);

                                //per ogni incontro...
                                foreach (var inc in incontri)
                                {
                                    //carico il tabellino
                                    List<int> idIncontri = new List<int>();
                                    idIncontri.Add(inc.idIncontro);
                                    Logger.log("Caricamento incontro id " + inc.idIncontro);
                                    Dictionary<int, Tabellino> tabellini;
                                    try
                                    {
                                        tabellini = dao.getTabellini(idIncontri);
                                    }
                                    catch (InvalidGiornataException e)
                                    {
                                        continue;
                                    }

                                    //inizializzo il calcoliHelper
                                    CalcoliHelper helper = new CalcoliHelper(
                                        regole,
                                        fasceModDifesa,
                                        fasceNumDifensori,
                                        fasceModCentrocampo,
                                        fasceGol);

                                    //arricchisco i tabellini con i dati dell'archivio voti
                                    Logger.log("Acquisizione dati archivio per l'incontro ");
                                    Tabellino tabCasa = tabellini[inc.casa];
                                    Tabellino tabTrasferta = tabellini[inc.trasferta];
                                    dao.aggiungiDettagliGiocatoriATabellino(tabCasa, regole.usaTabellino);
                                    dao.aggiungiDettagliGiocatoriATabellino(tabTrasferta, regole.usaTabellino);

                                    //applico le regole
                                    Logger.log("Applicazione regole custom ");
                                    enhanceIncontro(inc, tabCasa, tabTrasferta);

                                    //ricalcolo il match con i modificatori custom applicati
                                    Logger.log("Ricalcolo del match ");
                                    Match match = helper.calcolaMatch(tabCasa, tabTrasferta, true, true);

                                    //scrivo i dati nel db di FCM
                                    Logger.log("Scrittura sul DB FCM ");
                                    dao.setDatiIncontro(inc.idIncontro, inc.casa, inc.trasferta, match, new bool[] { regole.usaSpeciale1, regole.usaSpeciale2, regole.usaSpeciale3 });
                                    Logger.log("Fine elaborazione incontro id " + inc.idIncontro);
                                }
                            }
                        }

                        //a fine esecuzione cancello i file scaricati
                        dwn.cleanDataDir();
                        Console.Out.Flush();
                    }
                }
            }
        }

        private static int getUltimaOrMassima(FcmDao dao, string idCompetizione)
        {
            int res = dao.getUltimaGiornata(idCompetizione);
            if (res == -1)
            {
                res = dao.getMassimaGiornata(idCompetizione);
            }
            return res;
        }

        public static void enhanceIncontro(Incontro inc, Tabellino tabCasa, Tabellino tabFuori)
        {
            //inizializzo lo script engine
            var jsEngine = new Jurassic.ScriptEngine();
            jsEngine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(jsEngine));

            //produco il DTO incontro da inviare al javascript
            IncontroWrapper incontro = new IncontroWrapper(jsEngine);
            incontro.setCasa(new TabellinoWrapper(jsEngine, tabCasa));
            incontro.setTrasferta(new TabellinoWrapper(jsEngine, tabFuori));

            jsEngine.SetGlobalValue("incontro", incontro);

            //carico ed eseguo prima i js della cartella data, che sono quelli scaricati a runtime
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] filePaths = Directory.GetFiles(basePath+@"\data", "*.js", SearchOption.TopDirectoryOnly);
            foreach (var fName in filePaths)
            {
                jsEngine.ExecuteFile(fName);
            }
            //poi carico ed eseguo quelli delle regole
            filePaths = Directory.GetFiles(basePath + @"\script", "*.js", SearchOption.TopDirectoryOnly);
            foreach (var fName in filePaths)
            {
                jsEngine.ExecuteFile(fName);
            }

            //leggo i dati dei modificatori scritti dal javascript e li inserisco nei tabellini
            tabCasa.modPers1 = safeDouble(incontro.getCasa().get(TabellinoWrapper.ModM1Pers));
            tabCasa.modPers2 = safeDouble(incontro.getCasa().get(TabellinoWrapper.ModM2Pers));
            tabCasa.modPers3 = safeDouble(incontro.getCasa().get(TabellinoWrapper.ModM3Pers));
            tabFuori.modPers1 = safeDouble(incontro.getTrasferta().get(TabellinoWrapper.ModM1Pers));
            tabFuori.modPers2 = safeDouble(incontro.getTrasferta().get(TabellinoWrapper.ModM2Pers));
            tabFuori.modPers3 = safeDouble(incontro.getTrasferta().get(TabellinoWrapper.ModM3Pers));
            Logger.log("Modificatori calcolati: casa.modPers1=" + tabCasa.modPers1);
            Logger.log("Modificatori calcolati: casa.modPers2=" + tabCasa.modPers2);
            Logger.log("Modificatori calcolati: casa.modPers3=" + tabCasa.modPers3);
            Logger.log("Modificatori calcolati: fuori.modPers1=" + tabFuori.modPers1);
            Logger.log("Modificatori calcolati: fuori.modPers2=" + tabFuori.modPers2);
            Logger.log("Modificatori calcolati: fuori.modPers3=" + tabFuori.modPers3);
        }

        private static double safeDouble(object v)
        {
            if (v is int)
            {
                return (int)v;
            }
            if (v is double)
            {
                return (double)v;
            }
            return 0;
        }
    }
}
