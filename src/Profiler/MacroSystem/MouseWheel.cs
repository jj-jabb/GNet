using System;
using System.Collections.Generic;
using System.Text;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    public class MouseWheel : StepActionInput
    {
        int _delta;

        public MouseWheel() { }

        public MouseWheel(int delta)
        {
            Delta = delta;
        }

        public int Delta 
        {
            get { return _delta; }
            set
            {
                _delta = value;
                Inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.Wheel, _delta) };
            }
        }

        public override string ToString()
        {
            return "MouseWheel Delta: " + Delta;
        }
    }
}
