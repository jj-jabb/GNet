using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using GNet.Hid;
using GNet.Lib.IO;
using GNet.Lib.PInvoke;

using WFKeys = System.Windows.Forms.Keys;
using System.Threading;
using GNet.Lib;

namespace GNet.Scripting
{
    public partial class G13Script
    {
        static G13Script()
        {
            foreach (var name in Enum.GetNames(typeof(WFKeys)))
                nameToWFKey[name.ToLower()] = (WFKeys)Enum.Parse(typeof(WFKeys), name);
        }

        static Dictionary<string, WFKeys> nameToWFKey = new Dictionary<string, WFKeys>();
        static Dictionary<string, ScanCode> nameToCode = new Dictionary<string, ScanCode>
        {
            { "escape", (ScanCode)0x01 },
            { "f1", (ScanCode)0x3b },
            { "f2", (ScanCode)0x3c },
            { "f3", (ScanCode)0x3d },
            { "f4", (ScanCode)0x3e },
            { "f5", (ScanCode)0x3f },
            { "f6", (ScanCode)0x40 },
            { "f7", (ScanCode)0x41 },
            { "f8", (ScanCode)0x42 },
            { "f9", (ScanCode)0x43 },
            { "f10", (ScanCode)0x44 },
            { "f11", (ScanCode)0x57 },
            { "f12", (ScanCode)0x58 },
            { "f13", (ScanCode)0x64 },
            { "f14", (ScanCode)0x65 },
            { "f15", (ScanCode)0x66 },
            { "f16", (ScanCode)0x67 },
            { "f17", (ScanCode)0x68 },
            { "f18", (ScanCode)0x69 },
            { "f19", (ScanCode)0x6a },
            { "f20", (ScanCode)0x6b },
            { "f21", (ScanCode)0x6c },
            { "f22", (ScanCode)0x6d },
            { "f23", (ScanCode)0x6e },
            { "f24", (ScanCode)0x76 },
            { "printscreen", (ScanCode)0x137 },
            { "scrolllock", (ScanCode)0x46 },
            { "pause", (ScanCode)0x146 },
            { "tilde", (ScanCode)0x29 },
            { "1", (ScanCode)0x02 },
            { "2", (ScanCode)0x03 },
            { "3", (ScanCode)0x04 },
            { "4", (ScanCode)0x05 },
            { "5", (ScanCode)0x06 },
            { "6", (ScanCode)0x07 },
            { "7", (ScanCode)0x08 },
            { "8", (ScanCode)0x09 },
            { "9", (ScanCode)0x0a },
            { "0", (ScanCode)0x0b },
            { "minus", (ScanCode)0x0c },
            { "equal", (ScanCode)0x0d },
            { "backspace", (ScanCode)0x0e },
            { "tab", (ScanCode)0x0f },
            { "q", (ScanCode)0x10 },
            { "w", (ScanCode)0x11 },
            { "e", (ScanCode)0x12 },
            { "r", (ScanCode)0x13 },
            { "t", (ScanCode)0x14 },
            { "y", (ScanCode)0x15 },
            { "u", (ScanCode)0x16 },
            { "I", (ScanCode)0x17 },
            { "o", (ScanCode)0x18 },
            { "p", (ScanCode)0x19 },
            { "lbracket", (ScanCode)0x1a },
            { "rbracket", (ScanCode)0x1b },
            { "backslash", (ScanCode)0x2b },
            { "capslock", (ScanCode)0x3a },
            { "a", (ScanCode)0x1e },
            { "s", (ScanCode)0x1f },
            { "d", (ScanCode)0x20 },
            { "f", (ScanCode)0x21 },
            { "g", (ScanCode)0x22 },
            { "h", (ScanCode)0x23 },
            { "j", (ScanCode)0x24 },
            { "k", (ScanCode)0x25 },
            { "l", (ScanCode)0x26 },
            { "semicolon", (ScanCode)0x27 },
            { "quote", (ScanCode)0x28 },
            { "enter", (ScanCode)0x1c },
            { "lshift", (ScanCode)0x2a },
            { "non_us_slash", (ScanCode)0x56 },
            { "z", (ScanCode)0x2c },
            { "x", (ScanCode)0x2d },
            { "c", (ScanCode)0x2e },
            { "v", (ScanCode)0x2f },
            { "b", (ScanCode)0x30 },
            { "n", (ScanCode)0x31 },
            { "m", (ScanCode)0x32 },
            { "comma", (ScanCode)0x33 },
            { "period", (ScanCode)0x34 },
            { "slash", (ScanCode)0x35 },
            { "rshift", (ScanCode)0x36 },
            { "lctrl", (ScanCode)0x1d },
            { "lgui", (ScanCode)0x15b },
            { "lalt", (ScanCode)0x38 },
            { "spacebar", (ScanCode)0x39 },
            { "ralt", (ScanCode)0x138 },
            { "rgui", (ScanCode)0x15c },
            { "appkey", (ScanCode)0x15d },
            { "rctrl", (ScanCode)0x11d },
            { "insert", (ScanCode)0x152 },
            { "home", (ScanCode)0x147 },
            { "pageup", (ScanCode)0x149 },
            { "delete", (ScanCode)0x153 },
            { "end", (ScanCode)0x14f },
            { "pagedown", (ScanCode)0x151 },
            { "up", (ScanCode)0x148 },
            { "left", (ScanCode)0x14b },
            { "down", (ScanCode)0x150 },
            { "right", (ScanCode)0x14d },
            { "numlock", (ScanCode)0x45 },
            { "numslash", (ScanCode)0x135 },
            { "numminus", (ScanCode)0x4a },
            { "num7", (ScanCode)0x47 },
            { "num8", (ScanCode)0x48 },
            { "num9", (ScanCode)0x49 },
            { "numplus", (ScanCode)0x4e },
            { "num4", (ScanCode)0x4b },
            { "num5", (ScanCode)0x4c },
            { "num6", (ScanCode)0x4d },
            { "num1", (ScanCode)0x4f },
            { "num2", (ScanCode)0x50 },
            { "num3", (ScanCode)0x51 },
            { "numenter", (ScanCode)0x11c },
            { "num0", (ScanCode)0x52 },
            { "numperiod", (ScanCode)0x53 }
        };


