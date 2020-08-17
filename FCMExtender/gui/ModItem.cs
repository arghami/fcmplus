namespace FCMExtender.gui
{
    public class ModItem
    {
        public int id { get; set; }
        public string nome { get; set; }

        public ModItem (int id, string nome)
        {
            this.id = id;
            this.nome = nome;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ModItem cData = obj as ModItem;
            if (cData == null) return false;
            else return this.id.Equals(cData.id) && this.nome.Equals(cData.nome);
        }
    }
}
