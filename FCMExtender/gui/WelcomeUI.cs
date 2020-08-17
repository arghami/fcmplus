using fcm.dao;
using FCMExtender.gui;
using plus.enhancer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mainform
{
    public partial class WelcomeUI : Form
    {
        private string[] listaCompDaDAO;
        private Boolean configIncompleta;

        public WelcomeUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Scatta al click sulla selezione della lega. Apre il filechooser e gestisce l'OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                //ho selezionato il file. Copio il nome della lega nella label...
                textFileFCM.Text = openFileDialog.FileName;
                //disabilito il bottone di Elabora...
                buttonElabora.Enabled = false;
                //leggo e conservo la lista delle competizioni. Se non ne trovo lancio un alert.
                using (FcmDao dao = new FcmDao(textFileFCM.Text))
                {
                    listaCompDaDAO = dao.getListaCompetizioni();
                }
                if (listaCompDaDAO.Count() > 0)
                {
                    configuraButton.Enabled = true;
                }
                else
                {
                    configuraButton.Enabled = false;
                    MessageBox.Show("Non sono presenti competizioni in questa lega.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {

        }

        private void buttonElabora_Click(object sender, EventArgs e)
        {
            try
            {
                progressBarAvanzamento.Visible = true;
                Enhancer.enhance(textFileFCM.Text, checkBoxRicalcola.Checked, progressBarAvanzamento);
                MessageBox.Show("Elaborazione completata", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Si è verificato un errore: "+ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progressBarAvanzamento.Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxRicalcola_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Scatta alla pressione del tasto "Configurazione"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            string nomeLega = textFileFCM.Text.Split('/').Last().Split('\\').Last();
            //leggo la configurazione eventualmente già salvata
            List<ConfigData> listaConfigurazioni = ConfigData.Deserialize(nomeLega);
            //mock
            List<string> listaModificatori = new List<string>() { "Mod gazza", "Mod capitano" };
            //rimuovo i config di modificatori che non sono più presenti
            for (int i=listaConfigurazioni.Count-1; i>=0; i--)
            {
                if (!listaModificatori.Contains(listaConfigurazioni[i].nome))
                {
                    listaConfigurazioni.RemoveAt(i);
                }
            }

            //reinizializzo il flag configIncompleta
            configIncompleta = false;

            List<string> competizioni = new List<string>();
            //scorro le competizioni estratte dal DAO
            foreach (var comp in listaCompDaDAO)
            {
                //taglio via la parte dell'id
                string compPulita = comp.Split('-')[1].Trim();
                competizioni.Add(compPulita);
                //scorro i modificatori e controllo se in config esiste una entry per questa coppia competizione/modificatore
                foreach (var mod in listaModificatori)
                {
                    ConfigData data = new ConfigData(compPulita, mod, false, null);
                    //se non esiste, inserisco una entry di default e traccio che la configurazione è incompleta
                    if (!listaConfigurazioni.Contains(data))
                    {
                        listaConfigurazioni.Add(data);
                        configIncompleta = true;
                    }
                }
            }

            //apro il popup di configurazione
            Configuratore configuratoreForm = new Configuratore(competizioni, listaConfigurazioni, nomeLega, ()=>buttonElabora.Enabled=true);
            configuratoreForm.ShowDialog();
        }
    }
}
