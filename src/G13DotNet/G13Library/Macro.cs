using System;
using System.Collections.Generic;
using System.Text;

namespace G13Library
{
    public class Macro
    {
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
            int button;
        }

        public class MouseWheel : Step
        {
            int amount;
        }

        public class MouseSavePos : Step
        {
            string name;
        }

        public class MouseRecallPos : Step
        {
            string name;
        }

        public class MouseMoveTo : Step
        {
            int x;
            int y;
        }

        public class MouseNudge : Step
        {
            int x;
            int y;
        }

        public class WriteText : Step
        {
            string text;
        }
    }
}
