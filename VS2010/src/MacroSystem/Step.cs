using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public abstract class Step : IEquatable<Step>
    {
        bool notEnabled;
        int? hashCode;

        protected InputWrapper[] inputs;
        protected string toString;

        public InputWrapper[] Inputs { get { return inputs; } }
        public virtual Step Release { get { return null; } }

        public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        public long RunTimestamp { get; set; }
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

        public override bool Equals(object obj)
        {
            Step step = obj as Step;
            if (step == null)
                return false;

            return Equals(step);
        }

        public override int GetHashCode()
        {
            if (!hashCode.HasValue)
            {
                OatHash hash = new OatHash();
                if (inputs == null)
                    hashCode = 0;
                else
                    foreach (var input in inputs)
                        hash.Hash(input.GetHashCode());

                hashCode = hash.HashCode;
            }

            return hashCode.Value;
        }

        public virtual void Run()
        {
            if (!IsEnabled)
                return;

            long elapsedMs = MacroManager.Stopwatch.ElapsedMilliseconds;

            if (RunTimestamp > 0 && RunTimestamp + Cooldown < elapsedMs)
                return;

            RunTimestamp = elapsedMs;

            if (inputs != null)
                Interop.SendInput((uint)inputs.Length, inputs);
        }

        public override string ToString()
        {
            return toString;
        }
    }
}
