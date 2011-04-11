using System;
using System.Collections.Generic;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.MacroSystem
{
    public class WriteText : Step
    {
        public WriteText(string text)
        {
            List<InputWrapper> input = new List<InputWrapper>();
            for (int i = 0; i < text.Length; i++)
            {
                var sci = InputSimulator.AsciiToScanCode(text[i]);

                if (sci.IsShifted)
                    input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode }));
                input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = KeyboardFlags.KeyUp }));
                if (sci.IsShifted)
                    input.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode | KeyboardFlags.KeyUp }));
            }
        }
    }
}
