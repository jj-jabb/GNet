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
    public partial class MainForm : Form
    {
        delegate void InvokeAction();

        TextWriter originalout;
        WinFormsTextBoxStreamWriter tbsw;
        bool quit;

        List<Profile> openProfiles;

        public MainForm()
        {
            InitializeComponent();
            
            notifyIcon1.Icon = Properties.Resources.TrayIcon;
            notifyIcon1.ContextMenuStrip = trayMenuStrip;
            //notifyIcon1.Visible = false;

            openProfiles = new List<Profile>();

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            originalout = Console.Out;
            tbsw = new WinFormsTextBoxStreamWriter(output);
            Console.SetOut(tbsw);

            ProfileManager.Current.ScriptStarted += new EventHandler(Current_ScriptStarted);
            ProfileManager.Current.ScriptStopped += new EventHandler(Current_ScriptStopped);
        }

        void Current_ScriptStarted(object sender, EventArgs e)
        {
        }

        void Current_ScriptStopped(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new InvokeAction(ScriptStopped));
            else
                ScriptStopped();
        }

        void ScriptStopped()
        {
            stopToolStripButton.Enabled = false;
            runToolStripButton.Enabled = true;
            documentTabs.Enabled = true;
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quit)
            {
                e.Cancel = true;
                this.Hide();
                //WindowState = FormWindowState.Minimized;
            }
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            Console.SetOut(originalout);

            //foreach (TabPage tab in documentTabs.TabPages)
            //    foreach (Control ctrl in tab.Controls)
            //    {
            //        ScriptEditor editor = ctrl as ScriptEditor;
            //        if (editor != null)
            //            editor.DisposeScript();
            //    }

            ProfileManager.DisposeCurrent();
        }

        private string MakeSafeFilename(string filename)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            char fc;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < filename.Length; i++)
            {
                fc = filename[i];

                for (int j = 0; j < invalidChars.Length; j++)
                    if (fc == invalidChars[j])
                    {
                        sb.Append("%").Append(((byte)fc).ToString("x"));
                        fc = '\0';
                        break;
                    }

                if (fc != '\0')
                    sb.Append(fc);
            }

            return sb.ToString();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Profile newProfile;
            ProfileHeader header;
            ProfilePropertiesDialog d = new ProfilePropertiesDialog();

            var result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                header = d.Header;
                newProfile = new Profile(header);

                var fileName = MakeSafeFilename(header.Name);
                string ext;
                var basePath = ProfileManager.Basepath;

                switch (header.Language)
                {
                    case ScriptLanguage.Lua:
                        ext = ".lua";
                        break;

                    case ScriptLanguage.Boo:
                        ext = ".boo";
                        break;

                    default:
                        ext = ".txt";
                        break;
                }

                if (File.Exists(basePath + fileName + ext))
                {
                    int fileCount = 2;
                    while (File.Exists(basePath + fileName + fileCount + ext))
                        fileCount++;
                    fileName += fileCount;
                    header.Name += fileCount;
                }

                fileName = fileName + ext;

                bool copied = false;
                if (d.CopyFrom != null)
                {
                    var copyFrom = basePath + d.CopyFrom;
                    if (File.Exists(copyFrom))
                    {
                        File.Copy(copyFrom, basePath + fileName);
                        copied = true;
                    }
                }

                if (!copied)
                {
                    var templatePath = basePath + "_template" + Path.GetExtension(fileName);
                    if (File.Exists(templatePath))
                        File.Copy(templatePath, basePath + fileName);
                    else
                        using (var fs = File.CreateText(basePath + fileName)) { };
                }

                header.Headerpath = fileName + ".header";
                header.Filepath = fileName;
                header.Save();

                ProfileManager.Current.LoadProfiles();

                newProfile.Load();

                TabPage tabPage = new TabPage(header.Name);
                ScriptEditor editor = new ScriptEditor(newProfile);
                editor.Dock = DockStyle.Fill;
                tabPage.Controls.Add(editor);
                documentTabs.TabPages.Add(tabPage);
                documentTabs.SelectedTab = tabPage;

                this.saveToolStripMenuItem.Enabled = true;
                this.saveToolStripButton.Enabled = true;

                this.closeToolStripMenuItem.Enabled = true;
                this.closeToolStripButton.Enabled = true;

                this.profilePropertiesToolStripMenuItem.Enabled = true;
                this.profileToolStripButton.Enabled = true;

                this.runToolStripButton.Enabled = true;
            }
        }

        public void Open(string headerPath)
        {
            foreach (var p in openProfiles)
                if (p.Header.Headerpath == headerPath)
                    // already open
                    return;

            var profile = new Profile(headerPath).Load();
            openProfiles.Add(profile);

            TabPage tabPage = new TabPage(profile.Header.Name);
            ScriptEditor editor = new ScriptEditor(profile);
            editor.Dock = DockStyle.Fill;
            tabPage.Controls.Add(editor);
            documentTabs.TabPages.Add(tabPage);
            documentTabs.SelectedTab = tabPage;

            this.saveToolStripMenuItem.Enabled = true;
            this.saveToolStripButton.Enabled = true;

            this.closeToolStripMenuItem.Enabled = true;
            this.closeToolStripButton.Enabled = true;

            this.profilePropertiesToolStripMenuItem.Enabled = true;
            this.profileToolStripButton.Enabled = true;

            this.runToolStripButton.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editor = CurrentEditor;
            if (editor != null && editor.Profile != null)
            {
                editor.Profile.Contents = editor.Editor.Text;
                editor.Profile.Save();
            }
        }

        ScriptEditor CurrentEditor
        {
            get
            {
                if (documentTabs.SelectedTab == null)
                    return null;

                TabPage tabPage = documentTabs.SelectedTab;
                if (tabPage == null)
                    return null;

                ScriptEditor editor = null;
                foreach (var control in tabPage.Controls)
                {
                    editor = control as ScriptEditor;
                    if (editor != null)
                        break;
                }

                return editor;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (documentTabs.SelectedTab == null)
                return;

            var editor = CurrentEditor;
            if (editor != null && editor.Profile != null)
            {
                editor.Profile.Contents = editor.Editor.Text;
                editor.Profile.Save();
            }

            ProfileManager.Current.Stop();

            documentTabs.TabPages.Remove(documentTabs.SelectedTab);

            for (int i = 0; i < openProfiles.Count; i++)
            {
                var p = openProfiles[i];
                if (p.Header.Headerpath == editor.Profile.Header.Headerpath)
                {
                    openProfiles.RemoveAt(i);
                    break;
                }
            }

            if (documentTabs.TabPages.Count == 0)
            {
                this.saveToolStripMenuItem.Enabled = false;
                this.saveToolStripButton.Enabled = false;

                this.closeToolStripMenuItem.Enabled = false;
                this.closeToolStripButton.Enabled = false;

                this.profilePropertiesToolStripMenuItem.Enabled = false;
                this.profileToolStripButton.Enabled = false;

                this.runToolStripButton.Enabled = false;
            }
        }

        private void Quit()
        {
            quit = true;
            Application.Exit();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void profilePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editor = CurrentEditor;
            if (editor == null)
                return;

            ProfilePropertiesDialog d = new ProfilePropertiesDialog(editor.Profile.Header);
            d.Text = "Profile properties for " + editor.Profile.Header.Name;

            var result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                editor.Profile.Header.Save();
                ProfileManager.Current.LoadProfiles();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //notifyIcon1.BalloonTipTitle = "GNet Profiler";
            //notifyIcon1.BalloonTipText = "GNet Profiler has been minimized.";

            //if (FormWindowState.Minimized == this.WindowState)
            //{
            //    notifyIcon1.Visible = true;
            //    notifyIcon1.ShowBalloonTip(500);
            //    this.Hide();
            //}
            //else if (FormWindowState.Normal == this.WindowState)
            //{
            //    //notifyIcon1.Visible = false;
            //}
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void openTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            this.newToolStripMenuItem_Click(sender, e);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            this.manageProfilesToolStripMenuItem_Click(sender, e);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            this.saveToolStripMenuItem_Click(sender, e);
        }

        private void closeToolStripButton_Click(object sender, EventArgs e)
        {
            this.closeToolStripMenuItem_Click(sender, e);
        }

        private void profileToolStripButton_Click(object sender, EventArgs e)
        {
            this.profilePropertiesToolStripMenuItem_Click(sender, e);
        }

        private void runToolStripButton_Click(object sender, EventArgs e)
        {
            var editor = CurrentEditor;
            if (editor != null && editor.Profile != null)
            {
                editor.Profile.Contents = editor.Editor.Text;
                editor.Profile.Save();
                stopToolStripButton.Enabled = true;
                runToolStripButton.Enabled = false;
                documentTabs.Enabled = false;
                ProfileManager.Current.IsRunForExeEnabled = false;
                ProfileManager.Current.SetProfile(editor.Profile);
                ProfileManager.Current.Start();
            }
        }

        private void stopToolStripButton_Click(object sender, EventArgs e)
        {
            ProfileManager.Current.IsRunForExeEnabled = true;
            ProfileManager.Current.Stop();
        }

        private void manageProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var manageProfilesForm = new ManageProfilesForm(this);
            manageProfilesForm.ShowDialog();
        }
    }
}
