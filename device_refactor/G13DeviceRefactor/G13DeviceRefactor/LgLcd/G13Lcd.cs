using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GNet.LgLcd
{
    public class G13Lcd : Lcd
    {
        public override int DeviceType
        {
            get { return LGLCD_DEVICE_BW; }
        }

        public G13Lcd(string appName)
            : base(appName, LGLCD_APPLET_CAP_BW)
        {
            bitmap = new Bitmap(LGLCD_BMP_WIDTH, LGLCD_BMP_HEIGHT);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.Clear(Color.Black);

            OpenByType();
        }

        Bitmap bitmap;
        Graphics graphics;

        public Bitmap Bitmap { get { return bitmap; } }
        public Graphics Graphics { get { return graphics; } }

        void UpdateBitmap(int priority)
        {
            if (!IsOpen)
                return;

            var bitdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            lgLcdBitmap160x43x1 bmp = lgLcdBitmap160x43x1.New;

            unsafe
            {
                for (int y = 0; y < bitdata.Height; y++)
                {
                    byte* row = (byte*)bitdata.Scan0.ToPointer() + (y * bitdata.Stride);
                    for (int x = 0; x < bitdata.Width; x++)
                    {

                        byte* p = &row[x * 4];
                        byte val = *p;
                        val |= *(p + 1);
                        val |= *(p + 2);
                        byte pixel = val < (byte)0x80 ? (byte)0x00 : (byte)0xff;
                        bmp.pixels[(y * bitdata.Width) + x] = pixel;
                    }
                }

                lgLcdUpdateBitmapBW(openContext.device, ref bmp, priority);
            }


            bitmap.UnlockBits(bitdata);
        }

        public void DrawString(string text, string fontName, double fontSize, double fontStyle, string htmlColor, double x, double y, int priority = LGLCD_PRIORITY_NORMAL)
        {
            DrawString(text, fontName, fontSize, fontStyle, ColorTranslator.FromHtml(htmlColor), x, y, priority);
        }

        public void DrawString(string text, string fontName, double fontSize, double fontStyle, Color color, double x, double y, int priority = LGLCD_PRIORITY_NORMAL)
        {
            FontStyle fstyle = (FontStyle)fontStyle;
            float fsize = (float)fontSize;
            float fx = (float)x;
            float fy = (float)y;
            Brush brush = new SolidBrush(color);

            using (Font f = new Font(fontName, fsize, fstyle))
            {
                graphics.DrawString(text, f, brush, 0f, 0f);
            }

            UpdateBitmap(priority);
        }

        public void Clear(byte red = 0, byte green = 0, byte blue = 0, int priority = LGLCD_PRIORITY_NORMAL)
        {
            graphics.Clear(Color.FromArgb(0xff, red, green, blue));
            UpdateBitmap(priority);
        }
    }
}
