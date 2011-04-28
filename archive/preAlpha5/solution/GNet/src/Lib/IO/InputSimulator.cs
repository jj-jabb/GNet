using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

using GNet.Lib.PInvoke;

namespace GNet.Lib.IO
{
    public class InputSimulator
    {
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

        BackgroundWorker keyRepeater;
        RepeatingKey rkey;

        public InputSimulator()
        {
            keyRepeater = new BackgroundWorker() { WorkerSupportsCancellation = true };
            keyRepeater.DoWork += new DoWorkEventHandler(keyRepeater_DoWork);
            keyRepeater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(keyRepeater_RunWorkerCompleted);

            rkey = RepeatingKey.Empty;
        }

        void keyRepeater_DoWork(object sender, DoWorkEventArgs e)
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
        }

        void keyRepeater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (rkey.IsEmpty == false)
                keyRepeater.RunWorkerAsync();

            rkey = RepeatingKey.Empty;
        }

        void RepeatKey(RepeatingKey keyInfo)
        {
            rkey = keyInfo;
            rkey.IsRunning = false;
            rkey.IsEmpty = false;

            if (!keyRepeater.IsBusy)
                keyRepeater.RunWorkerAsync();
        }

        public static InputWrapper KeyWrapper(char c, bool keyUp = false, bool extended = false)
        {
            var vKCode = Interop.VkKeyScan(c);
            var scanCode = (ScanCode)Interop.MapVirtualKey((uint)(vKCode & 0x00ff), 0);

            return new InputWrapper
            {
                Type = SendInputType.Keyboard,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Keyboard = new KeyboardInputData
                    {
                        Key = vKCode,
                        Scan = scanCode,
                        Flags =
                            (keyUp ? KeyboardFlags.KeyUp : 0) |
                            (extended ? KeyboardFlags.ExtendedKey : 0)
                    }
                }
            };
        }

        public static InputWrapper KeyWrapper(ScanCode scanCode, bool keyUp = false, bool extended = false)
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

        public static InputWrapper KeyWrapper(KeyboardInputData data)
        {
            return new InputWrapper
            {
                Type = SendInputType.Keyboard,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Keyboard = data
                }
            };
        }

        public static InputWrapper MouseWrapper(MouseEventFlags flags, int data)
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

        public static uint DoKeyDown(ScanCode scanCode, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(scanCode, false, extended)
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint DoKeyUp(ScanCode scanCode, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(scanCode, true, extended)
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint DoKeyDown(char c, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(c, false, extended)
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint DoKeyUp(char c, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(c, true, extended)
            };

            return Interop.SendInput(1, inputData);
        }

        public uint KeyDown(ScanCode scanCode, bool extended = false, bool repeat = false, int initialPauseTime = 500, int repeatPauseTime = 30)
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


        public uint KeyUp(ScanCode scanCode, bool extended = false)
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

        public uint KeyTap(ScanCode scanCode, bool extended = false)
        {
            var inputData = new InputWrapper[]
            { 
                KeyWrapper(scanCode, false, extended),
                KeyWrapper(scanCode, true, extended)
            };

            return Interop.SendInput(2, inputData);
        }

        public uint KeyDown(char c, bool extended = false)
        {
            return DoKeyDown(c, extended);
        }


        public uint KeyUp(char c, bool extended = false)
        {
            return DoKeyUp(c, extended);
        }

        public uint KeyTap(char c, bool extended = false)
        {
            var inputData = new InputWrapper[]
            { 
                KeyWrapper(c, false, extended),
                KeyWrapper(c, true, extended)
            };

            return Interop.SendInput(2, inputData);
        }

        public static uint MouseDown(MouseDownFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton)
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint MouseUp(MouseUpFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton)
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint MouseTap(MouseTapFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton),
                MouseWrapper((MouseEventFlags)((int)flags << 1), xButton)
            };

            return Interop.SendInput(2, inputData);
        }

        /// <summary>
        /// Simulate a mouse wheel event
        /// </summary>
        /// <param name="amount">The amount of wheel movement (120 == one click)</param>
        /// <returns>1 if successful, 0 if Interop.SendInput failed</returns>
        public static uint MouseWheel(int amount)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper(MouseEventFlags.Wheel, amount)
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint MouseMoveTo(int x, int y, bool virtualDesk = false)
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

            return Interop.SendInput(1, inputData);
        }

        public static uint MouseMoveTo(Win32Point p, bool virtualDesk = false)
        {
            return MouseMoveTo(p.X, p.Y, virtualDesk);
        }

        public static uint MouseMoveToPixel(int x, int y)
        {
            var size = ScreenSize;

            var inputData = new InputWrapper[]
            {
                new InputWrapper
                {
                    Type = SendInputType.Mouse,
                    MKH = new MouseKeyboardHardwareUnion
                    {
                        Mouse = new MouseInputData
                        {
                            X = x * 65536 / size.X + 1,
                            Y = y * 65536 / size.Y + 1,
                            Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                        }
                    }
                }
            };

            return Interop.SendInput(1, inputData);
        }

        public static uint MouseMoveToPixel(Win32Point p)
        {
            return MouseMoveToPixel(p.X, p.Y);
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

            return Interop.SendInput(1, inputData);
        }

        public static uint MouseMoveBy(Win32Point p)
        {
            return MouseMoveBy(p.X, p.Y);
        }

        public static Win32Point MouseScreenPos
        {
            get
            {
                Win32Point p = new Win32Point();
                if (!Interop.GetCursorPos(ref p))
                    System.Diagnostics.Debug.WriteLine("GetCursorPos returned false");
                return p;
            }
        }

        public static Win32Point ScreenSize
        {
            get
            {
                return new Win32Point
                {
                    X = Interop.GetSystemMetrics(SystemMetric.SM_CXSCREEN),
                    Y = Interop.GetSystemMetrics(SystemMetric.SM_CYSCREEN)
                };
            }
        }

        /// <summary>
        /// Gets the absolute position on the primary monitor
        /// </summary>
        public static Win32Point MouseAbsolutePos
        {
            get
            {
                var pos = MouseScreenPos;
                var scr = ScreenSize;

                return new Win32Point
                {
                    X = (int)Math.Ceiling(pos.X * 65536 / (double)scr.X),
                    Y = (int)Math.Ceiling(pos.Y * 65536 / (double)scr.Y)
                };
            }
        }

        public static uint Write(string text)
        {
            List<InputWrapper> input = new List<InputWrapper>();
            for (int i = 0; i < text.Length; i++)
            {
                var sci = AsciiToScanCode(text[i]);

                if (sci.IsShifted)
                    input.Add(KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode }));
                input.Add(KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                input.Add(KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = KeyboardFlags.KeyUp }));
                if (sci.IsShifted)
                    input.Add(KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode | KeyboardFlags.KeyUp }));
            }

            return Interop.SendInput((uint)input.Count, input.ToArray());
        }

        public static ScanCodeInfo AsciiToScanCode(char ascii)
        {
            var VKCode = Interop.VkKeyScan(ascii);

            return new ScanCodeInfo
            {
                VkKey = VKCode,
                ScanCode = (ScanCode)Interop.MapVirtualKey((uint)(VKCode & 0x00ff), 0),
                IsShifted = VKCode > 0x00ff
            };
        }
    }
}
