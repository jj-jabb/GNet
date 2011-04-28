using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HidLibrary
{
    public delegate void DeviceReadHandler(HidDevice device, HidDeviceData data);

    public class HidDevice : IDisposable
    {
        public event DeviceMonitorHandler Inserted;
        public event DeviceMonitorHandler Removed;

        public event DeviceReadHandler DataRead;

        public enum DeviceMode
        {
            NonOverlapped = 0,
            Overlapped = 1
        }

        private readonly string devicePath;
        private readonly HidDeviceAttributes deviceAttributes;

        private readonly HidDeviceCapabilities deviceCapabilities;
        private DeviceMode deviceReadMode = DeviceMode.NonOverlapped;
        private DeviceMode deviceWriteMode = DeviceMode.NonOverlapped;

        private readonly HidDeviceEventMonitor deviceEventMonitor;

        private bool monitorDeviceEvents;
        //private delegate HidDeviceData ReadDelegate();
        //private delegate HidReport ReadReportDelegate();
        private delegate bool WriteDelegate(byte[] data);
        //private delegate bool WriteReportDelegate(HidReport report);

        //public delegate void ReadCallback(HidDeviceData data);
        //public delegate void ReadReportCallback(HidReport report);

        public delegate void WriteCallback(bool success);

        readonly BackgroundWorker<HidDeviceData> readWorker;

        internal HidDevice(string devicePath)
        {
            this.devicePath = devicePath;

            try
            {
                var hidHandle = OpenDeviceIO(devicePath, NativeMethods.ACCESS_NONE);

                deviceAttributes = GetDeviceAttributes(hidHandle);
                deviceCapabilities = GetDeviceCapabilities(hidHandle);

                CloseDeviceIO(hidHandle);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Error querying HID device '{0}'.", devicePath), exception);
            }

            deviceEventMonitor = new HidDeviceEventMonitor(this);
            deviceEventMonitor.Inserted += DeviceEventMonitorInserted;
            deviceEventMonitor.Removed += DeviceEventMonitorRemoved;

            readWorker = new BackgroundWorker<HidDeviceData>();
            readWorker.Updated += new EventHandler<EventArgs<HidDeviceData>>(readWorker_Updated);
            readWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(readWorker_DoWork);
        }

        public IntPtr ReadHandle { get; private set; }
        public IntPtr WriteHandle { get; private set; }
        public bool IsOpen { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsPathInDeviceList { get { return HidDevices.IsPathInDeviceList(devicePath); } }
        public string Description { get { return ToString(); } }
        public HidDeviceCapabilities Capabilities { get { return deviceCapabilities; } }
        public HidDeviceAttributes Attributes { get { return deviceAttributes; } }
        public string DevicePath { get { return devicePath; } }

        public bool MonitorDeviceEvents
        {
            get { return monitorDeviceEvents; }
            set
            {
                if (value & monitorDeviceEvents == false) deviceEventMonitor.Start();
                monitorDeviceEvents = value;
            }
        }

        void readWorker_Updated(object sender, EventArgs<HidDeviceData> e)
        {
            if (DataRead != null)
                DataRead(this, e.Data);
        }

        void readWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            HidDeviceData data;

            while (!readWorker.CancellationPending)
            {
                if (!IsConnected)
                    Thread.Sleep(100);
                else
                {
                    if (IsOpen == false)
                        OpenDevice();
                    try
                    {
                        data = ReadData(0);
                        if(data.Status == HidDeviceData.ReadStatus.Success)
                            readWorker.Update(data);
                    }
                    catch
                    {
                        if (IsPathInDeviceList)
                            readWorker.Update(new HidDeviceData(HidDeviceData.ReadStatus.ReadError));
                        else
                            DeviceEventMonitorRemoved(this);
                    }
                }
            }
        }

        public void StartReading()
        {
            if (!readWorker.IsBusy)
                readWorker.RunWorkerAsync();
        }

        public void StopReading()
        {
            if (readWorker.IsBusy)
                readWorker.CancelAsync();
        }

        public bool IsReading { get { return readWorker.IsBusy; } }

        public override string ToString()
        {
            return string.Format("VendorID={0}, ProductID={1}, Version={2}, DevicePath={3}", 
                                deviceAttributes.VendorHexId,
                                deviceAttributes.ProductHexId,
                                deviceAttributes.Version,
                                devicePath);
        }

        public void OpenDevice()
        {
            OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped);
        }

        public void OpenDevice(DeviceMode readMode, DeviceMode writeMode)
        {
            if (IsOpen) return;

            deviceReadMode = readMode;
            deviceWriteMode = writeMode;

            try
            {
                ReadHandle = OpenDeviceIO(devicePath, readMode, NativeMethods.GENERIC_READ);
                WriteHandle = OpenDeviceIO(devicePath, writeMode, NativeMethods.GENERIC_WRITE);
            }
            catch (Exception exception)
            {
                IsOpen = false;
                throw new Exception("Error opening HID device.", exception);
            }

            IsOpen = ReadHandle.ToInt32() != NativeMethods.INVALID_HANDLE_VALUE & WriteHandle.ToInt32() != NativeMethods.INVALID_HANDLE_VALUE;
        }


        public void CloseDevice()
        {
            if (!IsOpen) return;
            CloseDeviceIO(ReadHandle);
            CloseDeviceIO(WriteHandle);
            IsOpen = false;
        }

        public HidDeviceData Read()
        {
            return Read(0);
        }

        //public void Read(ReadCallback callback)
        //{
        //    var readDelegate = new ReadDelegate(Read);
        //    var asyncState = new HidAsyncState(readDelegate, callback);
        //    readDelegate.BeginInvoke(EndRead, asyncState);
        //}

        public HidDeviceData Read(int timeout)
        {
            if (IsPathInDeviceList)
            {
                if (IsOpen == false) OpenDevice();
                try
                {
                    return ReadData(timeout);
                }
                catch
                {
                    return new HidDeviceData(HidDeviceData.ReadStatus.ReadError);
                }

            }
            return new HidDeviceData(HidDeviceData.ReadStatus.NotConnected);
        }

        //public void ReadReport(ReadReportCallback callback)
        //{
        //    var readReportDelegate = new ReadReportDelegate(ReadReport);
        //    var asyncState = new HidAsyncState(readReportDelegate, callback);
        //    readReportDelegate.BeginInvoke(EndReadReport, asyncState);
        //}

        public HidReport ReadReport(int timeout)
        {
            return new HidReport(Capabilities.InputReportByteLength, Read(timeout));
        }

        public HidReport ReadReport()
        {
            return ReadReport(0);
        }

        public void Write(byte[] data, WriteCallback callback)
        {
            var writeDelegate = new WriteDelegate(Write);
            var asyncState = new HidAsyncState(writeDelegate, callback);
            writeDelegate.BeginInvoke(data, EndWrite, asyncState);
        }

        public bool Write(byte[] data)
        {
            return Write(data, 0);
        }

        public bool Write(byte[] data, int timeout)
        {
            //if (IsPathInDeviceList)
            if (IsConnected)
            {
                if (IsOpen == false) OpenDevice();
                try
                {
                    return WriteData(data, timeout);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        //public void WriteReport(HidReport report, WriteCallback callback)
        //{
        //    var writeReportDelegate = new WriteReportDelegate(WriteReport);
        //    var asyncState = new HidAsyncState(writeReportDelegate, callback);
        //    writeReportDelegate.BeginInvoke(report, EndWriteReport, asyncState);
        //}

        public bool WriteReport(HidReport report)
        {
            return WriteReport(report, 0);
        }

        public bool WriteReport(HidReport report, int timeout)
        {
            return Write(report.GetBytes(), timeout);
        }

        public HidReport CreateReport()
        {
            return new HidReport(Capabilities.OutputReportByteLength);
        }

        //private static void EndRead(IAsyncResult ar)
        //{
        //    var hidAsyncState = (HidAsyncState)ar.AsyncState;
        //    var callerDelegate = (ReadDelegate)hidAsyncState.CallerDelegate;
        //    var callbackDelegate = (ReadCallback)hidAsyncState.CallbackDelegate;
        //    var data = callerDelegate.EndInvoke(ar);

        //    if ((callbackDelegate != null)) callbackDelegate.Invoke(data);
        //}

        //private static void EndReadReport(IAsyncResult ar)
        //{
        //    var hidAsyncState = (HidAsyncState)ar.AsyncState;
        //    var callerDelegate = (ReadReportDelegate)hidAsyncState.CallerDelegate;
        //    var callbackDelegate = (ReadReportCallback)hidAsyncState.CallbackDelegate;
        //    var report = callerDelegate.EndInvoke(ar);

        //    if ((callbackDelegate != null)) callbackDelegate.Invoke(report);
        //}

        private static void EndWrite(IAsyncResult ar)
        {
            var hidAsyncState = (HidAsyncState)ar.AsyncState;
            var callerDelegate = (WriteDelegate)hidAsyncState.CallerDelegate;
            var callbackDelegate = (WriteCallback)hidAsyncState.CallbackDelegate;
            var result = callerDelegate.EndInvoke(ar);

            if ((callbackDelegate != null)) callbackDelegate.Invoke(result);
        }

        //private static void EndWriteReport(IAsyncResult ar)
        //{
        //    var hidAsyncState = (HidAsyncState)ar.AsyncState;
        //    var callerDelegate = (WriteReportDelegate)hidAsyncState.CallerDelegate;
        //    var callbackDelegate = (WriteCallback)hidAsyncState.CallbackDelegate;
        //    var result = callerDelegate.EndInvoke(ar);

        //    if ((callbackDelegate != null)) callbackDelegate.Invoke(result);
        //}

        private static HidDeviceAttributes GetDeviceAttributes(IntPtr hidHandle)
        {
            var deviceAttributes = default(NativeMethods.HIDD_ATTRIBUTES);
            deviceAttributes.Size = Marshal.SizeOf(deviceAttributes);
            NativeMethods.HidD_GetAttributes(hidHandle, ref deviceAttributes);
            return new HidDeviceAttributes(deviceAttributes);
        }

        private static HidDeviceCapabilities GetDeviceCapabilities(IntPtr hidHandle)
        {
            var capabilities = default(NativeMethods.HIDP_CAPS);
            var preparsedDataPointer = default(IntPtr);

            if (NativeMethods.HidD_GetPreparsedData(hidHandle, ref preparsedDataPointer))
            {
                NativeMethods.HidP_GetCaps(preparsedDataPointer, ref capabilities);
                NativeMethods.HidD_FreePreparsedData(preparsedDataPointer);
            }
            return new HidDeviceCapabilities(capabilities);
        }

        private bool WriteData(byte[] data, int timeout)
        {
            if (deviceCapabilities.OutputReportByteLength <= 0) return false;

            var buffer = new byte[Capabilities.OutputReportByteLength];
            var bytesWritten = 0;

            Array.Copy(data, 0, buffer, 0, Math.Min(data.Length, deviceCapabilities.OutputReportByteLength));

            if (deviceWriteMode == DeviceMode.Overlapped)
            {
                var security = new NativeMethods.SECURITY_ATTRIBUTES();
                var overlapped = new NativeMethods.OVERLAPPED();

                var overlapTimeout = timeout <= 0 ? NativeMethods.WAIT_INFINITE : timeout;

                security.lpSecurityDescriptor = IntPtr.Zero;
                security.bInheritHandle = true;
                security.nLength = Marshal.SizeOf(security);

                overlapped.Offset = 0;
                overlapped.OffsetHigh = 0;
                overlapped.hEvent = NativeMethods.CreateEvent(ref security, Convert.ToInt32(false), Convert.ToInt32(true), "");

                try
                {
                    NativeMethods.WriteFileOverlapped(WriteHandle, ref buffer[0], buffer.Length, ref bytesWritten, ref overlapped);
                }
                catch { return false; }

                var result = NativeMethods.WaitForSingleObject(overlapped.hEvent, overlapTimeout);

                switch (result)
                {
                    case NativeMethods.WAIT_OBJECT_0:
                        return true;
                    case NativeMethods.WAIT_TIMEOUT:
                        return false;
                    case NativeMethods.WAIT_FAILED:
                        return false;
                    default:
                        return false;
                }
            }
            try
            {
                return NativeMethods.WriteFile(WriteHandle, ref buffer[0], buffer.Length, ref bytesWritten, 0);
            }
            catch { return false; }
        }

        private HidDeviceData ReadData(int timeout)
        {
            var buffer = new byte[Capabilities.InputReportByteLength];
            var status = HidDeviceData.ReadStatus.NoDataRead;

            if (deviceCapabilities.InputReportByteLength > 0)
            {
                var bytesRead = 0;

                if (deviceReadMode == DeviceMode.Overlapped)
                {
                    var security = new NativeMethods.SECURITY_ATTRIBUTES();
                    var overlapped = new NativeMethods.OVERLAPPED();
                    var overlapTimeout = timeout <= 0 ? NativeMethods.WAIT_INFINITE : timeout;

                    security.lpSecurityDescriptor = IntPtr.Zero;
                    security.bInheritHandle = true;
                    security.nLength = Marshal.SizeOf(security);

                    overlapped.Offset = 0;
                    overlapped.OffsetHigh = 0;
                    overlapped.hEvent = NativeMethods.CreateEvent(ref security, Convert.ToInt32(false), Convert.ToInt32(true), string.Empty);

                    try
                    {
                        NativeMethods.ReadFileOverlapped(ReadHandle, ref buffer[0], buffer.Length, ref bytesRead, ref overlapped);

                        var result = NativeMethods.WaitForSingleObject(overlapped.hEvent, overlapTimeout);

                        switch (result)
                        {
                            case NativeMethods.WAIT_OBJECT_0: status = HidDeviceData.ReadStatus.Success; break;
                            case NativeMethods.WAIT_TIMEOUT: 
                                status = HidDeviceData.ReadStatus.WaitTimedOut;
                                buffer = new byte[] {};
                                break;
                            case NativeMethods.WAIT_FAILED:
                                status = HidDeviceData.ReadStatus.WaitFail;
                                buffer = new byte[] { };
                                break;
                            default:
                                status = HidDeviceData.ReadStatus.NoDataRead;
                                buffer = new byte[] { };
                                break;
                        }
                    }
                    catch { status = HidDeviceData.ReadStatus.ReadError; }
                }
                else
                {
                    try
                    {
                        NativeMethods.ReadFile(ReadHandle, ref buffer[0], buffer.Length, ref bytesRead, IntPtr.Zero);
                        if (bytesRead > 0)
                            status = HidDeviceData.ReadStatus.Success;
                    }
                    catch { status = HidDeviceData.ReadStatus.ReadError; }
                }
            }
            return new HidDeviceData(buffer, status);
        }

        private static IntPtr OpenDeviceIO(string devicePath, uint deviceAccess)
        {
            return OpenDeviceIO(devicePath, DeviceMode.NonOverlapped, deviceAccess);
        }

        private static IntPtr OpenDeviceIO(string devicePath, DeviceMode deviceMode, uint deviceAccess)
        {
            var security = new NativeMethods.SECURITY_ATTRIBUTES();
            var flags = 0;

            if (deviceMode == DeviceMode.Overlapped) flags = NativeMethods.FILE_FLAG_OVERLAPPED;

            security.lpSecurityDescriptor = IntPtr.Zero;
            security.bInheritHandle = true;
            security.nLength = Marshal.SizeOf(security);

            return NativeMethods.CreateFile(devicePath, deviceAccess, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, ref security, NativeMethods.OPEN_EXISTING, flags, 0);
        }

        private static void CloseDeviceIO(IntPtr handle)
        {
            NativeMethods.CloseHandle(handle);
        }

        private void DeviceEventMonitorInserted(HidDevice device)
        {
            IsConnected = true;
            if (!IsOpen) OpenDevice();
            if (Inserted != null) Inserted(this);
        }

        private void DeviceEventMonitorRemoved(HidDevice device)
        {
            IsConnected = false;
            if (IsOpen) CloseDevice();
            if (Removed != null) Removed(this);
        }

        public void Dispose()
        {
            if (MonitorDeviceEvents) MonitorDeviceEvents = false;
            if (IsOpen) CloseDevice();
        }
    }
}
