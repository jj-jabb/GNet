using System;

using GNet.Lib;
using GNet.Lib.IO;

namespace GNet.Scripting
{
    public interface IDeviceScript
    {
        Profile Profile { get; set; }
        bool SingleKeyEvents { get; set; }
        bool IsRunning { get; }

        void JoystickChanged(JoystickPosition position);
        void KeyStateChanged(G13KeyState newState);
        void KeysPressed(ulong keys);
        void KeysReleased(ulong keys);
        void SingleKeyPressed(G13Keys key);
        void SingleKeyReleased(G13Keys key);

        void Start();
        void Stop();

        event EventHandler Started;
        event EventHandler Stopped;
    }
}
