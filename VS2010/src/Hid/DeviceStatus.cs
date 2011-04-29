using System;

namespace GNet.Hid
{
    public enum DeviceStatus
    {
        Connected,
        Open,
        DataRead,
        ReadError,
        Exception,
        Closed,
        Disconnected
    }
}
