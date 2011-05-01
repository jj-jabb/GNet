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
            this.lvProfiles = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chExecutable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblEnabledItemsDesc = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.lvProfiles.Location = new System.Drawing.Point(12, 12);
            this.lvProfiles.Name = "lvProfiles";
            this.lvProfiles.Size = new System.Drawing.Size(808, 379);
            this.lvProfiles.TabIndex = 8;
            this.lvProfiles.UseCompatibleStateImageBehavior = false;
            this.lvProfiles.View = System.Windows.Forms.View.Details;
            this.lvProfiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvProfiles_MouseDoubleClick);
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 223;
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
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(664, 397);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 9;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(745, 397);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblEnabledItemsDesc
            // 
            this.lblEnabledItemsDesc.AutoSize = true;
            this.lblEnabledItemsDesc.Location = new System.Drawing.Point(12, 402);
            this.lblEnabledItemsDesc.Name = "lblEnabledItemsDesc";
            this.lblEnabledItemsDesc.Size = new System.Drawing.Size(147, 13);
            this.lblEnabledItemsDesc.TabIndex = 11;
            this.lblEnabledItemsDesc.Text = "Enabled Profiles are Checked";
            // 
            // ManageProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 432);
            this.Controls.Add(this.lblEnabledItemsDesc);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lvProfiles);
            this.Controls.Add(this.btnOpen);
            this.Name = "ManageProfilesForm";
            this.Text = "Manage Profiles";
            this.Load += new System.EventHandler(this.ManageProfilesForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvProfiles;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader chExecutable;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblEnabledItemsDesc;

    }
}