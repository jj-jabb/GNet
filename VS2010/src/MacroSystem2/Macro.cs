using System;
using System.Collections.Generic;
using System.Text;

using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class Macro : IStep
    {
        List<IStep> steps;
        bool notEnabled;
        int hashCode;
        string queue = "";

        public Macro()
        {
            steps = new List<IStep>();
            hashCode = 0;
        }

        public Macro(IEnumerable<IStep> steps)
        {
            SetSteps(steps);
        }

        public Macro(params IStep[] steps)
        {
            SetSteps(steps);
        }

        public void SetSteps(IEnumerable<IStep> steps)
        {
            this.steps = new List<IStep>(steps);

            OatHash hash = new OatHash();
            foreach (var step in this.steps)
                hash.Hash(step.GetHashCode());

            hashCode = hash.HashCode;
        }

        public StepType Type { get { return StepType.Macro; } }
        public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        public bool NoRelease { get; set; }
        public int Priority { get; set; }
        public bool TerminateCurrent { get; set; }
        public long Timestamp { get; set; }
        public int Cooldown { get; set; }
        public int Count { get { return steps.Count; } }
        public string Name { get; set; }
        public string Queue { get { return queue; } set { queue = value; } }
        public IStep this[int index] { get { return steps[index]; } }

        private int currentIndex;

        internal void ResetStepIndex() { currentIndex = 0; }
        internal IStep CurrentStep
        {
            get
            {
                if (currentIndex < steps.Count)
                    return steps[currentIndex++];

                return null;
            }
        }

        public InputWrapper[] Run()
        {
            return null;
        }

        public override bool Equals(object other)
        {
            Macro group = other as Macro;
            if (group == null)
                return false;

            if (group.steps.Count != steps.Count)
                return false;

            for (int i = 0; i < steps.Count; i++)
                if (!steps[i].Equals(group.steps[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
