using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInputData
    {
        public ushort Key;
        public ScanCode Scan;
        public KeyboardFlags Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
}
