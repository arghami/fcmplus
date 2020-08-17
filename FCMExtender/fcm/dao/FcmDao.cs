
using fcm.model;
using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace fcm.dao
{
    public class FcmDao : IDisposable
    {

        private OdbcConnection conn;

        public FcmDao(string filename) {
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

        public Regole getRegoleCompetizione (string idCompetizione) {
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
	
	    public List<Incontro> getIncontri (string idGirone, int idGiornata) throws PluginException{
		    logger.info("Estrazione incontri");
		    try (
				    Statement stmt = conn.createStatement();
				    ResultSet rs = stmt.executeQuery("SELECT id, idcasa, idfuori, idtipo FROM incontro WHERE idgirone = "+idGirone+" AND idGiornata = "+idGiornata);
				    ){

			    List<Incontro> listaIncontri = new ArrayList<>();
			    while (rs.next()){
				    Incontro inc = new Incontro();
				    if (rs.getInt(4)==1){
					    inc.fattoreCampo = true;
				    }
				    inc.idIncontro=rs.GetString(1);
				    inc.casa = rs.GetString(2);
				    inc.trasferta = rs.GetString(3);
				    listaIncontri.add(inc);
			    }
			    return listaIncontri;
		    } catch (SQLException e) {
			    logger.severe(e.getMessage());
			    throw new PluginException("");
		    }
	    }
	
	    public Map<Integer, Tabellino> getTabellini (ArrayList<string> idIncontri) throws PluginException, InvalidGiornataException{
		    logger.info("Estrazione tabellini");
		    string filtro = idIncontri.tostring().replace("[", "(").replace("]", ")");
		    try (
				    Statement stmt = conn.createStatement();
				    ResultSet rs = stmt.executeQuery("SELECT idsquadra, tot, ruolo, modportiere, modattacco, moddifesa, voto, modm1pers, modm2pers, modm3pers FROM tabellino WHERE idincontro IN "+filtro+" ORDER BY idsquadra");
				    ){

			    Map<Integer, Tabellino> mapTabellini = new HashMap<>();
			    boolean saltaGiornata = true;
			    while (rs.next()){
				    Tabellino tab = new Tabellino();
				    string votistring = rs.GetString(2);
				    if (votistring!=null){
					    tab.voti = votistring.split("%");
					    saltaGiornata = false;
				    }
				    string ruolistring = rs.GetString(3);
				    if (ruolistring!=null){
					    tab.ruoli = ruolistring.split("%");
					    saltaGiornata = false;
				    }
				    tab.modPortiere = rs.getDouble(4);
				    tab.modAttacco = rs.getDouble(5);
				    tab.modDifesa = rs.getDouble(6);

				    string votipuristring = rs.GetString(7);
				    if (votipuristring!=null){
					    tab.votipuri = votipuristring.split("%");
					    saltaGiornata = false;
				    }
				
				    tab.modPers1 = rs.getDouble(8);
				    tab.modPers2 = rs.getDouble(9);
				    tab.modPers3 = rs.getDouble(10);
				    mapTabellini.put(rs.getInt(1), tab);
			    }
			    if (saltaGiornata){
				    throw new InvalidGiornataException();
			    }
			    return mapTabellini;
		    } catch (SQLException e) {
			    logger.severe(e.getMessage());
			    throw new PluginException("");
		    }
	    }
	
	    public List<Fascia> getFasceConversioneGol (string idCompetizione) throws PluginException{
		    logger.info("Estrazione fasce gol");
		    return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellagol g, fascia f WHERE g.idCompetizione = "+idCompetizione+" AND g.idFascia=f.id");
	    }
	
	    public List<Fascia> getFasceModificatoreDifesa (string idCompetizione) throws PluginException{
		    logger.info("Estrazione fasce modificatore difesa");
		    return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabelladifesa d, fascia f WHERE d.idcompetizione = "+idCompetizione+" AND d.idfascia=f.id");
	    }
	
	    public List<Fascia> getContributoNumeroDifensoriModificatoreDifesa (string idCompetizione) throws PluginException{
		    logger.info("Estrazione contributo numero difensori per modificatore difesa");
		    return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellanumdifensori d, fascia f WHERE d.idcompetizione = "+idCompetizione+" AND d.idfascia=f.id");
	    }
	
	    public List<Fascia> getFasceModificatoreCentrocampo (string idCompetizione) throws PluginException{
		    logger.info("Estrazione fasce modifcatore centrocampo");
		    return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellacentrocampodiffe d, fascia f WHERE d.idcompetizione = "+idCompetizione+" AND d.idfascia=f.id");
	    }
	
	    private List<Fascia> getDatiDaFasceByQuery (string query) throws PluginException{
		    try (
				    Statement stmt = conn.createStatement();
				    ResultSet rs = stmt.executeQuery(query);
				    ){

			    List<Fascia> listaFasce = new ArrayList<>();
			    while (rs.next()){
				    Fascia fascia = new Fascia();
				    fascia.valore = rs.getDouble(1);
				    fascia.min = rs.getDouble(2);
				    fascia.max = rs.getDouble(3);
				    listaFasce.add(fascia);
			    }
			    return listaFasce;
		    } catch (SQLException e) {
			    logger.severe(e.getMessage());
			    throw new PluginException("");
		    }
	    }


    }
}