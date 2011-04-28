using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using GNet;
using System.Drawing.Text;

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
            if (lcd == null)
            {
                lcd = new Lcd("GNet Test", true, false, LcdDeviceType.LcdDeviceBW);
                lcd.Connect();
                lcd.Open();
                lcd.BringToFront();
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (lcd == null)
                return;

            lcd.LcdGraphics.Clear(Color.Black);

            string fontName;
            //fontName = "Microsoft Sans Serif";
            fontName = "Arial Narrow";
            using (Font f = new Font(fontName, 8f, FontStyle.Regular))
            {
                lcd.LcdGraphics.DrawString(textBox.Text, f, Brushes.White, 0f, 0f);
                //lcd.LcdGraphics.DrawString("The quick brown fox jumped over\nthe lazy dog", f, Brushes.White, 0f, 0f);
            }

            lcd.UpdateBitmap(LcdPriority.Normal);
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
