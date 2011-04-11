using System;
using System.Runtime.InteropServices;

namespace GNetLibrary.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputWrapper : IEquatable<InputWrapper>
    {
        public SendInputType Type;
        public MouseKeyboardHardwareUnion MKH;

        public bool Equals(InputWrapper other)
        {
            if (other.Type != Type)
                return false;

            switch (Type)
            {
                case SendInputType.Hardware :
                    return MKH.Hardware.Equals(other.MKH.Hardware);
                    
                case SendInputType.Keyboard:
                    return MKH.Keyboard.Equals(other.MKH.Keyboard);

                case SendInputType.Mouse:
                    return MKH.Mouse.Equals(other.MKH.Mouse);
            }

            return false;
        }
    }
}
