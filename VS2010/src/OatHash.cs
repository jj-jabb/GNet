using System;
using System.Runtime.InteropServices;

namespace GNet
{
    // for algorithm details, see: http://www.eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx
    /// <summary>
    /// One-at-a-time hash - provides a good general-purpose hashing function.
    /// </summary>
    public struct OatHash
    {
        int hashCode;

        [StructLayout(LayoutKind.Explicit)]
        struct xBytes
        {
            public static xBytes empty;

            public static xBytes Byte(byte x)
            {
                xBytes val = empty; val.b0 = x; return val;
            }

            public static xBytes SByte(sbyte x)
            {
                xBytes val = empty; val.sb = x; return val;
            }

            public static xBytes Int16(short x)
            {
                xBytes val = empty; val.s = x; return val;
            }

            public static xBytes UInt16(ushort x)
            {
                xBytes val = empty; val.us = x; return val;
            }

            public static xBytes Char(char x)
            {
                xBytes val = empty; val.c = x; return val;
            }

            public static xBytes Int32(int x)
            {
                xBytes val = empty; val.i = x; return val;
            }

            public static xBytes UInt32(uint x)
            {
                xBytes val = empty; val.u = x; return val;
            }

            public static xBytes Int64(long x)
            {
                xBytes val = empty; val.l = x; return val;
            }

            public static xBytes UInt64(ulong x)
            {
                xBytes val = empty; val.ul = x; return val;
            }

            public static xBytes Single(float x)
            {
                xBytes val = empty; val.f = x; return val;
            }

            public static xBytes Double(double x)
            {
                xBytes val = empty; val.d = x; return val;
            }

            [FieldOffset(0)]
            public sbyte sb;

            [FieldOffset(0)]
            public short s;

            [FieldOffset(0)]
            public ushort us;

            [FieldOffset(0)]
            public char c;

            [FieldOffset(0)]
            public int i;

            [FieldOffset(0)]
            public uint u;

            [FieldOffset(0)]
            public long l;

            [FieldOffset(0)]
            public ulong ul;

            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public double d;

            [FieldOffset(0)]
            public byte b0;
            [FieldOffset(1)]
            public byte b1;
            [FieldOffset(2)]
            public byte b2;
            [FieldOffset(3)]
            public byte b3;

            [FieldOffset(4)]
            public byte b4;
            [FieldOffset(5)]
            public byte b5;
            [FieldOffset(6)]
            public byte b6;
            [FieldOffset(7)]
            public byte b7;
        }

        OatHash HashFinish()
        {
            hashCode += hashCode << 3;
            hashCode ^= hashCode >> 11;
            hashCode += hashCode << 15;
            return this;
        }

        OatHash Hash8(xBytes val)
        {
            hashCode += val.b0; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            return this;
        }

        OatHash Hash16(xBytes val)
        {
            hashCode += val.b0; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b1; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            return this;
        }

        OatHash Hash32(xBytes val)
        {
            hashCode += val.b0; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b1; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b2; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b3; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            return this;
        }

        OatHash Hash64(xBytes val)
        {
            hashCode += val.b0; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b1; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b2; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b3; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b4; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b5; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b6; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            hashCode += val.b7; hashCode += hashCode << 10; hashCode ^= hashCode >> 6;
            return this;
        }

        public OatHash Hash(byte val)
        {
            xBytes x32val = xBytes.empty;
            x32val.b0 = val;
            return Hash8(x32val);
        }

        public OatHash Hash(bool val)
        {
            return Hash(val == true ? (byte)2 : (byte)1);
        }

        public OatHash Hash(int val)
        {
            xBytes x32val = xBytes.empty;
            x32val.i = val;
            return Hash32(x32val);
        }

        public OatHash Hash(float val)
        {
            xBytes x32val = xBytes.empty;
            x32val.f = val;
            return Hash32(x32val);
        }

        public OatHash Hash(double val)
        {
            xBytes x32val = xBytes.empty;
            x32val.d = val;
            return Hash64(x32val);
        }

        public OatHash Hash(string val)
        {
            if (val == null || val.Length == 0)
                return this;

            for (int i = 0; i < val.Length; i++)
                Hash((byte)val[i]);

            return this;
        }

        public OatHash Hash(object obj)
        {
            if (obj != null)
            {
                switch (Type.GetTypeCode(obj.GetType()))
                {
                    case TypeCode.Boolean:
                        return Hash((bool)obj);

                    case TypeCode.Byte:
                        return Hash8(xBytes.Byte((Byte)obj));

                    case TypeCode.SByte:
                        return Hash8(xBytes.SByte((SByte)obj));

                    case TypeCode.Int16:
                        return Hash16(xBytes.Int16((Int16)obj));

                    case TypeCode.UInt16:
                        return Hash8(xBytes.UInt16((UInt16)obj));

                    case TypeCode.Char:
                        return Hash8(xBytes.Char((Char)obj));

                    case TypeCode.Int32:
                        return Hash8(xBytes.Int32((Int32)obj));

                    case TypeCode.UInt32:
                        return Hash8(xBytes.UInt32((UInt32)obj));

                    case TypeCode.Int64:
                        return Hash8(xBytes.Int64((Int64)obj));

                    case TypeCode.UInt64:
                        return Hash8(xBytes.UInt64((UInt64)obj));

                    case TypeCode.Single:
                        return Hash8(xBytes.Single((Single)obj));

                    case TypeCode.Double:
                        return Hash8(xBytes.Double((Double)obj));

                    case TypeCode.String:
                        return Hash((string)obj);

                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        return this;

                    default:
                        return Hash(obj.GetHashCode());
                }
            }

            return this;
        }

        public int HashCode
        {
            get { HashFinish(); return hashCode; }
        }
    }
}
