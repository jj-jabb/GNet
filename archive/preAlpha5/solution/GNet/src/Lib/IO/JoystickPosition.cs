using System;

namespace GNet.Lib.IO
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
