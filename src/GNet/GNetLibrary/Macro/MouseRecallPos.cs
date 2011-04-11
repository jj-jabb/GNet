using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class MouseRecallPos : Step
    {
        public MouseRecallPos(string name)
        {
            Win32Point p;
            if (Macro.SavedPoints.TryGetValue(name, out p))
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
}
