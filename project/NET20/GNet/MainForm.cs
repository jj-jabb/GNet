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
        public MainForm()
        {
            InitializeComponent();
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
