using System;
using System.Collections.Generic;
using System.Text;

namespace GNet
{
    public interface IScriptRunner : IDisposable
    {
        IScriptRunner Run();
        void Stop();
    }
}
