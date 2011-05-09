using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class MouseRecallPos : Step
    {
        string name;

        public MouseRecallPos(string name)
        {
            this.name = name;

            toString = "MouseRecallPos(" + name + ")";
        }

        public override InputWrapper[] Run()
        {
            Win32Point p;

            if (MacroManager.SavedPoints.TryGetValue(name, out p))
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
                                    X = p.X,
                                    Y = p.Y,
                                    Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                                }
                            }
                        }
                    };

                return base.Run();
            }

            return null;
        }
    }
}
