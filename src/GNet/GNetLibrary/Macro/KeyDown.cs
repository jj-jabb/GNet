using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class KeyDown : Step
    {
        public KeyDown(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
            reverse = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode, true) };
        }

        public KeyDown(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
            reverse = new InputWrapper[] { InputSimulator.KeyWrapper(key, true) };
        }
    }
}
