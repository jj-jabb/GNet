using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32.SafeHandles;

namespace GNet.Hid
{
    public partial class Device : IDisposable
    {
        public delegate WriteType WriteDelegate(Device device, SafeFileHandle writeHandle);
        public enum WriteType
        {
            Incomplete,
            Complete
        }

        protected int vendorId;
        protected int[] productIds;

        protected SafeFileHandle readHandle;

        protected readonly BackgroundWorker<DeviceReport> readWorker;
        protected readonly ThreadStart writeThreadDelegate;

        protected AutoResetEvent writeAuto;
        protected Thread writeThread;
        protected bool isWriting;

        LinkedList<WriteDelegate> writeDelegates;

        // timeout on read after to refresh the read event 
        // (seemed like it would "sleep" after a little while)
        protected int readDataTimeout = 10000;
        protected int delayBetweenWrites = 10;
        protected int connectionCheckRate = 300;
        protected int openWait = 300;

        int cancelReadHEvent = -1;

        public Device(int vendorId, params int[] productIds)
        {
            this.vendorId = vendorId;
            this.productIds = productIds;

            readWorker = new BackgroundWorker<DeviceReport>();
            readWorker.Updated += new EventHandler<EventArgs<DeviceReport>>(readWorker_Updated);
            readWorker.DoWork += new DoWorkEventHandler(readWorker_DoWork);

            writeDelegates = new LinkedList<WriteDelegate>();
            writeThreadDelegate = new ThreadStart(WriteThread);
        }

        public DeviceInfo DeviceInfo { get; private set; }
        public bool IsOpen { get; private set; }
        public bool IsConnected { get; private set; }
        public int DelayBetweenWrites { get { return delayBetweenWrites; } set { delayBetweenWrites = value; } }
        public int ConnectionCheckRate { get { return connectionCheckRate; } set { connectionCheckRate = value; } }
        public int OpenWait { get { return openWait; } set { openWait = value; } }

        protected virtual DeviceMode DeviceReadMode { get { return DeviceMode.Overlapped; } }

#if debugwrite
        protected virtual void OnConnected() { Debug.WriteLine("OnConnected"); }
        protected virtual void OnOpened() { Debug.WriteLine("OnOpened"); }
        protected virtual void OnDataRead(DeviceData data) { Debug.WriteLine("OnDataRead - byte count: " + data.Bytes.Length); }
        protected virtual void OnReadError(DeviceData data) { Debug.WriteLine("OnReadError - status: " + data.Status); }
        protected virtual void OnException(Exception ex) { Debug.WriteLine("OnException: " + ex.ToString()); }
        protected virtual void OnDisconnected() { Debug.WriteLine("OnDisconnected"); }
        protected virtual void OnClosed() { Debug.WriteLine("OnClosed"); }
#else
        protected virtual void OnConnected() { }
        protected virtual void OnOpened() { }
        protected virtual void OnDataRead(DeviceData data) { }
        protected virtual void OnReadError(DeviceData data) { }
        protected virtual void OnException(Exception ex) { }
        protected virtual void OnClosed() { }
        protected virtual void OnDisconnected() { }
#endif

        public virtual void Start()
        {
            if (false == readWorker.IsBusy)
                readWorker.RunWorkerAsync();

            if (false == isWriting)
            {
                writeThread = new Thread(writeThreadDelegate);
                writeAuto = new AutoResetEvent(false);
                isWriting = true;
                writeThread.Start();
            }
        }

        public virtual void Stop()
        {
            if (true == readWorker.IsBusy)
            {
                readWorker.CancelAsync();
                if (IsOpen)
                    // CancelIoEx is only available on Vista +, so use an overlapped event
                    // to cancel a ReadFile wait
                    //NativeMethods.CancelIoEx(readHandle, IntPtr.Zero);
                    if (cancelReadHEvent >= 0)
                        NativeMethods.SetEvent(cancelReadHEvent);
            }

            if (true == isWriting)
            {
                lock (writeDelegates)
                {
                    writeDelegates.Clear();
                }

                isWriting = false;
                writeAuto.Set();
            }
        }

        public void AddWriteDelegate(WriteDelegate writeDelegate)
        {
            lock (writeDelegates)
            {
                writeDelegates.AddLast(writeDelegate);
            }

            if (isWriting)
                writeAuto.Set();
        }

        public void SetFeature(byte[] data, int length)
        {
            if (DeviceInfo == null || IsOpen == false)
                return;

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var safe = new SafeFileHandle(OpenDeviceIO(DeviceInfo.Path, DeviceMode.NonOverlapped, NativeMethods.GENERIC_WRITE), true);
            try
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
                NativeMethods.HidD_SetFeature(safe, buffer, length);
            }
            finally
            {
                handle.Free();
                safe.Dispose();
            }
        }

        protected bool Connect()
        {
            IsConnected = null != (DeviceInfo = GetDeviceInfo(vendorId, productIds));

            if (IsConnected && isWriting)
                writeAuto.Set();

            return IsConnected;
        }

