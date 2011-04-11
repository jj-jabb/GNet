using System;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class MouseSavePos : Step
    {
        public MouseSavePos(string name)
        {
            Macro.SavedPoints[name] = InputSimulator.MouseAbsolutePos;
        }
    }
}
