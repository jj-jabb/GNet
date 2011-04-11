using System;
using System.Collections.Generic;
using System.Text;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using GNetLibrary.IO;
using GNetLibrary.MacroSystem;
using GNetLibrary.PInvoke;

namespace GNetLibrary.TestApp
{
    public class G13PyProfile : G13Profile
    {
        string scriptPath;

        ScriptEngine pyEngine;
        ScriptScope pyScope;

        public G13PyProfile(TestForm form, string scriptPath)
            : base(form)
        {
            this.scriptPath = scriptPath;

            pyEngine = Python.CreateEngine();
            pyScope = pyEngine.CreateScope();
        }
    }
}
