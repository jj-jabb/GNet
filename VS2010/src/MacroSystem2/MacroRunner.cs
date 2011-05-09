using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Timers;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class MacroRunner
    {
        public static readonly Dictionary<string, Win32Point> SavedPoints = new Dictionary<string, Win32Point>();
        public static ISynchronizeInvoke SynchronizeInvoke { get; set; }

        Queue<Macro> macroQueue;

        Stack<Macro> macroStack;
        List<InputWrapper[]> releaseList;
        Dictionary<InputWrapper, int> releaseLookup;

        Macro currentMacro;
        bool isRunning;
        Timer timer;
        bool cleanup;

        Stopwatch stopwatch;
        
        public MacroRunner()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.SynchronizingObject = SynchronizeInvoke;
            timer.AutoReset = false;
        }

        public Macro CurrentMacro
        {
            get { return currentMacro; }
            set
            {
                if (isRunning)
                {
                }
                else
                    currentMacro = value;
            }
        }

        public void Run()
        {
            if (isRunning)
                return;

            if (currentMacro == null)
                return;

            isRunning = true;
            cleanup = false;

            macroStack = new Stack<Macro>();
            currentMacro.ResetStepIndex();
            macroStack.Push(currentMacro);

            releaseList = new List<InputWrapper[]>();
            releaseLookup = new Dictionary<InputWrapper, int>();
            
            RunImpl();
        }

        void RunImpl()
        {
            IStep step;
            Delay delay;
            Macro macroStep;
            InputWrapper[] release;

            int elapsedMs;
            int releaseIndex;
            long timestamp;

            Macro macro = macroStack.Peek();

            while (macroStack.Count > 0)
            {
                step = macro.CurrentStep;
                if (step == null)
                {
                    macroStack.Pop();
                    macro = macroStack.Peek();
                }
                else
                {
                    if (!step.IsEnabled)
                        continue;

                    elapsedMs = (int)stopwatch.ElapsedMilliseconds;
                    timestamp = step.Timestamp;
                    if (timestamp > 0 && timestamp + step.Cooldown < elapsedMs)
                        continue;

                    step.Timestamp = elapsedMs;

                    System.Diagnostics.Debug.WriteLine(step.ToString());

                    delay = step as Delay;
                    if (delay != null)
                    {
                        timer.Interval = delay.Milliseconds;
                        timer.Start();
                        return;
                    }
                    else
                    {
                        macroStep = step as Macro;
                        if (macroStep != null)
                        {
                            macroStep.ResetStepIndex();
                            macroStack.Push(macroStep);
                            macro = macroStep;
                        }
                        else
                        {
                            release = step.Run();
                            if (release != null)
                            {
                                if (releaseLookup.TryGetValue(release[0], out releaseIndex))
                                    releaseList[releaseIndex] = null;

                                releaseLookup[release[0]] = releaseList.Count;
                                releaseList.Add(release);
                            }
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("");

            Release();
            return;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (cleanup)
                return;

            RunImpl();
        }

        public void Release()
        {
            cleanup = true;

            InputWrapper[] release;

            for (int i = releaseList.Count - 1; i >= 0; i--)
            {
                release = releaseList[i];
                if (release != null)
                    Interop.SendInput((uint)release.Length, release);
            }

            isRunning = false;

            System.Diagnostics.Debug.WriteLine("");
        }


    }
}
