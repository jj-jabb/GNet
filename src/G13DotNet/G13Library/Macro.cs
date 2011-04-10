using System;
using System.Collections.Generic;
using System.Text;

namespace G13Library
{
    public class Macro
    {
        public abstract class Step
        {
        }

        public class KeyDown : Step
        {
        }

        public class KeyUp : Step
        {
        }

        public class KeyTap : Step
        {
        }

        public class MouseDown : Step
        {
        }

        public class MouseUp : Step
        {
        }

        public class MouseTap : Step
        {
        }

        public class MouseWheel : Step
        {
        }

        public class MouseSavePos : Step
        {
        }

        public class MouseRecallPos : Step
        {
        }

        public class MouseMoveTo : Step
        {
        }

        public class MouseNudge : Step
        {
        }

        public class WriteText : Step
        {
        }
    }
}
