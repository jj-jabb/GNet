using System;
using System.Collections.Generic;
using System.Text;

using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem_ORIG
{
    public class Macro : IStep
    {
        List<IStep> steps;
        //bool notEnabled;
        bool noRelease;
        int hashCode;

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

        //public bool IsEnabled { get { return !notEnabled; } set { notEnabled = !value; } }
        //public long Timestamp { get; set; }
        //public int Cooldown { get; set; }

        public bool Release { get { return !noRelease; } set { noRelease = value; } }
        public int Priority { get; set; }
        public bool Interrupt { get; set; }
        public int LoopCount { get; set; }
        public string Name { get; set; }
        public List<IStep> Steps { get { return steps; } set { SetSteps(steps); } }

        public override StepType Type { get { return StepType.Macro; } }
        public int Count { get { return steps.Count; } }
        public IStep this[int index] { get { return steps[index]; } }

        private int currentIndex;
        private int currentLoop;

        internal void ResetSteps() { currentIndex = 0; }
        internal void ResetLoop() { currentLoop = 0; }
        internal void Reset() { currentIndex = 0; currentLoop = 0; }

        internal IStep CurrentStep
        {
            get
            {
                IStep step = null;

                if (currentIndex < steps.Count)
                {
                    step = steps[currentIndex];
                }

                return step;
            }
        }

        internal void IncStep()
        {
            currentIndex++;
        }

        internal void IncLoop()
        {
            currentLoop++;
        }

        internal int CurrentLoop
        {
            get { return currentLoop; }
        }

        public override InputWrapper[] Run()
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
