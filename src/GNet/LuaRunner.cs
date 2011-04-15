using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;

using LuaInterface;

using GNet.Lib;

namespace GNet
{
    public class LuaRunner : IScriptRunner
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

        public LuaRunner(string contents)
        {
            this.contents = contents;

            keyEvents = new Queue<KeyEvent>();
        }

        public IScriptRunner Run()
        {
            device.KeyPressed += new G13Device.KeyHandler(device_KeyPressed);
            device.KeyReleased += new G13Device.KeyHandler(device_KeyReleased);

            isRunning = true;

            auto = new AutoResetEvent(false);
            thread = new Thread(new ThreadStart(RunThread));
            thread.Start();

            return this;
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

        void RunThread()
        {
            KeyEvent e = KeyEvent.Empty;

            try
            {
                lua = new Lua();

                Register("OutputLogMessage", this);
                Register("ClearLog", this);
                Register("Sleep", this, typeof(int));

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
}