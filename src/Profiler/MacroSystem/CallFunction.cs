using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class CallFunction : Step
    {
        public override StepActionType Type
        {
            get { return StepActionType.CallFunction; }
        }
    }
}
