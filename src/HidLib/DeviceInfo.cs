using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HidLib
{
    public class DeviceInfo
    {
        internal DeviceInfo(string path)
        {
            Path = path;

            var hidHandle = Device.OpenDeviceIO(path, NativeMethods.ACCESS_NONE);

            Attributes = GetDeviceAttributes(hidHandle);
            Capabilities = GetDeviceCapabilities(hidHandle);

            Device.CloseDeviceIO(hidHandle);
        }

        public DeviceAttributes Attributes { get; private set; }
        public DeviceCapabilities Capabilities { get; private set; }
        public string Path { get; private set; }


        internal static DeviceAttributes GetDeviceAttributes(IntPtr hidHandle)
        {
            var deviceAttributes = default(NativeMethods.HIDD_ATTRIBUTES);
            deviceAttributes.Size = Marshal.SizeOf(deviceAttributes);
            NativeMethods.HidD_GetAttributes(hidHandle, ref deviceAttributes);
            return new DeviceAttributes(deviceAttributes);
        }

        internal static DeviceCapabilities GetDeviceCapabilities(IntPtr hidHandle)
        {
            var capabilities = default(NativeMethods.HIDP_CAPS);
            var preparsedDataPointer = default(IntPtr);

            if (NativeMethods.HidD_GetPreparsedData(hidHandle, ref preparsedDataPointer))
            {
                NativeMethods.HidP_GetCaps(preparsedDataPointer, ref capabilities);
                NativeMethods.HidD_FreePreparsedData(preparsedDataPointer);
            }
            return new DeviceCapabilities(capabilities);
        }
    }
}
