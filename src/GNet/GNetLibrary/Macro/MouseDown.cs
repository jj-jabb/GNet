using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class MouseDown : Step
    {
        MouseUp reverse;

        public MouseDown(int button)
        {
            int data = 0;
            switch (button)
            {
                case 0:
                case 1:
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.LeftDown, data) };
                    break;

                case 2:
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.RightDown, data) };
                    break;

                case 3:
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.MiddleDown, data) };
                    break;

                default:
                    data = button - 3;
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.XDown, data) };
                    break;
            }

            reverse = new MouseUp(button);
        }

        public override Step Reverse
        {
            get
            {
                return reverse;
            }
        }
    }
}
