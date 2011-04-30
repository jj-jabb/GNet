using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GNet.LgLcd
{
    public abstract class Lcd : Sdk
    {
        public delegate void SoftButtonsChangedHandler(int buttons);

        protected Connection connection;

        protected lgLcdOpenByTypeContext openContext;
        protected lgLcdSoftbuttonsChangedContext onSoftbuttonsChangedContext;

        bool isOpen;

        protected Lcd(string appName, int capabilities)
        {
            connection = new Connection();
            connection.Notified += new Connection.NotificationEventHandler(OnNotified);
            connection.Connect(appName, capabilities);
        }

        ~Lcd()
        {
            connection.Notified -= new Connection.NotificationEventHandler(OnNotified);
            Close();
            connection.Disconnect();
        }

        public event SoftButtonsChangedHandler SoftButtonsChanged;

        public Connection Connection { get { return connection; } }
        public bool IsConnected { get { return connection.IsConnected; } }
        public bool IsOpen { get { return isOpen; } }
        public abstract int DeviceType { get; }

        protected virtual void OnNotified(int code, int param1, int param2, int param3, int param4)
        {
            switch (code)
            {
                case Sdk.LGLCD_NOTIFICATION_DEVICE_ARRIVAL:
                    OpenByType();
                    break;

                case Sdk.LGLCD_NOTIFICATION_DEVICE_REMOVAL:
                    //Close();
                    isOpen = false;
                    break;

                case Sdk.LGLCD_NOTIFICATION_CLOSE_CONNECTION:
                    Close();
                    connection.Disconnect();
                    break;
            }
        }

        protected bool OpenByType()
        {
            if (isOpen)
                return true;

            if (!IsConnected)
                return false;

            onSoftbuttonsChangedContext.softbuttonsChangedCallback = new lgLcdOnSoftButtonsCB(LcdOnSoftButtonsCB);
            openContext.connection = connection.Context.connection;
            openContext.deviceType = DeviceType;
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

            var val = lgLcdClose(openContext.device);
            if (ERROR_SUCCESS == val)
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

            return ERROR_SUCCESS == lgLcdSetAsLCDForegroundApp(openContext.device, 1);
        }

        public bool RemoveFromFront()
        {
            if (!isOpen)
                return false;

            return ERROR_SUCCESS == lgLcdSetAsLCDForegroundApp(openContext.device, 0);
        }

        int LcdOnSoftButtonsCB(int device, int dwButtons, IntPtr pContext)
        {
            if (SoftButtonsChanged != null)
                SoftButtonsChanged(dwButtons);

            return 0;
        }
    }
}