        protected bool Open()
        {
            if (!IsConnected) return false;
            if (IsOpen) return true;

            try
            {
                readHandle = new SafeFileHandle(OpenDeviceIO(DeviceInfo.Path, DeviceReadMode, NativeMethods.GENERIC_READ), true);
            }
            catch
            {
                IsOpen = false;
                throw;
            }

            IsOpen = !readHandle.IsInvalid;
            return IsOpen;
        }

        protected bool Close()
        {
            if (!IsOpen) return false;

            readHandle.Close();
            IsOpen = false;
            return true;
        }

        protected virtual void WriteThread()
        {
            while (isWriting)
            {
                if (IsConnected)
                {
                    lock (writeDelegates)
                    {
                        using (var safe = new SafeFileHandle(OpenDeviceIO(DeviceInfo.Path, DeviceMode.NonOverlapped, NativeMethods.GENERIC_WRITE), true))
                        {
                            var writeDelegateLink = writeDelegates.First;
                            var next = writeDelegateLink;

                            while (writeDelegateLink != null)
                            {
                                next = writeDelegateLink.Next;

                                if (WriteType.Complete == writeDelegateLink.Value(this, safe))
                                    writeDelegates.Remove(writeDelegateLink);

                                writeDelegateLink = next;
                            }
                        }
                    }

                    if (0 == writeDelegates.Count)
                        writeAuto.WaitOne();
                    else
                        Thread.Sleep(DelayBetweenWrites);
                }
                else
                    writeAuto.WaitOne();
            }

            writeAuto.Close();
        }

        void readWorker_Updated(object sender, EventArgs<DeviceReport> e)
        {
            var report = e.Data;
            switch (report.Status)
            {
                case DeviceStatus.Connected:
                    OnConnected();
                    break;

                case DeviceStatus.Open:
                    OnOpened();
                    break;

                case DeviceStatus.DataRead:
                    OnDataRead(report.Data);
                    break;

                case DeviceStatus.ReadError:
                    OnReadError(report.Data);
                    break;

                case DeviceStatus.Exception:
                    OnException(report.Error);
                    break;

                case DeviceStatus.Closed:
                    OnClosed();
                    break;

                case DeviceStatus.Disconnected:
                    OnDisconnected();
                    break;
            }
        }

        protected virtual void ReadWorker_Started()
        {
        }

        protected virtual void ReadWorker_Connected()
        {
            readWorker.Update(new DeviceReport(DeviceStatus.Connected));
        }

        protected virtual void ReadWorker_Opened()
        {
            readWorker.Update(new DeviceReport(DeviceStatus.Open));
        }

        protected virtual void ReadWorker_DataRead(DeviceData data)
        {
            readWorker.Update(new DeviceReport(data));
        }

        protected virtual void ReadWorker_Cancelled()
        {
        }

        protected virtual void ReadWorker_WaitTimedOut()
        {
        }

        protected virtual void ReadWorker_ReadError()
        {
            readWorker.Update(new DeviceReport(DeviceStatus.ReadError));
        }

        protected virtual void ReadWorker_Exception(Exception ex)
        {
            readWorker.Update(new DeviceReport(ex));
        }

        protected virtual void ReadWorker_Closed()
        {
            readWorker.Update(new DeviceReport(DeviceStatus.Closed));
        }

        protected virtual void ReadWorker_Disconnected()
        {
            readWorker.Update(new DeviceReport(DeviceStatus.Disconnected));
        }

        protected virtual void ReadWorker_Finished()
        {
        }

        void readWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DeviceData data;
            bool wasOpen = IsOpen;

            ReadWorker_Started();

            while (!readWorker.CancellationPending)
            {
                if (!IsConnected)
                {
                    if (!Connect())
                        Thread.Sleep(ConnectionCheckRate);
                    else
                        ReadWorker_Connected();
                }
                else
                {
                    if (!IsOpen)
                    {
                        if (wasOpen)
                        {
                            wasOpen = false;
                            ReadWorker_Closed();
                        }

                        try
                        {
                            if (!Open())
                                Thread.Sleep(OpenWait);
                            else
                            {
                                wasOpen = true;
                                ReadWorker_Opened();
                            }
                        }
                        catch(Exception ex)
                        {
                            ReadWorker_Exception(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            data = ReadData(readDataTimeout);
                            switch (data.Status)
                            {
                                case DeviceData.ReadStatus.Cancelled:
                                    ReadWorker_Cancelled();
                                    break;

                                case DeviceData.ReadStatus.WaitTimedOut:
                                    ReadWorker_WaitTimedOut();
                                    break;

                                case DeviceData.ReadStatus.Success:
                                        bool trueSuccess = false;
                                        for (int i = 0; i < data.Bytes.Length; i++)
                                        {
                                            if (data.Bytes[i] != 0)
                                            {
                                                trueSuccess = true;
                                                break;
                                            }
                                        }

                                        if (trueSuccess)
                                        {
                                            ReadWorker_DataRead(data);
                                        }
                                        else if (!IsPathInDeviceList(DeviceInfo.Path))
                                        {
                                            IsConnected = false;
                                            Close();
                                            wasOpen = false;
                                            ReadWorker_Closed();
                                            ReadWorker_Disconnected();
                                        }
                                    break;

                                case DeviceData.ReadStatus.ReadError:
                                    ReadWorker_ReadError();
                                    break;

                                case DeviceData.ReadStatus.NoDataRead:
                                    if (!readWorker.CancellationPending && !IsPathInDeviceList(DeviceInfo.Path))
                                    {
                                        IsConnected = false;
                                        Close();
                                        wasOpen = false;
                                        ReadWorker_Closed();
                                        ReadWorker_Disconnected();
                                    }
                                    break;
                            }
                        }
                        catch
                        {
                            if (IsPathInDeviceList(DeviceInfo.Path))
                                ReadWorker_ReadError();
                            else
                                ReadWorker_Disconnected();
                        }
                    }
                }
            }

            ReadWorker_Finished();
            if (Close()) ReadWorker_Closed();
        }

