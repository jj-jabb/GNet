using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using GNet.Hid;
using GNet.LgLcd;
using GNet.IO;

namespace GNet.Scripting
{
    public class G13Device : Device
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

        const int Logitech = 0x046d;
        const int G13 = 0xc21c;

        IG13DeviceScript _script;

        object scriptLock;

        JoystickPosition joystick;
        KeyRepeater keyRepeater;

        int mKeyState = 0;
        G13KeyState currentState = new G13KeyState();

        G13Device()
            : base(Logitech, G13)
        {
            scriptLock = new object();
            keyRepeater = new KeyRepeater();
        }

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
                    if (_script != null && _script.IsRunning)
                        _script.Stop();

                    _script = value;
                }
            }
        }

        public override bool Start()
        {
            if (!base.Start())
                return false;

            keyRepeater.Start();

            return true;
        }

        public override void Stop()
        {
            keyRepeater.ClearKey();
            keyRepeater.Stop();
            base.Stop();
        }

        public override void Dispose()
        {
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

        protected override void ReadThread_DataRead(DeviceData data)
        {
            DecodeData(data);
            base.ReadThread_DataRead(data);
        }

        protected void DecodeData(DeviceData data)
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
