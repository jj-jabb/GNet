using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class KeyTap : Step
    {
        public KeyTap(ScanCode scanCode)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode), InputSimulator.KeyWrapper(scanCode, true) };
            toString = "KeyTap(" + scanCode.ToString() + ")";
        }

        public KeyTap(char key)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key), InputSimulator.KeyWrapper(key, true) };
            toString = "KeyTap(" + key + ")";
        }
    }
}
