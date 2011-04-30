using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GNet.IO
{
    public partial class LgLcd : IDisposable
    {
        public delegate void NotificationEventHandler(int code, int param1, int param2, int param3, int param4);
        public delegate void SoftButtonsChangedHandler(int buttons);

        public class ConnectionManager : IDisposable
        {
            static ConnectionManager current;
            public static ConnectionManager Current
            {
                get
                {
                    if (current == null)
                        current = new ConnectionManager();

                    return current;
                }
            }

            bool disposed;
            bool isConnected;

            lgLcdConnectContextEx connectContext;
            lgLcdNotificationContext notifyContext;

            ConnectionManager()
            {
                LgLcdAvailable = lgLcdInit() == ERROR_SUCCESS;
                if (LgLcdAvailable)
                {


                    lgLcdConnectEx(ref connectContext);
                }
            }

            ~ConnectionManager()
            {
                Dispose();
            }

            public event NotificationEventHandler Notified;

            public bool LgLcdAvailable { get; private set; }
            public bool IsConnected { get { return isConnected; } }
            public int Connection { get { return connectContext.connection; } }

            public bool Connect(string appName, int capabilities = LGLCD_APPLET_CAP_BW)
            {
                if (!LgLcdAvailable)
                    return false;

                if (isConnected)
                    return false;

                notifyContext = new lgLcdNotificationContext
                {
                    notificationCallback = new lgLcdOnNotificationCB(OnNotify),
                    notifyContext = IntPtr.Zero
                };

                connectContext = new lgLcdConnectContextEx
                {
                    appFriendlyName = appName,
                    dwAppletCapabilitiesSupported = capabilities,
                    isAutostartable = false,
                    isPersistent = false,
                    onNotify = notifyContext
                };

                isConnected = lgLcdConnectEx(ref connectContext) == ERROR_SUCCESS;

                return isConnected;
            }

            public void Disconnect()
            {
                lgLcdDisconnect(connectContext.connection);
                isConnected = false;
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;
                Disconnect();
                lgLcdDeInit();
            }

            int OnNotify(
                int connection,
                IntPtr pContext,
                int notificationCode,
                int notifyParm1,
                int notifyParm2,
                int notifyParm3,
                int notifyParm4
                )
            {
                Notified(notificationCode, notifyParm1, notifyParm2, notifyParm3, notifyParm4);
                return 0;
            }
        }

        public static ConnectionManager Manager
        {
            get
            {
                return ConnectionManager.Current;
            }
        }

        static string appName;
        static int capabilities;

        public static string AppName { get { return appName; } set {appName = value; } }
        public static int Capabilities { get { return capabilities; } set { capabilities = value; } }

        LgLcdDeviceType deviceType;

        Bitmap bitmap;
        Graphics graphics;
        bool isOpen;

        lgLcdOpenByTypeContext openContext = new lgLcdOpenByTypeContext();
        lgLcdSoftbuttonsChangedContext onSoftbuttonsChangedContext = new lgLcdSoftbuttonsChangedContext();

        public LgLcd(LgLcdDeviceType deviceType)
        {
            if (!Manager.IsConnected)
                Manager.Connect(AppName, Capabilities);

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
        public bool IsOpen { get { return isOpen; } }

        public Bitmap Bitmap { get { return bitmap; } }
        public Graphics Graphics { get { return graphics; } }

        public bool OpenByType()
        {
            if (isOpen)
                return true;

            if (!Manager.IsConnected && !Manager.Connect(AppName, Capabilities))
                return false;

            onSoftbuttonsChangedContext.softbuttonsChangedCallback = new LgLcd.lgLcdOnSoftButtonsCB(LcdOnSoftButtonsCB);
            openContext.connection = Manager.Connection;
            openContext.deviceType = (int)deviceType;
            openContext.onSoftbuttonsChanged = onSoftbuttonsChangedContext;

            if (ERROR_SUCCESS == lgLcdOpenByType(ref openContext))
            {
                isOpen = true;
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

            return ERROR_SUCCESS == LgLcd.lgLcdSetAsLCDForegroundApp(openContext.device, 1);
        }

        public bool RemoveFromFront()
        {
            if (!isOpen)
                return false;

            return ERROR_SUCCESS == LgLcd.lgLcdSetAsLCDForegroundApp(openContext.device, 0);
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
            DrawString(text, fontName, fontSize, fontStyle, ColorTranslator.FromHtml(htmlColor), x, y, priority);
        }

        public void DrawString(string text, string fontName, double fontSize, double fontStyle, Color color, double x, double y, int priority = LgLcd.LGLCD_PRIORITY_ALERT)
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

        public void Clear(byte red = 0, byte green = 0, byte blue = 0, int priority = LgLcd.LGLCD_PRIORITY_NORMAL)
        {
            graphics.Clear(Color.FromArgb(0xff, red, green, blue));
            UpdateBitmap(priority);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
