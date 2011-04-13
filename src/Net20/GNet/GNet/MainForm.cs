using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace GNet
{
    public partial class MainForm : Form
    {
        const string GNetFileFilter = "Python Profiles (*.py)|*.py|All Files (*.*)|*.*";

        public MainForm()
        {
            InitializeComponent();

            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());
            var h = HighlightingManager.Manager.FindHighlighter("Python Profile");
            HighlightingManager.Manager.AddHighlightingStrategy(h);
            scriptEditor.SetHighlighting("Python Profile");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void profilePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
