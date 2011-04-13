using System;
using System.Collections.Generic;
using System.Text;

using GNetLibrary.IO;
using GNetLibrary.MacroSystem;
using GNetLibrary.PInvoke;

namespace GNetLibrary.TestApp
{
    class G13ProfileFPS : G13Profile
    {
        public G13ProfileFPS(TestForm form) : base(form) { }

        protected override void KeyPressed(G13Device device, G13Device.Keys key, ulong keyState)
        {
            switch (key)
            {
                case G13Device.Keys.G4:
                    KeyDown(ScanCode.w, repeat: false);
                    break;

                case G13Device.Keys.G11:
                    KeyDown(ScanCode.s);
                    break;

                case G13Device.Keys.G10:
                    KeyDown(ScanCode.a);
                    break;

                case G13Device.Keys.G12:
                    KeyDown(ScanCode.d);
                    break;

                case G13Device.Keys.G22:
                    KeyDown(ScanCode.space);
                    break;

                case G13Device.Keys.G15:
                    KeyDown(ScanCode.lcontrol);
                    break;

                case G13Device.Keys.G5:
                    KeyDown(ScanCode.e);
                    break;

                case G13Device.Keys.G6:
                    KeyDown(ScanCode.r);
                    break;

                case G13Device.Keys.G3:
                    KeyDown(ScanCode.q);
                    break;
            }
        }

        protected override void KeyReleased(G13Device device, G13Device.Keys key, ulong keyState)
        {
            switch (key)
            {
                case G13Device.Keys.G4:
                    KeyUp(ScanCode.w);
                    break;

                case G13Device.Keys.G11:
                    KeyUp(ScanCode.s);
                    break;

                case G13Device.Keys.G10:
                    KeyUp(ScanCode.a);
                    break;

                case G13Device.Keys.G12:
                    KeyUp(ScanCode.d);
                    break;

                case G13Device.Keys.G22:
                    KeyUp(ScanCode.space);
                    break;

                case G13Device.Keys.G15:
                    KeyUp(ScanCode.lcontrol);
                    break;

                case G13Device.Keys.G5:
                    KeyUp(ScanCode.e);
                    break;

                case G13Device.Keys.G6:
                    KeyUp(ScanCode.r);
                    break;

                case G13Device.Keys.G3:
                    KeyUp(ScanCode.q);
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
