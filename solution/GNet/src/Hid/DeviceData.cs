using System;
using System.Text;

namespace GNet.Hid
{
    public class DeviceData
    {
        public enum ReadStatus
        {
            Success = 0,
            WaitTimedOut = 1,
            WaitFail = 2,
            NoDataRead = 3,
            ReadError = 4,
            NotConnected = 5
        }

        public DeviceData(ReadStatus status)
        {
            Bytes = new byte[] { };
            Status = status;
        }

        public DeviceData(byte[] data, ReadStatus status)
        {
            Bytes = data;
            Status = status;
        }

        public byte[] Bytes { get; private set; }
        public ReadStatus Status { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Bytes != null)
                for (int i = 0; i < Bytes.Length; i++)
                    sb
                        .Append(i > 0 ? ", " : "")
                        .Append(Bytes[i]
                            .ToString("x")
                            .PadLeft(2, ' ')
                            );

            return sb.ToString();
        }
    }
}
