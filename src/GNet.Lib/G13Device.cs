using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using HidLibrary;
using GNet.Lib.IO;

namespace GNet.Lib
{
    public partial class G13Device
    {
        static G13Device current;
        public static G13Device Current
        {
            get
            {
                if (current == null)
                    current = new G13Device();

                return current;
            }
        }

        public delegate void DeviceEventHandler();
        public delegate void KeyHandler(Keys key);
        public delegate void JoystickChangedHandler(int x, int y);
        public delegate void JoystickAngleChangedHandler(double radians, double distanceSquared);

        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        HidDevice device;
        BackgroundWorker<HidDevice> connectedPoller;

        KeyStateStruct currentState = new KeyStateStruct();
        JoystickPosition joystick;

        bool deviceIsSetup;
        bool separateKeyEvents = true;
        int mKeyState = 1;

        G13Device()
        {
            device = HidDevices.GetDevice(Logitech, G13);
            SetupDevice();
        }

        public bool SeparateKeyEvents { get { return separateKeyEvents; } set { separateKeyEvents = value; } }

        public event DeviceEventHandler DeviceConnected;
        public event DeviceEventHandler Inserted;
        public event DeviceEventHandler Removed;
        public event KeyHandler KeyPressed;
        public event KeyHandler KeyReleased;
        public event JoystickChangedHandler JoystickChanged;
        public event JoystickAngleChangedHandler JoystickAngleChanged;

        public ulong KeyState
        {
            get { return currentState.UL; }
        }

        public int MKeyState { get { return mKeyState; } }

        public JoystickPosition Joystick
        {
            get { return joystick; }
        }

        public bool IsConnected
        {
            get
            {
                if (Device == null)
                    return false;

                return Device.IsConnected;
            }
        }

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

        HidDevice Device
        {
            get
            {
                if (device == null)
                {
                    device = HidDevices.GetDevice(Logitech, G13);
                    SetupDevice();
                }

                return device;
            }
        }

        void SetupDevice()
        {
            if (device == null)
                return;

            if (deviceIsSetup)
                return;

            deviceIsSetup = true;

            device.Inserted += new DeviceMonitorHandler(_device_Inserted);
            device.Removed += new DeviceMonitorHandler(_device_Removed);
            device.DataRead += new DeviceReadHandler(_device_DataRead);

            device.MonitorDeviceEvents = true;
            device.StartReading();
        }

        void _device_Inserted(HidDevice device)
        {
            if (Inserted != null)
                Inserted();
        }

        void _device_Removed(HidDevice device)
        {
            if (Removed != null)
                Removed();
        }

