using Jurassic;
using Jurassic.Library;

namespace bridge.model
{
    public class IncontroWrapper : ObjectInstance
    {
        public IncontroWrapper(ScriptEngine engine)
            : base(engine)
        {}


        public TabellinoWrapper getCasa()
        {
            return (TabellinoWrapper)this["casa"];
        }

        public void setCasa(TabellinoWrapper tab)
        {
            this["casa"] = tab;
        }

        public TabellinoWrapper getTrasferta()
        {
            return (TabellinoWrapper)this["trasferta"];
        }

        public void setTrasferta(TabellinoWrapper tab)
        {
            this["trasferta"] = tab;
        }

        public string getCompetizione()
        {
            return (string)this["competizione"];
        }

        public void setCompetizione(string comp)
        {
            this["competizione"] = comp;
        }

        public int getGiornata()
        {
            return (int)this["giornata"];
        }

        public void setGiornata(int gior)
        {
            this["giornata"] = gior;
        }

        public string getNomeCasa()
        {
            return (string)this["nomecasa"];
        }

        public void setNomeCasa(string comp)
        {
            this["nomecasa"] = comp;
        }

        public string getNomeTrasferta()
        {
            return (string)this["nometrasferta"];
        }

        public void setNomeTrasferta(string comp)
        {
            this["nometrasferta"] = comp;
        }
    }
}
