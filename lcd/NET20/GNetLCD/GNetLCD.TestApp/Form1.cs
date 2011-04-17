using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
            lcd.Connect();
            lcd.Open();
            lcd.BringToFront();
        }

        //http://www.vbforums.com/showthread.php?t=358917
        // import this 
        // using System.Runtime.InteropServices;
        //private unsafe byte[] BmpToBytes_Unsafe(Bitmap bmp)
        //{
        //    BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
        //        ImageLockMode.ReadOnly,
        //        PixelFormat.Format24bppRgb);
        //    // number of bytes in the bitmap
        //    int byteCount = bData.Stride * bmp.Height;
        //    byte[] bmpBytes = new byte[byteCount];

        //    // Copy the locked bytes from memory
        //    Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount);

        //    // don't forget to unlock the bitmap!!
        //    bmp.UnlockBits(bData);

        //    return bmpBytes;
        //}

        private void btnWrite_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(160, 43, PixelFormat.Format1bppIndexed);

            for (int i = 0; i < 160; i++)
                for (int j = 0; j < 43; j++)
                    bm.SetPixel(i, j, Color.White);

            bm.GetHbitmap()

            byte[] b = new byte[Lcd.BW_SIZE];

            for (int i = 0; i < b.Length; i++)
            {
                if (i % 2 == 0)
                    b[i] = 255;
            }

            lcd.UpdateBitmap(b, LcdPriority.Normal);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (lcd != null)
            {
                //lcd.Close();
                //lcd.Disconnect();

                lcd.Dispose();
                //lcd = null;
            }
        }
    }
}
