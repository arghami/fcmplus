using fcm.entity;
using Jurassic;
using Jurassic.Library;

namespace main
{
    public class IncontroWrapper : ObjectInstance
    {
        public IncontroWrapper(ScriptEngine engine)
            : base(engine)
        {}

        public void setCasa(TabellinoWrapper tab)
        {
            this["casa"] = tab;
        }

        public void setTrasferta(TabellinoWrapper tab)
        {
            this["trasferta"] = tab;
        }

        public TabellinoWrapper getCasa()
        {
            return (TabellinoWrapper)this["casa"];
        }

        public TabellinoWrapper getTrasferta()
        {
            return (TabellinoWrapper)this["trasferta"];
        }
    }
}
