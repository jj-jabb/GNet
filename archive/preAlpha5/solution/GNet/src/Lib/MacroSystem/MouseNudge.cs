using System;

using GNet.Lib.IO;
using GNet.Lib.PInvoke;

namespace GNet.Lib.MacroSystem
{
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

            toString = "MouseNudge(" + x + ", " + y + ")";
        }
    }
}
