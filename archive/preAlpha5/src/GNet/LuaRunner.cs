using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;

using LuaInterface;

using GNet.Lib;

namespace GNet
{
    public class LuaRunner : IScriptRunner, IDisposable
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

        Lcd lcd;
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

            lcd = new Lcd("GNet Profiler", false, false, LcdDeviceType.LcdDeviceBW);
            var connected = lcd.Connect();
            var open = lcd.Open();
        }

        public void Dispose()
        {
            lcd.Dispose();
            lcd = null;
        }

        public IScriptRunner Run()
        {
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
            lcd.Dispose();
            lcd = null;
        }

        void device_Inserted()
        {
            lcd = new Lcd("GNet Profiler", false, false, LcdDeviceType.LcdDeviceBW);
            lcd.Connect();
            lcd.Open();
            DrawString("GNet Profiler Alpha Release", "Microsoft Sans Serif", 10, 1, "#FFFFFF", 0, 0);
            lcd.BringToFront();
        }

        public void Stop()
        {
            if (isRunning)
            {
                device.KeyPressed -= new G13Device.KeyHandler(device_KeyPressed);
                device.KeyReleased -= new G13Device.KeyHandler(device_KeyReleased);

                isRunning = false;
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
            if (lcd == null)
                return;

            FontStyle fstyle = (FontStyle)fontStyle;
            float fsize = (float)fontSize;
            float fx = (float)x;
            float fy = (float)y;
            Brush brush = new SolidBrush(ColorTranslator.FromHtml(htmlColor));

            using (Font f = new Font(fontName, fsize, fstyle))
            {
                lcd.LcdGraphics.DrawString(text, f, brush, 0f, 0f);
            }

            lcd.UpdateBitmap(LcdPriority.Normal);
        }

        public void ClearLcd()
        {
            lcd.LcdGraphics.Clear(Color.Black);
            lcd.UpdateBitmap(LcdPriority.Normal);
        }



        void RunThread()
        {
            KeyEvent e = KeyEvent.Empty;

            if (!device.IsConnected)
                return;

            lcd.BringToFront();
            Thread.Sleep(100);
            DrawString("GNet Profiler\nAlpha Release", "Microsoft Sans Serif", 10, 1, "#FFFFFF", 0, 0);

            try
            {
                lua = new Lua();

                Register("OutputLogMessage", this);
                Register("ClearLog", this);
                Register("Sleep", this, typeof(int));
                Register("GetRunningTime", this);
                Register("DrawString", this, typeof(string), typeof(string), typeof(double), typeof(double), typeof(string), typeof(double), typeof(double));
                Register("ClearLcd", this);

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
                lcd.RemoveFromFront();
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

            if (lcd != null)
                lcd.RemoveFromFront();
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
}