        void _device_DataRead(HidDevice device, HidDeviceData data)
        {
            KeyStateStruct state = new KeyStateStruct();

            if (data.Status == HidDeviceData.ReadStatus.Success)
            {
                var bytes = data.Data;

                var reportId = bytes[0];
                var j = new JoystickPosition(bytes[1] - 0x80, bytes[2] - 0x80);

                if (j.X != joystick.X || j.Y != joystick.Y)
                {
                    if (JoystickChanged != null)
                        JoystickChanged(j.X, j.Y);

                    if (JoystickAngleChanged != null)
                    {
                        var r = Math.Atan2(j.Y, j.X);
                        var d2 = j.X * j.X + j.Y * j.Y;
                        JoystickAngleChanged(r, d2);
                    }

                    joystick = j;
                }

                state.B0 = bytes[3];
                state.B1 = bytes[4];
                state.B2 = bytes[5] < 0x80 ? bytes[5] : (byte)(bytes[5] - 0x80);
                state.B3 = bytes[6];
                state.B4 = bytes[7] < 0x80 ? bytes[7] : (byte)(bytes[7] - 0x80);

                var dif = currentState.UL ^ state.UL;
                var pressed = (dif & state.UL) > 0;

                if (dif > 0)
                {
                    if (pressed)
                    {
                        if (KeyPressed != null)
                        {
                            if (!separateKeyEvents)
                            {
                                if (((Keys)dif & Keys.M1) == Keys.M1)
                                    mKeyState = 1;

                                if (((Keys)dif & Keys.M2) == Keys.M2)
                                    mKeyState = 2;

                                if (((Keys)dif & Keys.M3) == Keys.M3)
                                    mKeyState = 3;

                                KeyPressed((Keys)dif);
                            }
                            else
                            {
                                // need to check each key, because more than one key can be pressed at the same time 
                                // and we want to create separate events for each key
                                #region separate key checks

                                if (((Keys)dif & Keys.G1) == Keys.G1)
                                    KeyPressed(Keys.G1);

                                if (((Keys)dif & Keys.G2) == Keys.G2)
                                    KeyPressed(Keys.G2);

                                if (((Keys)dif & Keys.G3) == Keys.G3)
                                    KeyPressed(Keys.G3);

                                if (((Keys)dif & Keys.G4) == Keys.G4)
                                    KeyPressed(Keys.G4);

                                if (((Keys)dif & Keys.G5) == Keys.G5)
                                    KeyPressed(Keys.G5);

                                if (((Keys)dif & Keys.G6) == Keys.G6)
                                    KeyPressed(Keys.G6);

                                if (((Keys)dif & Keys.G7) == Keys.G7)
                                    KeyPressed(Keys.G7);

                                if (((Keys)dif & Keys.G8) == Keys.G8)
                                    KeyPressed(Keys.G8);

                                if (((Keys)dif & Keys.G9) == Keys.G9)
                                    KeyPressed(Keys.G9);

                                if (((Keys)dif & Keys.G10) == Keys.G10)
                                    KeyPressed(Keys.G10);

                                if (((Keys)dif & Keys.G11) == Keys.G11)
                                    KeyPressed(Keys.G11);

                                if (((Keys)dif & Keys.G12) == Keys.G12)
                                    KeyPressed(Keys.G12);

                                if (((Keys)dif & Keys.G13) == Keys.G13)
                                    KeyPressed(Keys.G13);

                                if (((Keys)dif & Keys.G14) == Keys.G14)
                                    KeyPressed(Keys.G14);

                                if (((Keys)dif & Keys.G15) == Keys.G15)
                                    KeyPressed(Keys.G15);

                                if (((Keys)dif & Keys.G16) == Keys.G16)
                                    KeyPressed(Keys.G16);

                                if (((Keys)dif & Keys.G17) == Keys.G17)
                                    KeyPressed(Keys.G17);

                                if (((Keys)dif & Keys.G18) == Keys.G18)
                                    KeyPressed(Keys.G18);

                                if (((Keys)dif & Keys.G19) == Keys.G19)
                                    KeyPressed(Keys.G19);

                                if (((Keys)dif & Keys.G20) == Keys.G20)
                                    KeyPressed(Keys.G20);

                                if (((Keys)dif & Keys.G21) == Keys.G21)
                                    KeyPressed(Keys.G21);

                                if (((Keys)dif & Keys.G22) == Keys.G22)
                                    KeyPressed(Keys.G22);

                                if (((Keys)dif & Keys.J1) == Keys.J1)
                                    KeyPressed(Keys.J1);

                                if (((Keys)dif & Keys.J2) == Keys.J2)
                                    KeyPressed(Keys.J2);

                                if (((Keys)dif & Keys.J3) == Keys.J3)
                                    KeyPressed(Keys.J3);

                                if (((Keys)dif & Keys.M1) == Keys.M1)
                                {
                                    mKeyState = 1;
                                    KeyPressed(Keys.M1);
                                }

                                if (((Keys)dif & Keys.M2) == Keys.M2)
                                {
                                    mKeyState = 2;
                                    KeyPressed(Keys.M2);
                                }

                                if (((Keys)dif & Keys.M3) == Keys.M3)
                                {
                                    mKeyState = 3;
                                    KeyPressed(Keys.M3);
                                }

                                if (((Keys)dif & Keys.M4) == Keys.M4)
                                    KeyPressed(Keys.M4);

                                if (((Keys)dif & Keys.L1) == Keys.L1)
                                    KeyPressed(Keys.L1);

                                if (((Keys)dif & Keys.L2) == Keys.L2)
                                    KeyPressed(Keys.L2);

                                if (((Keys)dif & Keys.L3) == Keys.L3)
                                    KeyPressed(Keys.L3);

                                if (((Keys)dif & Keys.L4) == Keys.L4)
                                    KeyPressed(Keys.L4);

                                if (((Keys)dif & Keys.L5) == Keys.L5)
                                    KeyPressed(Keys.L5);

                                if (((Keys)dif & Keys.L6) == Keys.L6)
                                    KeyPressed(Keys.L6);

                                #endregion
                            }
                        }
                    }
                    else if (KeyReleased != null)
                        if (!separateKeyEvents)
                            KeyReleased((Keys)dif);
                        else
                        {
                            // need to check each key, because more than one key can be pressed at the same time 
                            // and we want to create separate events for each key
                            #region separate key checks

                            if (((Keys)dif & Keys.G1) == Keys.G1)
                                KeyReleased(Keys.G1);

                            if (((Keys)dif & Keys.G2) == Keys.G2)
                                KeyReleased(Keys.G2);

                            if (((Keys)dif & Keys.G3) == Keys.G3)
                                KeyReleased(Keys.G3);

                            if (((Keys)dif & Keys.G4) == Keys.G4)
                                KeyReleased(Keys.G4);

                            if (((Keys)dif & Keys.G5) == Keys.G5)
                                KeyReleased(Keys.G5);

                            if (((Keys)dif & Keys.G6) == Keys.G6)
                                KeyReleased(Keys.G6);

                            if (((Keys)dif & Keys.G7) == Keys.G7)
                                KeyReleased(Keys.G7);

                            if (((Keys)dif & Keys.G8) == Keys.G8)
                                KeyReleased(Keys.G8);

                            if (((Keys)dif & Keys.G9) == Keys.G9)
                                KeyReleased(Keys.G9);

                            if (((Keys)dif & Keys.G10) == Keys.G10)
                                KeyReleased(Keys.G10);

                            if (((Keys)dif & Keys.G11) == Keys.G11)
                                KeyReleased(Keys.G11);

                            if (((Keys)dif & Keys.G12) == Keys.G12)
                                KeyReleased(Keys.G12);

                            if (((Keys)dif & Keys.G13) == Keys.G13)
                                KeyReleased(Keys.G13);

                            if (((Keys)dif & Keys.G14) == Keys.G14)
                                KeyReleased(Keys.G14);

                            if (((Keys)dif & Keys.G15) == Keys.G15)
                                KeyReleased(Keys.G15);

                            if (((Keys)dif & Keys.G16) == Keys.G16)
                                KeyReleased(Keys.G16);

                            if (((Keys)dif & Keys.G17) == Keys.G17)
                                KeyReleased(Keys.G17);

                            if (((Keys)dif & Keys.G18) == Keys.G18)
                                KeyReleased(Keys.G18);

                            if (((Keys)dif & Keys.G19) == Keys.G19)
                                KeyReleased(Keys.G19);

                            if (((Keys)dif & Keys.G20) == Keys.G20)
                                KeyReleased(Keys.G20);

                            if (((Keys)dif & Keys.G21) == Keys.G21)
                                KeyReleased(Keys.G21);

                            if (((Keys)dif & Keys.G22) == Keys.G22)
                                KeyReleased(Keys.G22);

                            if (((Keys)dif & Keys.J1) == Keys.J1)
                                KeyReleased(Keys.J1);

                            if (((Keys)dif & Keys.J2) == Keys.J2)
                                KeyReleased(Keys.J2);

                            if (((Keys)dif & Keys.J3) == Keys.J3)
                                KeyReleased(Keys.J3);

                            if (((Keys)dif & Keys.M1) == Keys.M1)
                                KeyReleased(Keys.M1);

                            if (((Keys)dif & Keys.M2) == Keys.M2)
                                KeyReleased(Keys.M2);

                            if (((Keys)dif & Keys.M3) == Keys.M3)
                                KeyReleased(Keys.M3);

                            if (((Keys)dif & Keys.M4) == Keys.M4)
                                KeyReleased(Keys.M4);

                            if (((Keys)dif & Keys.L1) == Keys.L1)
                                KeyReleased(Keys.L1);

                            if (((Keys)dif & Keys.L2) == Keys.L2)
                                KeyReleased(Keys.L2);

                            if (((Keys)dif & Keys.L3) == Keys.L3)
                                KeyReleased(Keys.L3);

                            if (((Keys)dif & Keys.L4) == Keys.L4)
                                KeyReleased(Keys.L4);

                            if (((Keys)dif & Keys.L5) == Keys.L5)
                                KeyReleased(Keys.L5);

                            if (((Keys)dif & Keys.L6) == Keys.L6)
                                KeyReleased(Keys.L6);

                            #endregion
                        }

                    currentState = state;
                }
            }
        }

        void connectedPoller_Updated(object sender, EventArgs<HidDevice> e)
        {
            device = e.Data;
            SetupDevice();
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

        [StructLayout(LayoutKind.Explicit)]
        struct KeyStateStruct
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
        L1 = 0x0001000000, // LCD Select Applet
        L2 = 0x0002000000,
        L3 = 0x0004000000,
        L4 = 0x0008000000,
        L5 = 0x0010000000,
        L6 = 0x6000000000, // LCD Light
    }
}
