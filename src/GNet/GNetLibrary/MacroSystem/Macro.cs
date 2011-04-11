using System;
using System.Collections.Generic;
using System.Text;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.MacroSystem
{
    public class Macro
    {
        public static readonly Dictionary<string, Win32Point> SavedPoints = new Dictionary<string, Win32Point>();

        List<Step> steps;

        public Macro()
        {
            steps = new List<Step>();
        }

        public void Run()
        {
            foreach (var step in steps)
            {
                System.Diagnostics.Debug.WriteLine(step.GetType().Name);
                step.Run();
            }
        }

        public void Reverse()
        {
            Stack<Step> reverseSteps = new Stack<Step>();
            Step rstep;
            bool reversed;

            // check to see if reversable steps aren't already reversed in the macro
            for (int i = 0; i < steps.Count; i++)
            {
                rstep = steps[i].Reverse;
                if (rstep != null)
                {
                    reversed = false;
                    for (int j = i + 1; j < steps.Count; j++)
                        if (rstep.Equals(steps[j]))
                        {
                            reversed = true;
                            break;
                        }

                    if (!reversed)
                        reverseSteps.Push(rstep);
                }
            }

            while (reverseSteps.Count > 0)
            {
                rstep = reverseSteps.Pop();
                System.Diagnostics.Debug.WriteLine(rstep.GetType().Name);
                rstep.Run();
            }
        }
    }
}
