using System.Text;

namespace HidLibrary
{
    public class HidDeviceData
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

        public HidDeviceData(ReadStatus status)
	    {
		    Data = new byte[] {};
		    Status = status;
	    }

        public HidDeviceData(byte[] data, ReadStatus status)
        {
            Data = data;
            Status = status;
        }

        public byte[] Data { get; private set; }
        public ReadStatus Status { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Data != null)
                for (int i = 0; i < Data.Length; i++)
                    sb
                        .Append(i > 0 ? ", " : "")
                        .Append(Data[i]
                            .ToString("x")
                            .PadLeft(2, ' ')
                            );

            return sb.ToString();
        }
    }
}
