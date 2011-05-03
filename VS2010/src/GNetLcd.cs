using System;
using System.Collections.Generic;
using System.Text;
using GNet.LgLcd;

namespace GNet
{
    public class GNetLcd : G13Lcd
    {
        static GNetLcd current;
        public static GNetLcd Current
        {
            get
            {
                if (current == null)
                    current = new GNetLcd();

                return current;
            }
        }

        GNetLcd()
            : base("GNet Profiler")
        {
        }
    }
}
