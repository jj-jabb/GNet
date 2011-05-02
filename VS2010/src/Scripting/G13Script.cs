using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GNet;
using GNet.IO;
using System.Diagnostics;

namespace GNet.Scripting
{
    public abstract partial class G13Script : IG13DeviceScript
    {
        public delegate void ScriptErrorHandler(Exception ex);

        protected const string inputEventName = @"Local\InputEvent";
        protected const string inputExitName = @"Local\InputExit";

        protected struct KeyEvent
        {
            public G13Keys Key;
            public bool IsPressed;

            public KeyEventArgs KeyboardEvent;

            public MouseEventArgs MouseEvent;

            public readonly bool IsEmpty;

            public KeyEvent(G13Keys key, bool isPressed)
            {
                Key = key;
                IsPressed = isPressed;
                KeyboardEvent = null;
                MouseEvent = null;
                IsEmpty = false;
            }

            public KeyEvent(KeyEventArgs e, bool isPressed)
            {
                Key = G13Keys.None;
                IsPressed = isPressed;
                KeyboardEvent = e;
                MouseEvent = null;
                IsEmpty = false;
            }

            public KeyEvent(MouseEventArgs e, bool isPressed)
            {
                Key = G13Keys.None;
                IsPressed = isPressed;
                KeyboardEvent = null;
                MouseEvent = e;
                IsEmpty = false;
            }

            KeyEvent(bool isEmpty)
            {
                Key = (G13Keys)0;
                IsPressed = false;
                IsEmpty = isEmpty;
                KeyboardEvent = null;
                MouseEvent = null;
            }

            public static KeyEvent Empty = new KeyEvent(true);
        }

        G13Device device = G13Device.Current;

        Queue<KeyEvent> keyEvents;

        protected ThreadStart threadDelegate;
        protected Thread thread;

        protected EventWaitHandle inputEvent;
        protected EventWaitHandle inputExit;

        MouseHook mouseHook;
        KeyboardHook keyboardHook;

        int lastKeyDown;

        bool onStoppedCalled;

        public G13Script()
        {
            keyEvents = new Queue<KeyEvent>();

            mouseHook = new MouseHook();
            keyboardHook = new KeyboardHook();

            mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);

            threadDelegate = new ThreadStart(RunThread);
        }

        public event ScriptErrorHandler ScriptError;

        public G13Device Device
        {
            get
            {
                return device;
            }
            set { }
        }

        public Profile Profile { get; set; }
        public bool SingleKeyEvents { get; set; }
        public bool IsRunning { get; private set; }

        public virtual void JoystickChanged(JoystickPosition position)
        {
        }

        public virtual void KeyStateChanged(G13KeyState newState)
        {
        }

        public virtual void KeysPressed(ulong keys)
        {
        }

        public virtual void KeysReleased(ulong keys)
        {
        }

        public virtual void SingleKeyPressed(G13Keys key)
        {
            AddKeyEvent(key, true);
        }

        public virtual void SingleKeyReleased(G13Keys key)
        {
            AddKeyEvent(key, false);
        }

        protected virtual void OnScriptError(Exception ex)
        {
            Console.WriteLine(ex.ToString());

            if (ScriptError != null)
                ScriptError(ex);
        }

        public void Start()
        {
            if (IsRunning)
                return;

            if (Profile == null || Profile.Contents == null)
                return;

            G13Device.Current.Start();

            if (Profile.Header.KeyboardHook != HookOptions.None)
            {
                keyboardHook.IgnoreInjectedValues = Profile.Header.KeyboardHook == HookOptions.IgnoreInjected;
                keyboardHook.Start();
            }

            if (Profile.Header.MouseHook != HookOptions.None)
            {
                mouseHook.IgnoreInjectedValues = Profile.Header.MouseHook == HookOptions.IgnoreInjected;
                mouseHook.Start();
            }

            Device.Script = this;

            IsRunning = true;

            OnStart();

            inputEvent = new EventWaitHandle(false, EventResetMode.AutoReset, inputEventName);
            inputExit = new EventWaitHandle(false, EventResetMode.AutoReset, inputExitName);

            thread = new Thread(new ThreadStart(RunThread));
            thread.Start();

            onStoppedCalled = false;
            OnStarted();
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            var id = Thread.CurrentThread.ManagedThreadId;

            keyboardHook.Stop();
            mouseHook.Stop();

            Device.KeyRepeater.ClearKey();

            IsRunning = false;
            inputEvent.Set();
            inputEvent.Close();

            if (Thread.CurrentThread != thread)
            {
                if (!inputExit.WaitOne(1000))
                {
                    System.Diagnostics.Debug.WriteLine("G13Script.Stop: inputExit timed out");
                    thread.Abort();
                }
            }

            inputExit.Close();

            thread = null;

            Device.Stop();
            Device.Script = null;
            OnStop();
            OnStopped();
        }

        protected void OnStarted()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        protected void OnStopped()
        {
            if (onStoppedCalled)
                return;

            onStoppedCalled = true;

            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }

        public event EventHandler Started;
        public event EventHandler Stopped;

        protected abstract void RunThread();

        protected void AddKeyEvent(G13Keys key, bool isPressed)
        {
            if (!IsRunning)
                return;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(key, isPressed));

                if (keyEvents.Count > 4)
                {
                    // cleanup any duplicate MKey events
                    byte mkeyEvents = 0;
                    byte mkey;
                    int shift;
                    G13Keys kval;
                    KeyEvent k;

                    int length = keyEvents.Count;
                    for (int i = 0; i < length; i++)
                    {
                        k = keyEvents.Dequeue();

                        kval = k.Key;
                        if (kval >= G13Keys.M1 && kval <= G13Keys.M4)
                        {
                            shift = k.IsPressed ? 29 : 25;
                            mkey = (byte)((uint)kval >> shift);

                            // check to see if there is was already a key press and release event pair
                            // for this mkey
                            if ((mkey & mkeyEvents) > 0 && ((mkey >> 4) & mkeyEvents) > 0)
                            {
                                // the pair already exists
                                if (mkey <= 8 && i < length)
                                {
                                    // don't remove a key-press event if it's the last event in the queue (wait for a key-release)
                                    keyEvents.Enqueue(k);
                                }
                            }
                            else
                            {
                                mkeyEvents |= mkey;
                                keyEvents.Enqueue(k);
                            }
                        }
                        else
                        {
                            // put it back on the queue
                            keyEvents.Enqueue(k);
                        }
                    }
                }
            }

            inputEvent.Set();
        }

        protected KeyEvent GetKeyEvent()
        {
            KeyEvent keyEvent = KeyEvent.Empty;

            lock (keyEvents)
            {
                if (keyEvents.Count > 0)
                    keyEvent = keyEvents.Dequeue();
            }

            return keyEvent;
        }

        protected void ClearKeyEvents()
        {
            lock (keyEvents)
            {
                keyEvents.Clear();
            }
        }

        void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsRunning)
                return;

            if (e.KeyValue == lastKeyDown)
                return;

            lastKeyDown = e.KeyValue;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, true));
            }

            inputEvent.Set();
        }

        void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (!IsRunning)
                return;

            if (e.KeyValue == lastKeyDown)
                lastKeyDown = 0;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, false));
            }

            inputEvent.Set();
        }

        void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IsRunning)
                return;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, true));
            }

            inputEvent.Set();
        }

        void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            if (!IsRunning)
                return;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, false));
            }

            inputEvent.Set();
        }
    }
}
