using System;

using GNet.Lib.IO;
using GNet.Lib.PInvoke;

namespace GNet.Lib.MacroSystem
{
    public class MouseRecallPos : Step
    {
        string name;

        public MouseRecallPos(string name)
        {
            this.name = name;

            toString = "MouseRecallPos(" + name + ")";
        }

        public override void Run()
        {
            Win32Point p;
            if (Macro.SavedPoints.TryGetValue(name, out p))
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
                                    X = p.X,
                                    Y = p.Y,
                                    Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                                }
                            }
                        }
                    };

                Interop.SendInput((uint)inputs.Length, inputs);
            }
        }
    }
}
