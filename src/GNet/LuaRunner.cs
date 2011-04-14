using System;
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

        public IScriptRunner Run()
        {
            lua = new Lua();

            var park1 = typeof(G13Device).GetMethod("PressAndReleaseKey", new Type[] { typeof(string[]) });
            var park2 = typeof(G13Device).GetMethod("PressAndReleaseKey", new Type[] { typeof(ushort[]) });

            lua.RegisterFunction("OutputLogMessage", this, typeof(LuaRunner).GetMethod("OutputLogMessage"));
            lua.RegisterFunction("PressAndReleaseKey", device, park1);
            // this won't work - need to make PressAndReleaseKey take an object[] and check it for
            // strings / ushorts
            //lua.RegisterFunction("PressAndReleaseKey", device, park2);

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