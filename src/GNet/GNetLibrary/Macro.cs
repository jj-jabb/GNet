using System;
using System.Collections.Generic;
using System.Text;

namespace G13Library
{
    public class Macro
    {
        public static readonly Dictionary<string, InputManager.Win32Point> SavedPoints = new Dictionary<string, InputManager.Win32Point>();

        public abstract class Step
        {
            internal InputManager.InputWrapper[] inputs;
        }

        public class KeyDown : Step
        {
            public KeyDown(InputManager.ScanCode scanCode)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(scanCode) };
            }

            public KeyDown(char key)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(key) };
            }
        }

        public class KeyUp : Step
        {
            public KeyUp(InputManager.ScanCode scanCode)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(scanCode, true) };
            }

            public KeyUp(char key)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(key, true) };
            }
        }

        public class KeyTap : Step
        {
            public KeyTap(InputManager.ScanCode scanCode)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(scanCode) };
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(scanCode, true) };
            }

            public KeyTap(char key)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(key) };
                inputs = new InputManager.InputWrapper[] { InputManager.KeyWrapper(key, true) };
            }
        }

        public class MouseDown : Step
        {
            public MouseDown(int button)
            {
                int data = 0;
                InputManager.MouseEventFlags flags;
                switch (button)
                {
                    case 0:
                    case 1 :
                        flags = InputManager.MouseEventFlags.LeftDown;
                        break;

                    case 2:
                        flags = InputManager.MouseEventFlags.RightDown;
                        break;

                    case 3:
                        flags = InputManager.MouseEventFlags.MiddleDown;
                        break;

                    default:
                        flags = InputManager.MouseEventFlags.XDown;
                        data = button - 3;
                        break;
                }

                inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(flags, data) };
            }
        }

        public class MouseUp : Step
        {
            public MouseUp(int button)
            {
                int data = 0;
                InputManager.MouseEventFlags flags;
                switch (button)
                {
                    case 0:
                    case 1 :
                        flags = InputManager.MouseEventFlags.LeftUp;
                        break;

                    case 2:
                        flags = InputManager.MouseEventFlags.RightUp;
                        break;

                    case 3:
                        flags = InputManager.MouseEventFlags.MiddleUp;
                        break;

                    default:
                        flags = InputManager.MouseEventFlags.XUp;
                        data = button - 3;
                        break;
                }

                inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(flags, data) };
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
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.LeftDown, data) };
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.LeftUp, data) };
                        break;

                    case 2:
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.RightDown, data) };
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.RightUp, data) };
                        break;

                    case 3:
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.MiddleDown, data) };
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.MiddleUp, data) };
                        break;

                    default:
                        data = button - 3;
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.XDown, data) };
                        inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.XUp, data) };
                        break;
                }
            }
        }

        public class MouseWheel : Step
        {
            public MouseWheel(int amount)
            {
                inputs = new InputManager.InputWrapper[] { InputManager.MouseWrapper(InputManager.MouseEventFlags.Wheel, amount) };
            }
        }

        public class MouseSavePos : Step
        {
            public MouseSavePos(string name)
            {
                SavedPoints[name] = InputManager.MouseAbsolutePos;
            }
        }

        public class MouseRecallPos : Step
        {
            public MouseRecallPos(string name)
            {
                InputManager.Win32Point p;
                if (SavedPoints.TryGetValue(name, out p))
                    inputs = new InputManager.InputWrapper[]
                    {
                        new InputManager.InputWrapper
                        {
                            Type = InputManager.SendInputType.Mouse,
                            MKH = new InputManager.MouseKeyboardHardwareUnion
                            {
                                Mouse = new InputManager.MouseInputData
                                {
                                    X = p.X,
                                    Y = p.Y,
                                    Flags = InputManager.MouseEventFlags.Move | InputManager.MouseEventFlags.Absolute
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
                inputs = new InputManager.InputWrapper[]
                {
                    new InputManager.InputWrapper
                    {
                        Type = InputManager.SendInputType.Mouse,
                        MKH = new InputManager.MouseKeyboardHardwareUnion
                        {
                            Mouse = new InputManager.MouseInputData
                            {
                                X = x,
                                Y = y,
                                Flags = InputManager.MouseEventFlags.Move | InputManager.MouseEventFlags.Absolute
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
                inputs = new InputManager.InputWrapper[]
                {
                    new InputManager.InputWrapper
                    {
                        Type = InputManager.SendInputType.Mouse,
                        MKH = new InputManager.MouseKeyboardHardwareUnion
                        {
                            Mouse = new InputManager.MouseInputData
                            {
                                X = x,
                                Y = y,
                                Flags = InputManager.MouseEventFlags.Move
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
                List<InputManager.InputWrapper> input = new List<InputManager.InputWrapper>();
                for (int i = 0; i < text.Length; i++)
                {
                    var sci = InputManager.AsciiToScanCode(text[i]);

                    if (sci.IsShifted)
                        input.Add(InputManager.KeyWrapper(new InputManager.KeyboardInputData { Scan = InputManager.ScanCode.lshift, Flags = InputManager.KeyboardFlags.ScanCode }));
                    input.Add(InputManager.KeyWrapper(new InputManager.KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                    input.Add(InputManager.KeyWrapper(new InputManager.KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = InputManager.KeyboardFlags.KeyUp }));
                    if (sci.IsShifted)
                        input.Add(InputManager.KeyWrapper(new InputManager.KeyboardInputData { Scan = InputManager.ScanCode.lshift, Flags = InputManager.KeyboardFlags.ScanCode | InputManager.KeyboardFlags.KeyUp }));
                }
            }
        }
    }
}
