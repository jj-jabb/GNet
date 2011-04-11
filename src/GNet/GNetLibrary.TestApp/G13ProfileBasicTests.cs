using System;
using System.Collections.Generic;
using System.Text;

using GNetLibrary.IO;
using GNetLibrary.MacroSystem;
using GNetLibrary.PInvoke;

namespace GNetLibrary.TestApp
{
    class G13ProfileBasicTests : G13Profile
    {
        public G13ProfileBasicTests(TestForm form) : base(form) { }

        Win32Point screenPos;

        protected override void JoystickChanged(G13Device device, int x, int y)
        {
            // do nothing (i.e. don't print out events)
        }

        Macro savePos = new Macro(
            new MouseSavePos("test")
            );

        Macro writeAtSavedPos = new Macro(
            new MouseRecallPos("test"),
            new MouseDown(1),
            new KeyDown(ScanCode.lshift),
            new KeyDown('a')
            );

        Macro writeAtSavedPos2 = new Macro(
            new MouseRecallPos("test"),
            new MouseDown(1),
            new KeyDown(ScanCode.lshift),
            new KeyDown('a'),
            new Delay(200),
            new KeyTap('b'),
            new Delay(200),
            new KeyTap('c'),
            new Delay(200),
            new KeyTap('d'),
            new KeyUp(ScanCode.lshift)
            );

        protected override void KeyPressed(G13Device device, G13Device.Keys key, ulong keyState)
        {
            form.RtbInfo.AppendText(key.ToString() + " Pressed" + Environment.NewLine);


            switch (key)
            {
                case G13Device.Keys.G1:
                    MouseDown(PInvoke.MouseDownFlags.LeftDown);
                    break;

                // MouseWheel needs some love. Specifically, it's own background worker to continue wheelin' while the key is pressed
                // So really, a MouseWheelRepeat and MouseWheelEnd are needed in addition to MouseWheel. Might want to also add
                // MouseWheelHorizontal functions as well, I think it's available in the api...
                case G13Device.Keys.G2:
                    MouseWheel(120);
                    break;

                case G13Device.Keys.G3:
                    MouseWheel(-120);
                    break;

                case G13Device.Keys.G4:
                    screenPos = MouseAbsolutePos;
                    break;

                case G13Device.Keys.G5:
                    MouseMoveTo(screenPos);
                    break;

                case G13Device.Keys.G6:
                    MouseMoveBy(10, 10);
                    break;

                case G13Device.Keys.G7:
                    MouseMoveToPixel(567, 171);
                    var pos = MouseScreenPos;
                    form.RtbInfo.AppendText("Moved mouse to " + pos.X + ", " + pos.Y + Environment.NewLine);
                    break;

                case G13Device.Keys.G8:
                    MouseTap(MouseTapFlags.RightTap);
                    break;

                // tested this case using Adobe reader - double middle tap brings up a small view of the full page
                case G13Device.Keys.G9:
                    MouseTap(MouseTapFlags.MiddleTap);
                    MouseTap(MouseTapFlags.MiddleTap);
                    break;

                case G13Device.Keys.G10:
                    savePos.Run();
                    break;

                case G13Device.Keys.G11:
                    writeAtSavedPos2.Run();
                    break;
            }
        }

        protected override void KeyReleased(G13Device device, G13Device.Keys key, ulong keyState)
        {
            form.RtbInfo.AppendText(key.ToString() + " Released" + Environment.NewLine);

            switch (key)
            {
                case G13Device.Keys.G1:
                    MouseUp(MouseUpFlags.LeftUp);
                    break;

                case G13Device.Keys.G11:
                    writeAtSavedPos2.Cleanup();
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
