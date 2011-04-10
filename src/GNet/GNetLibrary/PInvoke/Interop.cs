using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    public static class Interop
    {
        // http://msdn.microsoft.com/en-us/library/ms724385(v=vs.85).aspx
        // http://www.pinvoke.net/default.aspx/user32.getsystemmetrics
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        public static extern ushort VkKeyScan(char ch);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint numberInputs, InputWrapper[] inputs, int sizeOfStructure);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(VirtualKeyStates nVirtKey);

        public static bool IsKeyPressed(VirtualKeyStates testKey)
        {
            bool keyPressed = false;
            short result = GetKeyState(testKey);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled on.
                    keyPressed = false;
                    break;

                case 1:
                    // Not pressed, but toggled on
                    keyPressed = false;
                    break;

                default:
                    // Pressed (and may be toggled on)
                    keyPressed = true;
                    break;
            }

            return keyPressed;
        }
    }
}
