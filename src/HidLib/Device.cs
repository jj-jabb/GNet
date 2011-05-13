using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32.SafeHandles;

namespace HidLib
{
    public partial class Device : IDisposable
    {
        public delegate WriteType WriteDelegate(Device device, SafeFileHandle writeHandle);
        public enum WriteType
        {
            Incomplete,
            Complete
        }

        const string readExitName = @"Local\ReadExit";
        const string writeEventName = @"Local\WriteEvent";
        const string writeExitName = @"Local\WriteExit";

        protected int vendorId;
        protected int[] productIds;

        SafeFileHandle readHandle;

        // timeout on read to refresh the read event 
        // (seemed like it would "sleep" after a little while)
        protected int readDataTimeout = 10000;
        protected int delayBetweenWrites = 10;
        protected int connectionCheckRate = 300;

        bool runReadThread;
        bool runWriteThread;

        protected LinkedList<WriteDelegate> writeDelegates;

        readonly ThreadStart readThreadDelegate;
        readonly ThreadStart writeThreadDelegate;

        Thread readThread;
        Thread writeThread;

        EventWaitHandle writeEvent;
        EventWaitHandle readExit;
        EventWaitHandle writeExit;

        NativeMethods.SECURITY_ATTRIBUTES cancelEventSecurity;
        NativeMethods.OVERLAPPED cancelEventOverlapped;

        NativeMethods.SECURITY_ATTRIBUTES readEventSecurity;
        NativeMethods.OVERLAPPED readEventOverlapped;

        public Device(int vendorId, params int[] productIds)
        {
            this.vendorId = vendorId;
            this.productIds = productIds;

            readThreadDelegate = new ThreadStart(ReadThread);
            
            writeDelegates = new LinkedList<WriteDelegate>();
            writeThreadDelegate = new ThreadStart(WriteThread);

            CreateOverlappedEvent(out cancelEventSecurity, out cancelEventOverlapped);
            CreateOverlappedEvent(out readEventSecurity, out readEventOverlapped);
        }

        public DeviceInfo DeviceInfo { get; private set; }
        protected virtual DeviceMode DeviceReadMode { get { return DeviceMode.Overlapped; } }

        public bool IsOpen { get; private set; }
        public bool IsConnected { get; private set; }
        public int DelayBetweenWrites { get { return delayBetweenWrites; } set { delayBetweenWrites = value; } }
        public int ConnectionCheckRate { get { return connectionCheckRate; } set { connectionCheckRate = value; } }

		public DeviceAttributes Attributes { get { return DeviceInfo.Attributes; } }
		public DeviceCapabilities Capabilities { get { return DeviceInfo.Capabilities; } }
		public string Path { get { return DeviceInfo.Path; } }

		/// <summary>
		/// Starts the read and write threads.
		/// </summary>
		/// <returns>Device returns true by default; subclasses may override this behavior.</returns>
        public virtual bool Start()
        {
            StartRead();
            StartWrite();

            return true;
        }

		/// <summary>
		/// Stops the read and write threads.
		/// </summary>
        public virtual void Stop()
        {
            StopWrite();
            StopRead();

			// close readHandle
            Close();
        }

