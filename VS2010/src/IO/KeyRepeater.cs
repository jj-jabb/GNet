using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using GNet.PInvoke;

namespace GNet.IO
{
    public class KeyRepeater : IDisposable
    {
        ThreadStart keyRepeatDelegate;
        Thread keyRepeatThread;
        EventWaitHandle keyRepeatEvent;
        bool keyRepeatRunning;

        ScanCode lastKeyPressed;

        public KeyRepeater()
        {
            keyRepeatDelegate = new ThreadStart(KeyRepeatThread);
            keyRepeatEvent = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\KeyRepeatEvent");
            keyRepeatRunning = true;
            keyRepeatThread = new Thread(keyRepeatDelegate);
        }

        public void Start()
        {
            keyRepeatThread.Start();
        }

        public void Stop()
        {
            if (!keyRepeatRunning)
                return;

            var keyRepeatStop = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\KeyRepeatStop");
            keyRepeatRunning = false;
            keyRepeatEvent.Set();
            keyRepeatStop.WaitOne();
            keyRepeatStop.Close();
        }

        void KeyRepeatThread()
        {
            var keyRepeatEvent = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\KeyRepeatEvent");
            ScanCode repeatingKey = 0;

            while (keyRepeatRunning)
            {
                if (lastKeyPressed != 0)
                {
                    repeatingKey = lastKeyPressed;
                    Thread.Sleep(300);
                }

                if (!keyRepeatRunning)
                    return;

                while (
                    lastKeyPressed != 0 &&
                    keyRepeatRunning &&
                    repeatingKey == lastKeyPressed)
                {
                    KeyRepeat(repeatingKey);
                    Thread.Sleep(30);
                }

                keyRepeatEvent.WaitOne();
            }

            keyRepeatEvent.Close();

            var keyRepeatStop = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\KeyRepeatStop");
            keyRepeatStop.Set();
            keyRepeatStop.Close();
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

        public void Dispose()
        {
            Stop();
            keyRepeatEvent.Close();
        }
    }
}
