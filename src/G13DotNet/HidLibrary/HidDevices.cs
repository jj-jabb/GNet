using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HidLibrary
{
    public class HidDevices
    {
        private static Guid _hidClassGuid = Guid.Empty;

        public static bool IsPathInDeviceList(string devicePath)
        {
            foreach (var d in EnumerateHidDevices())
                if (d == devicePath) return true;

            return false;
        }

        public static HidDevice GetDevice(string devicePath)
        {
            foreach (var path in EnumerateHidDevices())
                if (path == devicePath)
                    return new HidDevice(path);

            return null;
        }

        public static IEnumerable<HidDevice> Enumerate()
        {
            foreach (var path in EnumerateHidDevices())
                yield return new HidDevice(path);
        }

        public static IEnumerable<HidDevice> Enumerate(string devicePath)
        {
            foreach (var path in EnumerateHidDevices())
                if (path == devicePath)
                    yield return new HidDevice(path);
        }

        public static IEnumerable<HidDevice> Enumerate(int vendorId, params int[] productIds)
        {
            List<int> pids = new List<int>(productIds);

            foreach (var path in EnumerateHidDevices())
            {
                var device = new HidDevice(path);
                if (device.Attributes.VendorId == vendorId && pids.Contains(device.Attributes.ProductId))
                    yield return device;
            }
        }

        public static HidDevice GetDevice(int vendorId, params int[] productIds)
        {
            List<int> pids = new List<int>(productIds);

            foreach (var path in EnumerateHidDevices())
            {
                var device = new HidDevice(path);
                if (device.Attributes.VendorId == vendorId && pids.Contains(device.Attributes.ProductId))
                    return device;
            }

            return null;
        }

        public static IEnumerable<HidDevice> Enumerate(int vendorId)
        {
            foreach (var path in EnumerateHidDevices())
            {
                var device = new HidDevice(path);
                if (device.Attributes.VendorId == vendorId)
                    yield return device;
            }
        }

        private static IEnumerable<string> EnumerateHidDevices()
        {
            var devices = new List<string>();
            var hidClass = HidClassGuid;
            var deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref hidClass, null, 0, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);

            if (deviceInfoSet.ToInt32() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                var deviceInfoData = CreateDeviceInfoData();
                var deviceIndex = 0;

                while (NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
                {
                    deviceIndex += 1;

                    var deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                    deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                    var deviceInterfaceIndex = 0;

                    while (NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, 0, ref hidClass, deviceInterfaceIndex, ref deviceInterfaceData))
                    {
                        deviceInterfaceIndex++;
                        var devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                        if (devices.Contains(devicePath)) continue;
                        devices.Add(devicePath);
                    }
                }
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return devices;
        }

        private static NativeMethods.SP_DEVINFO_DATA CreateDeviceInfoData()
        {
            var deviceInfoData = new NativeMethods.SP_DEVINFO_DATA();

            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
            deviceInfoData.DevInst = 0;
            deviceInfoData.ClassGuid = Guid.Empty;
            deviceInfoData.Reserved = IntPtr.Zero;

            return deviceInfoData;
        }

        private static string GetDevicePath(IntPtr deviceInfoSet, NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            var bufferSize = 0;
            var interfaceDetail = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA { Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8 };

            NativeMethods.SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

            return NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero) ?
                interfaceDetail.DevicePath : null;
        }

        private static Guid HidClassGuid
        {
            get
            {
                if (_hidClassGuid.Equals(Guid.Empty)) NativeMethods.HidD_GetHidGuid(ref _hidClassGuid);
                return _hidClassGuid;
            }
        }
    }
}
