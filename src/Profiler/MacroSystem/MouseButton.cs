using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(MouseButtonDown))]
    [XmlInclude(typeof(MouseButtonUp))]
    [XmlInclude(typeof(MouseButtonTap))]
    public abstract class MouseButton : StepActionInput
    {
        public MouseButton() { }

        public MouseButton(int button)
        {
            Button = button;
        }

        public int Button { get; set; }

        protected abstract string ButtonType { get; }

        public override string ToString()
        {
            return "MouseButton " + ButtonType + ": " + Button;
        }
    }

    public class MouseButtonDown : MouseButton
    {
        public MouseButtonDown() { }
        public MouseButtonDown(int button) : base(button) { }

        protected override string ButtonType { get { return "Down"; } }
    }

    public class MouseButtonUp : MouseButton
    {
        public MouseButtonUp() { }
        public MouseButtonUp(int button) : base(button) { }

        protected override string ButtonType { get { return "Up"; } }
    }

    public class MouseButtonTap : MouseButton
    {
        public MouseButtonTap() { }
        public MouseButtonTap(int button) : base(button) { }

        protected override string ButtonType { get { return "Tap"; } }
    }
}
