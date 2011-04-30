using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GNet.Hid;
using GNet.IO;

namespace G13DeviceRefactor
{
    public partial class Form1 : Form
    {
        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        Device device;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            device = new Device(Logitech, G13);

            LgLcd.Manager.Notified += new LgLcd.NotificationEventHandler(Manager_Notified);
            LgLcd.Manager.Connect("G13 Test");
        }

        void Manager_Notified(int code, int param1, int param2, int param3, int param4)
        {
            Debug.WriteLine(code + " : " + param1 + ", " + param2 + ", " + param3 + ", " + param4);
        }

        private void btnStartDevice_Click(object sender, EventArgs e)
        {
            device.Start();
        }

        private void btnStopDevice_Click(object sender, EventArgs e)
        {
            device.Stop();
        }
    }
}
