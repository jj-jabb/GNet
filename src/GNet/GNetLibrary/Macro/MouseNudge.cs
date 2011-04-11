using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
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
        }
    }
}
