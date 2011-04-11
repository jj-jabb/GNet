using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    public delegate void WinEventDelegate(IntPtr hWinEventHook, WinEvent eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

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

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(WinEvent eventMin, WinEvent eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, SetWinEventHookFlags dwFlags);
        
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd,out Int32 lpdwProcessId);

		public static Int32 GetWindowProcessId(IntPtr hwnd)
		{
			//This Function is used to get Active process ID...
			Int32 pid;
			GetWindowThreadProcessId(hwnd, out pid);
			return pid;
		}

        public static Process GetProcess(IntPtr hwnd)
        {
            Int32 pid;
            GetWindowThreadProcessId(hwnd, out pid);
            return Process.GetProcessById(pid);
        }

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

        public static readonly int InputWrapperSize = Marshal.SizeOf(typeof(InputWrapper));

        internal static uint SendInput(uint p, InputWrapper[] inputs)
        {
            return SendInput(p, inputs, InputWrapperSize);
        }
    }
}
