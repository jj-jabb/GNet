using System;
using System.Runtime.InteropServices;

namespace GNet.Lib.PInvoke
{
    [Flags]
    public enum MouseDataFlags : uint
    {
        XButton1 = 0x0001,
        XButton2 = 0x0002
    }

    [Flags]
    public enum MouseEventFlags : uint
    {
        Move = 0x0001,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,
        VirtualDesktop = 0x4000,
        Absolute = 0x8000
    }

    [Flags]
    public enum MouseDownFlags : uint
    {
        LeftDown = 0x0002,
        RightDown = 0x0008,
        MiddleDown = 0x0020,
        XDown = 0x0080
    }

    [Flags]
    public enum MouseUpFlags : uint
    {
        LeftUp = 0x0004,
        RightUp = 0x0010,
        MiddleUp = 0x0040,
        XUp = 0x0100
    }

    [Flags]
    public enum MouseTapFlags : uint
    {
        LeftTap = 0x0002,
        RightTap = 0x0008,
        MiddleTap = 0x0020,
        XTap = 0x0080
    }
}
