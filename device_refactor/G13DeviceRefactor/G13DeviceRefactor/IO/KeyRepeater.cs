using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using GNet.PInvoke;

namespace GNet.IO
{
    public class KeyRepeater : IDisposable
    {
        const string repeatEvent = @"Local\KeyRepeatEvent";
        const string repeatExit = @"Local\KeyRepeatExit";

        ThreadStart keyRepeatDelegate;
        Thread keyRepeatThread;

        EventWaitHandle keyRepeatEvent;
        EventWaitHandle keyRepeatExit;
        
        bool keyRepeatRunning;

        ScanCode lastKeyPressed;

        public KeyRepeater()
        {
            keyRepeatDelegate = new ThreadStart(KeyRepeatThread);
        }

        public void Start()
        {
            if (keyRepeatRunning)
                return;

            keyRepeatRunning = true;
            keyRepeatEvent = new EventWaitHandle(false, EventResetMode.AutoReset, repeatEvent);
            keyRepeatExit = new EventWaitHandle(false, EventResetMode.AutoReset, repeatExit);
            keyRepeatThread = new Thread(keyRepeatDelegate);
            keyRepeatThread.Start();
        }

        public void Stop()
        {
            if (!keyRepeatRunning)
                return;

            lastKeyPressed = 0;
            keyRepeatRunning = false;
            keyRepeatEvent.Set();
            keyRepeatEvent.Close();

            if (Thread.CurrentThread != keyRepeatThread)
            {
                if (!keyRepeatExit.WaitOne(1000))
                {
                    System.Diagnostics.Debug.WriteLine("KeyRepeater.Stop: keyRepeatStop timed out");
                    keyRepeatThread.Abort();
                }
            }

            keyRepeatExit.Close();

            keyRepeatThread = null;
        }

        void KeyRepeatThread()
        {
            var keyRepeatEvent = new EventWaitHandle(false, EventResetMode.AutoReset, repeatEvent);
            ScanCode repeatingKey = 0;

            while (keyRepeatRunning)
            {
                if (lastKeyPressed != 0)
                {
                    repeatingKey = lastKeyPressed;
                    Thread.Sleep(300);
                }

                if (!keyRepeatRunning)
                    break;

                while (
                    lastKeyPressed != 0 &&
                    keyRepeatRunning &&
                    repeatingKey == lastKeyPressed)
                {
                    KeyRepeat(repeatingKey);
                    Thread.Sleep(30);
                }

                if (!keyRepeatRunning)
                    break;

                keyRepeatEvent.WaitOne();
            }

            keyRepeatEvent.Close();

            var keyRepeatExit = new EventWaitHandle(false, EventResetMode.AutoReset, repeatExit);
            keyRepeatExit.Set();
            keyRepeatExit.Close();
        }

        protected virtual void KeyRepeat(ScanCode repeatingKey)
        {
            var inputData = new InputWrapper[1]
            {
                InputSimulator.KeyWrapper(repeatingKey, false, ((int)repeatingKey & 0x100) == 0x100)
            };

            Interop.SendInput((uint)inputData.Length, inputData);
        }

        public void KeyDown(ScanCode scanCode)
        {
            lastKeyPressed = scanCode;
            keyRepeatEvent.Set();
        }

        public void KeyUp(ScanCode scanCode)
        {
            if (scanCode == lastKeyPressed)
                lastKeyPressed = 0;

            keyRepeatEvent.Set();
        }

        public void ClearKey()
        {
            lastKeyPressed = 0;
            keyRepeatEvent.Set();
        }

        public void Dispose()
        {
            Stop();
            keyRepeatEvent.Close();
        }
    }
}
