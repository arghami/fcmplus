using bridge.model;
using fcm.calcolatore;
using fcm.dao;
using fcm.exception;
using fcm.model;
using FCMExtender.gui;
using Jurassic;
using Jurassic.Library;
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
        public static List<ElabResult> enhance(String filename, bool tutteGiornate, ProgressBar bar, List<ConfigData> configurazioni)
        {
            List<ElabResult> elabResultList = new List<ElabResult>();
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
                        string nomeCompetizione = comp.Split('-')[1].Trim();
                        //filtro solo le configurazioni corrispondenti alla competizione corrente
                        List<ConfigData> confPerCompetizione = new List<ConfigData>();
                        bool nessunModConfigurato = true;
                        foreach (var theConf in configurazioni)
                        {
                            if (theConf.competizione.Equals(nomeCompetizione))
                            {
                                confPerCompetizione.Add(theConf);
                                if (theConf.abilitato)
                                {
                                    nessunModConfigurato = false;
                                }
                            }
                        }
                        if (nessunModConfigurato)
                        {
                            Logger.log("Nessun modificatore configurato per la competizione "+nomeCompetizione+". Nessun ricalcolo effettuato.");
                            continue;
                        }
                        //calcolo intervallo giornate da processare
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

                                    inc.competizione = nomeCompetizione;
                                    inc.giornata = gior;
                                    //carico il tabellino
                                    List<int> idIncontri = new List<int>();
                                    idIncontri.Add(inc.idIncontro);
                                    Logger.log("Caricamento incontro id " + inc.idIncontro);
                                    Dictionary<int, Tabellino> tabellini;
                                    try
                                    {
                                        tabellini = dao.getTabellini(idIncontri);
                                    }
                                    catch (InvalidGiornataException)
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
                                    Tabellino tabTrasferta;
                                    if (!inc.gp)
                                    {
                                        tabTrasferta = tabellini[inc.trasferta];
                                    }
                                    else
                                    {
                                        tabTrasferta = new Tabellino();
                                        tabTrasferta.giocatori = new Giocatore[0];
                                    }
                                    dao.aggiungiDettagliGiocatoriATabellino(inc.casa, tabCasa, regole.usaTabellino);
                                    if (!inc.gp)
                                    {
                                        dao.aggiungiDettagliGiocatoriATabellino(inc.trasferta, tabTrasferta, regole.usaTabellino);
                                    }

                                    //applico le regole
                                    Logger.log("Applicazione regole custom ");
                                    enhanceIncontro(inc, tabCasa, tabTrasferta, confPerCompetizione);

                                    //ricalcolo il match con i modificatori custom applicati
                                    Logger.log("Ricalcolo del match ");
                                    Match match = helper.calcolaMatch(tabCasa, tabTrasferta, true, inc.fattoreCampo, inc.gp);

                                    //scrivo i dati nel db di FCM
                                    Logger.log("Scrittura sul DB FCM ");
                                    dao.setDatiIncontro(inc.idIncontro, inc.casa, inc.trasferta, match, new bool[] { regole.usaSpeciale1, regole.usaSpeciale2, regole.usaSpeciale3 });
                                    Logger.log("Fine elaborazione incontro id " + inc.idIncontro);


                                    //preparo l'output per l'interfaccia grafica
                                    ElabResult elResult = new ElabResult();
                                    elResult.competizione = nomeCompetizione;
                                    elResult.giornata = gior.ToString();
                                    if (!inc.gp)
                                    {
                                        elResult.incontro = inc.nomeCasa + "-" + inc.nomeFuori;
                                        elResult.vecchioRisultato = inc.golcasa + "-" + inc.golfuori +
                                            " (" + inc.totcasa + "-" + inc.totfuori + ")";
                                        elResult.nuovoRisultato = match.squadra1.numeroGol + "-" + match.squadra2.numeroGol +
                                            " (" + match.squadra1.getTotale() + "-" + match.squadra2.getTotale() + ")";
                                    }
                                    else
                                    {
                                        elResult.incontro = inc.nomeCasa;
                                        elResult.vecchioRisultato = inc.totcasa+"";
                                        elResult.nuovoRisultato = match.squadra1.getTotale() + "";
                                    }
                                    elabResultList.Add(elResult);
                                }
                            }
                        }

                        //a fine esecuzione cancello i file scaricati
                        dwn.cleanDataDir();
                        Console.Out.Flush();
                    }
                }
            }
            return elabResultList;
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

        public static void enhanceIncontro(Incontro inc, Tabellino tabCasa, Tabellino tabFuori, List<ConfigData> configurazioni)
        {
            //inizializzo lo script engine
            var jsEngine = init();
            jsEngine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(jsEngine));

            //produco il DTO incontro da inviare al javascript
            IncontroWrapper incontro = new IncontroWrapper(jsEngine);
            incontro.setCasa(new TabellinoWrapper(jsEngine, tabCasa));
            incontro.setTrasferta(new TabellinoWrapper(jsEngine, tabFuori));
            incontro.setGiornata(inc.giornata);
            incontro.setCompetizione(inc.competizione);
            incontro.setNomeCasa(inc.nomeCasa);
            incontro.setNomeTrasferta(inc.nomeFuori);


            //carico ed eseguo prima i js della cartella data, che sono quelli scaricati a runtime
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] filePaths = Directory.GetFiles(basePath+@"\data", "*.js", SearchOption.TopDirectoryOnly);
            foreach (var fName in filePaths)
            {
                jsEngine.ExecuteFile(fName);
            }

            jsEngine.SetGlobalValue("incontro", incontro);

            //azzero nel tabellino solo i modPers ai quali è associato almeno una config
            foreach (var config in configurazioni)
            {
                if (config.abilitato)
                {
                    switch (config.destinazione)
                    {
                        case 0:
                            tabCasa.modPers1 = 0;
                            tabFuori.modPers1 = 0;
                            break;
                        case 1:
                            tabCasa.modPers2 = 0;
                            tabFuori.modPers2 = 0;
                            break;
                        case 2:
                            tabCasa.modPers3 = 0;
                            tabFuori.modPers3 = 0;
                            break;
                    }
                }
            }

            //scorro la configurazione e applico le funzioni, "appendendo" il risultato nel rispettivo modPers
            foreach (var config in configurazioni)
            {
                if (config.abilitato)
                {
                    string cust = "_customs['" + config.nome + "']";
                    jsEngine.Execute(cust+".func.apply(null, [incontro, null])");
                    //leggo i dati dei modificatori scritti dal javascript e li inserisco nei tabellini
                    double modCasa = safeDouble(incontro.getCasa().get("Mod"));
                    double modFuori = safeDouble(incontro.getTrasferta().get("Mod"));
                    switch (config.destinazione)
                    {
                        case 0:
                            tabCasa.modPers1 += modCasa;
                            tabFuori.modPers1 += modFuori;
                            break;
                        case 1:
                            tabCasa.modPers2 += modCasa;
                            tabFuori.modPers2 += modFuori;
                            break;
                        case 2:
                            tabCasa.modPers3 += modCasa;
                            tabFuori.modPers3 += modFuori;
                            break;
                    }
                    //resetto il parametro di out per la successiva esecuzione
                    incontro.getCasa().set("Mod",0);
                    incontro.getTrasferta().set("Mod", 0);
                }
            }

            Logger.log("Modificatori calcolati: casa.modPers1=" + tabCasa.modPers1);
            Logger.log("Modificatori calcolati: casa.modPers2=" + tabCasa.modPers2);
            Logger.log("Modificatori calcolati: casa.modPers3=" + tabCasa.modPers3);
            Logger.log("Modificatori calcolati: fuori.modPers1=" + tabFuori.modPers1);
            Logger.log("Modificatori calcolati: fuori.modPers2=" + tabFuori.modPers2);
            Logger.log("Modificatori calcolati: fuori.modPers3=" + tabFuori.modPers3);
        }

        /// <summary>
        /// Inizializza il jsEngine con le funzioni core
        /// </summary>
        /// <returns></returns>
        public static ScriptEngine init()
        {
            ScriptEngine jsEngine = new Jurassic.ScriptEngine();
            ObjectInstance customs = jsEngine.Object.Construct();
            jsEngine.SetGlobalValue("_customs", customs);
            jsEngine.Execute("function register (name, params, func) {_customs[name]={func:func, params:params}} ");
            
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] filePaths = Directory.GetFiles(basePath + @"\script", "*.js", SearchOption.TopDirectoryOnly);
            foreach (var fName in filePaths)
            {
                jsEngine.ExecuteFile(fName);
            }
            return jsEngine;
        }

        public static List<string> listModificatoriRegistrati(ScriptEngine jsEngine)
        {
            List<string> props = new List<string>();
            ObjectInstance customs = jsEngine.GetGlobalValue<ObjectInstance>("_customs");
            foreach (var prop in customs.Properties)
            {
                props.Add(prop.Name);
            }
            return props;
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
