using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using GNetLibrary.IO;
using GNetLibrary.MacroSystem;
using GNetLibrary.PInvoke;

namespace GNetLibrary.TestApp
{
    public class G13PyProfile : InputSimulator
    {
        protected readonly G13Device g13;
        protected readonly TestForm form;

        protected string scriptPath;

        protected ScriptEngine pyEngine;
        protected ScriptScope pyScope;
        protected ScriptSource pySource;

        protected TextBoxWriter output;

        public G13PyProfile(TestForm form, string scriptPath = "G13PyProfile.py")
        {
            this.form = form;
            this.scriptPath = scriptPath;

            MacroSystem.Macro.SynchronizeInvoke = form;

            output = new TextBoxWriter(form.RtbInfo);

            g13 = new G13Device();

            if (!g13.IsConnected)
            {
                g13.Inserted += new G13Device.DeviceEventHandler(g13_Inserted);
                g13.Removed += new G13Device.DeviceEventHandler(g13_Removed);
                g13.WaitForConnection();
            }
        }

        void g13_Inserted(G13Device device)
        {
            try
            {
                pyEngine = Python.CreateEngine();
                pyEngine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(G13Device)));
                pyEngine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(System.IO.File)));
                pyEngine.Runtime.IO.RedirectToConsole();

                pyScope = pyEngine.CreateScope();
                pyScope.SetVariable("g13", this.g13);

                Console.SetOut(output);

                pySource = pyEngine.CreateScriptSourceFromFile(scriptPath);
                pySource.Execute(pyScope);
            }
            catch (Exception ex)
            {
                form.RtbInfo.AppendText(ex.ToString() + Environment.NewLine);
            }
        }

        void g13_Removed(G13Device device)
        {
            // TODO: make sure this is the proper way to clean up
            pyEngine.Runtime.Shutdown();
            pySource = null;
            pyScope = null;
            pyEngine = null;
        }
    }
}
