using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInputData : IEquatable<MouseInputData>
    {
        public int X;
        public int Y;
        public int MouseData;
        public MouseEventFlags Flags;
        public uint Time;
        public IntPtr ExtraInfo;

        public bool Equals(MouseInputData other)
        {
            return
                other.X == X &&
                other.Y == Y &&
                other.MouseData == MouseData &&
                other.Flags == Flags &&
                other.Time == Time &&
                other.ExtraInfo == ExtraInfo
                ;
        }
    }
}
