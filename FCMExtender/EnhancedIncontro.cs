using bridge.model;
using System;
using System.Data.Odbc;

namespace main
{
    public class EnhancedIncontro
    {
        public IncontroWrapper incontro;

        public EnhancedIncontro(OdbcDataReader rea)
        {
            var jsEngine = new Jurassic.ScriptEngine();
            jsEngine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(jsEngine));
            incontro = new IncontroWrapper(jsEngine);
            TabellinoWrapper tabCasa = new TabellinoWrapper(jsEngine);
            tabCasa.build(jsEngine, rea);
            incontro.setCasa(tabCasa);
            rea.Read();
            TabellinoWrapper tabTrasferta = new TabellinoWrapper(jsEngine);
            tabTrasferta.build(jsEngine, rea);
            incontro.setTrasferta(tabTrasferta);

            jsEngine.SetGlobalValue("incontro", incontro);
            jsEngine.ExecuteFile(@"C:\Users\Dario\desktop\test.js");
            Console.WriteLine("sq1:" + tabCasa.get(TabellinoWrapper.ModM1Pers));
            Console.WriteLine("sq2:" + tabTrasferta.get(TabellinoWrapper.ModM1Pers));

        }

        public void apply (OdbcConnection conn)
        {

            OdbcCommand cmd = conn.CreateCommand();
            cmd.CommandText = "update tabellino set modm1pers=? , modm1persesiste=true where IDIncontro = ? and IDSquadra = ?";
            cmd.Parameters.AddWithValue("@modm1pers", incontro.getCasa().get(TabellinoWrapper.ModM1Pers));
            cmd.Parameters.AddWithValue("@IDIncontro", incontro.getCasa().get(TabellinoWrapper.IDIncontro));
            cmd.Parameters.AddWithValue("@IDSquadra", incontro.getCasa().get(TabellinoWrapper.IDSquadra));
            cmd.ExecuteNonQuery();

            cmd = conn.CreateCommand();
            cmd.CommandText = "update tabellino set modm1pers=? , modm1persesiste=true where IDIncontro = ? and IDSquadra = ?";
            cmd.Parameters.AddWithValue("@modm1pers", incontro.getTrasferta().get(TabellinoWrapper.ModM1Pers));
            cmd.Parameters.AddWithValue("@IDIncontro", incontro.getTrasferta().get(TabellinoWrapper.IDIncontro));
            cmd.Parameters.AddWithValue("@IDSquadra", incontro.getTrasferta().get(TabellinoWrapper.IDSquadra));
            cmd.ExecuteNonQuery();

            cmd = conn.CreateCommand();
            //TODO stabilire correttamente quale valore va in m1casa e quale in m1fuori
            cmd.CommandText = "update incontro set m1casa=? where id=? and idcasa=?";
            cmd.Parameters.AddWithValue("@m1casa", incontro.getCasa().get(TabellinoWrapper.ModM1Pers));
            cmd.Parameters.AddWithValue("@id", incontro.getCasa().get(TabellinoWrapper.IDIncontro));
            cmd.Parameters.AddWithValue("@idcasa", incontro.getCasa().get(TabellinoWrapper.IDSquadra));
            cmd.ExecuteNonQuery();
            cmd.CommandText = "update incontro set m1casa=? where id=? and idcasa=?";
            cmd.Parameters.AddWithValue("@m1casa", incontro.getTrasferta().get(TabellinoWrapper.ModM1Pers));
            cmd.Parameters.AddWithValue("@id", incontro.getTrasferta().get(TabellinoWrapper.IDIncontro));
            cmd.Parameters.AddWithValue("@idcasa", incontro.getTrasferta().get(TabellinoWrapper.IDSquadra));
            cmd.ExecuteNonQuery();
            cmd.CommandText = "update incontro set m1fuori=? where id=? and idfuori=?";
            cmd.Parameters.AddWithValue("@m1fuori", incontro.getCasa().get(TabellinoWrapper.ModM1Pers));
            cmd.Parameters.AddWithValue("@id", incontro.getCasa().get(TabellinoWrapper.IDIncontro));
            cmd.Parameters.AddWithValue("@idfuori", incontro.getCasa().get(TabellinoWrapper.IDSquadra));
            cmd.ExecuteNonQuery();
            cmd.CommandText = "update incontro set m1fuori=? where id=? and idfuori=?";
            cmd.Parameters.AddWithValue("@m1fuori", incontro.getTrasferta().get(TabellinoWrapper.ModM1Pers));
            cmd.Parameters.AddWithValue("@id", incontro.getTrasferta().get(TabellinoWrapper.IDIncontro));
            cmd.Parameters.AddWithValue("@idfuori", incontro.getTrasferta().get(TabellinoWrapper.IDSquadra));
            cmd.ExecuteNonQuery();
        }
    }
}
