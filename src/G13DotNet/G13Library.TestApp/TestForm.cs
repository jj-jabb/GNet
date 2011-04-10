using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace G13Library.TestApp
{
    public partial class TestForm : Form
    {
        G13Profile tester;

        public TestForm()
        {
            InitializeComponent();
        }

        public RichTextBox RtbInfo { get { return rtbInfo; } }

        private void TestForm_Load(object sender, EventArgs e)
        {
            tester = new G13ProfileBasicTests(this);

            // for FPS gaming:
            // tester = new G13ProfileFPS(this);
        }
    }
}
