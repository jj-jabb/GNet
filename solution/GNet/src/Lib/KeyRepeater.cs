using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GNet.Lib
{
    public class KeyRepeater : IDisposable
    {
        ThreadStart keyRepeatDelegate;
        Thread keyRepeatThread;
        AutoResetEvent keyRepeatEvent;
        bool keyRepeatRunning;
        bool keyRepeatThreadComplete;

        uint lastKeyPressed;
        uint lastKeyReleased;

        public KeyRepeater()
        {
            keyRepeatDelegate = new ThreadStart(KeyRepeatThread);
            keyRepeatEvent = new AutoResetEvent(false);
            keyRepeatRunning = true;
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
            uint repeatingKey = 0;

            keyRepeatThreadComplete = false;

            while (keyRepeatRunning)
            {
                if (lastKeyPressed != 0 && lastKeyPressed != lastKeyReleased)
                {
                    repeatingKey = lastKeyPressed;
                    Thread.Sleep(300);
                }

                if (!keyRepeatRunning)
                    return;

                while (
                    lastKeyPressed != 0 &&
                    keyRepeatRunning &&
                    repeatingKey == lastKeyPressed &&
                    lastKeyPressed != lastKeyReleased)
                {
                    KeyRepeat(repeatingKey);
                    Thread.Sleep(40);
                }
            }

            keyRepeatThreadComplete = true;
        }

        protected virtual void KeyRepeat(uint repeatingKey)
        {
        }

        public void Dispose()
        {
            Stop();
            keyRepeatEvent.Close();
        }
    }
}
