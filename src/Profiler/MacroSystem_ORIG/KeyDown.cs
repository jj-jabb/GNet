using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class KeyDown : Step
    {
        ScanCode scanCode;
        char key;

        public KeyDown()
        {
        }

        public KeyDown(ScanCode scanCode)
        {
            ScanCode = scanCode;
        }

        public KeyDown(char key)
        {
            Key = key;
        }

        public ScanCode ScanCode
        {
            get { return scanCode; }
            set
            {
                scanCode = value;
                Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(scanCode) };
                toString = "KeyDown(" + scanCode.ToString() + ")";
            }
        }

        public char Key
        {
            get { return key; }
            set
            {
                Inputs = new InputWrapper[] { InputSimulator.KeyWrapper(key) };
                toString = "KeyDown(" + key + ")";
                key = value;
            }
        }
    }
}
