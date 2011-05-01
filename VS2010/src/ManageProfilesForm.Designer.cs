namespace GNet
{
    partial class ManageProfilesForm
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
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cbxLanguage = new System.Windows.Forms.ComboBox();
            this.lvProfiles = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chExecutable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.gbxProfiles = new System.Windows.Forms.GroupBox();
            this.gbxProfiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(6, 22);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(94, 13);
            this.lblLanguage.TabIndex = 7;
            this.lblLanguage.Text = "Filter by Language";
            // 
            // cbxLanguage
            // 
            this.cbxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLanguage.FormattingEnabled = true;
            this.cbxLanguage.Items.AddRange(new object[] {
            "Lua"});
            this.cbxLanguage.Location = new System.Drawing.Point(106, 19);
            this.cbxLanguage.Name = "cbxLanguage";
            this.cbxLanguage.Size = new System.Drawing.Size(708, 21);
            this.cbxLanguage.TabIndex = 6;
            this.cbxLanguage.SelectedIndexChanged += new System.EventHandler(this.cbxLanguage_SelectedIndexChanged);
            // 
            // lvProfiles
            // 
            this.lvProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProfiles.CheckBoxes = true;
            this.lvProfiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chDescription,
            this.chExecutable});
            this.lvProfiles.FullRowSelect = true;
            this.lvProfiles.HideSelection = false;
            this.lvProfiles.Location = new System.Drawing.Point(9, 46);
            this.lvProfiles.Name = "lvProfiles";
            this.lvProfiles.Size = new System.Drawing.Size(805, 284);
            this.lvProfiles.TabIndex = 8;
            this.lvProfiles.UseCompatibleStateImageBehavior = false;
            this.lvProfiles.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 165;
            // 
            // chDescription
            // 
            this.chDescription.Text = "Description";
            this.chDescription.Width = 281;
            // 
            // chExecutable
            // 
            this.chExecutable.Text = "Executable";
            this.chExecutable.Width = 290;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpen.Location = new System.Drawing.Point(9, 336);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 9;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(90, 336);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Default Profile";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Lua"});
            this.comboBox1.Location = new System.Drawing.Point(91, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(741, 21);
            this.comboBox1.TabIndex = 11;
            // 
            // gbxProfiles
            // 
            this.gbxProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProfiles.Controls.Add(this.lvProfiles);
            this.gbxProfiles.Controls.Add(this.lblLanguage);
            this.gbxProfiles.Controls.Add(this.cbxLanguage);
            this.gbxProfiles.Controls.Add(this.btnDelete);
            this.gbxProfiles.Controls.Add(this.btnOpen);
            this.gbxProfiles.Location = new System.Drawing.Point(12, 33);
            this.gbxProfiles.Name = "gbxProfiles";
            this.gbxProfiles.Size = new System.Drawing.Size(820, 365);
            this.gbxProfiles.TabIndex = 13;
            this.gbxProfiles.TabStop = false;
            this.gbxProfiles.Text = "Profiles";
            // 
            // ManageProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 410);
            this.Controls.Add(this.gbxProfiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Name = "ManageProfilesForm";
            this.Text = "Manage Profiles";
            this.Load += new System.EventHandler(this.ManageProfilesForm_Load);
            this.gbxProfiles.ResumeLayout(false);
            this.gbxProfiles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox cbxLanguage;
        private System.Windows.Forms.ListView lvProfiles;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader chExecutable;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox gbxProfiles;

    }
}