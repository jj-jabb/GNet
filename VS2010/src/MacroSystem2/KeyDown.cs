using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class KeyDown : Step
    {
        public KeyDown(ScanCode scanCode)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
            toString = "KeyDown(" + scanCode.ToString() + ")";
        }

        public KeyDown(char key)
        {
            Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
            toString = "KeyDown(" + key + ")";
        }
    }
}