        protected bool Connect()
        {
            IsConnected = (DeviceInfo = GetDeviceInfo(vendorId, productIds)) != null;

			if (IsConnected && runWriteThread)
				writeEvent.Set();

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

        protected void Close()
        {
            if (!IsOpen) return;

            readHandle.Close();
            IsOpen = false;
        }

        #region Read

        protected void StartRead()
        {
            if (runReadThread)
                return;

            runReadThread = true;
            readExit = new EventWaitHandle(false, EventResetMode.AutoReset, readExitName);
            readThread = new Thread(readThreadDelegate);
            readThread.Start();
        }

        protected void StopRead()
        {
            if (!runReadThread)
                return;

            runReadThread = false;
            CancelRead();

            if (Thread.CurrentThread != readThread)
            {
                if (!readExit.WaitOne(1000))
                {
                    System.Diagnostics.Debug.WriteLine("StopRead: readExit.WaitOne timed out");
                    readThread.Abort();
                }
            }

            readThread = null;
        }

        protected void CancelRead()
        {
            NativeMethods.SetEvent(cancelEventOverlapped.hEvent);
        }

        protected virtual void ReadThread_Cancelled()
        {
        }

        protected virtual void ReadThread_WaitTimedOut()
        {
        }

        protected virtual void ReadThread_DataRead(DeviceData data)
        {
        }

        protected virtual void ReadThread_ReadError(Exception error)
        {
            Debug.WriteLine("ReadThread_ReadError: " + error);
        }

        protected virtual void ReadThread_NoDataRead()
        {
            PollForConnect();
        }

        void PollForConnect()
        {
            if (Connect() && Open())
                return;

            IsConnected = false;
            Close();

            while (runReadThread)
            {
                if (Connect() && Open())
                    break;
                else
                    Thread.Sleep(ConnectionCheckRate);
            }
        }

        void ReadThread()
        {
            DeviceData data;

            while (runReadThread)
            {
                try
                {
                    if (!IsOpen)
                        PollForConnect();

                    if (!runReadThread)
                        break;

                    data = ReadData(readDataTimeout);

                    switch (data.Status)
                    {
                        case DeviceData.ReadStatus.Cancelled:
                            ReadThread_Cancelled();
                            break;

                        case DeviceData.ReadStatus.WaitTimedOut:
                            ReadThread_WaitTimedOut();
                            break;

                        case DeviceData.ReadStatus.Success:
                            bool dataRead = false;
                            for (int i = 0; i < data.Bytes.Length; i++)
                            {
                                if (data.Bytes[i] != 0)
                                {
                                    dataRead = true;
                                    break;
                                }
                            }

                            if (dataRead)
                                ReadThread_DataRead(data);
                            else
                                ReadThread_NoDataRead();
                            break;

                        case DeviceData.ReadStatus.ReadError:
                            ReadThread_ReadError(data.Error);
                            break;

                        case DeviceData.ReadStatus.NoDataRead:
                            ReadThread_NoDataRead();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ReadThread_ReadError(ex);
                }
            }

            var readExit = new EventWaitHandle(false, EventResetMode.AutoReset, readExitName);
            readExit.Set();
            readExit.Close();
        }

        DeviceData ReadData(int timeout)
        {
            var buffer = new byte[DeviceInfo.Capabilities.InputReportByteLength];
            var status = DeviceData.ReadStatus.NoDataRead;

            if (DeviceInfo.Capabilities.InputReportByteLength > 0)
            {
                var bytesRead = 0;

                if (DeviceReadMode == DeviceMode.Overlapped)
                {
                    #region Read Overlapped


                    var readEventOverlapTimeout = timeout <= 0 ? NativeMethods.WAIT_INFINITE : timeout;
                    int[] hEvents = new int[] { readEventOverlapped.hEvent, cancelEventOverlapped.hEvent };

                    NativeMethods.ResetEvent(cancelEventOverlapped.hEvent);

                    try
                    {
                        NativeMethods.ReadFileOverlapped(readHandle, ref buffer[0], buffer.Length, ref bytesRead, ref readEventOverlapped);

                        var result = NativeMethods.WaitForMultipleObjects(2, ref hEvents[0], false, readEventOverlapTimeout);
                        
                        switch (result)
                        {
                            case NativeMethods.WAIT_OBJECT_0:
                                status = DeviceData.ReadStatus.Success;
                                break;

                            case NativeMethods.WAIT_OBJECT_1:
                                // cancelEventOverlapped.hEvent was set, indicating a cancel of readHandle
                                NativeMethods.CancelIo(readHandle);
                                NativeMethods.WaitForSingleObject(readEventOverlapped.hEvent, NativeMethods.WAIT_INFINITE);
                                buffer = new byte[] { };
                                status = DeviceData.ReadStatus.Cancelled;
                                break;

                            case NativeMethods.WAIT_TIMEOUT:
                                status = DeviceData.ReadStatus.WaitTimedOut;
                                NativeMethods.CancelIo(readHandle);
                                NativeMethods.WaitForSingleObject(readEventOverlapped.hEvent, NativeMethods.WAIT_INFINITE);
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
                    catch (Exception ex)
                    {
                        return new DeviceData(buffer, DeviceData.ReadStatus.ReadError, ex);
                    }

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
                    catch (Exception ex)
                    {
                        return new DeviceData(buffer, DeviceData.ReadStatus.ReadError, ex);
                    }
                }
            }

            return new DeviceData(buffer, status);
        }

        #endregion

        #region Write

        protected void StartWrite()
        {
            if (runWriteThread)
                return;

            runWriteThread = true;
            writeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, writeEventName);
            writeExit = new EventWaitHandle(false, EventResetMode.AutoReset, writeExitName);
            writeThread = new Thread(writeThreadDelegate);
            writeThread.Start();
        }

        protected void StopWrite()
        {
            if (!runWriteThread)
                return;

            runWriteThread = false;

            lock (writeDelegates)
            {
                writeDelegates.Clear();
            }

            writeEvent.Set();
            writeEvent.Close();

            if (Thread.CurrentThread != writeThread)
            {
                if (!writeExit.WaitOne(1000))
                {
                    System.Diagnostics.Debug.WriteLine("StopWrite: writeExit.WaitOne timed out");
                    writeThread.Abort();
                }
            }

            writeExit.Close();

            writeThread = null;
        }

        protected virtual void WriteThread()
        {
            var localWriteEvent = new EventWaitHandle(false, EventResetMode.AutoReset, writeEventName);

            while (runWriteThread)
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
                                if (!IsConnected || !runWriteThread)
                                    break;

                                next = writeDelegateLink.Next;

                                if (WriteType.Complete == writeDelegateLink.Value(this, safe))
                                    writeDelegates.Remove(writeDelegateLink);

                                writeDelegateLink = next;
                            }
                        }
                    }

                    if (runWriteThread)
                    {
                        if (writeDelegates.Count == 0)
                            localWriteEvent.WaitOne();
                        else
                            Thread.Sleep(DelayBetweenWrites);
                    }
                }
                else
                    localWriteEvent.WaitOne();
            }

