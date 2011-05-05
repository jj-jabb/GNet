using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.MacroSystem
{
    public class MacroManager
    {
        List<Macro> noQueueMacros = new List<Macro>();
        Dictionary<string, Queue<Macro>> macroQueues = new Dictionary<string,Queue<Macro>>();

        public MacroManager()
        {
        }

        public void AddMacro(Macro macro)
        {

        }
    }
}
