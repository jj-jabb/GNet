using System;
using System.Collections.Generic;
using System.Text;

using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class Delay : IStep
    {
        bool notEnabled;
        double milliseconds;

        protected string toString;

        public StepType Type { get { return StepType.Delay; } }
        public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        public long Timestamp { get; set; }
        public int Cooldown { get; set; }

        public Delay(double milliseconds)
        {
            this.milliseconds = milliseconds;
            toString = "Delay(" + milliseconds + ")";
        }

        public double Milliseconds { get { return milliseconds; } } 

        InputWrapper[] IStep.Run()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return toString;
        }
    }
}
