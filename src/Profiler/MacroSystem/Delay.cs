using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class Delay : Step
    {
        public Delay() { }

        public Delay(double milliseconds)
        {
            Milliseconds = milliseconds;
        }

        public override StepActionType Type { get { return StepActionType.Delay; } }

        public double Milliseconds { get; set;}

        public override string ToString()
        {
            return "Delay: " + Milliseconds;
        }
    }
}
