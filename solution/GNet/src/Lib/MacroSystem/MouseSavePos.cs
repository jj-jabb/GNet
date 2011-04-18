using System;

using GNet.Lib.IO;
using GNet.Lib.PInvoke;

namespace GNet.Lib.MacroSystem
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
