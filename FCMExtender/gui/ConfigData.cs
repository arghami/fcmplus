using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace FCMExtender.gui
{
    public class ConfigData
    {
        public string competizione { get; set; }
        public string nome { get; set; }
        public bool abilitato { get; set; }
        public int destinazione { get; set; }

        public ConfigData() { }
        public ConfigData (string competizione, string nome, bool abilitato, int destinazione)
        {
            this.competizione = competizione;
            this.nome = nome;
            this.abilitato = abilitato;
            this.destinazione = destinazione;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ConfigData cData = obj as ConfigData;
            if (cData == null) return false;
            else return this.competizione.Equals(cData.competizione) && this.nome.Equals(cData.nome);
        }

        public static void Serialize(List<ConfigData> tData, string nomeLega)
        {
            nomeLega = nomeLega.Split('/').Last().Split('\\').Last();
            var serializer = new XmlSerializer(typeof(List<ConfigData>));
            TextWriter writer = new StringWriter();
            serializer.Serialize(writer, tData);
            string data = writer.ToString();
            System.IO.File.WriteAllText("conf/"+nomeLega + ".xml", data);
        }

        public static List<ConfigData> Deserialize(string nomeLega)
        {
            nomeLega = nomeLega.Split('/').Last().Split('\\').Last();
            try
            {
                string ser = System.IO.File.ReadAllText("conf/" + nomeLega +".xml");
                var serializer = new XmlSerializer(typeof(List<ConfigData>));
                TextReader reader = new StringReader(ser);
                return (List<ConfigData>)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                return new List<ConfigData>();
            }
        }

        public static bool isConfigured(string nomeLega)
        {
            nomeLega = nomeLega.Split('/').Last().Split('\\').Last();
            return System.IO.File.Exists("conf/" + nomeLega + ".xml");
        }
    }
}
