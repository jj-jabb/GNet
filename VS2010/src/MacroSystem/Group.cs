using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.MacroSystem
{
    public class Group : Step
    {
        List<Step> steps;
        Stack<Step> cleanupSteps;
        Dictionary<Step, Step> cleanupLookup;

        public Group()
        {
            steps = new List<Step>();
        }

        public Group(IEnumerable<Step> steps)
        {
            this.steps = new List<Step>(steps);
        }

        public Group(params Step[] steps)
        {
            this.steps = new List<Step>(steps);
        }

        public override bool Equals(Step other)
        {
            Group group = other as Group;
            if (group == null)
                return false;

            if (group.steps.Count != steps.Count)
                return false;

            for (int i = 0; i < steps.Count; i++)
                if (!steps[i].Equals(group.steps[i]))
                    return false;

            return true;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}
