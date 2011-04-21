using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;

using Microsoft.Win32.SafeHandles;

using LuaInterface;

using GNet.Hid;
using GNet.Lib;
using GNet.Lib.IO;

namespace GNet
{
    public class LuaRunner : G13Device, IScriptRunner
    {
        struct KeyEvent
        {
            public G13Keys Key;
            public bool IsPressed;
            public readonly bool IsEmpty;

            public KeyEvent(G13Keys key, bool isPressed)
            {
                Key = key;
                IsPressed = isPressed;
                IsEmpty = false;
            }

            KeyEvent(bool isEmpty)
            {
                Key = (G13Keys)0;
                IsPressed = false;
                IsEmpty = isEmpty;
            }

            public static KeyEvent Empty = new KeyEvent(true);
        }

        Queue<KeyEvent> keyEvents;

        LuaFunction onEvent;
        bool isRunning;
        string contents;
        DateTime startTime;

        Thread thread;
        AutoResetEvent auto;

        public LuaRunner()
        {
            keyEvents = new Queue<KeyEvent>();
        }

        public void Run(string script)
        {
            contents = script;
            if (contents == null)
                return;

            isRunning = true;
            startTime = DateTime.Now;
            Start();
        }

        //public override void Stop()
        //{
        //    base.Stop();
        //    isRunning = false;
        //    auto.Set();
        //}

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
            //AddWriteDelegate((d, h) =>
            //{
                if (!IsOpen)
                    return WriteType.Complete;

                var isInFront = Lcd.BringToFront();
                if (isInFront)
                {
                    //Thread.Sleep(100);
                    DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, "#FFFFFF", 0, 0);
                }

                return isInFront ? WriteType.Complete : WriteType.Incomplete;
            //});
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
            SetBacklightColor(128, 255, 255);
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
                Register(lua, "SetBacklightColor", this, typeof(string));
                Register(lua, "SetMLight", this, typeof(double));

