using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;
using GNet.IO;
using GNet.PInvoke;

namespace GNet.Profiler.MacroSystem
{
    public class MacroRunner
    {
        const string runEventName = @"Local\RunEvent";
        const string runExitName = @"Local\RunExit";

        EventWaitHandle runEvent;
        EventWaitHandle runExit;

        bool running;
        bool releasing;

        ThreadStart runDelegate;
        Thread runThread;

        Queue<Macro> macroQueue;
        Stack<Macro> macroStack;

        List<InputWrapper[]> releaseList;
        Dictionary<InputWrapper, int> releaseLookup;

        Macro currentMacro;

        Timer timer;
        bool timerAborted;

        Stopwatch stopwatch;

        public MacroRunner()
        {
            macroQueue = new Queue<Macro>();

            runDelegate = new ThreadStart(Run);

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = false;

            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void Start()
        {
            if (running)
                return;

            macroStack = new Stack<Macro>();

            releaseList = new List<InputWrapper[]>();
            releaseLookup = new Dictionary<InputWrapper, int>();

            running = true;
            releasing = false;

            runEvent = new EventWaitHandle(false, EventResetMode.AutoReset, runEventName);
            runExit = new EventWaitHandle(false, EventResetMode.AutoReset, runExitName);

            runThread = new Thread(runDelegate);
            runThread.Start();
        }

        public void Stop()
        {
            if (!running)
                return;

            running = false;

            runEvent.Set();
            runEvent.Close();

            if (Thread.CurrentThread != runThread)
            {
                if (!runExit.WaitOne(1000))
                {
                    System.Diagnostics.Debug.WriteLine("KeyRepeater.Stop: keyRepeatStop timed out");
                    runThread.Abort();
                }
            }

            runExit.Close();
            runThread = null;
        }

        void Run()
        {
            Step step;
            StepAction action;
            StepActionInput actionInput;
            Delay delay;
            bool stepEnabled;
            long elapsedMs;
            InputWrapper[] release;
            int releaseIndex;
            Macro macro;
            int loopCount;
            int currentLoop;

            while (running)
            {
                if (currentMacro == null)
                {
                    if (macroStack.Count > 0)
                        currentMacro = macroStack.Pop();
                    else if (macroQueue.Count > 0)
                    {
                        lock (macroQueue)
                        {
                            currentMacro = macroQueue.Dequeue();
                        }

                        currentMacro.Reset();
                    }
                }
                else
                {
                    if (macroQueue.Count > 0)
                    {
                        lock (macroQueue)
                        {
                            macro = macroQueue.Peek();
                        }

                        if (macro.Interrupt && macro.Priority >= currentMacro.Priority)
                        {
                            timer.Stop();
                            if (currentMacro.Release)
                                Release();

                            lock (macroQueue)
                            {
                                currentMacro = macroQueue.Dequeue();
                            }
                        }
                    }
                }

                if (currentMacro == null)
                    runEvent.WaitOne();
                else
                {
                    step = currentMacro.CurrentStep;
                    currentMacro.IncStep();

                    if (step == null)
                    {
                        if (currentMacro.Release)
                            Release();

                        loopCount = currentMacro.LoopCount;
                        currentLoop = currentMacro.CurrentLoop;
                        currentMacro.IncLoop();
                        if (loopCount < 0 || currentLoop < loopCount - 1)
                        {
                            System.Diagnostics.Debug.WriteLine("loopCount: " + loopCount + ", currentLoop = " + currentLoop);
                            currentMacro.ResetSteps();
                        }
                        else
                            currentMacro = null;
                    }
                    else
                    {
                        elapsedMs = stopwatch.ElapsedMilliseconds;

                        stepEnabled = step.IsEnabled;
                        if (stepEnabled)
                        {
                            if (step.Timestamp > 0 && step.Timestamp + step.Cooldown > elapsedMs)
                                stepEnabled = false;
                        }

                        if (stepEnabled)
                        {
                            step.Timestamp = elapsedMs;

                            switch (step.Type)
                            {
                                case StepActionType.Delay:
                                    delay = step as Delay;
                                    timer.Interval = delay.Milliseconds;
                                    timer.Start();
                                    runEvent.WaitOne();
                                    break;

                                case StepActionType.Macro:
                                    macroStack.Push(currentMacro);
                                    currentMacro = step as Macro;
                                    currentMacro.Reset();
                                    break;

                                case StepActionType.Action:
                                    action = step as StepActionInput;
                                    action.Run();
                                    break;

                                case StepActionType.ActionInput:
                                    actionInput = step as StepActionInput;
                                    release = actionInput.Release;
                                    actionInput.Run();

                                    if (release != null)
                                    {
                                        if (releaseLookup.TryGetValue(release[0], out releaseIndex))
                                            releaseList[releaseIndex] = null;
                                        releaseLookup[release[0]] = releaseList.Count;
                                        releaseList.Add(release);
                                    }
                                    
                                    if (actionInput.Inputs != null)
                                    {
                                        foreach (var input in actionInput.Inputs)
                                            if (releaseLookup.TryGetValue(input, out releaseIndex))
                                                releaseList[releaseIndex] = null;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void Release()
        {
            releasing = true;

            InputWrapper[] release;
            for (int i = releaseList.Count - 1; i >= 0; i--)
            {
                release = releaseList[i];
                if (release != null)
                    Interop.SendInput((uint)release.Length, release);
            }

            releaseList.Clear();
            releaseLookup.Clear();
        }

        public void Enqueue(Macro macro)
        {
            lock (macroQueue)
            {
                macroQueue.Enqueue(macro);
            }

            runEvent.Set();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            runEvent.Set();
        }
    }
}