        int mKeyState = 0;

        public int GetMKeyState(string family = "lhc")
        {
            return mKeyState;
        }

        public void SetMKeyState(double dkey, string family = "lhc")
        {
            int mkey = (int)dkey;
            mKeyState = mkey;

            switch (mkey)
            {
                case 1:
                    SingleKeyPressed(G13Keys.M1);
                    break;

                case 2:
                    SingleKeyPressed(G13Keys.M2);
                    break;

                case 3:
                    SingleKeyPressed(G13Keys.M3);
                    break;
            }

            switch (mkey)
            {
                case 1:
                    SingleKeyReleased(G13Keys.M1);
                    break;

                case 2:
                    SingleKeyReleased(G13Keys.M2);
                    break;

                case 3:
                    SingleKeyReleased(G13Keys.M3);
                    break;
            }
        }

        #region PressKey

        public void PressKey(IList<ScanCode> scanCodes)
        {
            var inputData = new InputWrapper[scanCodes.Count];
            for (int i = 0; i < inputData.Length; i++)
                inputData[i] = InputSimulator.KeyWrapper(scanCodes[i], false, ((int)scanCodes[i] & 0x100) == 0x100);

            Interop.SendInput((uint)inputData.Length, inputData);

            Device.KeyRepeater.KeyDown(scanCodes[scanCodes.Count - 1]);
        }

        public void PressKey(params ScanCode[] scanCodes)
        {
            PressKey(GetCodes(scanCodes));
        }

        public void PressKey(params ushort[] scanCodes)
        {
            PressKey(GetCodes(scanCodes));
        }

        public void PressKey(params string[] keyNames)
        {
            PressKey(GetCodes(keyNames));
        }

        public void PressKey(params object[] keys)
        {
            PressKey(GetCodes(keys));
        }

        #endregion

        #region ReleaseKey

