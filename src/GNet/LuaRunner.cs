using System;
using System.Text;

using LuaInterface;

using GNet.Lib;

namespace GNet
{
    public class LuaRunner : IScriptRunner
    {
        static G13Device device = new G13Device();

        Lua lua;
        LuaFunction onEvent;
        bool isRunning;
        string contents;

        public LuaRunner(string contents)
        {
            this.contents = contents;
        }

        public IScriptRunner Run()
        {
            lua = new Lua();
            lua.RegisterFunction("OutputLogMessage", this, typeof(LuaRunner).GetMethod("OutputLogMessage"));
            lua.DoString(contents);

            onEvent = lua.GetFunction("OnEvent");
            if (onEvent != null)
            {
                device.KeyPressed += new G13Device.KeyPressedHandler(device_KeyPressed);
                device.KeyReleased += new G13Device.KeyReleasedHandler(device_KeyReleased);
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
                    device.KeyPressed -= new G13Device.KeyPressedHandler(device_KeyPressed);
                    device.KeyReleased -= new G13Device.KeyReleasedHandler(device_KeyReleased);
                }

                lua.Dispose();
                isRunning = false;
            }
        }

        void device_KeyPressed(G13Device.Keys key)
        {
            var keyName = key.ToString();
            var evnt = keyName.Substring(0, 1) + "_PRESSED";
            var arg = int.Parse(keyName.Substring(1));
            onEvent.Call(evnt, arg, "lhc");
        }

        void device_KeyReleased(G13Device.Keys key)
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