using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
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
