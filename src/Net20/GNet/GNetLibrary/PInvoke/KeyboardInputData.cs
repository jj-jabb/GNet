using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInputData : IEquatable<KeyboardInputData>
    {
        public ushort Key;
        public ScanCode Scan;
        public KeyboardFlags Flags;
        public uint Time;
        public IntPtr ExtraInfo;

        public bool Equals(KeyboardInputData other)
        {
            return
                other.Key == Key &&
                other.Scan == Scan &&
                other.Flags == Flags &&
                other.Time == Time &&
                other.ExtraInfo == ExtraInfo
                ;
        }
    }
}
