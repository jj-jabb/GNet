using System;
using System.Runtime.InteropServices;

namespace GNet.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInputData
    {
        public int Msg;
        public short ParamL;
        public short ParamH;

        public override int GetHashCode()
        {
            return new OatHash()
                .Hash(Msg)
                .Hash(ParamL)
                .Hash(ParamH)
                .HashCode;
        }
    }
}
