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

        public override StepType Type { get { return StepType.Delay; } }

        public double Milliseconds { get; set;}
        public double? RandomRange { get; set; }

        public override string ToString()
        {
            return "Delay: " + Milliseconds;
        }
    }
}