        public void ReleaseKey(IList<ScanCode> scanCodes)
        {
            var inputData = new InputWrapper[scanCodes.Count];
            for (int i = 0; i < inputData.Length; i++)
            {
                Device.KeyRepeater.KeyUp(scanCodes[i]);
                inputData[i] = InputSimulator.KeyWrapper(scanCodes[i], true, ((int)scanCodes[i] & 0x100) == 0x100);
            }

            Interop.SendInput((uint)inputData.Length, inputData);
        }

        public void ReleaseKey(params ScanCode[] scanCodes)
        {
            ReleaseKey(GetCodes(scanCodes));
        }

        public void ReleaseKey(params ushort[] scanCodes)
        {
            ReleaseKey(GetCodes(scanCodes));
        }

        public void ReleaseKey(params string[] keyNames)
        {
            ReleaseKey(GetCodes(keyNames));
        }

        public void ReleaseKey(params object[] keys)
        {
            ReleaseKey(GetCodes(keys));
        }

        #endregion

        #region PressAndReleaseKey

        public void PressAndReleaseKey(IList<ScanCode> scanCodes)
        {
            var inputData = new List<InputWrapper>();
            for (int i = 0; i < scanCodes.Count; i++)
            {
                var extended = ((int)scanCodes[i] & 0x100) == 0x100;
                inputData.Add(InputSimulator.KeyWrapper(scanCodes[i], false, extended));
                inputData.Add(InputSimulator.KeyWrapper(scanCodes[i], true, extended));
            }

            Interop.SendInput((uint)inputData.Count, inputData.ToArray());
        }

        public void PressAndReleaseKey(params ScanCode[] scanCodes)
        {
            PressAndReleaseKey(GetCodes(scanCodes));
        }

        public void PressAndReleaseKey(params ushort[] scanCodes)
        {
            PressAndReleaseKey(GetCodes(scanCodes));
        }

        public void PressAndReleaseKey(params string[] keyNames)
        {
            PressAndReleaseKey(GetCodes(keyNames));
        }

        public void PressAndReleaseKey(params object[] keys)
        {
            PressAndReleaseKey(GetCodes(keys));
        }

        #endregion

        public bool IsModifierPressed(string keyName)
        {
            switch (keyName)
            {
                case "lshift":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_LSHIFT);

                case "rshift":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_RSHIFT);

                case "shift":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_SHIFT);

