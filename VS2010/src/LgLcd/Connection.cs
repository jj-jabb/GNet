using System;
using System.Runtime.InteropServices;

namespace GNet.LgLcd
{
    public delegate void NotificationEventHandler(int code, int param1, int param2, int param3, int param4);

    public class Connection : Sdk, IDisposable
    {
        static InitManager initManager;

        static Connection()
        {
            initManager = new InitManager();
        }

        class InitManager
        {
            public InitManager()
            {
                LgLcdAvailable = lgLcdInit() == ERROR_SUCCESS;
            }

            ~InitManager()
            {
                lgLcdDeInit();
            }

            public bool LgLcdAvailable { get; private set; }
        }

        bool disposed;
        bool isConnected;

        lgLcdConnectContextEx connectContext;
        lgLcdNotificationContext notifyContext;

        public Connection()
        {
        }

        ~Connection()
        {
            Dispose();
        }

        public event NotificationEventHandler Notified;

        public bool LgLcdAvailable { get { return initManager.LgLcdAvailable; } }
        public bool IsConnected { get { return isConnected; } }
        public lgLcdConnectContextEx Context { get { return connectContext; } }

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

        protected virtual int OnNotify(
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
}
