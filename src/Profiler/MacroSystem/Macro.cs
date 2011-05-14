using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class Macro : Step
    {
        bool noRelease;
        List<Step> steps;

        public Macro() { }

        public override StepActionType Type { get { return StepActionType.Macro; } }
        public List<Step> Steps { get { return steps; } set { steps = value; } }

        public bool Release { get { return !noRelease; } set { noRelease = value; } }
        public int Priority { get; set; }
        public bool Interrupt { get; set; }
        public int LoopCount { get; set; }
        public string Name { get; set; }

        public int Count { get { return steps.Count; } }
        public Step this[int index] { get { return steps[index]; } }

        private int currentIndex;
        private int currentLoop;

        internal void ResetSteps() { currentIndex = 0; }
        internal void ResetLoop() { currentLoop = 0; }
        internal void Reset() { currentIndex = 0; currentLoop = 0; }

        internal Step CurrentStep
        {
            get
            {
                Step step = null;

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
    }
}
