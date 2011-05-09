using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class MouseWheel : Step
    {
        public MouseWheel(int amount)
        {
            Inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.Wheel, amount) };
            toString = "MouseWheel(" + amount + ")";
        }
    }
}
