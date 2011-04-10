using System;

namespace GNetLibrary.IO
{
    public struct JoystickPosition
    {
        public int X;
        public int Y;

        public JoystickPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
