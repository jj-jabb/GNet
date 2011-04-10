using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInputData
    {
        public int X;
        public int Y;
        public int MouseData;
        public MouseEventFlags Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
}
