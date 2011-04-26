using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Scripting
{
    public enum HookOptions
    {
        None,
        IgnoreInjected,
        All
    }

    public static class HookOptionsExtensions
    {
        public static string DisplayValue(HookOptions opts)
        {
            switch (opts)
            {
                case HookOptions.IgnoreInjected: return "Ignore Injected Events";
                case HookOptions.All: return "All";
                default: return "None";
            }
        }
    }
}
