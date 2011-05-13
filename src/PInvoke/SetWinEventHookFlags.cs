using System;

namespace GNet.PInvoke
{
    [Flags]
    public enum SetWinEventHookFlags
    {
        WINEVENT_OUTOFCONTEXT = 0,
        WINEVENT_SKIPOWNTHREAD = 1,
        WINEVENT_SKIPOWNPROCESS = 2,
        WINEVENT_INCONTEXT = 4,
    }
}
