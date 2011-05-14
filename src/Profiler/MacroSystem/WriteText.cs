using System;
using System.Collections.Generic;
using System.Text;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    public class WriteText : StepActionInput
    {
        string _text;

        public WriteText() { }

        public WriteText(string text)
        {
            Text = text;
        }

        public string Text 
        {
            get { return Text; }
            set
            {
                _text = value;

                var inputs = new List<InputWrapper>();

                for (int i = 0; i < value.Length; i++)
                {
                    var sci = InputSimulator.AsciiToScanCode(value[i]);

                    if (sci.IsShifted)
                        inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode }));
                    inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                    inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = KeyboardFlags.KeyUp }));
                    if (sci.IsShifted)
                        inputs.Add(InputSimulator.KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode | KeyboardFlags.KeyUp }));
                }

                Inputs = inputs.ToArray();
            }
        }

        public override string ToString()
        {
            return "WriteText: " + _text;
        }
    }
}
