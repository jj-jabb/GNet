using System;
using System.Reflection;
using System.Text;

using LuaInterface;

using GNet.Lib;

namespace GNet
{
    public class LuaRunner : IScriptRunner
    {
        static G13Device device = G13Device.Current;

        Lua lua;
        LuaFunction onEvent;
        bool isRunning;
        string contents;

        public LuaRunner(string contents)
        {
            this.contents = contents;
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

        public IScriptRunner Run()
        {
            lua = new Lua();

            var mi = device.GetType().GetMethods();

            Register("OutputLogMessage", this);

            // these don't work correctly
            //Register("GetMKeyState", device, typeof(string));
            //Register("SetMKeyState", device, typeof(int), typeof(string));

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
            if (onEvent != null)
            {
                device.KeyPressed += new G13Device.KeyHandler(device_KeyPressed);
                device.KeyReleased += new G13Device.KeyHandler(device_KeyReleased);
                onEvent.Call("PROFILE_ACTIVATED", 0, "lhc");
            }

            isRunning = true;

            return this;
        }

        public void Stop()
        {
            if (isRunning)
            {
                if (onEvent != null)
                {
                    onEvent.Call("PROFILE_DEACTIVATED", 0, "lhc");
                    device.KeyPressed -= new G13Device.KeyHandler(device_KeyPressed);
                    device.KeyReleased -= new G13Device.KeyHandler(device_KeyReleased);
                }

                lua.Dispose();
                isRunning = false;
            }
        }

        void device_KeyPressed(Keys key)
        {
            var keyName = key.ToString();
            var evnt = keyName.Substring(0, 1) + "_PRESSED";
            var arg = int.Parse(keyName.Substring(1));
            onEvent.Call(evnt, arg, "lhc");
        }

        void device_KeyReleased(Keys key)
        {
            var keyName = key.ToString();
            var evnt = keyName.Substring(0, 1) + "_RELEASED";
            var arg = int.Parse(keyName.Substring(1));
            onEvent.Call(evnt, arg, "lhc");
        }

        public void OutputLogMessage(object s)
        {
            Console.WriteLine(s.ToString());
        }
    }
}