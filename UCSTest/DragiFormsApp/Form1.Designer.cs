namespace DragiFormsApp
{
    partial class Form1
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
            this.listViewExistingNames = new System.Windows.Forms.ListView();
            this.listViewNamesToChange = new System.Windows.Forms.ListView();
            this.btnAddToChangeList = new System.Windows.Forms.Button();
            this.BtnRemoveFromChangeList = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.DragiNamnByte = new System.Windows.Forms.Label();
            this.txtBoxNewName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAcceptChanges = new System.Windows.Forms.Button();
            this.radioBtnUseExistingUserName = new System.Windows.Forms.RadioButton();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // listViewExistingNames
            // 
            this.listViewExistingNames.Location = new System.Drawing.Point(60, 182);
            this.listViewExistingNames.Name = "listViewExistingNames";
            this.listViewExistingNames.Size = new System.Drawing.Size(172, 245);
            this.listViewExistingNames.TabIndex = 0;
            this.listViewExistingNames.UseCompatibleStateImageBehavior = false;
            // 
            // listViewNamesToChange
            // 
            this.listViewNamesToChange.Location = new System.Drawing.Point(522, 182);
            this.listViewNamesToChange.Name = "listViewNamesToChange";
            this.listViewNamesToChange.Size = new System.Drawing.Size(172, 245);
            this.listViewNamesToChange.TabIndex = 1;
            this.listViewNamesToChange.UseCompatibleStateImageBehavior = false;
            // 
            // btnAddToChangeList
            // 
            this.btnAddToChangeList.Location = new System.Drawing.Point(334, 275);
            this.btnAddToChangeList.Name = "btnAddToChangeList";
            this.btnAddToChangeList.Size = new System.Drawing.Size(75, 36);
            this.btnAddToChangeList.TabIndex = 3;
            this.btnAddToChangeList.Text = "Lägg till namn";
            this.btnAddToChangeList.UseVisualStyleBackColor = true;
            this.btnAddToChangeList.Click += new System.EventHandler(this.btnAddToChangeList_Click);
            // 
            // BtnRemoveFromChangeList
            // 
            this.BtnRemoveFromChangeList.Location = new System.Drawing.Point(334, 317);
            this.BtnRemoveFromChangeList.Name = "BtnRemoveFromChangeList";
            this.BtnRemoveFromChangeList.Size = new System.Drawing.Size(75, 39);
            this.BtnRemoveFromChangeList.TabIndex = 4;
            this.BtnRemoveFromChangeList.Text = "Ta bort från byteslistan";
            this.BtnRemoveFromChangeList.UseVisualStyleBackColor = true;
            this.BtnRemoveFromChangeList.Click += new System.EventHandler(this.BtnRemoveFromChangeList_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(619, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Avsluta";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // DragiNamnByte
            // 
            this.DragiNamnByte.AutoSize = true;
            this.DragiNamnByte.Location = new System.Drawing.Point(306, 22);
            this.DragiNamnByte.Name = "DragiNamnByte";
            this.DragiNamnByte.Size = new System.Drawing.Size(127, 13);
            this.DragiNamnByte.TabIndex = 6;
            this.DragiNamnByte.Text = "Avtal referens namn byte!";
            // 
            // txtBoxNewName
            // 
            this.txtBoxNewName.Location = new System.Drawing.Point(285, 136);
            this.txtBoxNewName.Name = "txtBoxNewName";
            this.txtBoxNewName.Size = new System.Drawing.Size(193, 20);
            this.txtBoxNewName.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(322, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Ange det nya namnet";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Namn som finns på avtal";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(536, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Namn som ska bytas ut på avtal";
            // 
            // btnAcceptChanges
            // 
            this.btnAcceptChanges.Location = new System.Drawing.Point(334, 195);
            this.btnAcceptChanges.Name = "btnAcceptChanges";
            this.btnAcceptChanges.Size = new System.Drawing.Size(75, 38);
            this.btnAcceptChanges.TabIndex = 2;
            this.btnAcceptChanges.Text = "Genomför ändring!";
            this.btnAcceptChanges.UseVisualStyleBackColor = true;
            this.btnAcceptChanges.Click += new System.EventHandler(this.btnAcceptChanges_Click);
            // 
            // radioBtnUseExistingUserName
            // 
            this.radioBtnUseExistingUserName.AutoSize = true;
            this.radioBtnUseExistingUserName.Location = new System.Drawing.Point(265, 384);
            this.radioBtnUseExistingUserName.Name = "radioBtnUseExistingUserName";
            this.radioBtnUseExistingUserName.Size = new System.Drawing.Size(192, 17);
            this.radioBtnUseExistingUserName.TabIndex = 11;
            this.radioBtnUseExistingUserName.TabStop = true;
            this.radioBtnUseExistingUserName.Text = "Använd existerande anställds namn";
            this.radioBtnUseExistingUserName.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(265, 239);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(217, 30);
            this.progressBar.TabIndex = 12;
            this.progressBar.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.radioBtnUseExistingUserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoxNewName);
            this.Controls.Add(this.DragiNamnByte);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.BtnRemoveFromChangeList);
            this.Controls.Add(this.btnAddToChangeList);
            this.Controls.Add(this.btnAcceptChanges);
            this.Controls.Add(this.listViewNamesToChange);
            this.Controls.Add(this.listViewExistingNames);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewExistingNames;
        private System.Windows.Forms.ListView listViewNamesToChange;
        private System.Windows.Forms.Button btnAddToChangeList;
        private System.Windows.Forms.Button BtnRemoveFromChangeList;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label DragiNamnByte;
        private System.Windows.Forms.TextBox txtBoxNewName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAcceptChanges;
        private System.Windows.Forms.RadioButton radioBtnUseExistingUserName;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

