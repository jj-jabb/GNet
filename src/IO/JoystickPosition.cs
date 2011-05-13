using System;

namespace GNet.IO
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
