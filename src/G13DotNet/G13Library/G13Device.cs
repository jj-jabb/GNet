using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using HidLibrary;

namespace G13Library
{
    public class G13Device
    {
        public delegate void DeviceConnectedHandler();

        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        HidDevice _device;
        BackgroundWorker<HidDevice> connectedPoller;

        public G13Device()
        {
            _device = HidDevices.GetDevice(Logitech, G13);
        }

        HidDevice device
        {
            get
            {
                if (_device == null)
                    _device = HidDevices.GetDevice(Logitech, G13);

                return _device;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (device == null)
                    return false;

                return device.IsConnected;
            }
        }

        public event DeviceConnectedHandler DeviceConnected;
        public void WaitForConnection()
        {
            if (connectedPoller == null)
            {
                connectedPoller = new BackgroundWorker<HidDevice>();
                connectedPoller.Updated += new EventHandler<EventArgs<HidDevice>>(connectedPoller_Updated);
                connectedPoller.DoWork += new System.ComponentModel.DoWorkEventHandler(connectedPoller_DoWork);
            }

            if (!connectedPoller.IsBusy)
                connectedPoller.RunWorkerAsync();
        }

        public void CancelWait()
        {
            if (connectedPoller.IsBusy)
                connectedPoller.CancelAsync();
        }

        void connectedPoller_Updated(object sender, EventArgs<HidDevice> e)
        {
            _device = e.Data;
            if (DeviceConnected != null)
                DeviceConnected();
        }

        void connectedPoller_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            HidDevice d = null;

            while (!connectedPoller.CancellationPending && d == null)
                d = HidDevices.GetDevice(Logitech, G13);

            connectedPoller.Update(d);
        }



        [Flags]
        public enum Keys : ulong
        {
            // G keys
            G1 = 0x0000000001,
            G2 = 0x0000000002,
            G3 = 0x0000000004,
            G4 = 0x0000000008,
            G5 = 0x0000000010,
            G6 = 0x0000000020,
            G7 = 0x0000000040,
            G8 = 0x0000000080,
            G9 = 0x0000000100,
            G10 = 0x0000000200,
            G11 = 0x0000000400,
            G12 = 0x0000000800,
            G13 = 0x0000001000,
            G14 = 0x0000002000,
            G15 = 0x0000004000,
            G16 = 0x0000008000,
            G17 = 0x0000010000,
            G18 = 0x0000020000,
            G19 = 0x0000040000,
            G20 = 0x0000080000,
            G21 = 0x0000100000,
            G22 = 0x0000200000,

            // Joystick buttons
            J1 = 0x0200000000, // left
            J2 = 0x0400000000, // bottom
            J3 = 0x0800000000, // middle (i.e., on the stick)

            // M keys
            M1 = 0x0020000000,
            M2 = 0x0040000000,
            M3 = 0x0080000000,
            M4 = 0x0100000000,

            // LCD keys
            L0 = 0x0001000000, // LCD Select Applet
            L1 = 0x0002000000,
            L2 = 0x0004000000,
            L3 = 0x0008000000,
            L4 = 0x0010000000,
            L5 = 0x6000000000, // LCD Light
        }

        [StructLayout(LayoutKind.Explicit)]
        struct KeyState
        {
            [FieldOffset(0)]
            public byte B0;
            [FieldOffset(1)]
            public byte B1;
            [FieldOffset(2)]
            public byte B2;
            [FieldOffset(3)]
            public byte B3;
            [FieldOffset(4)]
            public byte B4;

            [FieldOffset(0)]
            public ulong UL;
        }
    }
}
