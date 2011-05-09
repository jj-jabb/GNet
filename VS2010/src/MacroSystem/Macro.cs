using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public class Macro
    {
        public static readonly Dictionary<string, Win32Point> SavedPoints = new Dictionary<string, Win32Point>();
        public static ISynchronizeInvoke SynchronizeInvoke { get; set; }

        List<Step> steps;
        Stack<Step> cleanupSteps;
        Dictionary<Step, Step> cleanupLookup;
        //List<Step> processed;

        Timer timer;
        int currentStep;
        bool isRunning;
        bool cleanup;

        public Macro()
        {
            steps = new List<Step>();
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.SynchronizingObject = SynchronizeInvoke;
            timer.AutoReset = false;
        }

        public Macro(params Step[] steps)
            : this()
        {
            foreach (var step in steps)
                this.steps.Add(step);
        }

        public List<Step> Steps { get { return steps; } }

        public bool IsRunning { get { return isRunning; } }
        public bool IsInterruptable { get; set; }

        public void Run()
        {
            if (isRunning)
                return;

            isRunning = true;
            cleanup = false;
            currentStep = 0;

            cleanupSteps = new Stack<Step>();
            cleanupLookup = new Dictionary<Step, Step>();
            //processed = new List<Step>();

            RunImpl();
        }

        void RunImpl()
        {
            Step step;
            Step rstep;
            Delay delay;

            while (currentStep < steps.Count)
            {
                step = steps[currentStep];
                rstep = step.Release;
                if (rstep != null)
                {
                    if (cleanupLookup.ContainsKey(rstep))
                    {
                        // disable the last rstep that's on the stack
                        cleanupLookup[rstep].IsEnabled = false;
                    }

                    cleanupLookup[rstep] = rstep;
                    cleanupSteps.Push(rstep);
                }

                //processed.Add(step);

                currentStep++;

                System.Diagnostics.Debug.WriteLine(step.ToString());
                
                delay = step as Delay;
                if (delay != null)
                {
                    timer.Interval = delay.Milliseconds;
                    timer.Start();
                    return;
                }
                else
                    step.Run();
            }

            isRunning = false;

            System.Diagnostics.Debug.WriteLine("");
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (cleanup)
                return;

            RunImpl();
        }

        public void Cleanup()
        {
            cleanup = true;

            Stack<Step> reverseSteps = new Stack<Step>();
            Step rstep;

            while (cleanupSteps.Count > 0)
            {
                rstep = cleanupSteps.Pop();

                System.Diagnostics.Debug.WriteLine(rstep.ToString());

                rstep.Run();
            }


            //bool reversed;

            //// check to see if reversable steps aren't already reversed in the macro
            //for (int i = 0; i < processed.Count; i++)
            //{
            //    rstep = processed[i].Cleanup;
            //    if (rstep != null)
            //    {
            //        reversed = false;
            //        for (int j = i + 1; j < processed.Count; j++)
            //            if (rstep.Equals(processed[j]))
            //            {
            //                reversed = true;
            //                break;
            //            }

            //        if (!reversed)
            //            reverseSteps.Push(rstep);
            //    }
            //}

            //while (reverseSteps.Count > 0)
            //{
            //    rstep = reverseSteps.Pop();

            //    System.Diagnostics.Debug.WriteLine(rstep.ToString());
                
            //    rstep.Run();
            //}

            isRunning = false;

            System.Diagnostics.Debug.WriteLine("");
        }
    }
}
