using System;

using GNet.IO;
using GNet.PInvoke;

namespace GNet.MacroSystem2
{
    public interface IStep
    {
        StepType Type { get; }
        bool IsEnabled { get; set; }
        int Cooldown { get; set; }
        long Timestamp { get; set; }

        InputWrapper[] Run();
    }
}
