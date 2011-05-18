using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Profiler.MacroSystem
{
    public abstract class Enabler : Step
    {
        public Enabler() { }
        public Enabler(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }

    public class Enable : Enabler
    {
        public Enable() { }
        public Enable(string path) : base(path) { }

        public override StepType Type
        {
            get { return StepType.Enable; }
        }
    }

    public class Disable : Enabler
    {
        public Disable() { }
        public Disable(string path) : base(path) { }

        public override StepType Type
        {
            get { return StepType.Disable; }
        }
    }
}
