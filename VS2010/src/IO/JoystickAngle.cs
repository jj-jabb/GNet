using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.IO
{
    public struct JoystickAngle
    {
        public readonly double R;
        public readonly double D2;

        public JoystickAngle(int x, int y)
        {
            R = Math.Atan2(x, y);
            D2 = x * x + y * y;
        }
    }
}
