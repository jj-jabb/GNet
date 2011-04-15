using System;
using System.Collections.Generic;
using System.Text;

namespace GNet
{
    public interface IScriptRunner
    {
        IScriptRunner Run();
        void Stop();
    }
}
