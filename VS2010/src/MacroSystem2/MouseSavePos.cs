using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public class MouseSavePos : Step
    {
        string name;

        public MouseSavePos(string name)
        {
            this.name = name;

            toString = "MouseSavePos(" + name + ")";
        }

        public override InputWrapper[] Run()
        {
            MacroManager.SavedPoints[name] = InputSimulator.MouseAbsolutePos;
            return null;
        }
    }
}
