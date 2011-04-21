using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GNet
{
    public partial class MainForm : Form
    {
        TextBoxStreamWriter tbsw;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tbsw = new TextBoxStreamWriter(output);
            Console.SetOut(tbsw);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (TabPage tab in documentTabs.TabPages)
                foreach (Control ctrl in tab.Controls)
                {
                    ScriptEditor editor = ctrl as ScriptEditor;
                    if (editor != null)
                        editor.StopScript();
                }
        }

        private void newAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tabPage = new TabPage("Untitled");
            ScriptEditor editor = new ScriptEditor();
            editor.Dock = DockStyle.Fill;
            tabPage.Controls.Add(editor);
            documentTabs.TabPages.Add(tabPage);
        }
    }
}
