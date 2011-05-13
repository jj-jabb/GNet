using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class MouseNudge : Step
    {
        public MouseNudge(int x, int y)
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
                                Flags = MouseEventFlags.Move
                            }
                        }
                    }
                };

            toString = "MouseNudge(" + x + ", " + y + ")";
        }
    }
}
