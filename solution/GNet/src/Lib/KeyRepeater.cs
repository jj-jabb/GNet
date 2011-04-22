using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using GNet.Lib.PInvoke;
using GNet.Lib.IO;

namespace GNet.Lib
{
    public class KeyRepeater : IDisposable
    {
        ThreadStart keyRepeatDelegate;
        Thread keyRepeatThread;
        AutoResetEvent keyRepeatEvent;
        bool keyRepeatRunning;
        bool keyRepeatThreadComplete;

        ScanCode lastKeyPressed;

        public KeyRepeater()
        {
            keyRepeatDelegate = new ThreadStart(KeyRepeatThread);
            keyRepeatEvent = new AutoResetEvent(false);
            keyRepeatRunning = true;
            keyRepeatThread = new Thread(keyRepeatDelegate);
        }

        public void Start()
        {
            keyRepeatThread.Start();
        }

        public void Stop()
        {
            keyRepeatRunning = false;
            keyRepeatEvent.Set();
            int waitCount;
            for (waitCount = 0; keyRepeatThreadComplete == false && waitCount < 10; waitCount++)
                // wait 10 counts before giving up
                Thread.Sleep(200);

            if (waitCount >= 10)
                Console.WriteLine("Failed to stop KeyRepeat thread.");
        }

        void KeyRepeatThread()
        {
            ScanCode repeatingKey = 0;

            keyRepeatThreadComplete = false;

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

            keyRepeatThreadComplete = true;
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
