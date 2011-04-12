using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Management;

using GNetLibrary.PInvoke;

namespace GNetLibrary.TestApp
{
    public partial class TestForm : Form
    {
        G13Profile tester;
        G13PyProfile pyTester;

        static TestForm form;

        public TestForm()
        {
            InitializeComponent();
            form = this;
        }

        public RichTextBox RtbInfo { get { return rtbInfo; } }

        private void TestForm_Load(object sender, EventArgs e)
        {
            //var q = new SelectQuery("Win32_Process", "SELECT * FROM Win32_Process");
            //System.Management.man


            GetActiveWindowTest();

            // for python testing:
            //pyTester = new G13PyProfile(this);

            // for general testing:
            // tester = new G13ProfileBasicTests(this);
            
            // for FPS gaming:
            // tester = new G13ProfileFPS(this);
        }

        IntPtr hEvent = IntPtr.Zero;
        static WinEventDelegate winEventDelegate = new WinEventDelegate(WinEventProcCallback);

        void GetActiveWindowTest()
        {
            hEvent = Interop.SetWinEventHook(WinEvent.EVENT_SYSTEM_FOREGROUND, WinEvent.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, winEventDelegate, 0, 0, SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT | SetWinEventHookFlags.WINEVENT_SKIPOWNPROCESS);
        }

        void UnkhookWinEvent()
        {
            if (hEvent != IntPtr.Zero)
                Interop.UnhookWinEvent(hEvent);
        }

        static void WinEventProcCallback(IntPtr hWinEventHook, WinEvent eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Process process = null;
            string name = "";
            string fileName = "";
            try
            {
                process = Interop.GetProcess(hwnd);
                int id = process.Id;
                name = process.ProcessName;


                ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT Name, ExecutablePath FROM Win32_Process WHERE ProcessId = " + id);
                foreach (ManagementObject obj in search.Get())
                {
                    try
                    {
                        name = obj.Properties["Name"].Value.ToString();
                        fileName = obj.Properties["ExecutablePath"].Value.ToString();

                        //foreach (var property in obj.Properties)
                        //    Debug.WriteLine("    " + property.Name + " = " + property.Value);

                    }
                    catch
                    {
                    }
                }  

                //fileName = process.MainModule.FileName;
            }
            catch(Exception ex)
            {
                // explorer throws when process.MainModule is accessed
                Debug.WriteLine(ex);
                form.rtbInfo.AppendText(ex.ToString() + Environment.NewLine);
                
            }

            Debug.WriteLine(name + " : " + fileName + " : " + eventType);
            form.rtbInfo.AppendText(name + " : " + fileName + " : " + eventType + Environment.NewLine);
        }

        private void TestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnkhookWinEvent();
        }
    }
}
