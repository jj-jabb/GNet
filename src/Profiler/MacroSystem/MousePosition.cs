using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    [XmlInclude(typeof(MousePositionSave))]
    [XmlInclude(typeof(MousePositionRecall))]
    public abstract class MousePosition : StepAction
    {
        public static readonly Dictionary<string, Win32Point> SavedPoints = new Dictionary<string, Win32Point>();

        public MousePosition()
        {
        }

        public MousePosition(string name)
        {
            PositionName = name;
        }

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

        public override void Run()
        {
            MousePosition.SavedPoints[PositionName] = InputSimulator.MouseAbsolutePos;
        }
    }

    public class MousePositionRecall : MousePosition
    {
        public MousePositionRecall() { }
        public MousePositionRecall(string name) : base(name) { }

        protected override string PositionType { get { return "Recall"; } }

        public override void Run()
        {
            if (PositionName == null)
                return;

            Win32Point p;

            if (MousePositionSave.SavedPoints.TryGetValue(PositionName, out p))
            {
                var inputs = new InputWrapper[]
                {
                    new InputWrapper
                    {
                        Type = SendInputType.Mouse,
                        MKH = new MouseKeyboardHardwareUnion
                        {
                            Mouse = new MouseInputData
                            {
                                X = p.X,
                                Y = p.Y,
                                Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                            }
                        }
                    }
                };

                Interop.SendInput((uint)inputs.Length, inputs);
            }
        }
    }
}
