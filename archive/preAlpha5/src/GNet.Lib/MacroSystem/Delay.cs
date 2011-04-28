using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Lib.MacroSystem
{
    public class Delay : Step
    {
        double milliseconds;

        public Delay(double milliseconds)
        {
            this.milliseconds = milliseconds;
            toString = "Delay(" + milliseconds + ")";
        }

        public double Milliseconds { get { return milliseconds; } } 

        public override void Run()
        {
        }
    }
}
