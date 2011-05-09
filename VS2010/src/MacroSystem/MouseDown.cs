using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
{
    public class MouseDown : Step
    {
        MouseUp reverse;

        public MouseDown(int button)
        {
            int data = 0;
            switch (button)
            {
                case 0:
                case 1:
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.LeftDown, data) };
                    toString = "MouseDown(" + MouseEventFlags.LeftDown.ToString() + ")";
                    break;

                case 2:
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.RightDown, data) };
                    toString = "MouseDown(" + MouseEventFlags.RightDown.ToString() + ")";
                    break;

                case 3:
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.MiddleDown, data) };
                    toString = "MouseDown(" + MouseEventFlags.MiddleDown.ToString() + ")";
                    break;

                default:
                    data = button - 3;
                    inputs = new InputWrapper[] { InputSimulator.MouseWrapper(MouseEventFlags.XDown, data) };
                    toString = "MouseDown(" + MouseEventFlags.XDown.ToString() + ", " + data + ")";
                    break;
            }

            reverse = new MouseUp(button);
        }

        public override Step Release
        {
            get
            {
                return reverse;
            }
        }
    }
}