        protected DeviceData ReadData(int timeout)
        {
            var buffer = new byte[DeviceInfo.Capabilities.InputReportByteLength];
            var status = DeviceData.ReadStatus.NoDataRead;

            if (DeviceInfo.Capabilities.InputReportByteLength > 0)
            {
                var bytesRead = 0;

                if (DeviceReadMode == DeviceMode.Overlapped)
                {
                    #region Read Overlapped

                    var security = new NativeMethods.SECURITY_ATTRIBUTES();
                    var overlapped = new NativeMethods.OVERLAPPED();
                    var overlapTimeout = timeout <= 0 ? NativeMethods.WAIT_INFINITE : timeout;

                    security.lpSecurityDescriptor = IntPtr.Zero;
                    security.bInheritHandle = true;
                    security.nLength = Marshal.SizeOf(security);

                    overlapped.Offset = 0;
                    overlapped.OffsetHigh = 0;
                    overlapped.hEvent = NativeMethods.CreateEvent(ref security, Convert.ToInt32(false), Convert.ToInt32(true), string.Empty);


                    var security2 = new NativeMethods.SECURITY_ATTRIBUTES();
                    var overlapped2 = new NativeMethods.OVERLAPPED();
                    var overlapTimeout2 = NativeMethods.WAIT_INFINITE;

                    security2.lpSecurityDescriptor = IntPtr.Zero;
                    security2.bInheritHandle = true;
                    security2.nLength = Marshal.SizeOf(security);

                    overlapped2.Offset = 0;
                    overlapped2.OffsetHigh = 0;
                    overlapped2.hEvent = NativeMethods.CreateEvent(ref security, Convert.ToInt32(true), Convert.ToInt32(false), string.Empty);

                    int[] hEvents = new int[] { overlapped.hEvent, overlapped2.hEvent };

                    cancelReadHEvent = overlapped2.hEvent;

                    try
                    {
                        NativeMethods.ReadFileOverlapped(readHandle, ref buffer[0], buffer.Length, ref bytesRead, ref overlapped);

                        //var result = NativeMethods.WaitForSingleObject(overlapped.hEvent, overlapTimeout);
                        var result = NativeMethods.WaitForMultipleObjects(2, ref hEvents[0], false, overlapTimeout);

                        cancelReadHEvent = -1;

                        switch (result)
                        {
                            case 1: // overlapped2.hEvent was set, indicating a cancel of readHandle
                                NativeMethods.CancelIo(readHandle);
                                NativeMethods.WaitForSingleObject(overlapped.hEvent, NativeMethods.WAIT_INFINITE);
                                buffer = new byte[] { };
                                status = DeviceData.ReadStatus.Cancelled;
                                break;
                            case NativeMethods.WAIT_OBJECT_0: status = DeviceData.ReadStatus.Success; break;
                            case NativeMethods.WAIT_TIMEOUT:
                                status = DeviceData.ReadStatus.WaitTimedOut;
                                NativeMethods.CancelIo(readHandle);
                                NativeMethods.WaitForSingleObject(overlapped.hEvent, NativeMethods.WAIT_INFINITE);
                                buffer = new byte[] { };
                                break;
                            case NativeMethods.WAIT_FAILED:
                                status = DeviceData.ReadStatus.WaitFail;
                                buffer = new byte[] { };
                                break;
                            default:
                                status = DeviceData.ReadStatus.NoDataRead;
                                buffer = new byte[] { };
                                break;
                        }
                    }
                    catch { status = DeviceData.ReadStatus.ReadError; }

                    #endregion
                }
                else
                {
                    try
                    {
                        NativeMethods.ReadFile(readHandle, ref buffer[0], buffer.Length, ref bytesRead, IntPtr.Zero);
                        if (bytesRead > 0)
                            status = DeviceData.ReadStatus.Success;
                    }
                    catch { status = DeviceData.ReadStatus.ReadError; }
                }
            }
            return new DeviceData(buffer, status);
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}
