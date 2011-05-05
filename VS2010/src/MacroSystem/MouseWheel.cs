using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public class MouseWheel : Step
    {
        public MouseWheel(int amount)
        {
            inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.Wheel, amount) };
            toString = "MouseWheel(" + amount + ")";
        }
    }
}
