using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.MacroSystem
{
    public class MouseSavePos : Step
    {
        string name;

        public MouseSavePos(string name)
        {
            this.name = name;

            toString = "MouseRecallPos(" + name + ")";
        }

        public override void Run()
        {
            Macro.SavedPoints[name] = InputSimulator.MouseAbsolutePos;
        }
    }
}
