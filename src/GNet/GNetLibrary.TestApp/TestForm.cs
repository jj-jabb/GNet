using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GNetLibrary.PInvoke;

namespace GNetLibrary.TestApp
{
    public partial class TestForm : Form
    {
        G13Profile tester;
        G13PyProfile pyTester;

        public TestForm()
        {
            InitializeComponent();
        }

        public RichTextBox RtbInfo { get { return rtbInfo; } }

        private void TestForm_Load(object sender, EventArgs e)
        {

            // for python testing:
            pyTester = new G13PyProfile(this);

            // for general testing:
            // tester = new G13ProfileBasicTests(this);
            
            // for FPS gaming:
            // tester = new G13ProfileFPS(this);
        }

        void GetActiveWindowTest()
        {
            var hEvent = Interop.SetWinEventHook(WinEvent.EVENT_SYSTEM_FOREGROUND, WinEvent.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, new WinEventDelegate(WinEventProcCallback), 0, 0, SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT | SetWinEventHookFlags.WINEVENT_SKIPOWNPROCESS);
        }

        void WinEventProcCallback(IntPtr hWinEventHook, WinEvent eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Process process = null;
            string name = "";
            string fileName = "";
            try
            {
                process = Interop.GetProcess(hwnd);
                name = process.ProcessName;
                fileName = process.MainModule.FileName;
            }
            catch(Exception ex)
            {
                // explorer throws when process.MainModule is accessed
                Debug.WriteLine(ex);
            }

            Debug.WriteLine(name + " : " + fileName + " : " + eventType);
            //Debug.WriteLine(process.ProcessName + " : " + process.MainModule.FileName + " : " + eventType);
        }
    }
}
