using System.Data.Odbc;
using Jurassic;
using Jurassic.Library;
using math.utils;

namespace fcm.entity
{
    public class TabellinoWrapper : ObjectInstance
    {
        public const string IDIncontro = "IDIncontro";
        public const string IDSquadra = "IDSquadra";
        public const string IDLega = "IDLega";
        public const string Ruolo = "Ruolo";
        public const string Voto = "Voto";
        public const string Modif = "Modif";
        public const string Tot = "Tot";
        public const string ParzialeSquadra = "ParzialeSquadra";
        public const string FattoreCampo = "FattoreCampo";
        public const string ModPortiere = "ModPortiere";
        public const string ModDifesa = "ModDifesa";
        public const string ModCentrocampo = "ModCentrocampo";
        public const string ModAttacco = "ModAttacco";
        public const string ModModulo = "ModModulo";
        public const string ModM1Pers = "ModM1Pers";
        public const string ModM2Pers = "ModM2Pers";
        public const string ModM3Pers = "ModM3Pers";
        public const string TotaleSquadra = "TotaleSquadra";
        public const string Gol = "Gol";
        public const string Formazione = "Formazione";
        public const string IDGirone = "IDGirone";

        public TabellinoWrapper(ScriptEngine engine)
            : base(engine)
        {
            
        }

        public void build(ScriptEngine engine, OdbcDataReader rea)
        {
            for (int i = 0; i < rea.FieldCount; i++)
            {
                set(rea.GetName(i), rea[i]);
            }
            buildListaGiocatori(engine);
        }

        private void buildListaGiocatori(ScriptEngine engine)
        {
            string[] ruoli = ((string)get(Ruolo)).Split('%');
            string[] voti = ((string)get(Voto)).Split('%');
            string[] modif = ((string)get(Modif)).Split('%');
            ArrayInstance form = engine.Array.Construct();
            
            for (int i=0; i<ruoli.Length; i++)
            {
                ObjectInstance gioc = engine.Object.Construct();
                gioc["ruolo"] = NumParser.parseInt(ruoli[i]);
                gioc["voto"] = NumParser.parseDouble(voti[i]);
                gioc["modif"] = NumParser.parseDouble(modif[i]);
                ArrayInstance.Push(form, gioc);
            }

            set(Formazione, form);
        }

        public void set (string field, object value)
        {
            this[field] = value;
        }

        public object get (string field)
        {
            return this[field];
        }
    }
}
