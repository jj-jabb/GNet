using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class KeyUp : Step
    {
        public KeyUp(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
        }

        public KeyUp(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
        }
    }
}
