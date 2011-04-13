using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [Flags]
    public enum KeyboardFlags : uint
    {
        ExtendedKey = 0x1,
        KeyUp = 0x2,
        Unicode = 0x4,
        ScanCode = 0x8
    }
}
