using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public abstract class Step : IEquatable<Step>
    {
        protected InputWrapper[] inputs;
        protected InputWrapper[] reverse;

        public InputWrapper[] Inputs { get { return inputs; } }
        public InputWrapper[] Reverse { get { return reverse; } }

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

        public void Run()
        {
            if (inputs != null)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                }
            }
        }
    }
}
