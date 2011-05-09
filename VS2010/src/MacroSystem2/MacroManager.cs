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

namespace GNet.MacroSystem2
{
    public class MacroManager
    {
        const string runEventName = @"Local\RunEvent";
        const string runExitName = @"Local\RunExit";

        EventWaitHandle runEvent;
        EventWaitHandle runExit;
        bool running;

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

        public MacroManager()
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
            IStep step;
            Delay delay;
            bool stepEnabled;
            long elapsedMs;
            InputWrapper[] release;
            int releaseIndex;
            Macro macro;

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

                        currentMacro.ResetStepIndex();
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

                        if (macro.TerminateCurrent && macro.Priority >= currentMacro.Priority)
                        {
                            if (!currentMacro.NoRelease)
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

                    if (step == null)
                    {
                        if (!currentMacro.NoRelease)
                            Release();

                        currentMacro = null;
                    }
                    else
                    {
                        elapsedMs = stopwatch.ElapsedMilliseconds;

                        stepEnabled = step.IsEnabled;
                        if (stepEnabled)
                        {
                            if (step.Timestamp > 0 && step.Timestamp + step.Cooldown < elapsedMs)
                                stepEnabled = false;
                        }

                        if (stepEnabled)
                        {
                            step.Timestamp = elapsedMs;

                            switch (currentMacro.Type)
                            {
                                case StepType.Delay:
                                    delay = step as Delay;
                                    timer.Interval = delay.Milliseconds;
                                    timer.Start();
                                    runEvent.WaitOne();
                                    break;

                                case StepType.Macro:
                                    macroStack.Push(currentMacro);
                                    currentMacro = step as Macro;
                                    currentMacro.ResetStepIndex();
                                    break;

                                default:
                                    release = step.Run();
                                    if (release != null && releaseLookup.TryGetValue(release[0], out releaseIndex))
                                        releaseList[releaseIndex] = null;
                                    releaseLookup[release[0]] = releaseList.Count;
                                    releaseList.Add(release);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void Release()
        {
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

        //public void Run_NOTHREAD(Macro macro)
        //{
        //    if (currentMacro == null)
        //    {
        //        macroStack = new Stack<Macro>();
        //        currentMacro = macro;
        //        RunImpl_NOTHREAD();
        //    }
        //    else
        //    {
        //        macroQueue.Enqueue(macro);
        //    }
        //}

        //void RunImpl_NOTHREAD()
        //{
        //    IStep step;
        //    Delay delay;
        //    Macro macro;

        //    while (currentMacro != null)
        //    {
        //        step = currentMacro.CurrentStep;

        //        if (step == null)
        //        {
        //            if (macroStack.Count > 0)
        //                currentMacro = macroStack.Pop();
        //            else if (macroQueue.Count > 0)
        //                currentMacro = macroQueue.Dequeue();
        //        }
        //        else
        //        {
        //            switch (currentMacro.Type)
        //            {
        //                case StepType.Delay:
        //                    break;

        //                case StepType.Macro:
        //                    break;

        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //}
    }
}