                case "lalt":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_LMENU);

                case "ralt":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_RMENU);

                case "alt":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_MENU);

                case "lctrl":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_LCONTROL);

                case "rctrl":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_RCONTROL);

                case "ctrl":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_CONTROL);

                default:
                    return false;
            }
        }

        public void PressMouseButton(int button)
        {
            switch (button)
            {
                case 1:
                    InputSimulator.MouseDown(MouseDownFlags.LeftDown);
                    break;

                case 2:
                    InputSimulator.MouseDown(MouseDownFlags.MiddleDown);
                    break;

                case 3:
                    InputSimulator.MouseDown(MouseDownFlags.RightDown);
                    break;

                case 4:
                    InputSimulator.MouseDown(MouseDownFlags.XDown, 1);
                    break;

                case 5:
                    InputSimulator.MouseDown(MouseDownFlags.XDown, 2);
                    break;
            }
        }

        public void ReleaseMouseButton(int button)
        {
            switch (button)
            {
                case 1:
                    InputSimulator.MouseUp(MouseUpFlags.LeftUp);
                    break;

                case 2:
                    InputSimulator.MouseUp(MouseUpFlags.MiddleUp);
                    break;

                case 3:
                    InputSimulator.MouseUp(MouseUpFlags.RightUp);
                    break;

                case 4:
                    InputSimulator.MouseUp(MouseUpFlags.XUp, 1);
                    break;

                case 5:
                    InputSimulator.MouseUp(MouseUpFlags.XUp, 2);
                    break;
            }
        }

        public void PressAndReleaseMouseButton(int button)
        {
            switch (button)
            {
                case 1:
                    InputSimulator.MouseTap(MouseTapFlags.LeftTap);
                    break;

                case 2:
                    InputSimulator.MouseTap(MouseTapFlags.MiddleTap);
                    break;

                case 3:
                    InputSimulator.MouseTap(MouseTapFlags.RightTap);
                    break;

                case 4:
                    InputSimulator.MouseTap(MouseTapFlags.XTap, 1);
                    break;

                case 5:
                    InputSimulator.MouseTap(MouseTapFlags.XTap, 2);
                    break;
            }
        }

        public bool IsMouseButtonPressed(int button)
        {
            switch (button)
            {
                case 1: return Interop.IsKeyPressed(VirtualKeyStates.VK_LBUTTON);
                case 2: return Interop.IsKeyPressed(VirtualKeyStates.VK_MBUTTON);
                case 3: return Interop.IsKeyPressed(VirtualKeyStates.VK_RBUTTON);
                case 4: return Interop.IsKeyPressed(VirtualKeyStates.VK_XBUTTON1);
                case 5: return Interop.IsKeyPressed(VirtualKeyStates.VK_XBUTTON2);

                default: return false;
            }
        }

        public void MoveMouseTo(int x, int y)
        {
            InputSimulator.MouseMoveTo(x, y);
        }

        public void MoveMouseWheel(int clicks)
        {
            InputSimulator.MouseWheel(clicks * 120);
        }

        public void MoveMouseRelative(int x, int y)
        {
            InputSimulator.MouseMoveBy(x, y);
        }

        /// <summary>
        /// Untested
        /// </summary>
        public void MoveMouseToVirtual(int x, int y)
        {
            InputSimulator.MouseMoveTo(x, y, true);
        }

        public void GetMousePosition(out int x, out int y)
        {
            var p = InputSimulator.MouseAbsolutePos;
            x = p.X;
            y = p.Y;
        }

        public bool IsKeyLockOn(string key)
        {
            switch (key)
            {
                case "scrolllock":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_SCROLL);

                case "capslock":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_CAPITAL);

                case "numlock":
                    return Interop.IsKeyPressed(VirtualKeyStates.VK_NUMLOCK);

                default:
                    return false;
            }
        }

        IList<ScanCode> GetCodes(ushort[] scanCodes)
        {
            IList<ScanCode> codes = new List<ScanCode>();
            if (scanCodes != null)
                for (int i = 0; i < scanCodes.Length; i++)
                    codes.Add((ScanCode)scanCodes[i]);

            return codes;
        }

        IList<ScanCode> GetCodes(ScanCode[] scanCodes)
        {
            IList<ScanCode> codes = new List<ScanCode>();
            if (scanCodes != null)
                for (int i = 0; i < scanCodes.Length; i++)
                    codes.Add(scanCodes[i]);

            return codes;
        }

        IList<ScanCode> GetCodes(string[] keyNames)
        {
            ScanCode code;
            IList<ScanCode> codes = new List<ScanCode>();

            if (keyNames != null)
                for (int i = 0; i < keyNames.Length; i++)
                    if (nameToCode.TryGetValue(keyNames[i], out code))
                        codes.Add(code);

            return codes;
        }

        IList<ScanCode> GetCodes(object[] keys)
        {
            ScanCode code;
            IList<ScanCode> codes = new List<ScanCode>();

            if (keys != null)
                foreach (var key in keys)
                    switch (Type.GetTypeCode(key.GetType()))
                    {
                        case TypeCode.String:
                        case TypeCode.Object:
                            if (nameToCode.TryGetValue(key.ToString(), out code))
                                codes.Add(code);
                            break;

                        case TypeCode.Single:
                            codes.Add((ScanCode)(float)key);
                            break;

                        case TypeCode.Double:
                            codes.Add((ScanCode)(double)key);
                            break;

                        case TypeCode.Int16:
                            codes.Add((ScanCode)(short)key);
                            break;

                        case TypeCode.Int32:
                            codes.Add((ScanCode)(int)key);
                            break;

                        case TypeCode.Int64:
                            codes.Add((ScanCode)(long)key);
                            break;

                        case TypeCode.UInt16:
                            codes.Add((ScanCode)(ushort)key);
                            break;

                        case TypeCode.UInt32:
                            codes.Add((ScanCode)(uint)key);
                            break;

                        case TypeCode.UInt64:
                            codes.Add((ScanCode)(ulong)key);
                            break;
                    }

            return codes;
        }
    }
}
