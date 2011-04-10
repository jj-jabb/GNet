using System;
using System.Collections.Generic;
using System.Text;

using GNetLibrary.IO;

namespace GNetLibrary.TestApp
{
    abstract class G13Profile : InputSimulator
    {
        protected readonly G13Device g13;
        protected readonly TestForm form;

        public G13Profile(TestForm form)
        {
            this.form = form;

            g13 = new G13Device();

            g13.JoystickChanged += new G13Device.JoystickChangedHandler(JoystickChanged);
            g13.KeyPressed += new G13Device.KeyPressedHandler(KeyPressed);
            g13.KeyReleased += new G13Device.KeyReleasedHandler(KeyReleased);

            g13.Inserted += new G13Device.DeviceEventHandler(Inserted);
            g13.Removed += new G13Device.DeviceEventHandler(Removed);

            if (!g13.IsConnected)
            {
                g13.DeviceConnected += new G13Device.DeviceEventHandler(DeviceConnected);
                g13.WaitForConnection();
            }
        }

        protected virtual void JoystickChanged(G13Device device, int x, int y)
        {
            form.RtbInfo.AppendText("Joystick Changed: " + x + ", " + y + Environment.NewLine);
        }

        protected virtual void KeyPressed(G13Device device, G13Device.Keys key, ulong keyState)
        {
            form.RtbInfo.AppendText(key.ToString() + " Pressed" + Environment.NewLine);
        }

        protected virtual void KeyReleased(G13Device device, G13Device.Keys key, ulong keyState)
        {
            form.RtbInfo.AppendText(key.ToString() + " Released" + Environment.NewLine);
        }

        protected virtual void Inserted(G13Device device)
        {
            form.RtbInfo.AppendText("G13 inserted" + Environment.NewLine);
        }

        protected virtual void Removed(G13Device device)
        {
            form.RtbInfo.AppendText("G13 removed" + Environment.NewLine);
        }

        protected virtual void DeviceConnected(G13Device device)
        {
            form.RtbInfo.AppendText("G13 connected" + Environment.NewLine);
        }
    }
}
