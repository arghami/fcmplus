
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
                        comps.Add(rea.GetString(1) + " - " + rea.GetString(2));
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
                        string girName = rea.GetString(2);
                        girName = girName != null ? girName : "Senza Nome";
                        girs.Add(rea.GetString(1) + " - " + girName);
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
                        nomiteam.Add(rea.GetInt32(1), rea.GetString(2));
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
                        giornate.Add(rea.GetInt32(1));
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
                        squadre.Add(rea.GetInt32(1));
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
                cmd.CommandText = "SELECT id, idcasa, idfuori, idtipo FROM incontro WHERE idgirone = " + idGirone + " AND idGiornata = " + idGiornata;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {

                    List<Incontro> listaIncontri = new List<Incontro>();
                    while (rea.Read())
                    {
                        Incontro inc = new Incontro();
                        if (rea.GetInt32(4) == 1)
                        {
                            inc.fattoreCampo = true;
                        }
                        inc.idIncontro = rea.GetString(1);
                        inc.casa = rea.GetString(2);
                        inc.trasferta = rea.GetString(3);
                        listaIncontri.Add(inc);
                    }
                    return listaIncontri;
                }
            }
        }

        public Dictionary<int, Tabellino> getTabellini(List<string> idIncontri)
        {
            //logger.info("Estrazione tabellini");
            string filtro = generaFilter(idIncontri);
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT idsquadra, tot, ruolo, modportiere, modattacco, moddifesa, voto, modm1pers, modm2pers, modm3pers FROM tabellino WHERE idincontro IN " + filtro + " ORDER BY idsquadra";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    Dictionary<int, Tabellino> mapTabellini = new Dictionary<int, Tabellino>();
                    Boolean saltaGiornata = true;
                    while (rea.Read())
                    {
                        Tabellino tab = new Tabellino();
                        string votistring = rea.GetString(2);
                        if (votistring != null)
                        {
                            tab.voti = votistring.Split('%');
                            saltaGiornata = false;
                        }
                        string ruolistring = rea.GetString(3);
                        if (ruolistring != null)
                        {
                            tab.ruoli = ruolistring.Split('%');
                            saltaGiornata = false;
                        }
                        tab.modPortiere = rea.GetDouble(4);
                        tab.modAttacco = rea.GetDouble(5);
                        tab.modDifesa = rea.GetDouble(6);

                        string votipuristring = rea.GetString(7);
                        if (votipuristring != null)
                        {
                            tab.votipuri = votipuristring.Split('%');
                            saltaGiornata = false;
                        }

                        tab.modPers1 = rea.GetDouble(8);
                        tab.modPers2 = rea.GetDouble(9);
                        tab.modPers3 = rea.GetDouble(10);
                        mapTabellini.Add(rea.GetInt32(1), tab);
                    }
                    if (saltaGiornata)
                    {
                        throw new InvalidGiornataException();
                    }
                    return mapTabellini;
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
                        fascia.valore = rea.GetDouble(1);
                        fascia.min = rea.GetDouble(2);
                        fascia.max = rea.GetDouble(3);
                        listaFasce.Add(fascia);
                    }
                    return listaFasce;
                }
            }
        }

        private string generaFilter(List<string> listaItems)
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
