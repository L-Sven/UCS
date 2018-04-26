namespace UcsGUI
{
    partial class radBtnHämtaAllData
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
            this.btnVäljFöretag = new System.Windows.Forms.Button();
            this.btnAvsluta = new System.Windows.Forms.Button();
            this.btnKörProgrammet = new System.Windows.Forms.Button();
            this.txtBoxInfo = new System.Windows.Forms.TextBox();
            this.datePickStartDatum = new System.Windows.Forms.DateTimePicker();
            this.datumLabel = new System.Windows.Forms.Label();
            this.btnÖppnaErrorLog = new System.Windows.Forms.Button();
            this.radBtnFrånVissDatum = new System.Windows.Forms.RadioButton();
            this.radBtnAllData = new System.Windows.Forms.RadioButton();
            this.btnStopTask = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnVäljFöretag
            // 
            this.btnVäljFöretag.Location = new System.Drawing.Point(713, 12);
            this.btnVäljFöretag.Name = "btnVäljFöretag";
            this.btnVäljFöretag.Size = new System.Drawing.Size(75, 23);
            this.btnVäljFöretag.TabIndex = 0;
            this.btnVäljFöretag.Text = "Välj Företag";
            this.btnVäljFöretag.UseVisualStyleBackColor = true;
            this.btnVäljFöretag.Click += new System.EventHandler(this.btnVäljFöretag_Click);
            // 
            // btnAvsluta
            // 
            this.btnAvsluta.Location = new System.Drawing.Point(713, 415);
            this.btnAvsluta.Name = "btnAvsluta";
            this.btnAvsluta.Size = new System.Drawing.Size(75, 23);
            this.btnAvsluta.TabIndex = 1;
            this.btnAvsluta.Text = "Avsluta";
            this.btnAvsluta.UseVisualStyleBackColor = true;
            this.btnAvsluta.Click += new System.EventHandler(this.btnAvsluta_Click);
            // 
            // btnKörProgrammet
            // 
            this.btnKörProgrammet.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKörProgrammet.Location = new System.Drawing.Point(357, 186);
            this.btnKörProgrammet.Name = "btnKörProgrammet";
            this.btnKörProgrammet.Size = new System.Drawing.Size(121, 57);
            this.btnKörProgrammet.TabIndex = 2;
            this.btnKörProgrammet.Text = "KÖR!!!!!";
            this.btnKörProgrammet.UseVisualStyleBackColor = true;
            this.btnKörProgrammet.Click += new System.EventHandler(this.btnKörProgrammet_Click);
            // 
            // txtBoxInfo
            // 
            this.txtBoxInfo.Location = new System.Drawing.Point(21, 12);
            this.txtBoxInfo.Multiline = true;
            this.txtBoxInfo.Name = "txtBoxInfo";
            this.txtBoxInfo.ReadOnly = true;
            this.txtBoxInfo.Size = new System.Drawing.Size(206, 426);
            this.txtBoxInfo.TabIndex = 3;
            this.txtBoxInfo.Visible = false;
            // 
            // datePickStartDatum
            // 
            this.datePickStartDatum.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datePickStartDatum.Location = new System.Drawing.Point(653, 237);
            this.datePickStartDatum.Name = "datePickStartDatum";
            this.datePickStartDatum.Size = new System.Drawing.Size(99, 20);
            this.datePickStartDatum.TabIndex = 4;
            this.datePickStartDatum.ValueChanged += new System.EventHandler(this.datePickStartDatum_ValueChanged);
            // 
            // datumLabel
            // 
            this.datumLabel.Location = new System.Drawing.Point(616, 211);
            this.datumLabel.Name = "datumLabel";
            this.datumLabel.Size = new System.Drawing.Size(170, 23);
            this.datumLabel.TabIndex = 0;
            this.datumLabel.Text = "Välj datum för när data ska hämtas";
            // 
            // btnÖppnaErrorLog
            // 
            this.btnÖppnaErrorLog.Location = new System.Drawing.Point(296, 364);
            this.btnÖppnaErrorLog.Name = "btnÖppnaErrorLog";
            this.btnÖppnaErrorLog.Size = new System.Drawing.Size(97, 23);
            this.btnÖppnaErrorLog.TabIndex = 5;
            this.btnÖppnaErrorLog.Text = "Läs Errorloggen";
            this.btnÖppnaErrorLog.UseVisualStyleBackColor = true;
            this.btnÖppnaErrorLog.Click += new System.EventHandler(this.btnÖppnaErrorLog_Click);
            // 
            // radBtnFrånVissDatum
            // 
            this.radBtnFrånVissDatum.AutoSize = true;
            this.radBtnFrånVissDatum.Location = new System.Drawing.Point(619, 169);
            this.radBtnFrånVissDatum.Name = "radBtnFrånVissDatum";
            this.radBtnFrånVissDatum.Size = new System.Drawing.Size(164, 17);
            this.radBtnFrånVissDatum.TabIndex = 6;
            this.radBtnFrånVissDatum.Text = "Hämta data fr.o.m vald datum";
            this.radBtnFrånVissDatum.UseVisualStyleBackColor = true;
            this.radBtnFrånVissDatum.CheckedChanged += new System.EventHandler(this.radBtnFrånVissDatum_CheckedChanged);
            // 
            // radBtnAllData
            // 
            this.radBtnAllData.AutoSize = true;
            this.radBtnAllData.Checked = true;
            this.radBtnAllData.Location = new System.Drawing.Point(619, 146);
            this.radBtnAllData.Name = "radBtnAllData";
            this.radBtnAllData.Size = new System.Drawing.Size(93, 17);
            this.radBtnAllData.TabIndex = 7;
            this.radBtnAllData.TabStop = true;
            this.radBtnAllData.Text = "Hämta all data";
            this.radBtnAllData.UseVisualStyleBackColor = true;
            this.radBtnAllData.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // btnStopTask
            // 
            this.btnStopTask.Enabled = false;
            this.btnStopTask.Location = new System.Drawing.Point(373, 262);
            this.btnStopTask.Name = "btnStopTask";
            this.btnStopTask.Size = new System.Drawing.Size(85, 23);
            this.btnStopTask.TabIndex = 8;
            this.btnStopTask.Text = "Stoppa Kör!!!";
            this.btnStopTask.UseVisualStyleBackColor = true;
            this.btnStopTask.Click += new System.EventHandler(this.btnStopTask_Click);
            // 
            // radBtnHämtaAllData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnStopTask);
            this.Controls.Add(this.radBtnAllData);
            this.Controls.Add(this.radBtnFrånVissDatum);
            this.Controls.Add(this.btnÖppnaErrorLog);
            this.Controls.Add(this.datumLabel);
            this.Controls.Add(this.datePickStartDatum);
            this.Controls.Add(this.txtBoxInfo);
            this.Controls.Add(this.btnKörProgrammet);
            this.Controls.Add(this.btnAvsluta);
            this.Controls.Add(this.btnVäljFöretag);
            this.Name = "radBtnHämtaAllData";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVäljFöretag;
        private System.Windows.Forms.Button btnAvsluta;
        private System.Windows.Forms.Button btnKörProgrammet;
        private System.Windows.Forms.TextBox txtBoxInfo;
        private System.Windows.Forms.DateTimePicker datePickStartDatum;
        private System.Windows.Forms.Label datumLabel;
        private System.Windows.Forms.Button btnÖppnaErrorLog;
        private System.Windows.Forms.RadioButton radBtnFrånVissDatum;
        private System.Windows.Forms.RadioButton radBtnAllData;
        private System.Windows.Forms.Button btnStopTask;
    }
}

