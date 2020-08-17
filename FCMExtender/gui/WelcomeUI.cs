using fcm.dao;
using fcm.model;
using FCMExtender.gui;
using plus.enhancer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace mainform
{
    public partial class WelcomeUI : Form
    {
        private string[] listaCompDaDAO;

        public WelcomeUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Scatta al click sulla selezione della lega. Apre il filechooser e gestisce l'OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSfogliaClick(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                //ho selezionato il file. Copio il nome della lega nella label...
                textFileFCM.Text = openFileDialog.FileName;
                //disabilito il bottone di Elabora...
                buttonElabora.Enabled = false;
                //nascondo il datagrid del resoconto
                label1.Visible = false;
                dataGridView1.Visible = false;
                //leggo e conservo la lista delle competizioni. Se non ne trovo lancio un alert.
                using (FcmDao dao = new FcmDao(textFileFCM.Text))
                {
                    listaCompDaDAO = dao.getListaCompetizioni();
                }
                if (listaCompDaDAO.Count() > 0)
                {
                    configuraButton.Enabled = true;
                    buttonElabora.Enabled = ConfigData.isConfigured(textFileFCM.Text);
                }
                else
                {
                    configuraButton.Enabled = false;
                    MessageBox.Show("Non sono presenti competizioni in questa lega.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonElabora_Click(object sender, EventArgs e)
        {
            try
            {
                label1.Visible = false;
                dataGridView1.Visible = false;
                progressBarAvanzamento.Visible = true;
                string nomeLega = textFileFCM.Text.Split('/').Last().Split('\\').Last();
                //leggo la configurazione eventualmente già salvata
                List<ConfigData> listaConfigurazioni = ConfigData.Deserialize(nomeLega);

                List<ElabResult> elabResultList = Enhancer.enhance(textFileFCM.Text, checkBoxRicalcola.Checked, progressBarAvanzamento, listaConfigurazioni);
                label1.Visible = true;
                dataGridView1.Visible = true;
                dataGridView1.DataSource = elabResultList;
                MessageBox.Show("Elaborazione completata", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Si è verificato un errore: "+ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progressBarAvanzamento.Visible = false;
        }

        /// <summary>
        /// Scatta alla pressione del tasto "Configurazione"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void configuraButtonClick(object sender, EventArgs e)
        {
            //leggo la configurazione eventualmente già salvata
            List<ConfigData> listaConfigurazioni = ConfigData.Deserialize(textFileFCM.Text);
            List<string> listaModificatori = Enhancer.listModificatoriRegistrati(Enhancer.init());
            //rimuovo i config di modificatori che non sono più presenti
            for (int i=listaConfigurazioni.Count-1; i>=0; i--)
            {
                if (!listaModificatori.Contains(listaConfigurazioni[i].nome))
                {
                    listaConfigurazioni.RemoveAt(i);
                }
            }

            List<string> competizioni = new List<string>();
            List<Regole> regole = new List<Regole>();
            //scorro le competizioni estratte dal DAO
            using (FcmDao dao = new FcmDao(textFileFCM.Text))
            {
                foreach (var comp in listaCompDaDAO)
                {
                    //taglio via la parte dell'id
                    string idComp = comp.Split('-')[0].Trim();
                    string nomeCompetizione = comp.Split('-')[1].Trim();
                    Regole reg = dao.getRegoleCompetizione(idComp);
                    competizioni.Add(nomeCompetizione);
                    regole.Add(reg);
                    //scorro i modificatori e controllo se in config esiste una entry per questa coppia competizione/modificatore
                    foreach (var mod in listaModificatori)
                    {
                        ConfigData data = new ConfigData(nomeCompetizione, mod, false, -1);
                        //se non esiste, inserisco una entry di default e traccio che la configurazione è incompleta
                        if (!listaConfigurazioni.Contains(data))
                        {
                            listaConfigurazioni.Add(data);
                        }
                    }
                }
            }

            //apro il popup di configurazione
            Configuratore configuratoreForm = new Configuratore(competizioni, listaConfigurazioni, textFileFCM.Text, ()=>buttonElabora.Enabled=true, regole);
            configuratoreForm.ShowDialog();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dataGridView1.Rows)
            {
                if (!Myrow.Cells[3].Value.Equals(Myrow.Cells[4].Value))
                {
                    Myrow.Cells[4].Style.BackColor = Color.Yellow;
                }
            }
        }
    }
}
