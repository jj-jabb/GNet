using System;
using System.Collections.Generic;
using System.Text;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class Macro
    {
        public static readonly Dictionary<string, Win32Point> SavedPoints = new Dictionary<string, Win32Point>();

        public abstract class Step
        {
            internal InputWrapper[] inputs;
        }

        public class KeyDown : Step
        {
            public KeyDown(ScanCode scanCode)
            {
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
            }

            public KeyDown(char key)
            {
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
            }
        }

        public class KeyUp : Step
        {
            public KeyUp(ScanCode scanCode)
            {
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
            }

            public KeyUp(char key)
            {
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
            }
        }

        public class KeyTap : Step
        {
            public KeyTap(ScanCode scanCode)
            {
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
            }

            public KeyTap(char key)
            {
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
                inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
            }
        }

        public class MouseDown : Step
        {
            public MouseDown(int button)
            {
                int data = 0;
                MouseEventFlags flags;
                switch (button)
                {
                    case 0:
                    case 1 :
                        flags = MouseEventFlags.LeftDown;
                        break;

                    case 2:
                        flags = MouseEventFlags.RightDown;
                        break;

                    case 3:
                        flags = MouseEventFlags.MiddleDown;
                        break;

                    default:
                        flags = MouseEventFlags.XDown;
                        data = button - 3;
                        break;
                }

                inputs = new InputWrapper[] { InputSimulator.MouseWrapper(flags, data) };
            }
        }

        public class MouseUp : Step
        {
            public MouseUp(int button)
            {
                int data = 0;
                MouseEventFlags flags;
                switch (button)
                {
                    case 0:
                    case 1 :
                        flags = MouseEventFlags.LeftUp;
                        break;

                    case 2:
                        flags = MouseEventFlags.RightUp;
                        break;

                    case 3:
                        flags = MouseEventFlags.MiddleUp;
                        break;

                    default:
                        flags = MouseEventFlags.XUp;
                        data = button - 3;
                        break;
                }

                inputs = new InputWrapper[] { InputSimulator.MouseWrapper(flags, data) };
            }
        }

        public class MouseTap : Step
        {
            public MouseTap(int button)
            {
                int data = 0;

                switch (button)
                {
                    case 0:
                    case 1 :
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.LeftDown, data) };
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.LeftUp, data) };
                        break;

                    case 2:
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.RightDown, data) };
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.RightUp, data) };
                        break;

                    case 3:
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.MiddleDown, data) };
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.MiddleUp, data) };
                        break;

                    default:
                        data = button - 3;
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.XDown, data) };
                        inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.XUp, data) };
                        break;
                }
            }
        }

        public class MouseWheel : Step
        {
            public MouseWheel(int amount)
            {
                inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.Wheel, amount) };
            }
        }

        public class MouseSavePos : Step
        {
            public MouseSavePos(string name)
            {
                SavedPoints[name] = InputSimulator.MouseAbsolutePos;
            }
        }

        public class MouseRecallPos : Step
        {
            public MouseRecallPos(string name)
            {
                Win32Point p;
                if (SavedPoints.TryGetValue(name, out p))
                    inputs = new InputWrapper[]
                    {
                        new InputWrapper
                        {
                            Type = SendInputType.Mouse,
                            MKH = new MouseKeyboardHardwareUnion
                            {
                                Mouse = new MouseInputData
                                {
                                    X = p.X,
                                    Y = p.Y,
                                    Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                                }
                            }
                        }
                    };
            }
        }

        public class MouseMoveTo : Step
        {
            public MouseMoveTo(int x, int y)
            {
                inputs = new InputWrapper[]
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
                                Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                            }
                        }
                    }
                };
            }
        }

        public class MouseNudge : Step
        {
            public MouseNudge(int x, int y)
            {
                inputs = new InputWrapper[]
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
            }
        }

        public class WriteText : Step
        {
            public WriteText(string text)
            {
                List<InputWrapper> input = new List<InputWrapper>();
                for (int i = 0; i < text.Length; i++)
                {
                    var sci = InputSimulator.AsciiToScanCode(text[i]);

                    if (sci.IsShifted)
                        input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode }));
                    input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                    input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = KeyboardFlags.KeyUp }));
                    if (sci.IsShifted)
                        input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode | KeyboardFlags.KeyUp }));
                }
            }
        }
    }
}
