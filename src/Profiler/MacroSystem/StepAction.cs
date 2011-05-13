using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.PInvoke;

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
        int hashCode;

        InputWrapper[] inputs;
        InputWrapper[] release;

        public InputWrapper[] Inputs 
        {
            get { return inputs; }
            set
            {
                inputs = value;

                if (inputs != null)
                    release = InputWrapper.GetRelease(inputs);

                hashCode = InputWrapper.CalcHashCode(inputs);
            }
        }

        public virtual void Run()
        {
            if (inputs != null)
                Interop.SendInput((uint)inputs.Length, inputs);
        }

        public override bool Equals(object obj)
        {
            StepAction other = obj as StepAction;
            if (other == null)
                return false;

            if (inputs == null || other.inputs == null)
                return false;

            if (inputs.Length != other.inputs.Length)
                return false;

            for (int i = 0; i < inputs.Length; i++)
                if (!inputs[i].Equals(other.inputs[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
