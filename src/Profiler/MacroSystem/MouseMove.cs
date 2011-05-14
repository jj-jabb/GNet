using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(MouseMoveAbsolute))]
    [XmlInclude(typeof(MouseMoveRelative))]
    public abstract class MouseMove : StepActionInput
    {
        public MouseMove() { }

        public MouseMove(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override StepActionType Type { get { return StepActionType.Action; } }

        public int X { get; set; }
        public int Y { get; set; }

        protected abstract string MoveType { get; }

        public override string ToString()
        {
            return "MouseMove " + MoveType + ": " + X + ", " + Y;
        }
    }

    public class MouseMoveAbsolute : MouseMove
    {
        public MouseMoveAbsolute() { }
        public MouseMoveAbsolute(int x, int y) : base(x, y) { }

        protected override string MoveType { get { return "Absolute"; } }
    }

    public class MouseMoveRelative : MouseMove
    {
        public MouseMoveRelative() { }
        public MouseMoveRelative(int x, int y) : base(x, y) { }

        protected override string MoveType { get { return "Relative"; } }
    }
}
