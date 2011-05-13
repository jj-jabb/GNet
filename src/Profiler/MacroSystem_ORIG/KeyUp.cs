using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class KeyUp : Step
    {
        public KeyUp(ScanCode scanCode)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
            toString = "KeyUp(" + scanCode.ToString() + ")";
        }

        public KeyUp(char key)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
            toString = "KeyUp(" + key + ")";
        }
    }
}
