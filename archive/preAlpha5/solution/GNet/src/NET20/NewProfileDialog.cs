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
    public partial class NewProfileDialog : Form
    {
        public NewProfileDialog()
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;
            Executables = new List<string>();
        }

        private void NewProfileDialog_Load(object sender, EventArgs e)
        {
            if (cbxLanguage.Items.Count <= 1)
                cbxLanguage.Enabled = false;

            if (cbxDevice.Items.Count <= 1)
                cbxDevice.Enabled = false;

            if (readFromProfile)
                return;

            cbxLanguage.SelectedIndex = 0;

            cbxCopyExisting.DataSource = GetCopyableProfiles();
            cbxCopyExisting.DisplayMember = "Name";

            cbxDevice.SelectedIndex = 0;

            cbxCopyExisting.SelectedIndex = 0;
            cbxKeyboardHook.SelectedIndex = 0;
            cbxMouseHook.SelectedIndex = 0;
        }

        private IList<Profile> GetCopyableProfiles()
        {
            var profiles = new List<Profile>();
            foreach (var profile in ProfileManager.Current.profiles)
                if (profile.Language.ToString() == cbxLanguage.SelectedItem.ToString())
                    profiles.Add(profile);

            return profiles;
        }

        public string ScriptName { get; private set; }
        public string Description { get; private set; }
        public string Language { get; private set; }
        public string Device { get; private set; }
        public List<string> Executables { get; private set; }
        public bool LockForExecutables { get; private set; }
        public string CopyFrom { get; private set; }
        public HookOptions KeyboardHook { get; private set; }
        public HookOptions MouseHook { get; private set; }
        public bool IsEnabled { get; private set; }

        public bool OkClicked { get; private set; }

        private void lbxExecs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxExecs.SelectedItem != null)
                btnRemove.Enabled = true;
            else
                btnRemove.Enabled = false;
        }

        private void btnExecBrowse_Click(object sender, EventArgs e)
        {
            var result = ofdBrowse.ShowDialog();

            if (result == DialogResult.OK)
                foreach (var exe in ofdBrowse.FileNames)
                    lbxExecs.Items.Add(exe);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Stack<int> indices = new Stack<int>(lbxExecs.SelectedIndices.Count);

            foreach (int index in lbxExecs.SelectedIndices)
                indices.Push(index);

            while (indices.Count > 0)
                lbxExecs.Items.RemoveAt(indices.Pop());
        }

        bool readFromProfile;
        public void ReadFromProfile(Profile profile)
        {
            readFromProfile = true;
            tbxName.Text = profile.Name;
            tbxDescription.Text = profile.Description;
            cbxLanguage.SelectedItem = profile.Language.ToString();
            cbxDevice.SelectedItem = profile.Device.ToString();
            chkLock.Checked = profile.Lock;

            foreach (var item in profile.Executables)
                Executables.Add(item);

            cbxKeyboardHook.SelectedItem = HookOptionsExtensions.DisplayValue(profile.KeyboardHook);
            cbxMouseHook.SelectedItem = HookOptionsExtensions.DisplayValue(profile.MouseHook);

            cbxIsEnabled.Checked = profile.IsEnabled;

            chkCopyExisting.Visible = false;
            cbxCopyExisting.Visible = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            ScriptName = tbxName.Text;
            Description = tbxDescription.Text;
            Language = cbxLanguage.SelectedItem.ToString();
            Device = cbxDevice.SelectedItem.ToString();
            LockForExecutables = chkLock.Checked;

            if (chkCopyExisting.Checked)
                CopyFrom = cbxCopyExisting.SelectedItem.ToString();

            foreach (var item in lbxExecs.Items)
                Executables.Add(item.ToString());

            KeyboardHook = (HookOptions)cbxKeyboardHook.SelectedIndex;
            MouseHook = (HookOptions)cbxMouseHook.SelectedIndex;

            IsEnabled = cbxIsEnabled.Checked;

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
