using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;

using LuaInterface;

using GNet;
using GNet.IO;
using GNet.LgLcd;
using GNet.Properties;

namespace GNet.Scripting
{
    public class LuaScript : G13Script, IDisposable
    {
        LuaFunction onEvent;
        LuaFunction onKEvent;
        LuaFunction onMEvent;

        DateTime startTime;

        GNetLcd lcd;

        public LuaScript()
        {
            lcd = GNetLcd.Current;
            lcd.Notified += new NotificationEventHandler(Connection_Notified);
            SingleKeyEvents = true;
        }

        public void Dispose()
        {
            lcd.Notified -= new NotificationEventHandler(Connection_Notified);
        }

        void Connection_Notified(int code, int param1, int param2, int param3, int param4)
        {
            if (code == Sdk.LGLCD_NOTIFICATION_DEVICE_ARRIVAL && IsRunning)
            {
                ClearGraphics();
                DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, 0, 0, 255, 255, 255);

                lcd.BringToFront();
            }
        }

        protected override void OnStart()
        {
            startTime = DateTime.Now;
        }

        protected override void OnStop()
        {
            
        }

        protected override void RunThread()
        {
            KeyEvent e = KeyEvent.Empty;
            Lua lua = null;
            bool inError = false;

            var inputEvent = new EventWaitHandle(false, EventResetMode.AutoReset, inputEventName);

            try
            {
                lcd.Graphics.Clear(Color.Black);
                DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, 0, 0, 255, 255, 255);

                lcd.DrawImage(Resources.checkmark_icon_16, 92f, 25f);
                lcd.DrawImage(Resources.cancel_icon_16, 132f, 25f);
                lcd.BringToFront();

                lua = new Lua();

                Register(lua, "OutputLogMessage", this);
                Register(lua, "ClearLog", this);
                Register(lua, "Sleep", this, typeof(int));
                Register(lua, "GetRunningTime", this);
                Register(lua, "DrawString", this, typeof(string), typeof(string), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double));
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

                lua.DoString(Profile.Contents);

                onEvent = lua.GetFunction("OnEvent");
                onKEvent = lua.GetFunction("OnKeyEvent");
                onMEvent = lua.GetFunction("OnMouseEvent");
            }
            catch (Exception ex)
            {
                if (lua != null)
                    lua.Close();

                lua = null;

                inError = true;
                OnScriptError(ex);
            }

            try
            {
                if (onEvent != null && !inError)
                    onEvent.Call("PROFILE_ACTIVATED", 0, "lhc");
            }
            catch (Exception ex)
            {
                OutputLogMessage(ex.ToString());
            }

            while (IsRunning)
            {
                if (!inError)
                {
                    for (e = GetKeyEvent(); e.IsEmpty == false; e = GetKeyEvent())
                    {
                        #region check input

                        if (e.Key != G13Keys.None)
                        {
                            try
                            {
                                var keyName = e.Key.ToString();
                                var evnt = keyName.Substring(0, 1) + (e.IsPressed ? "_PRESSED" : "_RELEASED");
                                var arg = int.Parse(keyName.Substring(1));
                                if (onEvent != null)
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
                                if (onEvent != null)
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
                                if (onEvent != null)
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

                        #endregion

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

            var inputExit = new EventWaitHandle(false, EventResetMode.AutoReset, inputExitName);
            inputExit.Set();
            inputExit.Close();

            try
            {
                if(onEvent != null && !inError)
                    onEvent.Call("PROFILE_DEACTIVATED", 0, "lhc");
            }
            catch (Exception ex)
            {
                OutputLogMessage(ex.ToString());
            }

            if (lua != null)
                lua.Close();
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

        int callCount = 0;
        public void SetMLight(double key)
        {
            callCount++;
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

        #endregion
    }
}