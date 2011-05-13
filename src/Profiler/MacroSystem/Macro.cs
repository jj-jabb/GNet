using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class Macro : Step
    {
        List<Step> steps;

        public Macro() { }

        public override StepActionType Type { get { return StepActionType.Macro; } }
        public List<Step> Steps { get { return steps; } set { steps = value; } }
    }
}
