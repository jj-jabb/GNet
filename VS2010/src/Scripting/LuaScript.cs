using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using LuaInterface;

using GNet;
using GNet.IO;
using System.Reflection;
using System.Drawing;

namespace GNet.Scripting
{
    public class LuaScript : G13Script
    {
        LuaFunction onEvent;
        LuaFunction onKEvent;
        LuaFunction onMEvent;

        DateTime startTime;

        public LuaScript()
        {
            SingleKeyEvents = true;
        }

        protected LgLcd Lcd { get { return Device == null ? null : Device.Lcd; } }

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

            var inputEvent = CreateInputEvent();

            try
            {
                DrawString("GNet Profiler\nLua Runner\nAlpha Release", "Tahoma", 9, 0, "#FFFFFF", 0, 0);
                Lcd.BringToFront();

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

                lua.DoString(Profile.Contents);

                onEvent = lua.GetFunction("OnEvent");
                onKEvent = lua.GetFunction("OnKeyEvent");
                onMEvent = lua.GetFunction("OnMouseEvent");
            }
            catch (Exception ex)
            {
                if (lua != null)
                    lua.Close();

                inError = true;
                OnScriptError(ex);
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
            }

            while (IsRunning)
            {
                if (!inError)
                {
                    for (e = GetKeyEvent(); e.IsEmpty == false; e = GetKeyEvent())
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

                        if (!IsRunning)
                            break;
                    }
                }

                inputEvent.WaitOne();
            }

            ClearKeyEvents();

            Lcd.RemoveFromFront();

            inputEvent.Close();

            var inputExit = CreateInputExit();
            inputExit.Set();
            inputExit.Close();

            try
            {
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