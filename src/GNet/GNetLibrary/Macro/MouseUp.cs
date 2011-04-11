using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class MouseUp : Step
    {
        public MouseUp(int button)
        {
            int data = 0;
            MouseEventFlags flags;
            switch (button)
            {
                case 0:
                case 1:
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
}
