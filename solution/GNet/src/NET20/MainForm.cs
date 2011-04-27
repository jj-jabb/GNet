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
        TextWriter originalout;
        TextBoxStreamWriter tbsw;
        bool quit;

        public MainForm()
        {
            InitializeComponent();
            notifyIcon1.Icon = Properties.Resources.TrayIcon;
            //notifyIcon1.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            G13Device.Init();

            originalout = Console.Out;
            tbsw = new TextBoxStreamWriter(output);
            Console.SetOut(tbsw);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quit)
            {
                e.Cancel = true;
                this.Hide();
                //WindowState = FormWindowState.Minimized;
            }
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
            NewProfileDialog d = new NewProfileDialog();

            var result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                var fileName = MakeSafeFilename(d.ScriptName);
                string ext;
                var basePath = "Profiles\\" + d.Language + "\\";

                switch (d.Language)
                {
                    case "Lua":
                        ext = ".lua";
                        break;

                    case "Boo":
                        ext = ".boo";
                        break;

                    default:
                        ext = ".txt";
                        break;
                }

                if (File.Exists(basePath + fileName + ext))
                {
                    int fileCount = 1;
                    while (File.Exists(basePath + fileName + fileCount + ext))
                        fileCount++;
                    fileName += fileCount;
                }

                fileName = fileName + ext;

                bool copied = false;
                if (d.CopyFrom != null)
                {
                    var copyFrom = basePath + d.CopyFrom + ext;
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

                newProfile = Profile.GetProfile(basePath + fileName);
                newProfile.ReadFile();

                newProfile.Name = d.ScriptName;
                newProfile.Description = d.Description;
                newProfile.Device = (DeviceType)Enum.Parse(typeof(DeviceType), d.Device);
                newProfile.Lock = d.LockForExecutables;

                for (int i = 0; i < d.Executables.Count; i++)
                    newProfile.Executables.Add(d.Executables[i]);

                newProfile.KeyboardHook = d.KeyboardHook;
                newProfile.MouseHook = d.MouseHook;

                newProfile.IsEnabled = d.IsEnabled;

                newProfile.Save();
                newProfile.ReadFile();

                TabPage tabPage = new TabPage(newProfile.Name);
                ScriptEditor editor = new ScriptEditor(newProfile);
                editor.Dock = DockStyle.Fill;
                tabPage.Controls.Add(editor);
                documentTabs.TabPages.Add(tabPage);
                documentTabs.SelectedTab = tabPage;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "GNet Script|*.lua;*.boo",
                InitialDirectory = ".\\Profiles\\",
                RestoreDirectory = false
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var profile = Profile.GetProfile(ofd.FileName);
                profile.ReadFile();

                TabPage tabPage = new TabPage(profile.Name);
                ScriptEditor editor = new ScriptEditor(profile);
                editor.Dock = DockStyle.Fill;
                tabPage.Controls.Add(editor);
                documentTabs.TabPages.Add(tabPage);
                documentTabs.SelectedTab = tabPage;

            }
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
                if (editor.Script != null)
                    editor.Script.Stop();
            }

            documentTabs.TabPages.Remove(documentTabs.SelectedTab);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.SetOut(originalout);

            foreach (TabPage tab in documentTabs.TabPages)
                foreach (Control ctrl in tab.Controls)
                {
                    ScriptEditor editor = ctrl as ScriptEditor;
                    if (editor != null)
                        editor.DisposeScript();
                }

            G13Device.Deinit();
            quit = true;
            Application.Exit();
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

            NewProfileDialog d = new NewProfileDialog();
            d.Text = "Profile properties for " + editor.Profile.Name;
            d.ReadFromProfile(editor.Profile);
            var result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                editor.Profile.Name = d.ScriptName;
                editor.Profile.Description = d.Description;
                editor.Profile.Device = (DeviceType)Enum.Parse(typeof(DeviceType), d.Device);
                editor.Profile.Lock = d.LockForExecutables;

                for (int i = 0; i < d.Executables.Count; i++)
                    editor.Profile.Executables.Add(d.Executables[i]);

                editor.Profile.KeyboardHook = d.KeyboardHook;
                editor.Profile.MouseHook = d.MouseHook;

                editor.Profile.IsEnabled = d.IsEnabled;

                editor.Profile.Save();
                editor.Profile.ReadFile();
                editor.Editor.Text = editor.Profile.Contents;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "GNet Profiler";
            notifyIcon1.BalloonTipText = "GNet Profiler has been minimized.";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                //notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
