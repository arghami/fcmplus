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
            this.SuspendLayout();
            // 
            // buttonSfoglia
            // 
            this.buttonSfoglia.Location = new System.Drawing.Point(348, 27);
            this.buttonSfoglia.Name = "buttonSfoglia";
            this.buttonSfoglia.Size = new System.Drawing.Size(65, 23);
            this.buttonSfoglia.TabIndex = 0;
            this.buttonSfoglia.Text = "Sfoglia";
            this.buttonSfoglia.UseVisualStyleBackColor = true;
            this.buttonSfoglia.Click += new System.EventHandler(this.button1_Click);
            this.buttonSfoglia.Paint += new System.Windows.Forms.PaintEventHandler(this.button1_Paint);
            this.buttonSfoglia.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button1_MouseClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "fcm";
            this.openFileDialog.Filter = "File FCM|*.fcm";
            this.openFileDialog.Title = "Seleziona file FCM di lega";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk_1);
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(49, 32);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(51, 13);
            this.lblFile.TabIndex = 1;
            this.lblFile.Text = "File FCM:";
            // 
            // textFileFCM
            // 
            this.textFileFCM.Enabled = false;
            this.textFileFCM.Location = new System.Drawing.Point(112, 29);
            this.textFileFCM.Name = "textFileFCM";
            this.textFileFCM.Size = new System.Drawing.Size(229, 20);
            this.textFileFCM.TabIndex = 2;
            // 
            // buttonElabora
            // 
            this.buttonElabora.Enabled = false;
            this.buttonElabora.Location = new System.Drawing.Point(178, 131);
            this.buttonElabora.Name = "buttonElabora";
            this.buttonElabora.Size = new System.Drawing.Size(92, 23);
            this.buttonElabora.TabIndex = 7;
            this.buttonElabora.Text = "Elabora";
            this.buttonElabora.UseVisualStyleBackColor = true;
            this.buttonElabora.Click += new System.EventHandler(this.buttonElabora_Click);
            // 
            // progressBarAvanzamento
            // 
            this.progressBarAvanzamento.Location = new System.Drawing.Point(30, 131);
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
            this.checkBoxRicalcola.Location = new System.Drawing.Point(134, 98);
            this.checkBoxRicalcola.Name = "checkBoxRicalcola";
            this.checkBoxRicalcola.Size = new System.Drawing.Size(207, 17);
            this.checkBoxRicalcola.TabIndex = 10;
            this.checkBoxRicalcola.Text = "Applica anche alle giornate precedenti";
            this.checkBoxRicalcola.UseVisualStyleBackColor = true;
            this.checkBoxRicalcola.CheckedChanged += new System.EventHandler(this.checkBoxRicalcola_CheckedChanged);
            // 
            // configuraButton
            // 
            this.configuraButton.Enabled = false;
            this.configuraButton.Location = new System.Drawing.Point(134, 64);
            this.configuraButton.Name = "configuraButton";
            this.configuraButton.Size = new System.Drawing.Size(194, 23);
            this.configuraButton.TabIndex = 11;
            this.configuraButton.Text = "Configura Competizioni";
            this.configuraButton.UseVisualStyleBackColor = true;
            this.configuraButton.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // WelcomeUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 171);
            this.Controls.Add(this.configuraButton);
            this.Controls.Add(this.checkBoxRicalcola);
            this.Controls.Add(this.progressBarAvanzamento);
            this.Controls.Add(this.buttonElabora);
            this.Controls.Add(this.textFileFCM);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.buttonSfoglia);
            this.Name = "WelcomeUI";
            this.Text = "FCM Plus";
            this.Load += new System.EventHandler(this.Form1_Load);
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
    }
}

