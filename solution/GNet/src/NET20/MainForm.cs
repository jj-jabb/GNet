using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GNet
{
    public partial class MainForm : Form
    {
        TextWriter originalout;
        TextBoxStreamWriter tbsw;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            originalout = Console.Out;
            tbsw = new TextBoxStreamWriter(output);
            Console.SetOut(tbsw);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.SetOut(originalout);

            foreach (TabPage tab in documentTabs.TabPages)
                foreach (Control ctrl in tab.Controls)
                {
                    ScriptEditor editor = ctrl as ScriptEditor;
                    if (editor != null)
                        editor.DisposeScript();
                }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProfileDialog d = new NewProfileDialog();
            var result = d.ShowDialog();

            string lineStart = "";
            string extension = ".txt";

            if (result == DialogResult.OK)
            {

                switch (d.Language)
                {
                    case "Lua":
                        lineStart = "-- ";
                        extension = ".lua";
                        break;

                    case "Boo":
                        lineStart = "# ";
                        extension = ".boo";
                        break;
                }

                StringBuilder script = new StringBuilder();
                script.Append(lineStart).Append("Name: ").Append(d.Name).Append(Environment.NewLine);
                script.Append(lineStart).Append("Description: ").Append(d.Description).Append(Environment.NewLine);
                script.Append(lineStart).Append("Language: ").Append(d.Language).Append(Environment.NewLine);
                script.Append(lineStart).Append("Device: ").Append(d.Device).Append(Environment.NewLine);
                script.Append(lineStart).Append("LockForExecutables: ").Append(d.LockForExecutables).Append(Environment.NewLine);
                script.Append(lineStart).Append("Executables: ");

                for(int i = 0; i < d.Executables.Count; i++)
                {
                    if (i > 0)
                        script.Append(", ");

                    script.Append("\"").Append(d.Executables[i]).Append("\"");
                }
                
                script.Append(Environment.NewLine).Append(Environment.NewLine);
                
                var basePath= "Profiles\\" + d.Language + "\\";

                //var copyFrom = d.CopyFrom ?? "_template" + extension;

                //if (copyFrom != null)
                //{
                //    if ((copyFrom == "Default Configuration") && !File.Exists(basePath + "Default Configuration" + extension))
                //        if (File.Exists(basePath + "_default" + extension))
                //            File.Copy(basePath + "Default Configuration" + extension, basePath + "_default" + extension);


                //}

                using (var fs = File.OpenText(basePath + "_default" + extension))
                {
                    script.Append(fs.ReadToEnd());
                }

                
                TabPage tabPage = new TabPage(d.Name);
                ScriptEditor editor = new ScriptEditor(script.ToString());
                editor.Dock = DockStyle.Fill;
                tabPage.Controls.Add(editor);
                documentTabs.TabPages.Add(tabPage);
            }

        }
    }
}
