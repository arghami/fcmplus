
using fcm.exception;
using fcm.model;
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
                        string girName = rea.GetString(1);
                        girName = girName != null ? girName : "Senza Nome";
                        girs.Add(rea.GetString(0) + " - " + girName);
                    }
                    return girs.ToArray();
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

        public List<Incontro> getIncontri(string idGirone, int idGiornata)
        {
            //logger.info("Estrazione incontri");
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, idcasa, idfuori, idtipo, parzcasa, parzfuori, totcasa, totfuori, golcasa, golfuori FROM incontro WHERE idgirone = " + idGirone + " AND idGiornata = " + idGiornata;
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
                cmd.CommandText = "SELECT idsquadra, tot, ruolo, modportiere, modattacco, moddifesa, voto, lista, modm1pers, modm2pers, modm3pers FROM tabellino WHERE idincontro IN " + filtro + " ORDER BY idsquadra";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    Dictionary<int, Tabellino> mapTabellini = new Dictionary<int, Tabellino>();
                    Boolean saltaGiornata = true;
                    while (rea.Read())
                    {
                        Tabellino tab = new Tabellino();
                        string votistring = rea.GetString(1);
                        if (votistring != null)
                        {
                            tab.voti = votistring.Split('%');
                            saltaGiornata = false;
                        }
                        string ruolistring = rea.GetString(2);
                        if (ruolistring != null)
                        {
                            tab.ruoli = ruolistring.Split('%');
                            saltaGiornata = false;
                        }
                        tab.modPortiere = rea.GetDouble(3);
                        tab.modAttacco = rea.GetDouble(4);
                        tab.modDifesa = rea.GetDouble(5);

                        string votipuristring = rea.GetString(6);
                        if (votipuristring != null)
                        {
                            tab.votipuri = votipuristring.Split('%');
                            saltaGiornata = false;
                        }

                        string listastring = rea.GetString(7);
                        if (listastring != null)
                        {
                            tab.lista = listastring.Split('%'); //TODO decidere dove gestire l'acquisizione del dettaglio voti
                            saltaGiornata = false;
                        }

                        tab.modPers1 = rea.GetDouble(8);
                        tab.modPers2 = rea.GetDouble(9);
                        tab.modPers3 = rea.GetDouble(10);
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

        public Dictionary<int, FCMData> getGiocaIn(List<int> idGiocatori, int giornata)
        {
            //logger.info("Estrazione squadre iscritte");
            int cnt = 0;
            string filtro = generaFilter(idGiocatori);
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                Dictionary<int, FCMData> datiGiocatori = new Dictionary<int, FCMData>();
                cmd.CommandText = "SELECT g.idGiocatore, g.giornata, p.golfatti1, p.golfatti2, p.golfatti3, "+
                    " p.golfattisurigore1, p.golfattisurigore2, p.golfattisurigore3, " +
                    " p.golvittoria, p.golpareggio, " + //questi sono booleani
                    " p.voto1, p.voto2, p.voto3, p.valoreSpeciale, " +
                    " p.rigsba, p.rigpar, p.autogol1, p.autogol2, p.autogol3, " +
                    " p.golsubiti, p.golsubitisurigore, p.assist, " +
                    " p.amm, p.esp " + //questi sono booleani
                    " FROM giocaIn g, punteggio p where g.idPunteggio=p.id " +
                    " AND g.giornata = " + giornata +
                    " AND g.idGiocatore in " + filtro;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                    {
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
                cmd.CommandText = "update tabellino set parzialeSquadra=?, totaleSquadra=?, gol=? ";
                cmd.Parameters.AddWithValue("@ParzialeSquadra", squadra.parziale);
                cmd.Parameters.AddWithValue("@TotaleSquadra", squadra.getTotale());
                cmd.Parameters.AddWithValue("@Gol", squadra.numeroGol);
                for (int i = 0; i < 3; i++)
                {
                    if (modPers[i])
                    {
                        cmd.CommandText += ",modm" + i + "pers=? , modm" + i + "persesiste=true ";
                        cmd.Parameters.AddWithValue("@modm" + i + "pers", getModPersValue(i, squadra));
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
                cmd.CommandText = "update incontro set parzCasa=?, parzFuori=?, totCasa=?, totFuori=?, golCasa?, golFuori=? ";
                cmd.Parameters.AddWithValue("@ParzCasa", match.squadra1.parziale);
                cmd.Parameters.AddWithValue("@ParzFuori", match.squadra2.parziale);
                cmd.Parameters.AddWithValue("@TotCasa", match.squadra1.getTotale());
                cmd.Parameters.AddWithValue("@TotFuori", match.squadra2.getTotale());
                cmd.Parameters.AddWithValue("@GolCasa", match.squadra1.numeroGol);
                cmd.Parameters.AddWithValue("@GolFuori", match.squadra2.numeroGol);
                for (int i = 0; i < 3; i++)
                {
                    if (modPers[i])
                    {
                        cmd.CommandText += ",m" + i + "Casa=? , m" + i + "Fuori=? ";
                        cmd.Parameters.AddWithValue("@M" + i + "Casa", getModPersValue(i, match.squadra1));
                        cmd.Parameters.AddWithValue("@M" + i + "Fuori", getModPersValue(i, match.squadra2));
                    }
                }
                cmd.CommandText += "where id=?";
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
