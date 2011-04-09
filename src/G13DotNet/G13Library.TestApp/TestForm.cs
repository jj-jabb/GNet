using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace G13Library.TestApp
{
    public partial class TestForm : Form
    {
        G13Device g13;

        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
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

        void JoystickChanged(G13Device device, int x, int y)
        {
        }


        void KeyPressed(G13Device device, G13Device.Keys key, ulong keyState)
        {
            rtbInfo.AppendText(key.ToString() + " Pressed" + Environment.NewLine);

            switch (key)
            {
                case G13Device.Keys.G1:

                case G13Device.Keys.G4:
                    InputManager.KeyDown(InputManager.ScanCode.w, repeat: true);
                    break;
            }
        }

        void KeyReleased(G13Device device, G13Device.Keys key, ulong keyState)
        {
            rtbInfo.AppendText(key.ToString() + " Released" + Environment.NewLine);

            switch (key)
            {
                case G13Device.Keys.G4:
                    InputManager.KeyUp(InputManager.ScanCode.w);
                    break;
            }
        }

        void Inserted(G13Device device)
        {
            rtbInfo.AppendText("G13 inserted" + Environment.NewLine);
        }

        void Removed(G13Device device)
        {
            rtbInfo.AppendText("G13 removed" + Environment.NewLine);
        }

        void DeviceConnected(G13Device device)
        {
        }
    }
}
