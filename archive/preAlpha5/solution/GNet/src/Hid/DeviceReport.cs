using System;
using System.Collections.Generic;
using System.Text;

namespace GNet.Hid
{
    public class DeviceReport
    {
        public DeviceData Data { get; private set; }
        public Exception Error { get; private set; }
        public DeviceStatus Status { get; private set; }

        public DeviceReport(DeviceData deviceData)
        {
            Data = deviceData;
            Status = DeviceStatus.DataRead;
        }

        public DeviceReport(DeviceStatus status)
        {
            Status = status;
        }

        public DeviceReport(Exception error)
        {
            Error = error;
            Status = DeviceStatus.Exception;
        }
    }
}
