
using fcm.exception;
using fcm.model;
using math.utils;
using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace fcm.dao
{
    public class FcmDao : IDisposable
    {

        private OdbcConnection conn;

        public FcmDao(string filename)
        {
            conn = new OdbcConnection();
            conn.ConnectionString = @"Driver={Microsoft Access Driver (*.mdb)};Dbq=" + filename + ";";
            conn.Open();
        }

        public void Dispose()
        {
            conn.Dispose();
        }

        public string[] getListaCompetizioni()
        {
            //logger.info("Estrazione dell'elenco delle competizioni");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, nome FROM competizione";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    List<string> comps = new List<string>();
                    while (rea.Read())
                    {
                        comps.Add(rea.GetString(0) + " - " + rea.GetString(1));
                    }
                    return comps.ToArray();
                }
            }
        }

        public string[] getListaGironi(string competizione)
        {
            //logger.info("Estrazione dell'elenco dei gironi");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, nome FROM girone WHERE idcompetizione = " + competizione;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    List<string> girs = new List<string>();
                    while (rea.Read())
                    {
                        string girName = "Senza Nome";
                        try
                        {
                            girName = rea.GetString(1);
                        }
                        catch (Exception e) { }
                        girs.Add(rea.GetInt32(0) + " - " + girName);
                    }
                    return girs.ToArray();
                }
            }
        }

        public int getUltimaGiornata(string competizione)
        {
            //logger.info("Estrazione dell'ultima giornata");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select max(giornatadia) from incontro where giocato = 1 and idgirone in (select id from girone where idcompetizione = " + competizione+")";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    if (rea.Read())
                    {
                        try
                        {
                            return rea.GetInt32(0);
                        }
                        catch (Exception e) { }
                    }
                    return -1;
                }
            }

        }

        public int getMassimaGiornata(string competizione)
        {
            //logger.info("Estrazione della prossima giornata");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select max(giornatadia) from incontro where idgirone in (select id from girone where idcompetizione = " + competizione + ")";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    if (rea.Read())
                    {
                        try
                        {
                            return rea.GetInt32(0);
                        }
                        catch (Exception e) { }
                    }
                    return -1;
                }
            }

        }

        public Dictionary<int, string> getSquadreGirone(string idGirone)
        {
            //logger.info("Estrazione squadre");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT i.idsquadra, f.nome FROM iscritta i, fantasquadra f WHERE i.idsquadra=f.id AND i.idgirone = " + idGirone;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    Dictionary<int, string> nomiteam = new Dictionary<int, string>();
                    while (rea.Read())
                    {
                        //estraggo l'elenco dei gironi
                        nomiteam.Add(rea.GetInt32(0), rea.GetString(1));
                    }
                    return nomiteam;

                }
            }
        }

        public Regole getRegoleCompetizione(string idCompetizione)
        {
            //logger.info("Estrazione delle regole della competizione");
            return new Regole(conn, idCompetizione);
        }

        public List<int> getGiornate(string idGirone)
        {
            //logger.info("Estrazione lista giornate");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT idGiornata FROM incontro WHERE idGirone = " + idGirone + " GROUP BY idGiornata ";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    List<int> giornate = new List<int>();
                    while (rea.Read())
                    {
                        giornate.Add(rea.GetInt32(0));
                    }
                    return giornate;
                }
            }
        }

        public int[] getSquadreIscritte(string idGirone)
        {
            //logger.info("Estrazione squadre iscritte");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT idSquadra FROM iscritta where idgirone=" + idGirone;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    List<int> squadre = new List<int>();
                    while (rea.Read())
                    {
                        squadre.Add(rea.GetInt32(0));
                    }
                    return squadre.ToArray();
                }
            }
        }

        public List<Incontro> getIncontri(string idGirone, int giornataDiA)
        {
            //logger.info("Estrazione incontri");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT i.id, i.idcasa, i.idfuori, i.idtipo, i.parzcasa, i.parzfuori, " +
                    " i.totcasa, i.totfuori, i.golcasa, i.golfuori, fqc.nome, fqf.nome " +
                    " FROM incontro i, fantasquadra fqc, fantasquadra fqf " +
                    " WHERE i.idgirone = " + idGirone +
                    " AND i.idcasa = fqc.id " +
                    " AND i.idfuori = fqf.id " +
                    " AND i.giornataDiA = " + giornataDiA;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {

                    List<Incontro> listaIncontri = new List<Incontro>();
                    while (rea.Read())
                    {
                        Incontro inc = new Incontro();
                        if (rea.GetInt32(3) == 1)
                        {
                            inc.fattoreCampo = true;
                        }
                        inc.idIncontro = rea.GetInt32(0);
                        inc.casa = rea.GetInt32(1);
                        inc.trasferta = rea.GetInt32(2);
                        inc.parzcasa = rea.GetDouble(4);
                        inc.parzfuori = rea.GetDouble(5);
                        inc.totcasa = rea.GetDouble(6);
                        inc.totfuori = rea.GetDouble(7);
                        inc.golcasa = rea.GetInt32(8);
                        inc.golfuori = rea.GetInt32(9);
                        inc.nomeCasa = rea.GetString(10);
                        inc.nomeFuori = rea.GetString(11);
                        listaIncontri.Add(inc);
                    }
                    return listaIncontri;
                }
            }
        }

        public Dictionary<int, Tabellino> getTabellini(List<int> idIncontri)
        {
            //logger.info("Estrazione tabellini");
            string filtro = generaFilter(idIncontri);
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT t.idsquadra, t.tot, t.ruolo, t.modportiere, t.modattacco, t.moddifesa, " +
                    " t.voto, t.lista, t.modm1pers, t.modm2pers, t.modm3pers, i.giornataDiA " +
                    " FROM tabellino t, incontro i  " +
                    " WHERE t.idIncontro=i.id " +
                    " AND t.idincontro IN " + filtro + " ORDER BY t.idsquadra";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    Dictionary<int, Tabellino> mapTabellini = new Dictionary<int, Tabellino>();
                    Boolean saltaGiornata = true;
                    while (rea.Read())
                    {
                        Tabellino tab = new Tabellino();
                        string votistring = rea.GetString(1);
                        string[] votiArray = null;
                        string[] ruoliArray = null;
                        string[] votiPuriArray = null;
                        string[] listaArray = null;
                        if (votistring != null)
                        {
                            votiArray = votistring.Split('%');
                            saltaGiornata = false;
                        }
                        string ruolistring = rea.GetString(2);
                        if (ruolistring != null)
                        {
                            ruoliArray = ruolistring.Split('%');
                            saltaGiornata = false;
                        }
                        tab.modPortiere = rea.GetDouble(3);
                        tab.modAttacco = rea.GetDouble(4);
                        tab.modDifesa = rea.GetDouble(5);

                        string votipuristring = rea.GetString(6);
                        if (votipuristring != null)
                        {
                            votiPuriArray = votipuristring.Split('%');
                            saltaGiornata = false;
                        }

                        string listastring = rea.GetString(7);
                        if (listastring != null)
                        {
                            listaArray = listastring.Split('%');
                            saltaGiornata = false;
                        }
                        
                        tab.giocatori = creaGiocatori(votiArray, ruoliArray, votiPuriArray, listaArray);

                        tab.modPers1 = rea.GetDouble(8);
                        tab.modPers2 = rea.GetDouble(9);
                        tab.modPers3 = rea.GetDouble(10);
                        tab.giornata = rea.GetInt32(11);
                        mapTabellini.Add(rea.GetInt32(0), tab);
                    }
                    if (saltaGiornata)
                    {
                        throw new InvalidGiornataException();
                    }
                    return mapTabellini;
                }
            }
        }



        public void aggiungiDettagliGiocatoriATabellino(int idSquadra, Tabellino tabellino, int regoleUsaTabellino)
        {
            //logger.info("Estrazione tabellini");
            List<int> idGiocatori = new List<int>();
            foreach (var gio in tabellino.giocatori)
            {
                idGiocatori.Add(NumParser.parseInt(gio.idGiocatore));
            }

            Dictionary<int, FCMData> fcmData = getGiocaIn(idSquadra, idGiocatori, tabellino.giornata);
            foreach (var gio in tabellino.giocatori)
            {
                if (NumParser.parseInt(gio.idGiocatore) > 0)
                {
                    FCMData data = fcmData[NumParser.parseInt(gio.idGiocatore)];
                    settaMaggioranzaTabellino(data, regoleUsaTabellino);
                    gio.fcmData = data;
                }
            }
        }

        private void settaMaggioranzaTabellino(FCMData data, int regoleUsaTabellino)
        {
            switch (regoleUsaTabellino)
            {
                case 1:
                    data.autogol = data.autogol1;
                    data.golfatti = data.golfatti1;
                    data.golfattisurigore = data.golfattisurigore1;
                    break;
                case 2:
                    data.autogol = data.autogol2;
                    data.golfatti = data.golfatti2;
                    data.golfattisurigore = data.golfattisurigore2;
                    break;
                case 3:
                    data.autogol = data.autogol3;
                    data.golfatti = data.golfatti3;
                    data.golfattisurigore = data.golfattisurigore3;
                    break;
                case 4:
                    data.autogol = maggioranza (data.autogol1, data.autogol2, data.autogol3);
                    data.golfatti = maggioranza(data.golfatti1, data.golfatti2, data.golfatti3);
                    data.golfattisurigore = maggioranza(data.golfattisurigore1, data.golfattisurigore2, data.golfattisurigore3);
                    break;
            }
        }

        private int maggioranza(int a, int b, int c)
        {
            if (a == b && a == c) return a; //tutti uguali, restituisco uno qualunque
            if (a == b) return a; //c è in minoranza, restituisco a (o b)
            if (a == c) return a; //b è in minoranza, restituisco a (o c)
            if (b == c) return b; //a è in minoranza, restituisco b (o c)
            return a; //sono tutti diversi, restituisco a (SPERO VIVAMENTE NON CAPITI MAI)
        }

        private Giocatore[] creaGiocatori(string[] votiArray, string[] ruoliArray, string[] votiPuriArray, string[] listaArray)
        {
            if (votiArray!=null && ruoliArray!=null && votiPuriArray!=null && listaArray != null)
            {
                Giocatore[] gio = new Giocatore[votiArray.Length];
                for (int i=0; i< votiArray.Length; i++)
                {
                    gio[i] = new Giocatore();
                    gio[i].idGiocatore = listaArray[i];
                    gio[i].voto = votiArray[i];
                    gio[i].ruolo = ruoliArray[i];
                    gio[i].votoPuro = votiPuriArray[i];
                }
                return gio;
            }
            return null;
        }

        public Dictionary<int, FCMData> getGiocaIn(int idSquadra, List<int> idGiocatori, int giornata)
        {
            //logger.info("Estrazione squadre iscritte");
            string filtro = generaFilter(idGiocatori);
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                Dictionary<int, FCMData> datiGiocatori = new Dictionary<int, FCMData>();
                cmd.CommandText = "SELECT g.idGiocatore, g.giornata, p.golfatti1, p.golfatti2, p.golfatti3, " +
                    " p.golfattisurigore1, p.golfattisurigore2, p.golfattisurigore3, " +
                    " p.golvittoria, p.golpareggio, " + //questi sono booleani
                    " p.voto1, p.voto2, p.voto3, p.valoreSpeciale, " +
                    " p.rigsba, p.rigpar, p.autogol1, p.autogol2, p.autogol3, " +
                    " p.golsubiti, p.golsubitisurigore, p.assist, " +
                    " p.amm, p.esp, " + //questi sono booleani
                    " ar.codicegazza, ar.dataDiNascita, t.annoDiContratto " +
                    " FROM giocaIn g, punteggio p, GIOCATOREA ar, tesserato t " +
                    " where g.idPunteggio=p.id " +
                    " AND g.idGiocatore=ar.id " +
                    " AND g.idGiocatore=t.idgiocatore " +
                    " AND g.giornata = " + giornata +
                    " AND t.idsquadra = " + idSquadra +
                    " AND g.idGiocatore in " + filtro +
                    " ORDER BY t.id desc"; //così ho prima i movimenti di mercato più recenti, a parità di giocatore
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                    {
                        //se ho un duplicato, significa che nella tabellino ho trovato più di un record. Inserisco solo
                        //il primo che trovo, che è il più recente grazie all'order by impostato
                        if (datiGiocatori.ContainsKey(rea.GetInt32(0)))
                        {
                            continue;
                        }
                        FCMData data = new FCMData();
                        data.idGiocatore = rea.GetInt32(0);
                        data.giornata = rea.GetInt32(1);
                        data.golfatti1 = rea.GetInt32(2);
                        data.golfatti2 = rea.GetInt32(3);
                        data.golfatti3 = rea.GetInt32(4);
                        data.golfattisurigore1 = rea.GetInt32(5);
                        data.golfattisurigore2 = rea.GetInt32(6);
                        data.golfattisurigore3 = rea.GetInt32(7);
                        data.golvittoria = rea.GetBoolean(8);
                        data.golpareggio = rea.GetBoolean(9);
                        data.voto1 = rea.GetDouble(10);
                        data.voto2 = rea.GetDouble(11);
                        data.voto3 = rea.GetDouble(12);
                        data.valoreSpeciale = rea.GetDouble(13);
                        data.rigsba = rea.GetInt32(14);
                        data.rigpar = rea.GetInt32(15);
                        data.autogol1 = rea.GetInt32(16);
                        data.autogol2 = rea.GetInt32(17);
                        data.autogol3 = rea.GetInt32(18);
                        data.golsubiti = rea.GetInt32(19);
                        data.golsubitisurigore = rea.GetInt32(20);
                        data.assist = rea.GetInt32(21);
                        data.amm = rea.GetBoolean(22);
                        data.esp = rea.GetBoolean(23);
                        data.codiceFCM = rea.GetInt32(24);
                        data.dataDiNascita = rea.GetDate(25);
                        data.primavera = rea.GetInt32(26)>100;
                        datiGiocatori.Add(data.idGiocatore, data);
                    }
                    return datiGiocatori;
                }
            }
        }

        public List<Fascia> getFasceConversioneGol(string idCompetizione)
        {
            //logger.info("Estrazione fasce gol");
            return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellagol g, fascia f WHERE g.idCompetizione = " + idCompetizione + " AND g.idFascia=f.id");
        }

        public List<Fascia> getFasceModificatoreDifesa(string idCompetizione)
        {
            //logger.info("Estrazione fasce modificatore difesa");
            return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabelladifesa d, fascia f WHERE d.idcompetizione = " + idCompetizione + " AND d.idfascia=f.id");
        }

        public List<Fascia> getContributoNumeroDifensoriModificatoreDifesa(string idCompetizione)
        {
            //logger.info("Estrazione contributo numero difensori per modificatore difesa");
            return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellanumdifensori d, fascia f WHERE d.idcompetizione = " + idCompetizione + " AND d.idfascia=f.id");
        }

        public List<Fascia> getFasceModificatoreCentrocampo(string idCompetizione)
        {
            //logger.info("Estrazione fasce modifcatore centrocampo");
            return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellacentrocampodiffe d, fascia f WHERE d.idcompetizione = " + idCompetizione + " AND d.idfascia=f.id");
        }

        public void setDatiIncontro(int idIncontro, int idSquadraCasa, int idSquadraFuori, Match match, Boolean[] modPers)
        {
            updateTabellino(idIncontro, idSquadraCasa, match.squadra1, modPers);
            updateTabellino(idIncontro, idSquadraFuori, match.squadra2, modPers);
            updateIncontro(idIncontro, idSquadraCasa, idSquadraFuori, match, modPers);
        }

        private void updateTabellino(int idIncontro, int idSquadra, Match.Team squadra, Boolean[] modPers)
        {
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "update tabellino set ParzialeSquadra=?, TotaleSquadra=?, Gol=? ";
                cmd.Parameters.AddWithValue("@ParzialeSquadra", squadra.parziale);
                cmd.Parameters.AddWithValue("@TotaleSquadra", squadra.getTotale());
                cmd.Parameters.AddWithValue("@Gol", squadra.numeroGol);
                for (int i = 0; i < 3; i++)
                {
                    int i1Based = i + 1;
                    if (modPers[i])
                    {
                        cmd.CommandText += ",modm" + i1Based + "pers=? , modm" + i1Based + "persesiste=true ";
                        cmd.Parameters.AddWithValue("@modm" + i1Based + "pers", getModPersValue(i, squadra));
                    }
                }
                cmd.CommandText += " where IDIncontro = ? and IDSquadra = ?";
                cmd.Parameters.AddWithValue("@IDIncontro", idIncontro);
                cmd.Parameters.AddWithValue("@IDSquadra", idSquadra);
                cmd.ExecuteNonQuery();
            }
        }

        private void updateIncontro(int idIncontro, int idSquadraCasa, int idSquadraFuori, Match match, Boolean[] modPers)
        {
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "update incontro set ParzCasa=?, ParzFuori=?, TotCasa=?, TotFuori=?, GolCasa=?, GolFuori=? ";
                cmd.Parameters.AddWithValue("@ParzCasa", match.squadra1.parziale);
                cmd.Parameters.AddWithValue("@ParzFuori", match.squadra2.parziale);
                cmd.Parameters.AddWithValue("@TotCasa", match.squadra1.getTotale());
                cmd.Parameters.AddWithValue("@TotFuori", match.squadra2.getTotale());
                cmd.Parameters.AddWithValue("@GolCasa", match.squadra1.numeroGol);
                cmd.Parameters.AddWithValue("@GolFuori", match.squadra2.numeroGol);
                for (int i = 0; i < 3; i++)
                {
                    int i1Based = i + 1;
                    if (modPers[i])
                    {
                        cmd.CommandText += " ,M" + i1Based + "Casa=? , M" + i1Based + "Fuori=? ";
                        cmd.Parameters.AddWithValue("@M" + i1Based + "Casa", getModPersValue(i, match.squadra1));
                        cmd.Parameters.AddWithValue("@M" + i1Based + "Fuori", getModPersValue(i, match.squadra2));
                    }
                }
                cmd.CommandText += " where ID=? ";
                cmd.Parameters.AddWithValue("@ID", idIncontro);
                cmd.ExecuteNonQuery();
            }
        }

        private double getModPersValue(int idMod, Match.Team squadra)
        {
            switch (idMod)
            {
                case 0:
                    return squadra.modSpeciale1;
                case 1:
                    return squadra.modSpeciale2;
                case 2:
                    return squadra.modSpeciale3;
            }
            return 0;
        }

        private List<Fascia> getDatiDaFasceByQuery(string query)
        {
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    List<Fascia> listaFasce = new List<Fascia>();
                    while (rea.Read())
                    {
                        Fascia fascia = new Fascia();
                        fascia.valore = rea.GetDouble(0);
                        fascia.min = rea.GetDouble(1);
                        fascia.max = rea.GetDouble(2);
                        listaFasce.Add(fascia);
                    }
                    return listaFasce;
                }
            }
        }

        private string generaFilter(List<int> listaItems)
        {
            string res = "(" + listaItems[0];
            foreach (var item in listaItems)
            {
                res += "," + item;
            }
            res += ")";
            return res;
        }
    }
}
