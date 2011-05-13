using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace GNet.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInputData : IEquatable<KeyboardInputData>
    {
        public ushort Key;
        public ScanCode Scan;
        public KeyboardFlags Flags;
        public uint Time;

        [XmlIgnore]
        public IntPtr ExtraInfo;

        public bool Equals(KeyboardInputData other)
        {
            return
                other.Key == Key &&
                other.Scan == Scan &&
                other.Flags == Flags //&&
                //other.Time == Time &&
                //other.ExtraInfo == ExtraInfo
                ;
        }

        public override int GetHashCode()
        {
            return new OatHash()
                .Hash(Key)
                .Hash((ushort)Scan)
                .Hash((ushort)Flags)
                //.Hash(Time)
                //.Hash(ExtraInfo)
                .HashCode;
        }
    }
}
