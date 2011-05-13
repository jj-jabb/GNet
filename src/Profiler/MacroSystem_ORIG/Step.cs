using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    [XmlInclude(typeof(Delay))]
    [XmlInclude(typeof(KeyDown))]
    public abstract class Step : IStep
    {
        //bool notEnabled;
        int hashCode;

        InputWrapper[] inputs;
        InputWrapper[] release;

        protected string toString;

        public override StepType Type { get { return StepType.Step; } }
        //public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        //public long Timestamp { get; set; }
        //public int Cooldown { get; set; }

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

        public override InputWrapper[] Run()
        {
            if (inputs != null)
                Interop.SendInput((uint)inputs.Length, inputs);

            return release;
        }

        public override bool Equals(object obj)
        {
            Step other = obj as Step;
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

        public override string ToString()
        {
            return toString;
        }
    }
}
