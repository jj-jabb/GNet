using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class CallFunction : Step
    {
        public override StepType Type
        {
            get { return StepType.CallFunction; }
        }
    }
}
