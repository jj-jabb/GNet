using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

using LuaInterface;

using GNet.Hid;
using GNet.Lib;
using GNet.Lib.IO;
using GNet.Lib.MKHook;

namespace GNet.Scripting
{
    public class LuaRunner : G13Device, IScriptRunner
    {
        struct KeyEvent
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

        Queue<KeyEvent> keyEvents;

        LuaFunction onEvent;
        LuaFunction onKEvent;
        LuaFunction onMEvent;

        bool isRunning;
        string contents;
        DateTime startTime;

        Thread thread;
        AutoResetEvent auto;

        MouseHook mouseHook;
        KeyboardHook keyboardHook;

        int lastKeyDown;

        public LuaRunner()
        {
            keyEvents = new Queue<KeyEvent>();

            mouseHook = new MouseHook();
            keyboardHook = new KeyboardHook();

            mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);
        }

        public void Run(string script)
        {
            mouseHook.Start();
            keyboardHook.Start();

            contents = script;
            if (contents == null)
                return;

            isRunning = true;
            startTime = DateTime.Now;
            Start();
        }

        public override void Stop()
        {
            keyboardHook.Stop();
            mouseHook.Stop();

            base.Stop();
        }

        public event EventHandler<EventArgs<string>> EventQueueUpdated;

        void OpenLcd()
        {
            if (Lcd != null && !Lcd.IsOpen)
            {
                Lcd.OpenByType();
                Lcd.Clear();
                Lcd.BringToFront();
            }
        }

        void CloseLcd()
        {
            if (Lcd != null)
            {
                Lcd.Disconnect();
            }
        }

        WriteType BringToFront()
        {
            if (!IsOpen)
                return WriteType.Complete;

            var isInFront = Lcd.BringToFront();
            if (isInFront)
                DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, "#FFFFFF", 0, 0);

