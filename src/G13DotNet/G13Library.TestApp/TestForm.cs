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

            rtbInfo.MouseEnter += new EventHandler(rtbInfo_MouseEnter);
        }

        void rtbInfo_MouseEnter(object sender, EventArgs e)
        {
            //InputManager.Write("This is a TEST (!?!)\n[{Awesome}]");
            rtbInfo.AppendText(InputManager.IsKeyPressed(InputManager.VirtualKeyStates.VK_LBUTTON).ToString() + Environment.NewLine);
        }

        public RichTextBox RtbInfo { get { return rtbInfo; } }

        private void TestForm_Load(object sender, EventArgs e)
        {
            var s0 = InputManager.AsciiToScanCode('/').ToString();
            var s1 = InputManager.AsciiToScanCode('?').ToString();
            //tester = new G13ProfileBasicTests(this);
            

            // for FPS gaming:
            // tester = new G13ProfileFPS(this);
        }
    }
}
