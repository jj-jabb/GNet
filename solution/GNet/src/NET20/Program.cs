using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

using GNet.Lib.PInvoke;
using Microsoft.VisualBasic.ApplicationServices;

namespace GNet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    if (!IsRunning())
        //    {
        //        Application.EnableVisualStyles();
        //        Application.SetCompatibleTextRenderingDefault(false);
        //        Application.Run(new MainForm());
        //    }
        //}

        //static bool IsRunning()
        //{
        //    var id = Process.GetCurrentProcess().Id;
        //    var name = Process.GetCurrentProcess().ProcessName;
        //    foreach (var p in Process.GetProcesses())
        //        if (p.ProcessName == name && p.Id != id)
        //        {
        //            Interop.ShowWindow(p.MainWindowHandle, 1);
        //            Interop.SetForegroundWindow(p.MainWindowHandle);
        //            return true;
        //        }

        //    return false;
        //}



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string[] args = Environment.GetCommandLineArgs();
            SingleInstanceController controller = new SingleInstanceController();
            controller.Run(args);
        }

        public class SingleInstanceController : WindowsFormsApplicationBase
        {
            public SingleInstanceController()
            {
                IsSingleInstance = true;

                StartupNextInstance += this_StartupNextInstance;
            }

            void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
            {
                MainForm.Show();
            }

            protected override void OnCreateMainForm()
            {
                MainForm = new MainForm();
            }
        }
    }
}
