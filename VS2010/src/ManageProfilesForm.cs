using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GNet.Scripting;

namespace GNet
{
    public partial class ManageProfilesForm : Form
    {
        MainForm mainForm;

        public ManageProfilesForm(MainForm mainForm)
        {
            this.mainForm = mainForm;

            InitializeComponent();
        }

        private void ManageProfilesForm_Load(object sender, EventArgs e)
        {
            string groupName;
            ListViewItem lvi;
            
            Dictionary<ScriptLanguage, string> groups = new Dictionary<ScriptLanguage,string>();

            foreach (var profile in ProfileManager.Current.LoadProfiles())
            {
                groupName = profile.Header.Language.ToString();

                if (!groups.ContainsKey(profile.Header.Language))
                {
                    groups[profile.Header.Language] = groupName;
                    lvProfiles.Groups.Add(groupName, groupName + " Profiles");
                }

                lvProfiles.Items.Add(lvi = new ListViewItem
                {
                    Checked = profile.Header.IsEnabled,
                    Name = profile.Header.Name,
                    Text = profile.Header.Name,
                    Group = lvProfiles.Groups[groupName],
                    Tag = profile
                });

                //lv.SubItems.Add(profile.Header.Name);
                lvi.SubItems.Add(profile.Header.Description);
                lvi.SubItems.Add(profile.Header.Executable);

                lvi.UseItemStyleForSubItems = true;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvProfiles.SelectedItems)
            {
                var profile = lvi.Tag as Profile;
                if (profile != null)
                    mainForm.Open(profile.Header.Headerpath);
            }

            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Delete the selected profiles?", "Confirm Delete", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {

                for (int i = lvProfiles.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    var lvi = lvProfiles.Items[lvProfiles.SelectedIndices[i]];
                    var profile = lvi.Tag as Profile;
                    if (profile != null)
                    {
                        File.Delete(profile.Header.Filepath);
                        File.Delete(profile.Header.Headerpath);
                    }

                    lvProfiles.Items.RemoveAt(lvProfiles.SelectedIndices[i]);
                }
            }
        }

        private void lvProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var info = lvProfiles.HitTest(e.X, e.Y);

            var lvi = info.Item;
            if (lvi != null && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                var profile = lvi.Tag as Profile;
                if (profile != null && profile.Header != null)
                {
                    ProfilePropertiesDialog ppd = new ProfilePropertiesDialog(profile.Header);
                    ppd.Text = "Profile properties for " + profile.Header.Name;

                    if (ppd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        profile.Header.Save();
                        ProfileManager.Current.LoadProfiles();
                    }
                }
            }
        }

        private void lvProfiles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var profile = e.Item.Tag as Profile;
            profile.Header.IsEnabled = e.Item.Checked;
            profile.Header.Save();
        }

        private void lvProfiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lvProfiles.SelectedItems.Count > 0)
            {
                btnOpen.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnOpen.Enabled = false;
                btnDelete.Enabled = false;
            }
        }
    }
}
