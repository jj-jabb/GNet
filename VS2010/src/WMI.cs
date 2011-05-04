using System;
using System.Diagnostics;
using System.Management;

using GNet.PInvoke;

namespace GNet
{
    public delegate void EventSystemForegroundHandler(int processId, string processName, string filePath);
    public delegate void EventHookExceptionHandler(Exception ex);

    public class WMI : IDisposable
    {
        static WMI current;
        static WinEventDelegate winEventDelegate;

        public static WMI Current
        {
            get
            {
                if (current == null)
                    current = new WMI();

                return current;
            }
        }

        public static void DisposeCurrent()
        {
            current.Dispose();
            current = null;
        }

        bool disposed;
        IntPtr hEvent = IntPtr.Zero;

        WMI()
        {
            winEventDelegate = new WinEventDelegate(WinEventProcCallback);
            hEvent = Interop.SetWinEventHook(WinEvent.EVENT_SYSTEM_FOREGROUND, WinEvent.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, winEventDelegate, 0, 0, SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT);
        }

        ~WMI()
        {
            Dispose();
        }

        public event EventSystemForegroundHandler EventSystemForeground;
        public event EventHookExceptionHandler EventHookException;

        static void WinEventProcCallback(IntPtr hWinEventHook, WinEvent eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //var orgHwnd = hwnd;
            //System.Threading.Thread.Sleep(300);
            //hwnd = Interop.GetForegroundWindow();

            //Debug.WriteLine("Foreground changed: orgHwnd = " + orgHwnd + ", hwnd = " + hwnd);

            Process process = null;
            string name = "";
            string fileName = "";

            try
            {
                process = Interop.GetProcess(hwnd);
                int id = process.Id;

                ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT Name, ExecutablePath FROM Win32_Process WHERE ProcessId = " + id);
                foreach (ManagementObject obj in search.Get())
                {
                    try
                    {
                        name = obj.Properties["Name"].Value.ToString();
                        fileName = obj.Properties["ExecutablePath"].Value.ToString();
                    }
                    catch(Exception ex)
                    {
                        if (Current.EventHookException != null)
                            Current.EventHookException(ex);

                        return;
                    }
                }

                if (Current.EventSystemForeground != null)
                    Current.EventSystemForeground(id, name, fileName);
            }
            catch (Exception ex)
            {
                // explorer throws when process.MainModule is accessed
                if (Current.EventHookException != null)
                    Current.EventHookException(ex);

                return;

            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            if (hEvent != IntPtr.Zero)
                Interop.UnhookWinEvent(hEvent);

            disposed = true;
        }
    }
}
