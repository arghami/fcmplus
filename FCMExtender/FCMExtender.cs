using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Reflection;

namespace main
{
    class FCMExtender
    {
        private static StreamWriter w;
        public static void pippo(string[] args)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            using (w = File.AppendText(basePath + @"log.txt"))
            {
                log("inizio");
                string filename = basePath+@"\Materdei League 2016-1-2016 - Copia.fcm";
                if (args.Length == 3)
                {
                    filename = args[2].Split('=')[1].Replace("\"", "");
                }
                log(filename);

                using (OdbcConnection conn = new OdbcConnection())
                {
                    conn.ConnectionString = @"Driver={Microsoft Access Driver (*.mdb)};Dbq=" + filename + ";";
                    conn.Open();
                    OdbcCommand cmd = conn.CreateCommand();
                    int giornataDiA = 38;
                    cmd.CommandText = "select t.*, i.idgirone from tabellino t, incontro i where t.idincontro = i.id and i.giornatadia=" + giornataDiA;
                    OdbcDataReader rea = cmd.ExecuteReader();
                    List<EnhancedIncontro> listIncontri = new List<EnhancedIncontro>();
                    while (rea.Read())
                    {
                        EnhancedIncontro inc = new EnhancedIncontro(rea);
                        listIncontri.Add(inc);

                    }
                    rea.Close();

                    foreach (var inc in listIncontri)
                    {
                        inc.apply(conn);
                    }
                }
            }
            if (args.Length != 3)
            {
                Console.ReadLine();
            }
        }

        public static void log(string logMessage)
        {
            w.WriteLine("[{0} {1}] {2}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString(), logMessage);
        }
    }
}
