using System;
using System.Collections.Generic;
using System.Text;

namespace G13Library.TestApp
{
    class G13ProfileBasicTests : G13Profile
    {
        public G13ProfileBasicTests(TestForm form) : base(form) { }

        InputManager.Win32Point screenPos;

        protected override void JoystickChanged(G13Device device, int x, int y)
        {
            // do nothing (i.e. don't print out events)
        }

        protected override void KeyPressed(G13Device device, G13Device.Keys key, ulong keyState)
        {
            form.RtbInfo.AppendText(key.ToString() + " Pressed" + Environment.NewLine);

            switch (key)
            {
                case G13Device.Keys.G1:
                    InputManager.MouseDown(InputManager.MouseDownFlags.LeftDown);
                    break;

                // MouseWheel needs some love. Specifically, it's own background worker to continue wheelin' while the key is pressed
                // So really, a MouseWheelRepeat and MouseWheelEnd are needed in addition to MouseWheel. Might want to also add
                // MouseWheelHorizontal functions as well, I think it's available in the api...
                case G13Device.Keys.G2:
                    InputManager.MouseWheel(120);
                    break;

                case G13Device.Keys.G3:
                    InputManager.MouseWheel(-120);
                    break;

                case G13Device.Keys.G4:
                    screenPos = InputManager.MouseAbsolutePos;
                    break;

                case G13Device.Keys.G5:
                    InputManager.MouseMoveTo(screenPos);
                    break;

                case G13Device.Keys.G6:
                    InputManager.MouseMoveBy(10, 10);
                    break;

                case G13Device.Keys.G7:
                    InputManager.MouseMoveToPixel(567, 171);
                    var pos = InputManager.MouseScreenPos;
                    form.RtbInfo.AppendText("Moved mouse to " + pos.X + ", " + pos.Y + Environment.NewLine);
                    break;

                case G13Device.Keys.G8:
                    InputManager.MouseTap(InputManager.MouseTapFlags.RightTap);
                    break;

                // tested this case using Adobe reader - double middle tap brings up a small view of the full page
                case G13Device.Keys.G9:
                    InputManager.MouseTap(InputManager.MouseTapFlags.MiddleTap);
                    InputManager.MouseTap(InputManager.MouseTapFlags.MiddleTap);
                    break;
            }
        }

        protected override void KeyReleased(G13Device device, G13Device.Keys key, ulong keyState)
        {
            form.RtbInfo.AppendText(key.ToString() + " Released" + Environment.NewLine);

            switch (key)
            {
                case G13Device.Keys.G1:
                    InputManager.MouseUp(InputManager.MouseUpFlags.LeftUp);
                    break;
            }
        }

        protected override void Inserted(G13Device device)
        {
            form.RtbInfo.AppendText("G13 inserted" + Environment.NewLine);
        }

        protected override void Removed(G13Device device)
        {
            form.RtbInfo.AppendText("G13 removed" + Environment.NewLine);
        }
    }
}
