
using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace fcm.dao
{
    public class FcmDao : IDisposable
    {

        private OdbcConnection conn;

        public FcmDao(String filename) {
            conn.ConnectionString = @"Driver={Microsoft Access Driver (*.mdb)};Dbq=" + filename + ";";
            conn.Open();
        }

        public void Dispose()
        {
            conn.Dispose();
        }

        public string[] getListaCompetizioni() {
            //logger.info("Estrazione dell'elenco delle competizioni");
            OdbcCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, nome FROM competizione";
            OdbcDataReader rea = cmd.ExecuteReader();

            //estraggo l'elenco delle competizioni
            List<String> comps = new List<String>();
            while (rea.Read()) {
                comps.Add(rea.GetString(1) + " - " + rea.GetString(2));
            }
            rea.Close();
            return comps.ToArray();
        }

        public string[] getListaGironi(string competizione)
        {
            //logger.info("Estrazione dell'elenco dei gironi");
            OdbcCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, nome FROM girone WHERE idcompetizione = " + competizione;
            OdbcDataReader rea = cmd.ExecuteReader();

            //estraggo l'elenco delle competizioni
            List<String> girs = new List<String>();
            while (rea.Read())
            {
                string girName = rea.GetString(2);
                girName = girName != null ? girName : "Senza Nome";
                girs.Add(rea.GetString(1) + " - " + girName);
            }
            rea.Close();
            return girs.ToArray();
        }

        public Dictionary<int, string> getSquadreGirone(String idGirone)
        {
            //logger.info("Estrazione squadre");

            OdbcCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT i.idsquadra, f.nome FROM iscritta i, fantasquadra f WHERE i.idsquadra=f.id AND i.idgirone = " + idGirone;
            OdbcDataReader rea = cmd.ExecuteReader();

            //estraggo l'elenco dei gironi
            Dictionary<int, string> nomiteam = new Dictionary<int, string>();
            while (rea.Read())
            {
                nomiteam.Add(rea.GetInt32(1), rea.GetString(2));
            }
            rea.Close();
            return nomiteam;
        }

    public Regole getRegoleCompetizione (String idCompetizione) throws PluginException{
		logger.info("Estrazione delle regole della competizione");
		try (
				Statement stmt = conn.createStatement();
				){
			return new Regole(stmt, idCompetizione);
		} catch (SQLException e) {
			logger.severe(e.getMessage());
			throw new PluginException("");
		}
	}
	
	public ArrayList<Integer> getGiornate (String idGirone) throws PluginException{
		logger.info("Estrazione lista giornate");
		try (
				Statement stmt = conn.createStatement();
				ResultSet rs = stmt.executeQuery("SELECT idGiornata FROM incontro WHERE idGirone = "+idGirone+ " GROUP BY idGiornata ");
				){

			ArrayList<Integer> giornate = new ArrayList<Integer>();
			while (rs.next()){
				giornate.add(rs.getInt(1));
			}
			return giornate;
		} catch (SQLException e) {
			logger.severe(e.getMessage());
			throw new PluginException("");
		}
	}
	
	public Integer[] getSquadreIscritte (String idGirone) throws PluginException{
		logger.info("Estrazione squadre iscritte");
		try (
				Statement stmt = conn.createStatement();
				ResultSet rs = stmt.executeQuery("SELECT idSquadra FROM iscritta where idgirone="+idGirone);
				){

			ArrayList<Integer> squadre = new ArrayList<Integer>();
			while (rs.next()){
				squadre.add(rs.getInt(1));
			}
			Integer[] squadreArray = new Integer[squadre.size()];
			squadre.toArray(squadreArray);
			return squadreArray;
		} catch (SQLException e) {
			logger.severe(e.getMessage());
			throw new PluginException("");
		}
	}
	
	public List<Incontro> getIncontri (String idGirone, int idGiornata) throws PluginException{
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
				inc.idIncontro=rs.getString(1);
				inc.casa = rs.getString(2);
				inc.trasferta = rs.getString(3);
				listaIncontri.add(inc);
			}
			return listaIncontri;
		} catch (SQLException e) {
			logger.severe(e.getMessage());
			throw new PluginException("");
		}
	}
	
	public Map<Integer, Tabellino> getTabellini (ArrayList<String> idIncontri) throws PluginException, InvalidGiornataException{
		logger.info("Estrazione tabellini");
		String filtro = idIncontri.toString().replace("[", "(").replace("]", ")");
		try (
				Statement stmt = conn.createStatement();
				ResultSet rs = stmt.executeQuery("SELECT idsquadra, tot, ruolo, modportiere, modattacco, moddifesa, voto, modm1pers, modm2pers, modm3pers FROM tabellino WHERE idincontro IN "+filtro+" ORDER BY idsquadra");
				){

			Map<Integer, Tabellino> mapTabellini = new HashMap<>();
			boolean saltaGiornata = true;
			while (rs.next()){
				Tabellino tab = new Tabellino();
				String votiString = rs.getString(2);
				if (votiString!=null){
					tab.voti = votiString.split("%");
					saltaGiornata = false;
				}
				String ruoliString = rs.getString(3);
				if (ruoliString!=null){
					tab.ruoli = ruoliString.split("%");
					saltaGiornata = false;
				}
				tab.modPortiere = rs.getDouble(4);
				tab.modAttacco = rs.getDouble(5);
				tab.modDifesa = rs.getDouble(6);

				String votipuriString = rs.getString(7);
				if (votipuriString!=null){
					tab.votipuri = votipuriString.split("%");
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
	
	public List<Fascia> getFasceConversioneGol (String idCompetizione) throws PluginException{
		logger.info("Estrazione fasce gol");
		return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellagol g, fascia f WHERE g.idCompetizione = "+idCompetizione+" AND g.idFascia=f.id");
	}
	
	public List<Fascia> getFasceModificatoreDifesa (String idCompetizione) throws PluginException{
		logger.info("Estrazione fasce modificatore difesa");
		return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabelladifesa d, fascia f WHERE d.idcompetizione = "+idCompetizione+" AND d.idfascia=f.id");
	}
	
	public List<Fascia> getContributoNumeroDifensoriModificatoreDifesa (String idCompetizione) throws PluginException{
		logger.info("Estrazione contributo numero difensori per modificatore difesa");
		return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellanumdifensori d, fascia f WHERE d.idcompetizione = "+idCompetizione+" AND d.idfascia=f.id");
	}
	
	public List<Fascia> getFasceModificatoreCentrocampo (String idCompetizione) throws PluginException{
		logger.info("Estrazione fasce modifcatore centrocampo");
		return getDatiDaFasceByQuery("SELECT f.valore, f.min, f.max FROM tabellacentrocampodiffe d, fascia f WHERE d.idcompetizione = "+idCompetizione+" AND d.idfascia=f.id");
	}
	
	private List<Fascia> getDatiDaFasceByQuery (String query) throws PluginException{
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

	@Override
	public void close() throws Exception {
		if (conn!=null){
			conn.close();
		}
	}


}
}