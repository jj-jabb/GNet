using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GNet;
using GNet.IO;
using GNet.LgLcd;
using GNet.Profiler.MacroSystem;
using System.Drawing;

namespace GNet.Profiler
{
    public abstract partial class G13ProfileRunner : IProfileRunner, IDisposable
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

        static G13Lcd _lcd;
        protected static G13Lcd lcd
        {
            get
            {
                if (_lcd == null)
                    _lcd = new G13Lcd("GNet Profiler");

                return _lcd;
            }
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

        MacroRunner macroRunner;

        DateTime startTime;

        Profile profile;

        List<Macro> joystickMacros;
        List<Macro> joystickAngleMacros;
        Dictionary<G13Keys, Macro> keyMacros;

        public G13ProfileRunner(Profile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile", "Profile cannot be null");

            this.profile = profile;

            keyEvents = new Queue<KeyEvent>();

            mouseHook = new MouseHook();
            keyboardHook = new KeyboardHook();

            mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);

            threadDelegate = new ThreadStart(RunThread);

            macroRunner = new MacroRunner();

            lcd.Notified += new NotificationEventHandler(Connection_Notified);

            SetupMacros();
        }

        public void SetupMacros()
        {
            joystickMacros = new List<Macro>();
            joystickAngleMacros = new List<Macro>();
            keyMacros = new Dictionary<G13Keys, Macro>();

            foreach (var input in profile.InputAssignments)
            {
                if (input.MacroName != null)
                {
                    var macro = profile.Macros.Find(x => x.Name == input.MacroName);
                    if (macro != null)
                    {
                        if (input.Joystick != null)
                            joystickMacros.Add(macro);
                        else if (input.MinJoystickAngle != null && input.MaxJoystickAngle != null)
                            joystickAngleMacros.Add(macro);
                        else if (input.Key != null && !keyMacros.ContainsKey(input.Key.Value))
                            keyMacros[input.Key.Value] = macro;
                    }
                }
            }
        }

        public void Dispose()
        {
            lcd.Notified -= new NotificationEventHandler(Connection_Notified);
        }

        protected virtual void Connection_Notified(int code, int param1, int param2, int param3, int param4)
        {
            if (code == Sdk.LGLCD_NOTIFICATION_DEVICE_ARRIVAL && IsRunning)
            {
                OnActivated();
            }
        }

        protected virtual void OnActivated()
        {
            ClearGraphics();
            DrawString(LcdActivationText, "Tahoma", 9, 0, 0, 0, 255, 255, 255);

            lcd.BringToFront();
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

        public bool SingleKeyEvents { get; protected set; }
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
            Macro macro;

            if (keyMacros.TryGetValue(key, out macro))
                macroRunner.Enqueue(macro);
            else
                AddKeyEvent(key, true);
        }

        public virtual void SingleKeyReleased(G13Keys key)
        {
            Macro macro;

            if (keyMacros.TryGetValue(key, out macro))
                macroRunner.Enqueue(macro);
            else
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

            if (profile == null || profile.Script == null)
                return;

            G13Device.Current.Start();

            macroRunner.Start();

            if (profile.KeyboardHook != HookOptions.None)
            {
                keyboardHook.IgnoreInjectedValues = profile.KeyboardHook == HookOptions.IgnoreInjected;
                keyboardHook.Start();
            }

            if (profile.MouseHook != HookOptions.None)
            {
                mouseHook.IgnoreInjectedValues = profile.MouseHook == HookOptions.IgnoreInjected;
                mouseHook.Start();
            }

            Device.ProfileRunner = this;

            startTime = DateTime.Now;
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

            macroRunner.Stop();

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
            Device.ProfileRunner = null;
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

        protected abstract string LcdActivationText { get; set; }
        protected abstract bool BeforeRunThread();
        protected abstract void AfterRunThread();
        protected abstract void OnKeyEvent(KeyEvent keyEvent);

        void RunThread()
        {
            var canRun = true;
            var e = KeyEvent.Empty;
            var inputEvent = new EventWaitHandle(false, EventResetMode.AutoReset, inputEventName);

            try
            {
                OnActivated();
            }
            catch (Exception ex)
            {
                canRun = false;
                OnScriptError(ex);
            }

            canRun = BeforeRunThread();

            while (IsRunning)
            {
                if (canRun)
                {
                    for (e = GetKeyEvent(); e.IsEmpty == false; e = GetKeyEvent())
                    {
                        OnKeyEvent(e);

                        if (!IsRunning)
                        {
                            break;
                        }
                    }
                }

                inputEvent.WaitOne();
            }

            ClearKeyEvents();

            SetBacklightColor(128, 255, 255);
            lcd.RemoveFromFront();

            inputEvent.Close();

            AfterRunThread();

            var inputExit = new EventWaitHandle(false, EventResetMode.AutoReset, inputExitName);
            inputExit.Set();
            inputExit.Close();
        }

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

        public void OutputLogMessage(object s)
        {
            Console.WriteLine(s.ToString());
        }

        delegate void InvokeAction();

        public void ClearLog()
        {
            WinFormsTextBoxStreamWriter._output.Invoke(new InvokeAction(OnClearLog));
        }

        void OnClearLog()
        {
            try
            {
                WinFormsTextBoxStreamWriter._output.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Sleep(int timeout)
        {
            Thread.Sleep(timeout);
        }

        public int GetRunningTime()
        {
            TimeSpan ts = DateTime.Now - startTime;
            return (int)ts.TotalMilliseconds;
        }

        public void DrawString(string text, string fontName, double fontSize, double fontStyle, double x, double y, double red, double green, double blue)
        {
            if (lcd == null || !lcd.IsOpen)
                return;

            lcd.DrawString(text, fontName, fontSize, fontStyle, x, y, Color.FromArgb(255, (int)red, (int)green, (int)blue));
        }

        public void SetBacklightColorHtml(string htmlColor)
        {
            try
            {
                Color color = ColorTranslator.FromHtml(htmlColor);
                if (!color.IsEmpty)
                    Device.SetBacklightColorBytes(color.R, color.G, color.B);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SetBacklightColor(double red, double green, double blue)
        {
            try
            {
                byte r = (byte)red;
                byte g = (byte)green;
                byte b = (byte)blue;

                Device.SetBacklightColorBytes(r, g, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SetMLight(double key)
        {
            try
            {
                Device.SetMLight((byte)key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClearLcd()
        {
            if (lcd == null || !lcd.IsOpen)
                return;

            lcd.Clear();
        }

        public void ClearGraphics()
        {
            lcd.Graphics.Clear(Color.Black);
        }
    }
}
