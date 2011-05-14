using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(Delay))]
    [XmlInclude(typeof(Macro))]
    [XmlInclude(typeof(StepAction))]
    [XmlInclude(typeof(CallFunction))]
    [XmlInclude(typeof(WaitFunction))]
    public abstract class Step
    {
        bool notEnabled;

        public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        public int Cooldown { get; set; }
        public string FunctionName { get; set; }

        [XmlIgnore]
        public long Timestamp { get; set; }

        public abstract StepActionType Type { get; }
    }
}
