using System;
using System.Xml.Serialization;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    [XmlInclude(typeof(Macro))]
    [XmlInclude(typeof(Step))]
    public abstract class IStep
    {
        bool notEnabled;

        public abstract StepType Type { get; }

        public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        public int Cooldown { get; set; }
        public long Timestamp { get; set; }

        public abstract InputWrapper[] Run();
    }
}
