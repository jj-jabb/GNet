using System;
using System.Collections.Generic;
using System.Text;

namespace G13Library.TestApp
{
    class G13ProfileFPS : G13Profile
    {
        public G13ProfileFPS(TestForm form) : base(form) { }

        protected override void KeyPressed(G13Device device, G13Device.Keys key, ulong keyState)
        {
            switch (key)
            {
                case G13Device.Keys.G4:
                    InputManager.KeyDown(InputManager.ScanCode.w, repeat: false);
                    break;

                case G13Device.Keys.G11:
                    InputManager.KeyDown(InputManager.ScanCode.s);
                    break;

                case G13Device.Keys.G10:
                    InputManager.KeyDown(InputManager.ScanCode.a);
                    break;

                case G13Device.Keys.G12:
                    InputManager.KeyDown(InputManager.ScanCode.d);
                    break;

                case G13Device.Keys.G22:
                    InputManager.KeyDown(InputManager.ScanCode.space);
                    break;

                case G13Device.Keys.G15:
                    InputManager.KeyDown(InputManager.ScanCode.lcontrol);
                    break;

                case G13Device.Keys.G5:
                    InputManager.KeyDown(InputManager.ScanCode.e);
                    break;

                case G13Device.Keys.G6:
                    InputManager.KeyDown(InputManager.ScanCode.r);
                    break;

                case G13Device.Keys.G3:
                    InputManager.KeyDown(InputManager.ScanCode.q);
                    break;
            }
        }

        protected override void KeyReleased(G13Device device, G13Device.Keys key, ulong keyState)
        {
            switch (key)
            {
                case G13Device.Keys.G4:
                    InputManager.KeyUp(InputManager.ScanCode.w);
                    break;

                case G13Device.Keys.G11:
                    InputManager.KeyUp(InputManager.ScanCode.s);
                    break;

                case G13Device.Keys.G10:
                    InputManager.KeyUp(InputManager.ScanCode.a);
                    break;

                case G13Device.Keys.G12:
                    InputManager.KeyUp(InputManager.ScanCode.d);
                    break;

                case G13Device.Keys.G22:
                    InputManager.KeyUp(InputManager.ScanCode.space);
                    break;

                case G13Device.Keys.G15:
                    InputManager.KeyUp(InputManager.ScanCode.lcontrol);
                    break;

                case G13Device.Keys.G5:
                    InputManager.KeyUp(InputManager.ScanCode.e);
                    break;

                case G13Device.Keys.G6:
                    InputManager.KeyUp(InputManager.ScanCode.r);
                    break;

                case G13Device.Keys.G3:
                    InputManager.KeyUp(InputManager.ScanCode.q);
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
