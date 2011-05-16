using System;

using GNet;
using GNet.IO;

namespace GNet.Profiler
{
    public interface IProfileRunner
    {
        //Profile Profile { get; set; }
        bool SingleKeyEvents { get; }
        bool IsRunning { get; }

        void JoystickChanged(JoystickPosition position);
        void KeyStateChanged(G13KeyState newState);
        void KeysPressed(ulong keys);
        void KeysReleased(ulong keys);
        void SingleKeyPressed(G13Keys key);
        void SingleKeyReleased(G13Keys key);

        bool Start();
        void Stop();

        event EventHandler Started;
        event EventHandler Stopped;
    }
}
