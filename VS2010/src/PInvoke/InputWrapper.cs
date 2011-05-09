using System;
using System.Runtime.InteropServices;

namespace GNet.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputWrapper : IEquatable<InputWrapper>
    {
        //static InputWrapper empty = new InputWrapper() { Type = SendInputType.Empty };
        //public static InputWrapper Empty { get { return empty; } }

        public SendInputType Type;
        public MouseKeyboardHardwareUnion MKH;
        //public bool IsEmpty { get { return Type == SendInputType.Empty; } }

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

        public override int GetHashCode()
        {
            switch (Type)
            {
                case SendInputType.Hardware:
                    return MKH.Hardware.GetHashCode();

                case SendInputType.Keyboard:
                    return MKH.Keyboard.GetHashCode();

                case SendInputType.Mouse:
                    return MKH.Mouse.GetHashCode();

                default:
                    return base.GetHashCode();
            }
        }

        public static int CalcHashCode(InputWrapper[] inputs)
        {
            OatHash hash = new OatHash();
            if (inputs == null)
                return 0;
            else
                foreach (var input in inputs)
                    hash.Hash(input.GetHashCode());

            return hash.HashCode;
        }

        public static InputWrapper[] GetRelease(InputWrapper[] inputs)
        {
            if (inputs != null && inputs.Length == 1)
            {
                InputWrapper release = new InputWrapper();
                InputWrapper input = inputs[0];

                switch (input.Type)
                {
                    case SendInputType.Keyboard:
                        if ((input.MKH.Keyboard.Flags & KeyboardFlags.KeyUp) == 0)
                        {
                            release.MKH.Keyboard = input.MKH.Keyboard;
                            release.MKH.Keyboard.Flags |= KeyboardFlags.KeyUp;
                            return new InputWrapper[] { release };
                        }
                        break;

                    case SendInputType.Mouse:
                        bool isMouseDown = false;
                        release.MKH.Mouse = input.MKH.Mouse;
                        switch (input.MKH.Mouse.Flags)
                        {
                            case MouseEventFlags.LeftDown:
                                release.MKH.Mouse.Flags &= ~MouseEventFlags.LeftDown;
                                release.MKH.Mouse.Flags |= MouseEventFlags.LeftUp;
                                isMouseDown = true;
                                break;

                            case MouseEventFlags.MiddleDown:
                                release.MKH.Mouse.Flags &= ~MouseEventFlags.MiddleDown;
                                release.MKH.Mouse.Flags |= MouseEventFlags.MiddleUp;
                                isMouseDown = true;
                                break;

                            case MouseEventFlags.RightDown:
                                release.MKH.Mouse.Flags &= ~MouseEventFlags.RightDown;
                                release.MKH.Mouse.Flags |= MouseEventFlags.RightUp;
                                isMouseDown = true;
                                break;

                            case MouseEventFlags.XDown:
                                release.MKH.Mouse.Flags &= ~MouseEventFlags.XDown;
                                release.MKH.Mouse.Flags |= MouseEventFlags.XUp;
                                isMouseDown = true;
                                break;
                        }

                        if (isMouseDown)
                            return new InputWrapper[] { release };

                        break;
                }
            }

            return null;
        }
    }
}
