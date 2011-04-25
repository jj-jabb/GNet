using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GNet.Lib
{
    public partial class LgLcd : IDisposable
    {
        public delegate void SoftButtonsChangedHandler(int buttons);

        string appName;
        LgLcdDeviceType deviceType;

        Bitmap bitmap;
        Graphics graphics;

        bool isInited;
        bool isConnected;
        bool isOpen;

        lgLcdConnectContext connectContext = new lgLcdConnectContext();
        lgLcdOpenByTypeContext openContext = new lgLcdOpenByTypeContext();
        lgLcdSoftbuttonsChangedContext onSoftbuttonsChangedContext = new lgLcdSoftbuttonsChangedContext();

        public LgLcd(string appName, LgLcdDeviceType deviceType)
        {
            this.appName = appName;
            this.deviceType = deviceType;

            switch (deviceType)
            {
                case LgLcdDeviceType.LGLCD_DEVICE_BW:
                    bitmap = new Bitmap(LgLcd.LGLCD_BMP_WIDTH, LgLcd.LGLCD_BMP_HEIGHT);
                    graphics = Graphics.FromImage(bitmap);
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    graphics.Clear(Color.Black);
                    break;

                case LgLcdDeviceType.LGLCD_DEVICE_QVGA:
                    bitmap = new Bitmap(LgLcd.LGLCD_QVGA_BMP_WIDTH, LgLcd.LGLCD_QVGA_BMP_HEIGHT);
                    // not sure these are the right settings for a color display - might want anti-aliasing and
                    // a different background color
                    graphics = Graphics.FromImage(bitmap);
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    graphics.Clear(Color.Black);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Unknown device type " + deviceType);
            }
        }

        public event SoftButtonsChangedHandler SoftButtonsChanged;

        public string AppName { get { return appName; } set { appName = value; } }

        public bool IsInited { get { return isInited; } }
        public bool IsConnected { get { return isConnected; } }
        public bool IsOpen { get { return isOpen; } }

        public Bitmap Bitmap { get { return bitmap; } }
        public Graphics Graphics { get { return graphics; } }

        public bool Init()
        {
            if (isInited)
                return true;

            if (ERROR_SUCCESS == lgLcdInit())
            {
                isInited = true;
                return true;
            }

            return false;
        }

        public bool Connect(bool isAutostartable = false)
        {
            if (isConnected)
                return true;

            if (!isInited && !Init())
                return false;

            connectContext.appFriendlyName = appName;
            connectContext.isAutostartable = isAutostartable;
            connectContext.isPersistent = false; // ignored by LCDMon 3.0 +

            if (ERROR_SUCCESS == lgLcdConnect(ref connectContext))
            {
                isConnected = true;
                return true;
            }

            return false;
        }

        public bool OpenByType()
        {
            if (isOpen)
                return true;

            if (!isConnected && !Connect())
                return false;

            onSoftbuttonsChangedContext.softbuttonsChangedCallback = new LgLcd.lgLcdOnSoftButtonsCB(LcdOnSoftButtonsCB);
            openContext.connection = connectContext.connection;
            openContext.deviceType = (int)deviceType;
            openContext.onSoftbuttonsChanged = onSoftbuttonsChangedContext;

            if (ERROR_SUCCESS == lgLcdOpenByType(ref openContext))
            {
                isOpen = true;
                return true;
            }

            return false;
        }

        public bool DeInit()
        {
            if (!isInited)
                return true;

            if (isConnected && !Disconnect())
                return false;

            if (ERROR_SUCCESS == lgLcdDeInit())
            {
                isInited = false;
                return true;
            }

            return false;
        }

        public bool Disconnect()
        {
            if (!isConnected)
                return true;

            if (isOpen && !Close())
                return false;

            if (ERROR_SUCCESS == lgLcdDisconnect(connectContext.connection))
            {
                isConnected = false;
                return true;
            }

            return false;
        }

        public bool Close()
        {
            if (!isOpen)
                return true;

            if (ERROR_SUCCESS == lgLcdClose(openContext.device))
            {
                isOpen = false;
                return true;
            }

            return false;
        }

        public bool BringToFront()
        {
            if (!isOpen)
                return false;

            return ERROR_SUCCESS ==  LgLcd.lgLcdSetAsLCDForegroundApp(openContext.device, 1);
        }

        public bool RemoveFromFront()
        {
            if (!isOpen)
                return false;

            return ERROR_SUCCESS == LgLcd.lgLcdSetAsLCDForegroundApp(openContext.device, 0);
        }

        public void Dispose()
        {
            if (isInited)
                DeInit();
        }

        int LcdOnSoftButtonsCB(int device, int dwButtons, IntPtr pContext)
        {
            if (SoftButtonsChanged != null)
                SoftButtonsChanged(dwButtons);

            return 0;
        }

        void UpdateBitmap(int priority)
        {
            if (!isOpen)
                return;

            var bitdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            LgLcd.lgLcdBitmap160x43x1 bmp = LgLcd.lgLcdBitmap160x43x1.New;

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

                LgLcd.lgLcdUpdateBitmapBW(openContext.device, ref bmp, priority);
            }


            bitmap.UnlockBits(bitdata);
        }

        public void DrawString(string text, string fontName, double fontSize, double fontStyle, string htmlColor, double x, double y, int priority = LgLcd.LGLCD_PRIORITY_ALERT)
        {
            FontStyle fstyle = (FontStyle)fontStyle;
            float fsize = (float)fontSize;
            float fx = (float)x;
            float fy = (float)y;
            Brush brush = new SolidBrush(ColorTranslator.FromHtml(htmlColor));

            using (Font f = new Font(fontName, fsize, fstyle))
            {
                graphics.DrawString(text, f, brush, 0f, 0f);
            }

            UpdateBitmap(priority);
        }

        public void Clear(byte red = 0, byte green = 0, byte blue = 0, int priority = LgLcd.LGLCD_PRIORITY_NORMAL)
        {
            graphics.Clear(Color.FromArgb(0xff, red, green, blue));
            UpdateBitmap(priority);
        }
    }
}
