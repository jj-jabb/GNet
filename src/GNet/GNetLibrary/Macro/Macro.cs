using System;
using System.Collections.Generic;
using System.Text;

using GNetLibrary.IO;
using GNetLibrary.PInvoke;

namespace GNetLibrary.Macro
{
    public class Macro
    {
        public static readonly Dictionary<string, Win32Point> SavedPoints = new Dictionary<string, Win32Point>();

        List<Step> steps;

        public Macro()
        {
            steps = new List<Step>();
        }
    }
}
