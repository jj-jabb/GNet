using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class MouseWheel : StepAction
    {
        public MouseWheel() { }

        public MouseWheel(int delta)
        {
            Delta = delta;
        }

        public override StepActionType Type { get { return StepActionType.Action; } }

        public int Delta { get; set; }

        public override string ToString()
        {
            return "MouseWheel Delta: " + Delta;
        }
    }
}
