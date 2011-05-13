using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class MouseUp : Step
    {
        public MouseUp(int button)
        {
            int data = 0;
            switch (button)
            {
                case 0:
                case 1:
                    Inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.LeftUp, data) };
                    toString = "MouseUp(" + MouseEventFlags.LeftUp.ToString() + ")";
                    break;

                case 2:
                    Inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.RightUp, data) };
                    toString = "MouseUp(" + MouseEventFlags.RightUp.ToString() + ")";
                    break;

                case 3:
                    Inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.MiddleUp, data) };
                    toString = "MouseUp(" + MouseEventFlags.MiddleUp.ToString() + ")";
                    break;

                default:
                    data = button - 3;
                    Inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.XUp, data) };
                    toString = "MouseUp(" + MouseEventFlags.XUp.ToString() + ", " + data + ")";
                    break;
            }
        }
    }
}
