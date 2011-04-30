﻿using System;
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

        const string readExitName = @"Local\ReadExit";
        const string writeEventName = @"Local\WriteEvent";
        const string writeExitName = @"WriteExit";

        protected int vendorId;
        protected int[] productIds;

        int cancelReadHEvent = -1;
        SafeFileHandle readHandle;

        // timeout on read after to refresh the read event 
        // (seemed like it would "sleep" after a little while)
        protected int readDataTimeout = 10000;
        protected int delayBetweenWrites = 10;

        bool runReadThread;
        bool runWriteThread;

        LinkedList<WriteDelegate> writeDelegates;

        readonly ThreadStart readThreadDelegate;
        readonly ThreadStart writeThreadDelegate;

        Thread readThread;
        Thread writeThread;

        EventWaitHandle writeEvent;
        EventWaitHandle readExit;
        EventWaitHandle writeExit;

        public Device(int vendorId, params int[] productIds)
        {
            this.vendorId = vendorId;
            this.productIds = productIds;

            readThreadDelegate = new ThreadStart(ReadThread);
            
            writeDelegates = new LinkedList<WriteDelegate>();
            writeThreadDelegate = new ThreadStart(WriteThread);
        }

        public DeviceInfo DeviceInfo { get; private set; }
        protected virtual DeviceMode DeviceReadMode { get { return DeviceMode.Overlapped; } }

        public bool IsOpen { get; private set; }
        public bool IsConnected { get; private set; }
        public int DelayBetweenWrites { get { return delayBetweenWrites; } set { delayBetweenWrites = value; } }

        public bool Start()
        {
            if (!Connect())
                return false;

            if (!Open())
                return false;

            StartRead();
            StartWrite();

            return true;
        }

        public void Stop()
        {
            StopWrite();
            StopRead();

            Close();
        }

        protected bool Connect()
        {
            IsConnected = (DeviceInfo = GetDeviceInfo(vendorId, productIds)) != null;

            //if (IsConnected && runWriteThread)
            //    writeEvent.Set();

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
            if (cancelReadHEvent >= 0)
                NativeMethods.SetEvent(cancelReadHEvent);
        }

        protected virtual void ReadThread_Cancelled()
        {
        }

        protected virtual void ReadThread_WaitTimedOut()
        {
        }

        protected virtual void ReadThread_DataRead(DeviceData data)
        {
            Debug.Write("ReadThread_DataRead: ");
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                Debug.Write(data.Bytes[i].ToString("x"));
                Debug.Write(" ");
            }
            Debug.WriteLine(" ");
        }

        protected virtual void ReadThread_ReadError(Exception error)
        {
            Debug.WriteLine("ReadThread_ReadError: " + error);
        }

        protected virtual void ReadThread_NoDataRead()
        {
            Stop();
        }

        void ReadThread()
        {
            DeviceData data;

            while (runReadThread)
            {
                try
                {
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

                    var readEventSecurity = new NativeMethods.SECURITY_ATTRIBUTES();
                    var readEventOverlapped = new NativeMethods.OVERLAPPED();
                    var readEventOverlapTimeout = timeout <= 0 ? NativeMethods.WAIT_INFINITE : timeout;

                    readEventSecurity.lpSecurityDescriptor = IntPtr.Zero;
                    readEventSecurity.bInheritHandle = true;
                    readEventSecurity.nLength = Marshal.SizeOf(readEventSecurity);

                    readEventOverlapped.Offset = 0;
                    readEventOverlapped.OffsetHigh = 0;
                    readEventOverlapped.hEvent = NativeMethods.CreateEvent(ref readEventSecurity, Convert.ToInt32(false), Convert.ToInt32(true), string.Empty);


                    var cancelEventSecurity = new NativeMethods.SECURITY_ATTRIBUTES();
                    var cancelEventOverlapped = new NativeMethods.OVERLAPPED();

                    cancelEventSecurity.lpSecurityDescriptor = IntPtr.Zero;
                    cancelEventSecurity.bInheritHandle = true;
                    cancelEventSecurity.nLength = Marshal.SizeOf(readEventSecurity);

                    cancelEventOverlapped.Offset = 0;
                    cancelEventOverlapped.OffsetHigh = 0;
                    cancelEventOverlapped.hEvent = NativeMethods.CreateEvent(ref readEventSecurity, Convert.ToInt32(true), Convert.ToInt32(false), string.Empty);

                    int[] hEvents = new int[] { readEventOverlapped.hEvent, cancelEventOverlapped.hEvent };

                    cancelReadHEvent = cancelEventOverlapped.hEvent;

                    try
                    {
                        NativeMethods.ReadFileOverlapped(readHandle, ref buffer[0], buffer.Length, ref bytesRead, ref readEventOverlapped);

                        var result = NativeMethods.WaitForMultipleObjects(2, ref hEvents[0], false, readEventOverlapTimeout);

                        cancelReadHEvent = -1;

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
                                next = writeDelegateLink.Next;

                                if (WriteType.Complete == writeDelegateLink.Value(this, safe))
                                    writeDelegates.Remove(writeDelegateLink);

                                writeDelegateLink = next;

                                if (!runWriteThread)
                                    break;
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}