using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(StepActionInput))]
    [XmlInclude(typeof(MousePosition))]
    public abstract class StepAction : Step
    {
        public override StepType Type { get { return StepType.Action; } }
        public abstract void Run();
    }
}
