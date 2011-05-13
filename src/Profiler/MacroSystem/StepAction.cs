using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(KeyChar))]
    [XmlInclude(typeof(KeyScanCode))]
    [XmlInclude(typeof(MouseButton))]
    [XmlInclude(typeof(MouseMove))]
    [XmlInclude(typeof(MousePosition))]
    [XmlInclude(typeof(MouseWheel))]
    [XmlInclude(typeof(WriteText))]
    public abstract class StepAction : Step
    {
        public string FunctionName { get; set; }
    }
}
