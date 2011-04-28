using System;

namespace GNet.Lib.PInvoke
{
    public struct ScanCodeInfo
    {
        public ushort VkKey;
        public ScanCode ScanCode;
        public bool IsShifted;

        public override string ToString()
        {
            var str = ScanCode.ToString("x");
            if (IsShifted)
                str += " : shifted";

            return str;
        }
    }
}
