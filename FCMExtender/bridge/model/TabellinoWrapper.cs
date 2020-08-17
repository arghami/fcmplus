using System.Data.Odbc;
using Jurassic;
using Jurassic.Library;
using math.utils;
using fcm.model;

namespace bridge.model
{
    public class TabellinoWrapper : ObjectInstance
    {
        public const string Formazione = "Formazione";

        public TabellinoWrapper(ScriptEngine engine, Tabellino tab)
            : base(engine)
        {
            ArrayInstance form = engine.Array.Construct();

            for (int i = 0; i < tab.giocatori.Length; i++)
            {
                Giocatore gio = tab.giocatori[i];
                ObjectInstance gioc = engine.Object.Construct();
                //anagrafici
                gioc["id"] = NumParser.parseInt(gio.idGiocatore);
                gioc["codiceFCM"] = gio.fcmData.codiceFCM;
                System.DateTime d = gio.fcmData.dataDiNascita;
                gioc["dataDiNascita"] = engine.Date.Construct(d.Year, d.Month, d.Day);
                gioc["primavera"] = gio.fcmData.primavera;
                //da tabellino
                gioc["ruolo"] = NumParser.parseInt(gio.ruolo);
                gioc["voto"] = NumParser.parseDouble(gio.votoPuro);
                gioc["modif"] = NumParser.parseDouble(gio.voto) - NumParser.parseDouble(gio.votoPuro);
                //da archivio voti
                gioc["ammonito"] = gio.fcmData.amm;
                gioc["espulso"] = gio.fcmData.esp;
                gioc["assist"] = gio.fcmData.assist;
                gioc["golfatti"] = gio.fcmData.golfatti;
                gioc["golfattisurigore"] = gio.fcmData.golfattisurigore;
                gioc["golsubiti"] = gio.fcmData.golsubiti;
                gioc["golsubitisurigore"] = gio.fcmData.golsubitisurigore;
                gioc["golvittoria"] = gio.fcmData.golvittoria;
                gioc["golpareggio"] = gio.fcmData.golpareggio;
                gioc["rigoriparati"] = gio.fcmData.rigpar;
                gioc["valoreSpeciale"] = gio.fcmData.valoreSpeciale;
                gioc["rigorisbagliati"] = gio.fcmData.rigsba;
                gioc["autogol"] = gio.fcmData.autogol;
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
