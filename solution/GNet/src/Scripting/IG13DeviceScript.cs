using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Scripting
{
    public interface IG13DeviceScript : IDeviceScript
    {
        G13Scriptable Device { get; set; }
    }
}
