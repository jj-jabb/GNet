using System;
using System.Runtime.InteropServices;

namespace GNet.Lib.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInputData
    {
        public int Msg;
        public short ParamL;
        public short ParamH;
    }
}
