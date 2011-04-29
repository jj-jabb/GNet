using System;
using System.Runtime.InteropServices;

namespace GNet.Scripting
{
    [StructLayout(LayoutKind.Explicit)]
    public struct G13KeyState
    {
        [FieldOffset(0)]
        public byte B0;
        [FieldOffset(1)]
        public byte B1;
        [FieldOffset(2)]
        public byte B2;
        [FieldOffset(3)]
        public byte B3;
        [FieldOffset(4)]
        public byte B4;

        [FieldOffset(0)]
        public ulong UL;
    }
}
