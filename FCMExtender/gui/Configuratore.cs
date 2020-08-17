using fcm.dao;
using fcm.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FCMExtender.gui
{
    public partial class Configuratore : Form
    {
        private List<TabPage> tabs = new List<TabPage>();
        private List<string> competizioni;
        private List<Regole> regole;
        private List<ConfigData> modificatori;
        private string nomeLega;
        private Action callback;

        public Configuratore(List<string> competizioni, List<ConfigData> modificatori, string nomeLega, Action callback, List<Regole> regole)
        {
            this.competizioni = competizioni;
            this.regole = regole;
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
                TabPage newTab = creaTab(comp, modPerCompetizione, i, regole[i]);
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
        private TabPage creaTab(string comp, List<ConfigData> modificatori, int tabIndex, Regole regole)
        {
            TabPage newTab = new TabPage();
            newTab.Location = new System.Drawing.Point(4, 22);
            newTab.Name = comp;
            newTab.Padding = new System.Windows.Forms.Padding(3);
            newTab.Size = new System.Drawing.Size(494, 169);
            newTab.TabIndex = tabIndex;
            newTab.Text = comp;
            newTab.UseVisualStyleBackColor = true;
            TableLayoutPanel[] tableLayoutPanels = buildTableLayout(modificatori, regole);
            newTab.Controls.Add(tableLayoutPanels[0]);
            newTab.Controls.Add(tableLayoutPanels[1]);
            return newTab;
        }

        /// <summary>
        /// Creo il layout tabellare che contiene tutti i dati di configurazione
        /// </summary>
        /// <param name="modificatori"></param>
        /// <returns></returns>
        private TableLayoutPanel[] buildTableLayout(List<ConfigData> modificatori, Regole regole)
        {
            TableLayoutPanel[] tableLayouts = new TableLayoutPanel[2];
            TableLayoutPanel tableLayoutHeader = new TableLayoutPanel();
            tableLayoutHeader.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutHeader.ColumnCount = 3;
            tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260));
            tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80));
            tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140));
            tableLayoutHeader.Location = new System.Drawing.Point(20, 20);
            tableLayoutHeader.Name = "tableLayoutHeader";
            tableLayoutHeader.RowCount = 1;
            tableLayoutHeader.Size = new System.Drawing.Size(500, 20);
            var l1 = new Label();
            l1.Text = "Nome";
            tableLayoutHeader.Controls.Add(l1, 0, 0);
            var l2 = new Label();
            l2.Text = "Attivo";
            tableLayoutHeader.Controls.Add(l2, 1, 0);
            var l3 = new Label();
            l3.Text = "Destinazione";
            tableLayoutHeader.Controls.Add(l3, 2, 0);

            TableLayoutPanel tableLayoutContent = new TableLayoutPanel();
            tableLayoutContent.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutContent.ColumnCount = 3;
            tableLayoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260));
            tableLayoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80));
            tableLayoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140));
            tableLayoutContent.Location = new System.Drawing.Point(20, 40);
            tableLayoutContent.Name = "tableLayoutPanel1";
            tableLayoutContent.RowCount = modificatori.Count;
            tableLayoutContent.Size = new System.Drawing.Size(500, 150);

            for (int i=0; i<modificatori.Count; i++)
            {
                buildTableRow(tableLayoutContent, modificatori[i], i, regole);
            }
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            tableLayoutContent.AutoScroll = true;
            tableLayoutHeader.Padding = new Padding(0, 0, vertScrollWidth, 0);
            tableLayoutContent.Padding = new Padding(0, 0, vertScrollWidth, 0);
            tableLayoutContent.MouseEnter += new System.EventHandler((sender, args) => tableLayoutContent.Focus());

            tableLayouts[0] = tableLayoutHeader;
            tableLayouts[1] = tableLayoutContent;
            return tableLayouts;
        }

        /// <summary>
        /// Creo la riga i-esima della tabella valorizzandola con i dati provenienti dalla configurazione
        /// </summary>
        /// <param name="tableLayoutPanel1"></param>
        /// <param name="mod"></param>
        /// <param name="rowPos"></param>
        private void buildTableRow(TableLayoutPanel tableLayoutPanel1, ConfigData mod, int rowPos, Regole regole)
        {
            var labl = new Label();
            labl.AutoSize = true;
            labl.Text = mod.nome;
            tableLayoutPanel1.Controls.Add(labl, 0, rowPos);

            var chkBox = new CheckBox();
            chkBox.Checked = mod.abilitato;
            tableLayoutPanel1.Controls.Add(chkBox, 1, rowPos);

            var comboBx = new ComboBox();
            comboBx.Items.Add(new ModItem(1, (regole.usaSpeciale1 ? "" : "NON ATTIVO - ")+regole.nomeSpeciale1));
            comboBx.Items.Add(new ModItem(2, (regole.usaSpeciale2 ? "" : "NON ATTIVO - ") + regole.nomeSpeciale2));
            comboBx.Items.Add(new ModItem(3, (regole.usaSpeciale3 ? "" : "NON ATTIVO - ") + regole.nomeSpeciale3));
            comboBx.ValueMember = "id";
            comboBx.DisplayMember = "nome";
            comboBx.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBx.SelectedIndex = mod.destinazione;
            tableLayoutPanel1.Controls.Add(comboBx, 2, rowPos);
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
                TableLayoutPanel table = (TableLayoutPanel)theTab.Controls[1];
                for (int i=0; i<table.RowCount; i++)
                {
                    //scorro tutte le righe della tabella e prelevo le scelte dell'utente
                    Label labl = (Label)table.GetControlFromPosition(0, i);
                    CheckBox chkBox = (CheckBox)table.GetControlFromPosition(1, i);
                    ComboBox comboBox = (ComboBox)table.GetControlFromPosition(2, i);
                    ConfigData data = new ConfigData(theTab.Text, labl.Text, chkBox.Checked, comboBox.SelectedIndex);
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
