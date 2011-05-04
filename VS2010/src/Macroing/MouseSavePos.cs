using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem
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
