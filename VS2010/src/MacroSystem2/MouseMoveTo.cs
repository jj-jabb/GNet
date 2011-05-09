using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class MouseMoveTo : Step
    {
        public MouseMoveTo(int x, int y)
        {
            Inputs = new InputWrapper[]
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

            toString = "MouseMoveTo(" + x + ", " + y + ")";
        }
    }
}
