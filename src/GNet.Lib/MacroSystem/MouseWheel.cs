using System;

using GNet.Lib.IO;
using GNet.Lib.PInvoke;

namespace GNet.Lib.MacroSystem
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
