using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class WaitFunction : Step
    {
        public override StepType Type
        {
            get { return StepType.WaitFunction; }
        }
    }
}
