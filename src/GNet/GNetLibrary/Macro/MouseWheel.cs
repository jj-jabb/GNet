using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class MouseWheel : Step
    {
        public MouseWheel(int amount)
        {
            inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.Wheel, amount) };
        }
    }
}
