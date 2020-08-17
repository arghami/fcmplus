namespace mainform
{
    partial class WelcomeUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSfoglia = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblFile = new System.Windows.Forms.Label();
            this.textFileFCM = new System.Windows.Forms.TextBox();
            this.buttonElabora = new System.Windows.Forms.Button();
            this.progressBarAvanzamento = new System.Windows.Forms.ProgressBar();
            this.checkBoxRicalcola = new System.Windows.Forms.CheckBox();
            this.configuraButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Competizione = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Incontro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VecchioRisultato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NuovoRisultato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSfoglia
            // 
            this.buttonSfoglia.Location = new System.Drawing.Point(437, 27);
            this.buttonSfoglia.Name = "buttonSfoglia";
            this.buttonSfoglia.Size = new System.Drawing.Size(65, 23);
            this.buttonSfoglia.TabIndex = 0;
            this.buttonSfoglia.Text = "Sfoglia";
            this.buttonSfoglia.UseVisualStyleBackColor = true;
            this.buttonSfoglia.Click += new System.EventHandler(this.buttonSfogliaClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "fcm";
            this.openFileDialog.Filter = "File FCM|*.fcm";
            this.openFileDialog.Title = "Seleziona file FCM di lega";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(138, 32);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(51, 13);
            this.lblFile.TabIndex = 1;
            this.lblFile.Text = "File FCM:";
            // 
            // textFileFCM
            // 
            this.textFileFCM.Enabled = false;
            this.textFileFCM.Location = new System.Drawing.Point(201, 29);
            this.textFileFCM.Name = "textFileFCM";
            this.textFileFCM.Size = new System.Drawing.Size(229, 20);
            this.textFileFCM.TabIndex = 2;
            // 
            // buttonElabora
            // 
            this.buttonElabora.Enabled = false;
            this.buttonElabora.Location = new System.Drawing.Point(267, 131);
            this.buttonElabora.Name = "buttonElabora";
            this.buttonElabora.Size = new System.Drawing.Size(92, 23);
            this.buttonElabora.TabIndex = 7;
            this.buttonElabora.Text = "Elabora";
            this.buttonElabora.UseVisualStyleBackColor = true;
            this.buttonElabora.Click += new System.EventHandler(this.buttonElabora_Click);
            // 
            // progressBarAvanzamento
            // 
            this.progressBarAvanzamento.Location = new System.Drawing.Point(119, 131);
            this.progressBarAvanzamento.Name = "progressBarAvanzamento";
            this.progressBarAvanzamento.Size = new System.Drawing.Size(383, 23);
            this.progressBarAvanzamento.TabIndex = 8;
            this.progressBarAvanzamento.Visible = false;
            // 
            // checkBoxRicalcola
            // 
            this.checkBoxRicalcola.AccessibleDescription = "";
            this.checkBoxRicalcola.AccessibleName = "";
            this.checkBoxRicalcola.AutoSize = true;
            this.checkBoxRicalcola.Location = new System.Drawing.Point(223, 98);
            this.checkBoxRicalcola.Name = "checkBoxRicalcola";
            this.checkBoxRicalcola.Size = new System.Drawing.Size(207, 17);
            this.checkBoxRicalcola.TabIndex = 10;
            this.checkBoxRicalcola.Text = "Applica anche alle giornate precedenti";
            this.checkBoxRicalcola.UseVisualStyleBackColor = true;
            // 
            // configuraButton
            // 
            this.configuraButton.Enabled = false;
            this.configuraButton.Location = new System.Drawing.Point(223, 64);
            this.configuraButton.Name = "configuraButton";
            this.configuraButton.Size = new System.Drawing.Size(194, 23);
            this.configuraButton.TabIndex = 11;
            this.configuraButton.Text = "Configura Competizioni";
            this.configuraButton.UseVisualStyleBackColor = true;
            this.configuraButton.Click += new System.EventHandler(this.configuraButtonClick);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Competizione,
            this.Gio,
            this.Incontro,
            this.VecchioRisultato,
            this.NuovoRisultato});
            this.dataGridView1.Location = new System.Drawing.Point(12, 202);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(622, 132);
            this.dataGridView1.TabIndex = 12;
            this.dataGridView1.Visible = false;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            // 
            // Competizione
            // 
            this.Competizione.DataPropertyName = "competizione";
            this.Competizione.HeaderText = "Competizione";
            this.Competizione.Name = "Competizione";
            this.Competizione.ReadOnly = true;
            this.Competizione.Width = 120;
            // 
            // Gio
            // 
            this.Gio.DataPropertyName = "giornata";
            this.Gio.HeaderText = "Gio";
            this.Gio.Name = "Gio";
            this.Gio.ReadOnly = true;
            this.Gio.Width = 30;
            // 
            // Incontro
            // 
            this.Incontro.DataPropertyName = "incontro";
            this.Incontro.HeaderText = "Incontro";
            this.Incontro.Name = "Incontro";
            this.Incontro.ReadOnly = true;
            this.Incontro.Width = 150;
            // 
            // VecchioRisultato
            // 
            this.VecchioRisultato.DataPropertyName = "vecchioRisultato";
            this.VecchioRisultato.HeaderText = "Vecchio Risultato";
            this.VecchioRisultato.Name = "VecchioRisultato";
            this.VecchioRisultato.ReadOnly = true;
            this.VecchioRisultato.Width = 150;
            // 
            // NuovoRisultato
            // 
            this.NuovoRisultato.DataPropertyName = "nuovoRisultato";
            this.NuovoRisultato.HeaderText = "Nuovo Risultato";
            this.NuovoRisultato.Name = "NuovoRisultato";
            this.NuovoRisultato.ReadOnly = true;
            this.NuovoRisultato.Width = 150;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Esito elaborazione (i risultati modificati sono evidenziati in giallo)";
            this.label1.Visible = false;
            // 
            // WelcomeUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 346);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.configuraButton);
            this.Controls.Add(this.checkBoxRicalcola);
            this.Controls.Add(this.progressBarAvanzamento);
            this.Controls.Add(this.buttonElabora);
            this.Controls.Add(this.textFileFCM);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.buttonSfoglia);
            this.Name = "WelcomeUI";
            this.Text = "FCM Plus";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSfoglia;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox textFileFCM;
        private System.Windows.Forms.Button buttonElabora;
        private System.Windows.Forms.ProgressBar progressBarAvanzamento;
        private System.Windows.Forms.CheckBox checkBoxRicalcola;
        private System.Windows.Forms.Button configuraButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Competizione;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Incontro;
        private System.Windows.Forms.DataGridViewTextBoxColumn VecchioRisultato;
        private System.Windows.Forms.DataGridViewTextBoxColumn NuovoRisultato;
        private System.Windows.Forms.Label label1;
    }
}

