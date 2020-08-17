using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FCMExtender.gui
{
    public partial class Configuratore : Form
    {
        private List<TabPage> tabs = new List<TabPage>();
        private List<string> competizioni;
        private List<ConfigData> modificatori;
        private string nomeLega;
        private Func<bool> callback;

        public Configuratore(List<string> competizioni, List<ConfigData> modificatori, string nomeLega, Func<bool> callback)
        {
            this.competizioni = competizioni;
            this.modificatori = modificatori;
            this.nomeLega = nomeLega;
            this.callback = callback;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < competizioni.Count; i++)
            {
                string comp = competizioni[i];
                //filtro solo le entry di configurazione relative al tab (quindi alla competizione)
                List<ConfigData> modPerCompetizione = new List<ConfigData>();
                foreach (var m in modificatori)
                {
                    if (m.competizione.Equals(comp))
                    {
                        modPerCompetizione.Add(m);
                    }
                }
                //creo il tab per l'i-esima competizione e lo aggiungo al tabContainer
                TabPage newTab = creaTab(comp, modPerCompetizione, i);
                this.tabControl1.Controls.Add(newTab);
                tabs.Add(newTab);
            }
        }

        /// <summary>
        /// Creo l'i-esimo tab
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="modificatori"></param>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        private TabPage creaTab(string comp, List<ConfigData> modificatori, int tabIndex)
        {
            TabPage newTab = new TabPage();
            newTab.Location = new System.Drawing.Point(4, 22);
            newTab.Name = comp;
            newTab.Padding = new System.Windows.Forms.Padding(3);
            newTab.Size = new System.Drawing.Size(494, 169);
            newTab.TabIndex = tabIndex;
            newTab.Text = comp;
            newTab.UseVisualStyleBackColor = true;
            TableLayoutPanel tableLayoutPanel1 = buildTableLayout(modificatori);
            newTab.Controls.Add(tableLayoutPanel1);
            return newTab;
        }

        /// <summary>
        /// Creo il layout tabellare che contiene tutti i dati di configurazione
        /// </summary>
        /// <param name="modificatori"></param>
        /// <returns></returns>
        private TableLayoutPanel buildTableLayout(List<ConfigData> modificatori)
        {
            TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.Location = new System.Drawing.Point(49, 20);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = modificatori.Count + 1;
            //tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            //tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 67F));
            tableLayoutPanel1.Size = new System.Drawing.Size(412, 100);

            var l1 = new Label();
            l1.Text = "Nome";
            tableLayoutPanel1.Controls.Add(l1, 0, 0);
            var l2 = new Label();
            l2.Text = "Abilitato";
            tableLayoutPanel1.Controls.Add(l2, 1, 0);
            var l3 = new Label();
            l3.Text = "Destinazione";
            tableLayoutPanel1.Controls.Add(l3, 2, 0);

            for (int i=0; i<modificatori.Count; i++)
            {
                buildTableRow(tableLayoutPanel1, modificatori[i], i+1);
            }
            return tableLayoutPanel1;
        }

        /// <summary>
        /// Creo la riga i-esima della tabella valorizzandola con i dati provenienti dalla configurazione
        /// </summary>
        /// <param name="tableLayoutPanel1"></param>
        /// <param name="mod"></param>
        /// <param name="rowPos"></param>
        private void buildTableRow(TableLayoutPanel tableLayoutPanel1, ConfigData mod, int rowPos)
        {
            var labl = new Label();
            labl.Text = mod.nome;
            tableLayoutPanel1.Controls.Add(labl, 0, rowPos);

            var chkBox = new CheckBox();
            chkBox.Checked = mod.abilitato;
            tableLayoutPanel1.Controls.Add(chkBox, 1, rowPos);

            var comboBx = new ComboBox();
            comboBx.Items.Add("ModPers1");
            comboBx.Items.Add("ModPers2");
            comboBx.Items.Add("ModPers3");
            comboBx.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBx.SelectedItem = mod.destinazione;
            tableLayoutPanel1.Controls.Add(comboBx, 2, rowPos);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Triggera l'evento click sul pulsante di conferma del configuratore.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //scorro tutti i tab e raccolgo le opzioni selezionate dall'utente in una nuova lista di ConfigData che poi serializzo
            List<ConfigData> newData = new List<ConfigData>();
            foreach (var theTab in tabs)
            {
                TableLayoutPanel table = (TableLayoutPanel)theTab.Controls[0];
                for (int i=1; i<table.RowCount; i++)
                {
                    //scorro tutte le righe della tabella e prelevo le scelte dell'utente
                    Label labl = (Label)table.GetControlFromPosition(0, i);
                    CheckBox chkBox = (CheckBox)table.GetControlFromPosition(1, i);
                    ComboBox comboBox = (ComboBox)table.GetControlFromPosition(2, i);
                    ConfigData data = new ConfigData(theTab.Text, labl.Text, chkBox.Checked, (string)comboBox.SelectedItem);
                    newData.Add(data);
                    //se un modificatore è checkato, devo avere una destination not null
                    if (chkBox.Checked && comboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Configurazione errata. "+data.nome+" per "+data.competizione+" è abilitato ma non ha una destinazione.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            //se è tutto ok, serializzo la configurazione ed eseguo la callback che sblocca il pulsante nel form chiamante
            ConfigData.Serialize(newData, nomeLega);
            callback.Invoke();
            Dispose();
        }
    }
}
