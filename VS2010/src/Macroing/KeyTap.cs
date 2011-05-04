using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public class KeyTap : Step
    {
        public KeyTap(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode), InputSimulator.KeyWrapper(scanCode, true) };
            toString = "KeyTap(" + scanCode.ToString() + ")";
        }

        public KeyTap(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key), InputSimulator.KeyWrapper(key, true) };
            toString = "KeyTap(" + key + ")";
        }
    }
}
