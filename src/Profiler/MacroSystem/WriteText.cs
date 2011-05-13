using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class WriteText : StepAction
    {
        public WriteText() { }

        public WriteText(string text)
        {
            Text = text;
        }

        public override StepActionType Type { get { return StepActionType.Action; } }

        public string Text { get; set; }

        public override string ToString()
        {
            return "WriteText: " + Text;
        }
    }
}
