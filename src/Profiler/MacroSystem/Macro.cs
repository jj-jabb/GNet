using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class Macro : Step
    {
        bool release = true;
        bool endOnKeyup = true;
        List<Step> steps;

        private int currentIndex;
        private int currentLoop;

        public Macro() { }

        public string Name { get; set; }
        public bool Release { get { return release; } set { release = value; } }
        public bool EndOnKeyup { get { return endOnKeyup; } set { endOnKeyup = value; } }
        public int Priority { get; set; }
        public bool IsInterrupting { get; set; }
        public int LoopCount { get; set; }
        public List<Step> Steps { get { return steps; } set { steps = value; } }

        public bool ShouldSerializeRelease() { return !Release; }
        public bool ShouldSerializeEndOnKeyup() { return !EndOnKeyup; }
        public bool ShouldSerializePriority() { return Priority > 0; }
        public bool ShouldSerializeIsInterrupting() { return IsInterrupting; }
        public bool ShouldSerializeLoopCount() { return LoopCount > 0; }

        public override StepActionType Type { get { return StepActionType.Macro; } }

        public int Count { get { return steps.Count; } }
        public Step this[int index] { get { return steps[index]; } }
        //public bool Cancel { get; set; }

        internal void ResetSteps() { currentIndex = 0; }
        internal void ResetLoop() { currentLoop = 0; }
        internal void Reset() { currentIndex = 0; currentLoop = 0; } //Cancel = false; }

        internal Step CurrentStep
        {
            get
            {
                if (steps == null)
                    return null;

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
