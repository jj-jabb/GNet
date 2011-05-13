using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(MousePositionSave))]
    [XmlInclude(typeof(MousePositionRecall))]
    public abstract class MousePosition : StepAction
    {
        public MousePosition()
        {
        }

        public MousePosition(string name)
        {
            PositionName = name;
        }

        public override StepActionType Type { get { return StepActionType.Action; } }

        public string PositionName { get; set; }

        protected abstract string PositionType { get; }

        public override string ToString()
        {
            return "MousePosition " + PositionType + ": " + PositionName;
        }
    }

    public class MousePositionSave : MousePosition
    {
        public MousePositionSave() { }
        public MousePositionSave(string name) : base(name) { }

        protected override string PositionType { get { return "Save"; } }
    }

    public class MousePositionRecall : MousePosition
    {
        public MousePositionRecall() { }
        public MousePositionRecall(string name) : base(name) { }

        protected override string PositionType { get { return "Recall"; } }
    }
}
