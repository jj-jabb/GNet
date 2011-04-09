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
            public uint MouseData;
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
        static Queue<RepeatingKey> repeatingKeyQueue;
        static RepeatingKey currentRepeatingKey;

        struct RepeatingKey
        {
            public ScanCode Code;
            public bool Extended;
            public int InitialPauseTime;
            public int RepeatPauseTime;

            public static RepeatingKey Empty = new RepeatingKey();
        }

        static InputManager()
        {
            InputWrapperSize = Marshal.SizeOf(typeof(InputWrapper));

            keyRepeater = new BackgroundWorker() { WorkerSupportsCancellation = true };
            keyRepeater.DoWork += new DoWorkEventHandler(keyRepeater_DoWork);
            keyRepeater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(keyRepeater_RunWorkerCompleted);

            repeatingKeyQueue = new Queue<RepeatingKey>();
        }

        static void keyRepeater_DoWork(object sender, DoWorkEventArgs e)
        {
            RepeatingKey args;

            if (e.Argument is RepeatingKey)
                args = (RepeatingKey)e.Argument;
            else
                return;

            DoKeyDown(args.Code, args.Extended);
            Thread.Sleep(args.InitialPauseTime);
            DoKeyUp(args.Code, args.Extended);

            while (!keyRepeater.CancellationPending)
            {
                DoKeyDown(args.Code, args.Extended);
                Thread.Sleep(args.RepeatPauseTime);
                DoKeyUp(args.Code, args.Extended);
            }
        }

        static void keyRepeater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            currentRepeatingKey = RepeatingKey.Empty;

            if (!keyRepeater.IsBusy && repeatingKeyQueue.Count > 0)
            {
                currentRepeatingKey = repeatingKeyQueue.Dequeue();
                keyRepeater.RunWorkerAsync(currentRepeatingKey);
            }
        }

        static void RepeatKey(RepeatingKey keyInfo)
        {
            repeatingKeyQueue.Enqueue(keyInfo);
            if (!keyRepeater.IsBusy && repeatingKeyQueue.Count > 0)
            {
                currentRepeatingKey = repeatingKeyQueue.Dequeue();
                keyRepeater.RunWorkerAsync(currentRepeatingKey);
            }
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
            if (repeat)
                RepeatKey(new RepeatingKey { Code = scanCode, Extended = extended, InitialPauseTime = initialPauseTime, RepeatPauseTime = repeatPauseTime });
            else
                return DoKeyDown(scanCode, extended);

            return 0;
        }


        public static uint KeyUp(ScanCode scanCode, bool extended = false)
        {
            if (scanCode == currentRepeatingKey.Code && extended == currentRepeatingKey.Extended)
                keyRepeater.CancelAsync();
            else
                return DoKeyUp(scanCode, extended);

            return 0;
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

        public static uint MouseDown(MouseDownFlags flags, uint xButton = 0)
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
                            Flags = (MouseEventFlags)flags,
                            MouseData = xButton
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseUp(MouseUpFlags flags, uint xButton = 0)
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
                            Flags = (MouseEventFlags)flags,
                            MouseData = xButton
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }
    }
}
