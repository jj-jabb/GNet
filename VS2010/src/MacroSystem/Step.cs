using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public abstract class Step : IEquatable<Step>
    {
        protected InputWrapper[] inputs;
        protected string toString;

        public InputWrapper[] Inputs { get { return inputs; } }
        public virtual Step Cleanup { get { return null; } }

        public bool IsEnabled { get; set; }
        public int RunTimestamp { get; set; }
        public int Cooldown { get; set; }

        public virtual bool Equals(Step other)
        {
            if (inputs == null || other.inputs == null)
                return false;

            if (inputs.Length != other.inputs.Length)
                return false;

            for (int i = 0; i < inputs.Length; i++)
                if (!inputs[i].Equals(other.inputs[i]))
                    return false;

            return true;
        }

        public virtual void Run()
        {
            if (inputs != null)
                Interop.SendInput((uint)inputs.Length, inputs);
        }

        public override string ToString()
        {
            return toString;
        }
    }
}
