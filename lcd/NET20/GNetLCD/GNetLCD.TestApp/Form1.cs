using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GNet;

namespace GNetLCD.TestApp
{
    public partial class Form1 : Form
    {
        Lcd lcd;

        public Form1()
        {
            InitializeComponent();


        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            lcd = new Lcd("GNet Test", true, false, LcdDeviceType.LcdDeviceBW);
            lcd.BringToFront();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (lcd != null)
            {
                lcd.Dispose();
                lcd = null;
            }
        }
    }
}
