using System;

using GNet.Lib.IO;
using GNet.Lib.PInvoke;

namespace GNet.Lib.MacroSystem
{
    public class KeyDown : Step
    {
        KeyUp reverse;

        public KeyDown(ScanCode scanCode)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
            reverse = new KeyUp(scanCode);
            toString = "KeyDown(" + scanCode.ToString() + ")";
        }

        public KeyDown(char key)
        {
            inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
            reverse = new KeyUp(key);
            toString = "KeyDown(" + key + ")";
        }

        public override Step Cleanup
        {
            get
            {
                return reverse;
            }
        }
    }
}
