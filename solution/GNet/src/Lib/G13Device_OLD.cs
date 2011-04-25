using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using GNet.Hid;
using GNet.Lib.IO;

namespace GNet.Lib
{
    public partial class G13Device_OLD : Device
    {
        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        LgLcd lcd;
        string lcdAppName;
        int mKeyState = 0;
        KeyStateStruct currentState = new KeyStateStruct();
        JoystickPosition joystick;

        KeyRepeater keyRepeater;

        public G13Device_OLD(string lcdAppName = "G13 GNet Profiler")
            : base(Logitech, G13)
        {
            this.lcdAppName = lcdAppName ?? "G13 GNet Profiler";
            SingleKeyEvents = true;
            keyRepeater = new KeyRepeater();
            keyRepeater.Start();
        }

        public LgLcd Lcd { get { return lcd; } }
        public ulong KeyState { get { return currentState.UL; } }
        public int MKeyState { get { return mKeyState; } }
        public JoystickPosition Joystick { get { return joystick; } }
        public JoystickAngle JoystickAngle { get { return new JoystickAngle(joystick.X, joystick.Y); } }
        public bool SingleKeyEvents { get; set; }

        public override void Start()
        {
            lcd = new LgLcd(lcdAppName, LgLcdDeviceType.LGLCD_DEVICE_BW);

            if (!lcd.Init())
            {
                lcd = null;
                Console.WriteLine("Could not initialize the G13 LCD; is the Logitech Gamepanel Software 3.06 installed?");
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();

            if (lcd != null)
                lcd.DeInit();
        }

        public override void Dispose()
        {
            keyRepeater.Stop();
            Stop();
            base.Dispose();
        }

        public void SetBacklightColorBytes(byte red, byte green, byte blue)
        {
            if (DeviceInfo == null)
                return;

            var length = DeviceInfo.Capabilities.FeatureReportByteLength;
            var data = new byte[length];
            data[0] = 7;
            data[1] = red;
            data[2] = green;
            data[3] = blue;
            data[4] = 0;
            SetFeature(data, DeviceInfo.Capabilities.FeatureReportByteLength);
        }

        public void SetMLight(byte key)
        {
            if (DeviceInfo == null)
                return;

            var length = DeviceInfo.Capabilities.FeatureReportByteLength;
            var data = new byte[length];
            data[0] = 5;
            data[1] = key;
            data[2] = 0xff;
            data[3] = 110;
            data[4] = 0;
            SetFeature(data, DeviceInfo.Capabilities.FeatureReportByteLength);
        }

        protected override void ReadWorker_DataRead(DeviceData data)
        {
            ReadWorker_DecodeData(data);
            base.ReadWorker_DataRead(data);
        }

        protected override void ReadWorker_WaitTimedOut()
        {
        }

        protected virtual void ReadWorker_JoystickChanged(JoystickPosition position)
        {
        }

        protected virtual void ReadWorker_KeyStateChanged(KeyStateStruct newState)
        {
        }

        protected virtual void ReadWorker_KeysPressed(ulong keys)
        {
        }

        protected virtual void ReadWorker_KeysReleased(ulong keys)
        {
        }

        protected virtual void ReadWorker_SingleKeyPressed(G13Keys key)
        {
        }

        protected virtual void ReadWorker_SingleKeyReleased(G13Keys key)
        {
        }

        protected void ReadWorker_DecodeData(DeviceData data)
        {
            KeyStateStruct state = new KeyStateStruct();

            if (DeviceData.ReadStatus.Success == data.Status)
            {
                
                var bytes = data.Bytes;

                var reportId = bytes[0];
                var j = new JoystickPosition(bytes[1] - 0x80, bytes[2] - 0x80);

                if (j.X != joystick.X || j.Y != joystick.Y)
                {
                    joystick = j;
                    ReadWorker_JoystickChanged(j);
                }

                state.B0 = bytes[3];
                state.B1 = bytes[4];
                state.B2 = bytes[5] < 0x80 ? bytes[5] : (byte)(bytes[5] - 0x80);
                state.B3 = bytes[6];
                state.B4 = bytes[7] < 0x80 ? bytes[7] : (byte)(bytes[7] - 0x80);

                ReadWorker_KeyStateChanged(state);

                var keys = currentState.UL ^ state.UL;
                var pressed = (keys & state.UL) > 0;
                currentState = state;

                if (keys > 0)
                {
                    if (pressed)
                    {
                        if (SingleKeyEvents)
                            FireSingleKey(keys, true);
                        else
                            ReadWorker_KeysPressed(keys);
                    }
                    else
                    {
                        if (SingleKeyEvents)
                            FireSingleKey(keys, false);
                        else
                            ReadWorker_KeysReleased(keys);
                    }
                }
            }
        }

        void FireSingleKey(ulong keys, bool pressed)
        {
            for (ulong k = (ulong)G13Keys.G1; k <= (ulong)G13Keys.G22; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) ReadWorker_SingleKeyPressed((G13Keys)k);
                    else ReadWorker_SingleKeyReleased((G13Keys)k);
                }

            for (ulong k = (ulong)G13Keys.J1; k <= (ulong)G13Keys.J3; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) ReadWorker_SingleKeyPressed((G13Keys)k);
                    else ReadWorker_SingleKeyReleased((G13Keys)k);
                }

            for (ulong k = (ulong)G13Keys.M1; k <= (ulong)G13Keys.M4; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) ReadWorker_SingleKeyPressed((G13Keys)k);
                    else ReadWorker_SingleKeyReleased((G13Keys)k);
                }

            for (ulong k = (ulong)G13Keys.L1; k <= (ulong)G13Keys.L5; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) ReadWorker_SingleKeyPressed((G13Keys)k);
                    else ReadWorker_SingleKeyReleased((G13Keys)k);
                }

            if ((keys & (ulong)G13Keys.L6) > 0)
            {
                if (pressed) ReadWorker_SingleKeyPressed(G13Keys.L6);
                else ReadWorker_SingleKeyReleased(G13Keys.L6);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct KeyStateStruct
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