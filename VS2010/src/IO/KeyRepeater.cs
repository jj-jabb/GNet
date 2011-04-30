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
        }

        public void Start()
        {
            if (keyRepeatRunning)
                return;

            keyRepeatThread = new Thread(keyRepeatDelegate);
            keyRepeatThread.Start();

            keyRepeatRunning = true;
        }

        public void Stop()
        {
            if (!keyRepeatRunning)
                return;

            var keyRepeatStop = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\KeyRepeatStop");
            keyRepeatRunning = false;
            keyRepeatEvent.Set();
            if (!keyRepeatStop.WaitOne(1000))
                System.Diagnostics.Debug.WriteLine("KeyRepeater.Stop: keyRepeatStop timed out");
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
