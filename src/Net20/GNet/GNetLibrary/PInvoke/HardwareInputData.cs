using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInputData
    {
        public int Msg;
        public short ParamL;
        public short ParamH;
    }
}
