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
    public partial class ManageProfilesForm : Form
    {
        public ManageProfilesForm()
        {
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
                lvi.SubItems.Add("Desc");//profile.Header.Description);
                lvi.SubItems.Add("Exec");//profile.Header.Executable);

                lvi.UseItemStyleForSubItems = true;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        ListViewItem lviDoubleClicked;
        private void lvProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var info = lvProfiles.HitTest(e.X, e.Y);

            var lvi = info.Item;
            if (lvi != null && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                lvi.Checked = !lvi.Checked;
                var profile = lvi.Tag as Profile;
                if (profile != null && profile.Header != null)
                {
                    ProfilePropertiesDialog ppd = new ProfilePropertiesDialog(profile.Header);
                    ppd.ShowDialog();
                }
            }
        }
    }
}
