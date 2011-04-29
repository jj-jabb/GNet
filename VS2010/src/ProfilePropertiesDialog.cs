using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GNet.Scripting;

namespace GNet
{
    public partial class ProfilePropertiesDialog : Form
    {
        bool isNew;
        ProfileHeader header;

        public ProfilePropertiesDialog()
            : this(null)
        {
        }

        public ProfileHeader Header
        {
            get { return header; }
        }

        public ProfilePropertiesDialog(ProfileHeader header)
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;

            if (header == null)
            {
                header = new ProfileHeader();
                isNew = true;
            }

            this.header = header;

            tbxName.Text = header.Name;
            tbxDescription.Text = header.Description;
            cbxLanguage.SelectedItem = header.Language.ToString();
            cbxDevice.SelectedItem = header.Device.ToString();
            chkLock.Checked = header.Lock;

            tbxExecutable.Text = header.Executable;

            cbxKeyboardHook.SelectedItem = HookOptionsExtensions.DisplayValue(header.KeyboardHook);
            cbxMouseHook.SelectedItem = HookOptionsExtensions.DisplayValue(header.MouseHook);

            cbxIsEnabled.Checked = header.IsEnabled;

            if (!isNew)
            {
                chkCopyExisting.Visible = false;
                cbxCopyExisting.Visible = false;
            }
        }

        private void NewProfileDialog_Load(object sender, EventArgs e)
        {
            if (cbxLanguage.Items.Count <= 1)
                cbxLanguage.Enabled = false;

            if (cbxDevice.Items.Count <= 1)
                cbxDevice.Enabled = false;

            if (!isNew)
                return;

            cbxLanguage.SelectedIndex = 0;

            var profiles = GetCopyableProfiles();
            if (profiles.Count > 0)
            {
                cbxCopyExisting.DataSource = profiles;
                cbxCopyExisting.DisplayMember = "Name";
                cbxCopyExisting.SelectedIndex = 0;
            }

            cbxDevice.SelectedIndex = 0;

            cbxKeyboardHook.SelectedIndex = 0;
            cbxMouseHook.SelectedIndex = 0;
        }

        private IList<Profile> GetCopyableProfiles()
        {
            var profiles = new List<Profile>();
            foreach (var profile in ProfileManager.Current.profiles)
                if (profile.Header.Language.ToString() == cbxLanguage.SelectedItem.ToString())
                    profiles.Add(profile);

            return profiles;
        }

        public string CopyFrom { get; private set; }
        public bool OkClicked { get; private set; }

        private void btnExecBrowse_Click(object sender, EventArgs e)
        {
            var result = ofdBrowse.ShowDialog();

            if (result == DialogResult.OK)
                tbxExecutable.Text = ofdBrowse.FileName;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        void ReadHeader()
        {
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            header.Name = tbxName.Text;
            header.Description = tbxDescription.Text;
            header.Language = (ScriptLanguage)Enum.Parse(typeof(ScriptLanguage), cbxLanguage.SelectedItem.ToString());
            header.Device = (DeviceType)Enum.Parse(typeof(DeviceType), cbxDevice.SelectedItem.ToString());
            header.Lock = chkLock.Checked;

            if (chkCopyExisting.Checked)
                CopyFrom = cbxCopyExisting.SelectedItem.ToString();

            header.Executable = tbxExecutable.Text;

            header.KeyboardHook = (HookOptions)cbxKeyboardHook.SelectedIndex;
            header.MouseHook = (HookOptions)cbxMouseHook.SelectedIndex;

            header.IsEnabled = cbxIsEnabled.Checked;

            OkClicked = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OkClicked = false;
            Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
        }

        private void chkCopyExisting_CheckedChanged(object sender, EventArgs e)
        {
            cbxCopyExisting.Enabled = chkCopyExisting.Checked;
        }

        private void cbxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxCopyExisting.DataSource = GetCopyableProfiles();
            cbxCopyExisting.DisplayMember = "Name";
        }
    }
}
