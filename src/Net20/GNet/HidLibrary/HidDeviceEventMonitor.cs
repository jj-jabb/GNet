using System;
using System.Threading;

namespace HidLibrary
{
    public delegate void DeviceMonitorHandler(HidDevice device);

    internal class HidDeviceEventMonitor
    {
        public event DeviceMonitorHandler Inserted;
        public event DeviceMonitorHandler Removed;

        readonly HidDevice device;
        bool wasConnected;

        BackgroundWorker<bool> monitor;

        public HidDeviceEventMonitor(HidDevice device)
        {
            this.device = device;

            monitor = new BackgroundWorker<bool>();
            monitor.Updated += new EventHandler<EventArgs<bool>>(monitor_Updated);
            monitor.DoWork += new System.ComponentModel.DoWorkEventHandler(monitor_DoWork);
        }

        public void Start()
        {
            if (monitor.IsBusy)
                return;

            monitor.RunWorkerAsync();
        }

        public void Stop()
        {
            if (!monitor.IsBusy)
                return;

            monitor.CancelAsync();
        }

        void monitor_Updated(object sender, EventArgs<bool> e)
        {
            if (e.Data && Inserted != null) Inserted(device);
            else if (!e.Data && Removed != null) Removed(device);
        }

        void monitor_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (monitor.CancellationPending != null)
            {
                var isConnected = device.IsPathInDeviceList;

                if (isConnected != wasConnected)
                {
                    wasConnected = isConnected;
                    monitor.Update(isConnected);
                }

                Thread.Sleep(500);
            }
        }
    }
}
