using System;
using System.Runtime.InteropServices;

namespace GNet.Lib.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public Int32 X;
        public Int32 Y;

        public override string ToString()
        {
            return "Win32Point: " + X + ", " + Y;
        }
    };
}
