namespace GNet
{
    partial class NewProfileDialog
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
            this.lblName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tbxName = new System.Windows.Forms.TextBox();
            this.tbxDescription = new System.Windows.Forms.TextBox();
            this.cbxLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.gbxExecutables = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExecBrowse = new System.Windows.Forms.Button();
            this.lbxExecs = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.chkCopyExisting = new System.Windows.Forms.CheckBox();
            this.cbxCopyExisting = new System.Windows.Forms.ComboBox();
            this.ofdBrowse = new System.Windows.Forms.OpenFileDialog();
            this.lblDevice = new System.Windows.Forms.Label();
            this.cbxDevice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxKeyboardHook = new System.Windows.Forms.ComboBox();
            this.cbxMouseHook = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxIsEnabled = new System.Windows.Forms.CheckBox();
            this.gbxExecutables.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 39);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description";
            // 
            // tbxName
            // 
            this.tbxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxName.Location = new System.Drawing.Point(78, 10);
            this.tbxName.Name = "tbxName";
            this.tbxName.Size = new System.Drawing.Size(448, 20);
            this.tbxName.TabIndex = 2;
            this.tbxName.Text = "Untitled";
            // 
            // tbxDescription
            // 
            this.tbxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxDescription.Location = new System.Drawing.Point(78, 36);
            this.tbxDescription.Name = "tbxDescription";
            this.tbxDescription.Size = new System.Drawing.Size(448, 20);
            this.tbxDescription.TabIndex = 3;
            // 
            // cbxLanguage
            // 
            this.cbxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLanguage.FormattingEnabled = true;
            this.cbxLanguage.Items.AddRange(new object[] {
            "Lua"});
            this.cbxLanguage.Location = new System.Drawing.Point(78, 63);
            this.cbxLanguage.Name = "cbxLanguage";
            this.cbxLanguage.Size = new System.Drawing.Size(448, 21);
            this.cbxLanguage.TabIndex = 4;
            this.cbxLanguage.SelectedIndexChanged += new System.EventHandler(this.cbxLanguage_SelectedIndexChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(12, 66);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(55, 13);
            this.lblLanguage.TabIndex = 5;
            this.lblLanguage.Text = "Language";
            // 
            // gbxExecutables
            // 
            this.gbxExecutables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxExecutables.Controls.Add(this.btnRemove);
            this.gbxExecutables.Controls.Add(this.btnSelect);
            this.gbxExecutables.Controls.Add(this.btnExecBrowse);
            this.gbxExecutables.Controls.Add(this.lbxExecs);
            this.gbxExecutables.Location = new System.Drawing.Point(12, 117);
            this.gbxExecutables.Name = "gbxExecutables";
            this.gbxExecutables.Size = new System.Drawing.Size(514, 212);
            this.gbxExecutables.TabIndex = 6;
            this.gbxExecutables.TabStop = false;
            this.gbxExecutables.Text = "Associated Executables";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(168, 183);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 12;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelect.Location = new System.Drawing.Point(87, 183);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 11;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExecBrowse
            // 
            this.btnExecBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExecBrowse.Location = new System.Drawing.Point(6, 183);
            this.btnExecBrowse.Name = "btnExecBrowse";
            this.btnExecBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnExecBrowse.TabIndex = 10;
            this.btnExecBrowse.Text = "Browse";
            this.btnExecBrowse.UseVisualStyleBackColor = true;
            this.btnExecBrowse.Click += new System.EventHandler(this.btnExecBrowse_Click);
            // 
            // lbxExecs
            // 
            this.lbxExecs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxExecs.FormattingEnabled = true;
            this.lbxExecs.Location = new System.Drawing.Point(6, 20);
            this.lbxExecs.Name = "lbxExecs";
            this.lbxExecs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxExecs.Size = new System.Drawing.Size(502, 147);
            this.lbxExecs.TabIndex = 0;
            this.lbxExecs.SelectedIndexChanged += new System.EventHandler(this.lbxExecs_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(289, 485);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(370, 485);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(451, 485);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 9;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // chkLock
            // 
            this.chkLock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLock.AutoSize = true;
            this.chkLock.Location = new System.Drawing.Point(15, 358);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(229, 17);
            this.chkLock.TabIndex = 10;
            this.chkLock.Text = "Lock profile while application(s) are running";
            this.chkLock.UseVisualStyleBackColor = true;
            this.chkLock.Visible = false;
            // 
            // chkCopyExisting
            // 
            this.chkCopyExisting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCopyExisting.AutoSize = true;
            this.chkCopyExisting.Location = new System.Drawing.Point(15, 381);
            this.chkCopyExisting.Name = "chkCopyExisting";
            this.chkCopyExisting.Size = new System.Drawing.Size(157, 17);
            this.chkCopyExisting.TabIndex = 11;
            this.chkCopyExisting.Text = "Copy from an existing profile";
            this.chkCopyExisting.UseVisualStyleBackColor = true;
            this.chkCopyExisting.CheckedChanged += new System.EventHandler(this.chkCopyExisting_CheckedChanged);
            // 
            // cbxCopyExisting
            // 
            this.cbxCopyExisting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxCopyExisting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCopyExisting.Enabled = false;
            this.cbxCopyExisting.FormattingEnabled = true;
            this.cbxCopyExisting.Items.AddRange(new object[] {
            "Default Configuration"});
            this.cbxCopyExisting.Location = new System.Drawing.Point(31, 404);
            this.cbxCopyExisting.Name = "cbxCopyExisting";
            this.cbxCopyExisting.Size = new System.Drawing.Size(495, 21);
            this.cbxCopyExisting.TabIndex = 12;
            // 
            // ofdBrowse
            // 
            this.ofdBrowse.Filter = "Executables|*.exe|All files|*.*";
            this.ofdBrowse.Multiselect = true;
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(12, 93);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(41, 13);
            this.lblDevice.TabIndex = 14;
            this.lblDevice.Text = "Device";
            // 
            // cbxDevice
            // 
            this.cbxDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDevice.FormattingEnabled = true;
            this.cbxDevice.Items.AddRange(new object[] {
            "G13"});
            this.cbxDevice.Location = new System.Drawing.Point(78, 90);
            this.cbxDevice.Name = "cbxDevice";
            this.cbxDevice.Size = new System.Drawing.Size(448, 21);
            this.cbxDevice.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 434);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Keyboard Hook";
            // 
            // cbxKeyboardHook
            // 
            this.cbxKeyboardHook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxKeyboardHook.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxKeyboardHook.FormattingEnabled = true;
            this.cbxKeyboardHook.Items.AddRange(new object[] {
            "None",
            "Ignore Injected Events",
            "All"});
            this.cbxKeyboardHook.Location = new System.Drawing.Point(99, 431);
            this.cbxKeyboardHook.Name = "cbxKeyboardHook";
            this.cbxKeyboardHook.Size = new System.Drawing.Size(427, 21);
            this.cbxKeyboardHook.TabIndex = 16;
            // 
            // cbxMouseHook
            // 
            this.cbxMouseHook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxMouseHook.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMouseHook.FormattingEnabled = true;
            this.cbxMouseHook.Items.AddRange(new object[] {
            "None",
            "Ignore Injected Events",
            "All"});
            this.cbxMouseHook.Location = new System.Drawing.Point(99, 458);
            this.cbxMouseHook.Name = "cbxMouseHook";
            this.cbxMouseHook.Size = new System.Drawing.Size(427, 21);
            this.cbxMouseHook.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 461);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Mouse Hook";
            // 
            // cbxIsEnabled
            // 
            this.cbxIsEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxIsEnabled.AutoSize = true;
            this.cbxIsEnabled.Location = new System.Drawing.Point(15, 335);
            this.cbxIsEnabled.Name = "cbxIsEnabled";
            this.cbxIsEnabled.Size = new System.Drawing.Size(65, 17);
            this.cbxIsEnabled.TabIndex = 19;
            this.cbxIsEnabled.Text = "Enabled";
            this.cbxIsEnabled.UseVisualStyleBackColor = true;
            // 
            // NewProfileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 520);
            this.Controls.Add(this.cbxIsEnabled);
            this.Controls.Add(this.cbxMouseHook);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbxKeyboardHook);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDevice);
            this.Controls.Add(this.cbxDevice);
            this.Controls.Add(this.cbxCopyExisting);
            this.Controls.Add(this.chkCopyExisting);
            this.Controls.Add(this.chkLock);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gbxExecutables);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.cbxLanguage);
            this.Controls.Add(this.tbxDescription);
            this.Controls.Add(this.tbxName);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProfileDialog";
            this.Text = "New Profile";
            this.Load += new System.EventHandler(this.NewProfileDialog_Load);
            this.gbxExecutables.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox tbxName;
        private System.Windows.Forms.TextBox tbxDescription;
        private System.Windows.Forms.ComboBox cbxLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.GroupBox gbxExecutables;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnExecBrowse;
        private System.Windows.Forms.ListBox lbxExecs;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.CheckBox chkCopyExisting;
        private System.Windows.Forms.ComboBox cbxCopyExisting;
        private System.Windows.Forms.OpenFileDialog ofdBrowse;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.ComboBox cbxDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxKeyboardHook;
        private System.Windows.Forms.ComboBox cbxMouseHook;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbxIsEnabled;
    }
}