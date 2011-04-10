using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputWrapper
    {
        public SendInputType Type;
        public MouseKeyboardHardwareUnion MKH;
    }
}
