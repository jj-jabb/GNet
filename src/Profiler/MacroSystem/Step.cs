using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(Delay))]
    [XmlInclude(typeof(Macro))]
    [XmlInclude(typeof(StepAction))]
    public abstract class Step
    {
        bool notEnabled;

        public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        public int Cooldown { get; set; }

        public abstract StepActionType Type { get; }
    }
}
