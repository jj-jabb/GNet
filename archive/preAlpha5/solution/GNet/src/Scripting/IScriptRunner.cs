using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Scripting
{
    public interface IScriptRunner : IDisposable
    {
        void Run(string script);
        void Stop();
    }
}
