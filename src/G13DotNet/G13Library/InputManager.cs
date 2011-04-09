using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace G13Library
{
    public class InputManager
    {
        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern uint SendInput(uint numberInputs, [MarshalAs(UnmanagedType.LPArray)]InputWrapper[] inputs, int sizeOfStructure);

        #region pinvoke

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public Int32 X;
            public Int32 Y;

            public override string ToString()
            {
                return "Win32Point: " + X + ", " + Y;
            }
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberInputs, InputWrapper[] inputs, int sizeOfStructure);

        [StructLayout(LayoutKind.Sequential)]
        private struct InputWrapper
        {
            public SendInputType Type;
            public MouseKeyboardHardwareUnion MKH;
        }

        private enum SendInputType : int
        {
            Mouse = 0,
            Keyboard,
            Hardware
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MouseKeyboardHardwareUnion
        {
            [FieldOffset(0)]
            public MouseInputData Mouse;
            [FieldOffset(0)]
            public KeyboardInputData Keyboard;
            [FieldOffset(0)]
            public HardwareInputData Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInputData
        {
            public int X;
            public int Y;
            public int MouseData;
            public MouseEventFlags Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInputData
        {
            public ushort Key;
            public ScanCode Scan;
            public KeyboardFlags Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInputData
        {
            public int Msg;
            public short ParamL;
            public short ParamH;
        }

        [Flags]
        enum MouseDataFlags : uint
        {
            XButton1 = 0x0001,
            XButton2 = 0x0002
        }

        [Flags]
        enum KeyboardFlags : uint
        {
            ExtendedKey = 0x1,
            KeyUp = 0x2,
            Unicode = 0x4,
            ScanCode = 0x8
        }

        [Flags]
        enum MouseEventFlags : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesktop = 0x4000,
            Absolute = 0x8000
        }

        [Flags]
        public enum MouseDownFlags : uint
        {
            LeftDown = 0x0002,
            RightDown = 0x0008,
            MiddleDown = 0x0020,
            XDown = 0x0080
        }

        [Flags]
        public enum MouseUpFlags : uint
        {
            LeftUp = 0x0004,
            RightUp = 0x0010,
            MiddleUp = 0x0040,
            XUp = 0x0100
        }

        public enum ScanCode : ushort
        {
            Escape = 0x01,

            k1 = 0x02,
            k2 = 0x03,
            k3 = 0x04,
            k4 = 0x05,
            k5 = 0x06,
            k6 = 0x07,
            k7 = 0x08,
            k8 = 0x09,
            k9 = 0x0a,
            k0 = 0x0b,

            minus = 0x0c,
            equals = 0x0d,
            backspace = 0x0e,
            tab = 0x0f,

            q = 0x10,
            w = 0x11,
            e = 0x12,
            r = 0x13,
            t = 0x14,
            y = 0x15,
            u = 0x16,
            i = 0x17,
            o = 0x18,
            p = 0x19,
            lbracket = 0x1a,
            rbracket = 0x1b,
            enter = 0x1c,
            lcontrol = 0x1d,
            a = 0x1e,
            s = 0x1f,
            d = 0x20,
            f = 0x21,
            g = 0x22,
            h = 0x23,
            j = 0x24,
            k = 0x25,
            l = 0x26,
            semicolon = 0x27,
            apostrophe = 0x28,
            accentGrave = 0x29,
            lshift = 0x2a,
            backslash = 0x2b,
            z = 0x2c,
            x = 0x2d,
            c = 0x2e,
            v = 0x2f,
            b = 0x30,
            n = 0x31,
            m = 0x32,
            comma = 0x33,
            period = 0x34,
            forwardSlash = 0x35,
            rshift = 0x36,
            asterisk = 0x37,
            lalt = 0x38,
            space = 0x39,
            capital = 0x3a,
            f1 = 0x3b,
            f2 = 0x3c,
            f3 = 0x3d,
            f4 = 0x3e,
            f5 = 0x3f,
            f6 = 0x40,
            f7 = 0x41,
            f8 = 0x42,
            f9 = 0x43,
            f10 = 0x44,
            numlock = 0x45,
            scrollLock = 0x46,
            numpad7 = 0x47,
            numpad8 = 0x48,
            numpad9 = 0x49,
            numpadSubtract = 0x4a,
            numpad4 = 0x4b,
            numpad5 = 0x4c,
            numpad6 = 0x4d,
            numpadAdd = 0x4e,
            numpad1 = 0x4f,
            numpad2 = 0x50,
            numpad3 = 0x51,
            numpad0 = 0x52,
            numpadPeriod = 0x53,
            f11 = 0x57,
            f12 = 0x58,
        }

        #endregion

        static int InputWrapperSize;

        static BackgroundWorker keyRepeater;
        //static Queue<RepeatingKey> repeatingKeyQueue;
        static RepeatingKey rkey;

        struct RepeatingKey
        {
            public ScanCode Code;
            public bool Extended;
            public int InitialPauseTime;
            public int RepeatPauseTime;
            public bool IsRunning;
            public bool IsEmpty;

            public void Clear()
            {
                IsRunning = false;
                IsEmpty = true;
            }

            public static RepeatingKey Empty = new RepeatingKey() { IsEmpty = true };
        }

        static InputManager()
        {
            InputWrapperSize = Marshal.SizeOf(typeof(InputWrapper));

            keyRepeater = new BackgroundWorker() { WorkerSupportsCancellation = true };
            keyRepeater.DoWork += new DoWorkEventHandler(keyRepeater_DoWork);
            keyRepeater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(keyRepeater_RunWorkerCompleted);

            //repeatingKeyQueue = new Queue<RepeatingKey>();
            rkey = RepeatingKey.Empty;
        }

        static void keyRepeater_DoWork(object sender, DoWorkEventArgs e)
        {
            if (rkey.IsEmpty)
                return;

            while (!rkey.IsEmpty)
            {
                rkey.IsRunning = true;
                var code = rkey.Code;
                var ext = rkey.Extended;
                var initialPauseTime = rkey.InitialPauseTime;
                var repeatPauseTime = rkey.RepeatPauseTime;

                Thread.Sleep(initialPauseTime);
                while (rkey.IsRunning)
                {
                    DoKeyUp(code, ext);
                    if (rkey.IsRunning)
                        Thread.Sleep(repeatPauseTime);
                    if (rkey.IsRunning)
                        DoKeyDown(code, ext);
                }
            }

            //RepeatingKey args;

            //if (e.Argument is RepeatingKey)
            //    args = (RepeatingKey)e.Argument;
            //else
            //    return;

            //DoKeyDown(args.Code, args.Extended);
            //Thread.Sleep(args.InitialPauseTime);
            //DoKeyUp(args.Code, args.Extended);

            //while (!keyRepeater.CancellationPending)
            //{
            //    DoKeyDown(args.Code, args.Extended);
            //    Thread.Sleep(args.RepeatPauseTime);
            //    DoKeyUp(args.Code, args.Extended);
            //}
        }

        static void keyRepeater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (rkey.IsEmpty == false)
                keyRepeater.RunWorkerAsync();

            rkey = RepeatingKey.Empty;
        }

        static void RepeatKey(RepeatingKey keyInfo)
        {
            rkey = keyInfo;
            rkey.IsRunning = false;
            rkey.IsEmpty = false;

            if (!keyRepeater.IsBusy)
                keyRepeater.RunWorkerAsync();
        }

        static InputWrapper KeyWrapper(ScanCode scanCode, bool keyUp, bool extended)
        {
            return new InputWrapper
            {
                Type = SendInputType.Keyboard,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Keyboard = new KeyboardInputData
                    {
                        Scan = scanCode,
                        Flags =
                            KeyboardFlags.ScanCode |
                            (keyUp ? KeyboardFlags.KeyUp : 0) |
                            (extended ? KeyboardFlags.ExtendedKey : 0)
                    }
                }
            };
        }

        static InputWrapper MouseWrapper(MouseEventFlags flags, int data)
        {
            return new InputWrapper
            {
                Type = SendInputType.Mouse,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Mouse = new MouseInputData
                    {
                        Flags = flags,
                        MouseData = data
                    }
                }
            };
        }

        static uint DoKeyDown(ScanCode scanCode, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(scanCode, false, extended)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        static uint DoKeyUp(ScanCode scanCode, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(scanCode, true, extended)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint KeyDown(ScanCode scanCode, bool extended = false, bool repeat = false, int initialPauseTime = 500, int repeatPauseTime = 30)
        {
            uint retval;
            if (repeat)
            {
                retval = DoKeyDown(scanCode, extended);
                RepeatKey(new RepeatingKey { Code = scanCode, Extended = extended, InitialPauseTime = initialPauseTime, RepeatPauseTime = repeatPauseTime });
            }
            else
                retval = DoKeyDown(scanCode, extended);

            return retval;
        }


        public static uint KeyUp(ScanCode scanCode, bool extended = false)
        {
            uint retval;
            if (scanCode == rkey.Code && extended == rkey.Extended)
            {
                retval = DoKeyUp(scanCode, extended);
                rkey.Clear();
            }
            else
                retval = DoKeyUp(scanCode, extended);

            return retval;
        }

        public static uint KeyTap(ScanCode scanCode, bool extended = false)
        {
            var inputData = new InputWrapper[]
            { 
                KeyWrapper(scanCode, false, extended),
                KeyWrapper(scanCode, true, extended)
            };

            return SendInput(2, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseDown(MouseDownFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseUp(MouseUpFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        /// <summary>
        /// Simulate a mouse wheel event
        /// </summary>
        /// <param name="amount">The amount of wheel movement (120 == one click)</param>
        /// <returns>1 if successful, 0 if SendInput failed</returns>
        public static uint MouseWheel(int amount)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper(MouseEventFlags.Wheel, amount)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseMoveTo(int x, int y, bool virtualDesk)
        {
            var inputData = new InputWrapper[]
            {
                new InputWrapper
                {
                    Type = SendInputType.Mouse,
                    MKH = new MouseKeyboardHardwareUnion
                    {
                        Mouse = new MouseInputData
                        {
                            X = x,
                            Y = y,
                            Flags = MouseEventFlags.Move | MouseEventFlags.Absolute | (virtualDesk ? MouseEventFlags.VirtualDesktop : 0)
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseMoveBy(int x, int y)
        {
            var inputData = new InputWrapper[]
            {
                new InputWrapper
                {
                    Type = SendInputType.Mouse,
                    MKH = new MouseKeyboardHardwareUnion
                    {
                        Mouse = new MouseInputData
                        {
                            X = x,
                            Y = y,
                            Flags = MouseEventFlags.Move
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static Win32Point MouseScreenPos
        {
            get
            {
                Win32Point p = new Win32Point();
                if (!GetCursorPos(ref p))
                    System.Diagnostics.Debug.WriteLine("GetCursorPos returned false");
                return p;
            }
        }
    }
}
