using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace GNet.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInputData : IEquatable<MouseInputData>
    {
        public int X;
        public int Y;
        public int MouseData;
        public MouseEventFlags Flags;
        public uint Time;

        [XmlIgnore]
        public IntPtr ExtraInfo;

        public bool Equals(MouseInputData other)
        {
            return
                other.X == X &&
                other.Y == Y &&
                other.MouseData == MouseData &&
                other.Flags == Flags //&&
                //other.Time == Time &&
                //other.ExtraInfo == ExtraInfo
                ;
        }

        public override int GetHashCode()
        {
            return new OatHash()
                .Hash(X)
                .Hash(Y)
                .Hash(MouseData)
                .Hash((uint)Flags)
                //.Hash(Time)
                //.Hash(ExtraInfo)
                .HashCode;
        }
    }
}
