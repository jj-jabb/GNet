using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GNet.Hid;
using GNet.IO;
using GNet.Scripting;

namespace GNet
{
    public partial class MainForm : Form
    {
        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        Device device = new Device(Logitech, G13);


        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            device.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            device.Stop();
        }
    }
}
