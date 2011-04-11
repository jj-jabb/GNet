using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.MacroSystem
{
    public class KeyTap : Step
    {
        public KeyTap(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
        }

        public KeyTap(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
        }
    }
}