            return isInFront ? WriteType.Complete : WriteType.Incomplete;
        }

        protected override void ReadWorker_Started()
        {
            auto = new AutoResetEvent(false);
            thread = new Thread(new ThreadStart(RunThread));
            thread.Start();
        }

        protected override void ReadWorker_Opened()
        {
            OpenLcd();
            BringToFront();

            base.ReadWorker_Opened();
        }

        protected override void ReadWorker_Closed()
        {
            CloseLcd();

            base.ReadWorker_Closed();
        }

        protected override void ReadWorker_JoystickChanged(JoystickPosition position)
        {
        }

        protected override void ReadWorker_SingleKeyPressed(G13Keys key)
        {
            addKeyEvent(key, true);
        }

        protected override void ReadWorker_SingleKeyReleased(G13Keys key)
        {
            addKeyEvent(key, false);
        }

        protected override void ReadWorker_Finished()
        {
            isRunning = false;
            SetBacklightColorBytes(128, 255, 255);
            auto.Set();
            base.ReadWorker_Finished();
        }

        void RunThread()
        {
            KeyEvent e = KeyEvent.Empty;

            BringToFront();
            Lua lua = null;
            try
            {
                lua = new Lua();

                Register(lua, "OutputLogMessage", this);
                Register(lua, "ClearLog", this);
                Register(lua, "Sleep", this, typeof(int));
                Register(lua, "GetRunningTime", this);
                Register(lua, "DrawString", this, typeof(string), typeof(string), typeof(double), typeof(double), typeof(string), typeof(double), typeof(double));
                Register(lua, "ClearLcd", this);
                Register(lua, "ClearGraphics", this);
                Register(lua, "SetBacklightColor", this, typeof(double), typeof(double), typeof(double));
                Register(lua, "SetBacklightColorHtml", this, typeof(string));
                Register(lua, "SetMLight", this, typeof(double));

                Register(lua, "GetMKeyState", this, typeof(string));
                Register(lua, "SetMKeyState", this, typeof(int), typeof(string));

                Register(lua, "PressAndReleaseKey", this, typeof(object[]));
                Register(lua, "PressKey", this, typeof(object[]));
                Register(lua, "ReleaseKey", this, typeof(object[]));
                Register(lua, "IsModifierPressed", this, typeof(string));
                Register(lua, "PressMouseButton", this, typeof(int));
                Register(lua, "ReleaseMouseButton", this, typeof(int));
                Register(lua, "PressAndReleaseMouseButton", this, typeof(int));
                Register(lua, "IsMouseButtonPressed", this, typeof(int));
                Register(lua, "MoveMouseTo", this, typeof(int), typeof(int));
                Register(lua, "MoveMouseWheel", this, typeof(int));
                Register(lua, "MoveMouseRelative", this, typeof(int), typeof(int));
                Register(lua, "MoveMouseToVirtual", this, typeof(int), typeof(int));
                Register(lua, "GetMousePosition", this, typeof(int).MakeByRefType(), typeof(int).MakeByRefType());
                Register(lua, "IsKeyLockOn", this, typeof(string));

                lua.DoString(contents);

                onEvent = lua.GetFunction("OnEvent");
                onKEvent = lua.GetFunction("OnKeyEvent");
                onMEvent = lua.GetFunction("OnMouseEvent");
            }
            catch (Exception ex)
            {
                if (lua != null)
                    lua.Close();
                Console.WriteLine(ex.ToString());
                CloseLcd();
                return;
            }

            if (onEvent != null)
            {
                try
                {
                    onEvent.Call("PROFILE_ACTIVATED", 0, "lhc");
                }
                catch (Exception ex)
                {
                    OutputLogMessage(ex.ToString());
                }

                while (isRunning)
                {
                    for (e = getKeyEvent(); e.IsEmpty == false; e = getKeyEvent())
                    {
                        if (e.Key != G13Keys.None)
                        {
                            try
                            {
                                var keyName = e.Key.ToString();
                                var evnt = keyName.Substring(0, 1) + (e.IsPressed ? "_PRESSED" : "_RELEASED");
                                var arg = int.Parse(keyName.Substring(1));
                                onEvent.Call(evnt, arg, "lhc");
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
                            }
                        }
                        else if (e.KeyboardEvent != null)
                        {
                            try
                            {
                                onEvent.Call(
                                    "KBD_" + (e.IsPressed ? "_PRESSED" : "_RELEASED"),
                                    e.KeyboardEvent.KeyCode.ToString(),
                                    "lhc",
                                    e.KeyboardEvent.Shift.ToString(),
                                    e.KeyboardEvent.Alt.ToString(),
                                    e.KeyboardEvent.Control.ToString()
                                    );
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
                            }
                        }
                        else if (e.MouseEvent != null)
                        {
                            try
                            {
                                onEvent.Call(
                                    "MOU_" + (e.IsPressed ? "_PRESSED" : "_RELEASED"),
                                    e.MouseEvent.Button.ToString(),
                                    "lhc",
                                    e.MouseEvent.X.ToString(),
                                    e.MouseEvent.Y.ToString()
                                    );
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
                            }
                        }

                        if (!isRunning)
                            break;
                    }

                    auto.WaitOne();
                }

                ClearKeyEvents();

                try
                {
                    onEvent.Call("PROFILE_DEACTIVATED", 0, "lhc");
                }
                catch (Exception ex)
                {
                    OutputLogMessage(ex.ToString());
                }
            }

            if (lua != null)
                lua.Close();

            auto.Close();
        }

        void addKeyEvent(G13Keys key, bool isPressed)
        {
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


            //if (EventQueueUpdated != null)
            //{
            //    StringBuilder qs = new StringBuilder();

            //    foreach (var val in keyEvents)
            //        qs.Append(val.Key).Append(val.IsPressed ? "_P" : "_R").Append(" ");

            //    EventQueueUpdated(null, new EventArgs<string>(qs.ToString()));
            //}

            auto.Set();
        }

        void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == lastKeyDown)
                return;

            lastKeyDown = e.KeyValue;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, true));
            }

            auto.Set();
        }

        void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == lastKeyDown)
                lastKeyDown = 0;

            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, false));
            }

            auto.Set();
        }

        void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, true));
            }

            auto.Set();
        }

        void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(e, false));
            }

            auto.Set();
        }

        KeyEvent getKeyEvent()
        {
            lock (keyEvents)
            {
                if (keyEvents.Count > 0)
                    return keyEvents.Dequeue();

                return KeyEvent.Empty;
            }
        }

        void ClearKeyEvents()
        {
            lock (keyEvents)
            {
                keyEvents.Clear();
            }
        }

        #region Lua API Calls

        void Register(Lua lua, string name, object owner, params Type[] parameters)
        {
            if (lua == null)
                return;

            MethodInfo mi;

            if (parameters == null || parameters.Length == 0)
                mi = owner.GetType().GetMethod(name);
            else
                mi = owner.GetType().GetMethod(name, parameters);

            if (mi != null)
                lua.RegisterFunction(name, owner, mi);
            else
                System.Diagnostics.Debug.WriteLine("not found: " + name);
        }

        public void OutputLogMessage(object s)
        {
            Console.WriteLine(s.ToString());
        }

        delegate void InvokeAction();

        public void ClearLog()
        {
            TextBoxStreamWriter._output.Invoke(new InvokeAction(OnClearLog));
        }

        void OnClearLog()
        {
            try
            {
                TextBoxStreamWriter._output.Clear();
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

        public void DrawString(string text, string fontName, double fontSize, double fontStyle, string htmlColor, double x, double y)
        {
            if (Lcd == null || !Lcd.IsOpen)
                return;

            Lcd.DrawString(text, fontName, fontSize, fontStyle, htmlColor, x, y);
        }

        public void SetBacklightColorHtml(string htmlColor)
        {
            try
            {
                Color color = ColorTranslator.FromHtml(htmlColor);
                if (!color.IsEmpty)
                    SetBacklightColorBytes(color.R, color.G, color.B);
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

                SetBacklightColorBytes(r, g, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        int callCount = 0;
        public void SetMLight(double key)
        {
            callCount++;
            try
            {
                base.SetMLight((byte)key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClearLcd()
        {
            if (Lcd == null || !Lcd.IsOpen)
                return;

            Lcd.Clear();
        }

        public void ClearGraphics()
        {
            Lcd.Graphics.Clear(Color.Black);
        }

        #endregion
    }
}