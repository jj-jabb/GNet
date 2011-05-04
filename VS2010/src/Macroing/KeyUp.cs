using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public class KeyUp : Step
    {
        public KeyUp(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
            toString = "KeyUp(" + scanCode.ToString() + ")";
        }

        public KeyUp(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
            toString = "KeyUp(" + key + ")";
        }
    }
}