            localWriteEvent.Close();

            var writeExit = new EventWaitHandle(false, EventResetMode.AutoReset, writeExitName);
            writeExit.Set();
            writeExit.Close();
        }

        #endregion

        void CreateOverlappedEvent(out NativeMethods.SECURITY_ATTRIBUTES security, out NativeMethods.OVERLAPPED overlappedEvent)
        {
            security = new NativeMethods.SECURITY_ATTRIBUTES
            {
                lpSecurityDescriptor = IntPtr.Zero,
                bInheritHandle = true,
                nLength = Marshal.SizeOf(cancelEventSecurity)
            };

            overlappedEvent = new NativeMethods.OVERLAPPED
            {
                Offset = 0,
                OffsetHigh = 0,
                hEvent = NativeMethods.CreateEvent(ref cancelEventSecurity, Convert.ToInt32(true), Convert.ToInt32(false), string.Empty)
            };
        }

		public bool GetFeature(byte[] data)
		{
			if (!IsConnected && !Connect())
				return false;

			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			int length = data.Length;
			var safeHandle = new SafeFileHandle(OpenDeviceIO(DeviceInfo.Path, DeviceMode.NonOverlapped, NativeMethods.GENERIC_READ), true);
			try
			{
				IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
				return NativeMethods.HidD_GetFeature(safeHandle, buffer, length);
			}
			finally
			{
				handle.Free();
				safeHandle.Dispose();
			}
		}

		public bool SetFeature(byte[] data)
		{
			if (!IsConnected && !Connect())
				return false;

			GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			int length = data.Length;
			SafeFileHandle safe = new SafeFileHandle(OpenDeviceIO(DeviceInfo.Path, DeviceMode.NonOverlapped, NativeMethods.GENERIC_WRITE), true);
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

			return true;
		}

		///  <summary>
		///  Read a single feature report. Use ".Capabilities.FeatureReportByteLength"
		///  to determine the length of the buffer.
		///  </summary>
		///  
		///  <param name="reportID">Report ID to be read.</param>
		///  <param name="reportBuffer">Buffer that holds the data that was read.</param>
		///  
		///  <returns></returns>
		public bool ReadFeatureReport(byte reportID, Byte[] reportBuffer)
		{
			reportBuffer[0] = reportID;
			return GetFeature(reportBuffer);
		}

		///  <summary>
		///  Write a single feature report. Use ".Capabilities.FeatureReportByteLength"
		///  to determine the length of the buffer.
		///  </summary>
		///  
		///  <param name="reportID">Report ID to be written.</param>
		///  <param name="reportBuffer">Buffer that holds the data that will be written.</param>
		///  
		///  <returns></returns>
		public bool WriteFeatureReport(byte reportID, Byte[] reportBuffer)
		{
			reportBuffer[0] = reportID;
			return SetFeature(reportBuffer);
		}

        public virtual void Dispose()
        {
            Close();
        }
    }
}
