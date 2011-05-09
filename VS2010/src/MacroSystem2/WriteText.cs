using System;
using System.Collections.Generic;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class WriteText : Step
    {
        InputWrapper[] inputs;
        public WriteText(string text)
        {
            List<InputWrapper>  inputs = new List<InputWrapper>();
            for (int i = 0; i < text.Length; i++)
            {
                var sci = InputSimulator.AsciiToScanCode(text[i]);

                if (sci.IsShifted)
                    inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode }));
                inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = KeyboardFlags.KeyUp }));
                if (sci.IsShifted)
                    inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode | KeyboardFlags.KeyUp }));
            }

            this.inputs = inputs.ToArray();
            toString = "WriteText(" + text + ")";
        }

        public override InputWrapper[] Run()
        {
            if (inputs != null)
                Interop.SendInput((uint)inputs.Length, inputs);

            return null;
        }
    }
}
