namespace GNet
{
    partial class ProfilePropertiesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilePropertiesDialog));
            this.lblName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tbxName = new System.Windows.Forms.TextBox();
            this.tbxDescription = new System.Windows.Forms.TextBox();
            this.cbxLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.gbxExecutables = new System.Windows.Forms.GroupBox();
            this.tbxExecutable = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExecBrowse = new System.Windows.Forms.Button();
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
            this.cbxLanguage.Enabled = false;
            this.cbxLanguage.FormattingEnabled = true;
            this.cbxLanguage.Items.AddRange(new object[] {
            "Lua",
            "Boo"});
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
            this.gbxExecutables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxExecutables.Controls.Add(this.tbxExecutable);
            this.gbxExecutables.Controls.Add(this.btnClear);
            this.gbxExecutables.Controls.Add(this.btnSelect);
            this.gbxExecutables.Controls.Add(this.btnExecBrowse);
            this.gbxExecutables.Location = new System.Drawing.Point(12, 117);
            this.gbxExecutables.Name = "gbxExecutables";
            this.gbxExecutables.Size = new System.Drawing.Size(514, 77);
            this.gbxExecutables.TabIndex = 6;
            this.gbxExecutables.TabStop = false;
            this.gbxExecutables.Text = "Associated Executable";
            // 
            // tbxExecutable
            // 
            this.tbxExecutable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxExecutable.Location = new System.Drawing.Point(6, 19);
            this.tbxExecutable.Name = "tbxExecutable";
            this.tbxExecutable.Size = new System.Drawing.Size(502, 20);
            this.tbxExecutable.TabIndex = 20;
            this.tbxExecutable.TextChanged += new System.EventHandler(this.tbxExecutable_TextChanged);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(168, 45);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(87, 45);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 11;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExecBrowse
            // 
            this.btnExecBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecBrowse.Location = new System.Drawing.Point(6, 45);
            this.btnExecBrowse.Name = "btnExecBrowse";
            this.btnExecBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnExecBrowse.TabIndex = 10;
            this.btnExecBrowse.Text = "Browse";
            this.btnExecBrowse.UseVisualStyleBackColor = true;
            this.btnExecBrowse.Click += new System.EventHandler(this.btnExecBrowse_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(289, 355);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(370, 355);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(451, 355);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 9;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // chkLock
            // 
            this.chkLock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLock.AutoSize = true;
            this.chkLock.Location = new System.Drawing.Point(15, 228);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(229, 17);
            this.chkLock.TabIndex = 10;
            this.chkLock.Text = "Lock profile while application(s) are running";
            this.chkLock.UseVisualStyleBackColor = true;
            this.chkLock.Visible = false;
            // 
            // chkCopyExisting
            // 
            this.chkCopyExisting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCopyExisting.AutoSize = true;
            this.chkCopyExisting.Location = new System.Drawing.Point(15, 251);
            this.chkCopyExisting.Name = "chkCopyExisting";
            this.chkCopyExisting.Size = new System.Drawing.Size(157, 17);
            this.chkCopyExisting.TabIndex = 11;
            this.chkCopyExisting.Text = "Copy from an existing profile";
            this.chkCopyExisting.UseVisualStyleBackColor = true;
            this.chkCopyExisting.CheckedChanged += new System.EventHandler(this.chkCopyExisting_CheckedChanged);
            // 
            // cbxCopyExisting
            // 
            this.cbxCopyExisting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxCopyExisting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCopyExisting.Enabled = false;
            this.cbxCopyExisting.FormattingEnabled = true;
            this.cbxCopyExisting.Items.AddRange(new object[] {
            "Default Configuration"});
            this.cbxCopyExisting.Location = new System.Drawing.Point(31, 274);
            this.cbxCopyExisting.Name = "cbxCopyExisting";
            this.cbxCopyExisting.Size = new System.Drawing.Size(495, 21);
            this.cbxCopyExisting.TabIndex = 12;
            // 
            // ofdBrowse
            // 
            this.ofdBrowse.Filter = "Executables|*.exe|All files|*.*";
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
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Keyboard Hook";
            // 
            // cbxKeyboardHook
            // 
            this.cbxKeyboardHook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxKeyboardHook.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxKeyboardHook.FormattingEnabled = true;
            this.cbxKeyboardHook.Items.AddRange(new object[] {
            "None",
            "Ignore Injected Events",
            "All"});
            this.cbxKeyboardHook.Location = new System.Drawing.Point(99, 301);
            this.cbxKeyboardHook.Name = "cbxKeyboardHook";
            this.cbxKeyboardHook.Size = new System.Drawing.Size(427, 21);
            this.cbxKeyboardHook.TabIndex = 16;
            // 
            // cbxMouseHook
            // 
            this.cbxMouseHook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxMouseHook.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMouseHook.FormattingEnabled = true;
            this.cbxMouseHook.Items.AddRange(new object[] {
            "None",
            "Ignore Injected Events",
            "All"});
            this.cbxMouseHook.Location = new System.Drawing.Point(99, 328);
            this.cbxMouseHook.Name = "cbxMouseHook";
            this.cbxMouseHook.Size = new System.Drawing.Size(427, 21);
            this.cbxMouseHook.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 331);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Mouse Hook";
            // 
            // cbxIsEnabled
            // 
            this.cbxIsEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxIsEnabled.AutoSize = true;
            this.cbxIsEnabled.Location = new System.Drawing.Point(15, 205);
            this.cbxIsEnabled.Name = "cbxIsEnabled";
            this.cbxIsEnabled.Size = new System.Drawing.Size(65, 17);
            this.cbxIsEnabled.TabIndex = 19;
            this.cbxIsEnabled.Text = "Enabled";
            this.cbxIsEnabled.UseVisualStyleBackColor = true;
            // 
            // ProfilePropertiesDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(530, 379);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(546, 417);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(546, 417);
            this.Name = "ProfilePropertiesDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "New Profile";
            this.Load += new System.EventHandler(this.NewProfileDialog_Load);
            this.gbxExecutables.ResumeLayout(false);
            this.gbxExecutables.PerformLayout();
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
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.CheckBox chkCopyExisting;
        private System.Windows.Forms.ComboBox cbxCopyExisting;
        private System.Windows.Forms.OpenFileDialog ofdBrowse;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.ComboBox cbxDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxKeyboardHook;
        private System.Windows.Forms.ComboBox cbxMouseHook;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbxIsEnabled;
        private System.Windows.Forms.TextBox tbxExecutable;
    }
}