using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace GNet
{
    public static class CrossFramework
    {
        public static bool IsInvokeRequired(this DispatcherObject obj)
        {
            return obj.Dispatcher.Thread != Thread.CurrentThread;
        }

        public static void Invoke(this DispatcherObject obj, Delegate method, params object[] args)
        {
            obj.Dispatcher.BeginInvoke(method, args);
        }
    }
}
