using System;
using System.Collections.Generic;
using System.Text;

using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class Delay : IStep
    {
        //bool notEnabled;
        double milliseconds;

        protected string toString;

        public override StepType Type { get { return StepType.Delay; } }
        //public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        //public long Timestamp { get; set; }
        //public int Cooldown { get; set; }

        public Delay()
        {
            Milliseconds = 30;
        }

        public Delay(double milliseconds)
        {
            Milliseconds = milliseconds;
        }

        public double Milliseconds
        {
            get
            {
                return milliseconds;
            }
            set
            {
                milliseconds = value;
                toString = "Delay(" + milliseconds + ")";
            }
        }

        public override InputWrapper[] Run()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return toString;
        }
    }
}
