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
            ListViewItem lv;
            
            Dictionary<ScriptLanguage, string> groups;

            foreach (var profile in ProfileManager.Current.LoadProfiles())
            {
                lvProfiles.Items.Add(lv = new ListViewItem
                {
                    Checked = profile.Header.IsEnabled,
                    Name = profile.Header.Name,
                    Text = profile.Header.Name
                });


                //lv.SubItems.Add(profile.Header.Name);
                lv.SubItems.Add("Desc");//profile.Header.Description);
                lv.SubItems.Add("Exec");//profile.Header.Executable);

                lv.UseItemStyleForSubItems = true;
            }
        }

        private void cbxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
