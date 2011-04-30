using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GNet.IO
{
    public partial class LgLcd : IDisposable
    {
        public delegate void NotificationEventHandler(int code, int param1, int param2, int param3, int param4);

        public class NotificationEventArgs : EventArgs
        {
            public int Code { get; private set; }
            public int Param1 { get; private set; }
            public int Param2 { get; private set; }
            public int Param3 { get; private set; }
            public int Param4 { get; private set; }

            public NotificationEventArgs(int code, int param1, int param2, int param3, int param4)
            {
                Code = code;
                Param1 = param1;
                Param2 = param2;
                Param3 = param3;
                Param4 = param4;
            }
        }

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
            bool connected;

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
            public bool Connected { get { return connected; } }
            public int Connection { get { return connectContext.connection; } }

            public bool Connect(string appName, int capabilities = LGLCD_APPLET_CAP_BW)
            {
                if (!LgLcdAvailable)
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

                connected = lgLcdConnectEx(ref connectContext) == ERROR_SUCCESS;

                return connected;
            }

            public void Disconnect()
            {
                lgLcdDisconnect(connectContext.connection);
                connected = false;
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



        public void Dispose()
        {
        }
    }
}
