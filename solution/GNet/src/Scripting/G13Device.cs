using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using GNet.Hid;
using GNet.Lib;
using GNet.Lib.IO;

namespace GNet.Scripting
{
    public class G13Device : Device
    {
        static G13Device current;

        public static void Init()
        {
            if (current != null)
                return;

            current = new G13Device();
            current.Start();
        }

        public static void Deinit()
        {
            if (current == null)
                return;

            current.Dispose();
            current = null;
        }

        public static G13Device Current
        {
            get
            {
                return current;
            }
        }

        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        LgLcd lcd;
        string lcdAppName;
        int mKeyState = 0;
        G13KeyState currentState = new G13KeyState();
        JoystickPosition joystick;
        IG13DeviceScript _script;
        object scriptLock;
        ReaderWriterLock scriptLocker;

        KeyRepeater keyRepeater;

        G13Device()
            : base(Logitech, G13)
        {
            this.lcdAppName = "G13 GNet Profiler";
            scriptLock = new object();
            scriptLocker = new ReaderWriterLock();
            keyRepeater = new KeyRepeater();
            keyRepeater.Start();
        }

        public LgLcd Lcd { get { return lcd; } }
        public ulong KeyState { get { return currentState.UL; } }
        public int MKeyState { get { return mKeyState; } }
        public JoystickPosition Joystick { get { return joystick; } }
        public JoystickAngle JoystickAngle { get { return new JoystickAngle(joystick.X, joystick.Y); } }
        public KeyRepeater KeyRepeater { get { return keyRepeater; } }

        public IG13DeviceScript Script
        {
            get
            {
                lock (scriptLock)
                {
                    return _script;
                }
            }
            set
            {
                lock (scriptLock)
                {
                    if (_script != null)
                    {
                        if (_script.IsRunning)
                            _script.Stop();
                        
                        if (_script != null)
                            _script.Device = null;
                    }

                    _script = value;
                    if (_script != null)
                        _script.Device = this;
                }
            }
        }

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

        protected override void ReadWorker_Opened()
        {
            Lcd.OpenByType();
            Lcd.Clear();
        }

        protected override void ReadWorker_Closed()
        {
            lcd.Disconnect();
        }

        protected override void ReadWorker_DataRead(DeviceData data)
        {
            ReadWorker_DecodeData(data);
            base.ReadWorker_DataRead(data);
        }

        protected override void ReadWorker_WaitTimedOut()
        {
        }

        protected void ReadWorker_DecodeData(DeviceData data)
        {
            G13KeyState state = new G13KeyState();

            var script = Script;
            if (script == null || !script.IsRunning)
                return;

            if (DeviceData.ReadStatus.Success == data.Status)
            {
                
                var bytes = data.Bytes;

                var reportId = bytes[0];
                var j = new JoystickPosition(bytes[1] - 0x80, bytes[2] - 0x80);

                if (j.X != joystick.X || j.Y != joystick.Y)
                {
                    joystick = j;
                    script.JoystickChanged(j);
                }

                state.B0 = bytes[3];
                state.B1 = bytes[4];
                state.B2 = bytes[5] < 0x80 ? bytes[5] : (byte)(bytes[5] - 0x80);
                state.B3 = bytes[6];
                state.B4 = bytes[7] < 0x80 ? bytes[7] : (byte)(bytes[7] - 0x80);

                script.KeyStateChanged(state);

                var keys = currentState.UL ^ state.UL;
                var pressed = (keys & state.UL) > 0;
                currentState = state;

                if (keys > 0)
                {
                    if (pressed)
                    {
                        if (script.SingleKeyEvents)
                            FireSingleKey(script, keys, true);
                        else
                            script.KeysPressed(keys);
                    }
                    else
                    {
                        if (script.SingleKeyEvents)
                            FireSingleKey(script, keys, false);
                        else
                            script.KeysReleased(keys);
                    }
                }
            }
        }

        void FireSingleKey(IDeviceScript script, ulong keys, bool pressed)
        {
            for (ulong k = (ulong)G13Keys.G1; k <= (ulong)G13Keys.G22; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) script.SingleKeyPressed((G13Keys)k);
                    else script.SingleKeyReleased((G13Keys)k);
                }

            for (ulong k = (ulong)G13Keys.J1; k <= (ulong)G13Keys.J3; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) script.SingleKeyPressed((G13Keys)k);
                    else script.SingleKeyReleased((G13Keys)k);
                }

            for (ulong k = (ulong)G13Keys.M1; k <= (ulong)G13Keys.M4; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) script.SingleKeyPressed((G13Keys)k);
                    else script.SingleKeyReleased((G13Keys)k);
                }

            for (ulong k = (ulong)G13Keys.L1; k <= (ulong)G13Keys.L5; k <<= 1)
                if ((keys & k) > 0)
                {
                    if (pressed) script.SingleKeyPressed((G13Keys)k);
                    else script.SingleKeyReleased((G13Keys)k);
                }

            if ((keys & (ulong)G13Keys.L6) > 0)
            {
                if (pressed) script.SingleKeyPressed(G13Keys.L6);
                else script.SingleKeyReleased(G13Keys.L6);
            }
        }
    }
}