                //string text, string fontName, double fontSize, double fontStyle, string htmlColor, double x, double y
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
                        if (e.IsPressed)
                            try
                            {
                                var keyName = e.Key.ToString();
                                var evnt = keyName.Substring(0, 1) + "_PRESSED";
                                var arg = int.Parse(keyName.Substring(1));
                                onEvent.Call(evnt, arg, "lhc");
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
                            }
                        else
                            try
                            {
                                var keyName = e.Key.ToString();
                                var evnt = keyName.Substring(0, 1) + "_RELEASED";
                                var arg = int.Parse(keyName.Substring(1));
                                onEvent.Call(evnt, arg, "lhc");
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
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

        public void SetBacklightColor(string htmlColor)
        {
            try
            {
                Color color = ColorTranslator.FromHtml(htmlColor);
                if (!color.IsEmpty)
                    SetBacklightColor(color.R, color.G, color.B);
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

#if false
    public class LuaRunner_OLD : G13Device, IScriptRunner
    {
        struct KeyEvent
        {
            public G13Keys Key;
            public bool IsPressed;
            public readonly bool IsEmpty;

            public KeyEvent(G13Keys key, bool isPressed)
            {
                Key = key;
                IsPressed = isPressed;
                IsEmpty = false;
            }

            KeyEvent(bool isEmpty)
            {
                Key = (G13Keys)0;
                IsPressed = false;
                IsEmpty = isEmpty;
            }

            public static KeyEvent Empty = new KeyEvent(true);
        }

        Queue<KeyEvent> keyEvents;

        Lua lua;
        LuaFunction onEvent;
        bool isRunning;
        string contents;
        DateTime startTime;

        public LuaRunner_OLD(string scriptCode = null)
        {
            this.contents = scriptCode;

            keyEvents = new Queue<KeyEvent>();

            OpenLcd();
        }

        public string ScriptCode { get { return contents; } set { contents = value; } }

        public IScriptRunner Run()
        {
            if (contents == null)
                return this;

            startTime = DateTime.Now;
            Start();

            return this;
        }

        void OpenLcd()
        {
            if (Lcd != null && !Lcd.IsOpen)
            {
                Lcd.AppFriendlyName = "GNet Profiler";
                Lcd.Connect();
                Lcd.Open();
            }
        }

        void CloseLcd()
        {
            if (Lcd != null)
            {
                Lcd.RemoveFromFront();
                Lcd.Disconnect();
                Lcd.Close();
            }
        }

        void BringToFront()
        {
            AddWriteDelegate((d, h) =>
            {
                if (!IsOpen)
                    return WriteType.Complete;

                var isInFront = Lcd.BringToFront() == 0;
                if (isInFront)
                    DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, "#FFFFFF", 0, 0);

                return isInFront ? WriteType.Complete : WriteType.Incomplete;
            });
        }

        protected override void ReadWorker_Started()
        {
            try
            {
                OpenLcd();
                BringToFront();

                lua = new Lua();

                Register("OutputLogMessage", this);
                Register("ClearLog", this);
                Register("Sleep", this, typeof(int));
                Register("GetRunningTime", this);
                Register("DrawString", this, typeof(string), typeof(string), typeof(double), typeof(double), typeof(string), typeof(double), typeof(double));
                Register("ClearLcd", this);
                Register("SetBacklightColor", this, typeof(string));
                Register("SetMLight", this, typeof(double));

                //string text, string fontName, double fontSize, double fontStyle, string htmlColor, double x, double y
                Register("GetMKeyState", this, typeof(string));
                Register("SetMKeyState", this, typeof(int), typeof(string));

                Register("PressAndReleaseKey", this, typeof(object[]));
                Register("PressKey", this, typeof(object[]));
                Register("ReleaseKey", this, typeof(object[]));
                Register("IsModifierPressed", this, typeof(string));
                Register("PressMouseButton", this, typeof(int));
                Register("ReleaseMouseButton", this, typeof(int));
                Register("PressAndReleaseMouseButton", this, typeof(int));
                Register("IsMouseButtonPressed", this, typeof(int));
                Register("MoveMouseTo", this, typeof(int), typeof(int));
                Register("MoveMouseWheel", this, typeof(int));
                Register("MoveMouseRelative", this, typeof(int), typeof(int));
                Register("MoveMouseToVirtual", this, typeof(int), typeof(int));
                Register("GetMousePosition", this, typeof(int).MakeByRefType(), typeof(int).MakeByRefType());
                Register("IsKeyLockOn", this, typeof(string));

                lua.DoString(contents);

                onEvent = lua.GetFunction("OnEvent");

                if (onEvent != null)
                    onEvent.Call("PROFILE_ACTIVATED", 0, "lhc");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Stop();
                return;
            }

            base.ReadWorker_Started();
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
            try
            {
                var keyName = key.ToString();
                var evnt = keyName.Substring(0, 1) + "_PRESSED";
                var arg = int.Parse(keyName.Substring(1));
                onEvent.Call(evnt, arg, "lhc");
            }
            catch (Exception ex)
            {
                OutputLogMessage(ex.ToString());
            }
        }

        protected override void ReadWorker_SingleKeyReleased(G13Keys key)
        {
            try
            {
                var keyName = key.ToString();
                var evnt = keyName.Substring(0, 1) + "_RELEASED";
                var arg = int.Parse(keyName.Substring(1));
                onEvent.Call(evnt, arg, "lhc");
            }
            catch (Exception ex)
            {
                OutputLogMessage(ex.ToString());
            }
        }

        #region Lua API Calls

        void Register(string name, object owner, params Type[] parameters)
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

        public void ClearLog()
        {
            TextBoxStreamWriter._output.Clear();
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

            FontStyle fstyle = (FontStyle)fontStyle;
            float fsize = (float)fontSize;
            float fx = (float)x;
            float fy = (float)y;
            Brush brush = new SolidBrush(ColorTranslator.FromHtml(htmlColor));

            using (Font f = new Font(fontName, fsize, fstyle))
            {
                Lcd.LcdGraphics.DrawString(text, f, brush, 0f, 0f);
            }

            Lcd.UpdateBitmap(LcdPriority.Normal);
        }

        public void SetBacklightColor(string htmlColor)
        {
            try
            {
                Color color = ColorTranslator.FromHtml(htmlColor);
                if (!color.IsEmpty)
                    SetBacklightColor(color.R, color.G, color.B);
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
                SetMLight((byte)key);
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

            Lcd.LcdGraphics.Clear(Color.Black);
            Lcd.UpdateBitmap(LcdPriority.Normal);
        }

        #endregion
    }
#endif
#if false
    public class LuaRunner_OLD : IScriptRunner, IDisposable
    {
        static G13Device device = G13Device.Current;

        struct KeyEvent
        {
            public Keys Key;
            public bool IsPressed;
            public readonly bool IsEmpty;

            public KeyEvent(Keys key, bool isPressed)
            {
                Key = key;
                IsPressed = isPressed;
                IsEmpty = false;
            }

            KeyEvent(bool isEmpty)
            {
                Key = (Keys)0;
                IsPressed = false;
                IsEmpty = isEmpty;
            }

            public static KeyEvent Empty = new KeyEvent(true);
        }

        Queue<KeyEvent> keyEvents;

        Lua lua;
        LuaFunction onEvent;
        bool isRunning;
        string contents;

        Thread thread;
        AutoResetEvent auto;

        DateTime startTime;

        public LuaRunner(string contents)
        {
            this.contents = contents;

            keyEvents = new Queue<KeyEvent>();

        }

        public void Dispose()
        {
        }

        public IScriptRunner Run()
        {
            if (!device.Lcd.IsOpen)
                device_Inserted();

            //device.SetBacklightColor(255, 0, 0);
            //device.SetMLight(1);

            device.KeyPressed += new G13Device.KeyHandler(device_KeyPressed);
            device.KeyReleased += new G13Device.KeyHandler(device_KeyReleased);
            //device.Inserted += new G13Device.DeviceEventHandler(device_Inserted);
            //device.Removed += new G13Device.DeviceEventHandler(device_Removed);

            isRunning = true;
            startTime = DateTime.Now;

            auto = new AutoResetEvent(false);
            thread = new Thread(new ThreadStart(RunThread));
            thread.Start();

            return this;
        }

        void device_Removed()
        {
            device.Lcd.RemoveFromFront();
            device.Lcd.Disconnect();
            device.Lcd.Close();
        }

        void device_Inserted()
        {
            device.Lcd.AppFriendlyName = "GNet Profiler";
            device.Lcd.Connect();
            device.Lcd.Open();
        }

        public void Stop()
        {
            if (isRunning)
            {
                device.SetBacklightColor(128, 255, 255);

                device.KeyPressed -= new G13Device.KeyHandler(device_KeyPressed);
                device.KeyReleased -= new G13Device.KeyHandler(device_KeyReleased);

                isRunning = false;
                
                device_Removed();

                if (auto != null)
                    auto.Set();
            }
        }

        void device_KeyPressed(Keys key)
        {
            addKeyEvent(key, true);
        }

        void device_KeyReleased(Keys key)
        {
            addKeyEvent(key, false);
        }

        public void OutputLogMessage(object s)
        {
            Console.WriteLine(s.ToString());
        }

        public void ClearLog()
        {
            TextBoxStreamWriter._output.Clear();
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
            if (device.Lcd == null || !device.Lcd.IsOpen)
                return;

            FontStyle fstyle = (FontStyle)fontStyle;
            float fsize = (float)fontSize;
            float fx = (float)x;
            float fy = (float)y;
            Brush brush = new SolidBrush(ColorTranslator.FromHtml(htmlColor));

            using (Font f = new Font(fontName, fsize, fstyle))
            {
                device.Lcd.LcdGraphics.DrawString(text, f, brush, 0f, 0f);
            }

            device.Lcd.UpdateBitmap(LcdPriority.Normal);
        }

        public void SetBacklightColor(string htmlColor)
        {
            try
            {
                Color color = ColorTranslator.FromHtml(htmlColor);
                if (!color.IsEmpty)
                    device.SetBacklightColor(color.R, color.G, color.B);
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
                device.SetMLight((byte)key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClearLcd()
        {
            if (device.Lcd == null || !device.Lcd.IsOpen)
                return;

            device.Lcd.LcdGraphics.Clear(Color.Black);
            device.Lcd.UpdateBitmap(LcdPriority.Normal);
        }



        void RunThread()
        {
            KeyEvent e = KeyEvent.Empty;

            if (device.Lcd.IsOpen)
                device.Lcd.BringToFront();

            Thread.Sleep(100);
            DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, "#FFFFFF", 0, 0);

            try
            {
                lua = new Lua();

                Register("OutputLogMessage", this);
                Register("ClearLog", this);
                Register("Sleep", this, typeof(int));
                Register("GetRunningTime", this);
                Register("DrawString", this, typeof(string), typeof(string), typeof(double), typeof(double), typeof(string), typeof(double), typeof(double));
                Register("ClearLcd", this);
                Register("SetBacklightColor", this, typeof(string));
                Register("SetMLight", this, typeof(double));

                //string text, string fontName, double fontSize, double fontStyle, string htmlColor, double x, double y
                Register("GetMKeyState", device, typeof(string));
                Register("SetMKeyState", device, typeof(int), typeof(string));

                Register("PressAndReleaseKey", device, typeof(object[]));
                Register("PressKey", device, typeof(object[]));
                Register("ReleaseKey", device, typeof(object[]));
                Register("IsModifierPressed", device, typeof(string));
                Register("PressMouseButton", device, typeof(int));
                Register("ReleaseMouseButton", device, typeof(int));
                Register("PressAndReleaseMouseButton", device, typeof(int));
                Register("IsMouseButtonPressed", device, typeof(int));
                Register("MoveMouseTo", device, typeof(int), typeof(int));
                Register("MoveMouseWheel", device, typeof(int));
                Register("MoveMouseRelative", device, typeof(int), typeof(int));
                Register("MoveMouseToVirtual", device, typeof(int), typeof(int));
                Register("GetMousePosition", device, typeof(int).MakeByRefType(), typeof(int).MakeByRefType());
                Register("IsKeyLockOn", device, typeof(string));

                lua.DoString(contents);

                onEvent = lua.GetFunction("OnEvent");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                device.Lcd.RemoveFromFront();
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
                        if (e.IsPressed)
                            try
                            {
                                var keyName = e.Key.ToString();
                                var evnt = keyName.Substring(0, 1) + "_PRESSED";
                                var arg = int.Parse(keyName.Substring(1));
                                onEvent.Call(evnt, arg, "lhc");
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
                            }
                        else
                            try
                            {
                                var keyName = e.Key.ToString();
                                var evnt = keyName.Substring(0, 1) + "_RELEASED";
                                var arg = int.Parse(keyName.Substring(1));
                                onEvent.Call(evnt, arg, "lhc");
                            }
                            catch (Exception ex)
                            {
                                OutputLogMessage(ex.ToString());
                            }

                    if (!isRunning)
                        break;

                    auto.WaitOne();
                }

                try
                {
                    onEvent.Call("PROFILE_DEACTIVATED", 0, "lhc");
                }
                catch (Exception ex)
                {
                    OutputLogMessage(ex.ToString());
                }
            }

            auto.Close();
            auto = null;
        }

        void addKeyEvent(Keys key, bool isPressed)
        {
            lock (keyEvents)
            {
                keyEvents.Enqueue(new KeyEvent(key, isPressed));
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

        void Register(string name, object owner, params Type[] parameters)
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
    }

#endif
}