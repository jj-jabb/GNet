using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class KeyDown : Step
    {
        KeyUp reverse;

        public KeyDown(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
            reverse = new KeyUp(scanCode);
        }

        public KeyDown(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
            reverse = new KeyUp(key);
        }
    }
}
