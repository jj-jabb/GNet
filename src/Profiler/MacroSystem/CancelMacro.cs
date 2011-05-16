using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public class CancelMacro : Macro
    {
        public CancelMacro(Macro macro)
        {
            Macro = macro;
            Priority = int.MaxValue;
            IsInterrupting = true;
        }

        public Macro Macro { get; set; }
    }
}
