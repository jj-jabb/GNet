using System;
using System.Runtime.InteropServices;

namespace GNet.Lib.PInvoke
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MouseKeyboardHardwareUnion
    {
        [FieldOffset(0)]
        public MouseInputData Mouse;
        [FieldOffset(0)]
        public KeyboardInputData Keyboard;
        [FieldOffset(0)]
        public HardwareInputData Hardware;
    }
}
