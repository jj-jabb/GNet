using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GNet.MacroSystem
{
    public class MacroManager
    {
        static Stopwatch stopwatch = new Stopwatch();
        public static Stopwatch Stopwatch { get { return stopwatch; } }

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
