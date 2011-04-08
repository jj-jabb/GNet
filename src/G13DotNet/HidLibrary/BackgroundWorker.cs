using System;
using System.ComponentModel;
using System.Threading;

namespace HidLibrary
{
    /// <summary>
    /// A background work that reports updates
    /// </summary>
    /// <typeparam name="T">The type of update to report</typeparam>
    public class BackgroundWorker<T>
    {
        // Fields
        private AsyncOperation _asyncOperation;
        private bool _cancellationPending;
        private bool _completed;
        private bool _isRunning;
        private bool _workerReportsUpdates = true;
        private bool _workerSupportsCancellation = true;

        // Events
        public event DoWorkEventHandler DoWork;

        public event EventHandler<EventArgs<T>> Updated;

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        // Methods
        public void CancelAsync()
        {
            if (!this._workerSupportsCancellation)
            {
                throw new InvalidOperationException("BackgroundWorker: cancellation not supported");
            }
            this._cancellationPending = true;
        }

        protected virtual void OnDoWork(DoWorkEventArgs e)
        {
            DoWorkEventHandler doWork = this.DoWork;
            if (doWork != null)
            {
                doWork(this, e);
            }
        }

        protected virtual void OnUpdated(EventArgs<T> e)
        {
            if (Updated != null)
            {
                Updated(this, e);
            }
        }

        private void OnRun(object argument)
        {
            bool cancelled = false;
            Exception error = null;
            DoWorkEventArgs e = new DoWorkEventArgs(argument);
            try
            {
                this.OnDoWork(e);
                cancelled = e.Cancel;
            }
            catch (Exception exception2)
            {
                error = exception2;
            }
            RunWorkerCompletedEventArgs arg = new RunWorkerCompletedEventArgs(e.Result, error, cancelled);
            SendOrPostCallback d = delegate(object state)
            {
                this.OnRunWorkerCompleted((RunWorkerCompletedEventArgs)state);
            };
            if (this._asyncOperation != null)
            {
                this._asyncOperation.PostOperationCompleted(d, arg);
            }
            else
            {
                d(arg);
            }
            this._isRunning = false;
            this._cancellationPending = false;
            this._completed = true;
        }

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompletedEventHandler runWorkerCompleted = this.RunWorkerCompleted;
            if (runWorkerCompleted != null)
            {
                runWorkerCompleted(this, e);
            }
        }

        public void Update(T data)
        {
            if (!this._workerReportsUpdates)
            {
                throw new InvalidOperationException("BackgroundWorker: Update reports not supported");
            }
            
            if (this._completed)
            {
                throw new InvalidOperationException("BackgroundWorker: Operation completed");
            }

            EventArgs<T> arg = new EventArgs<T>(data);
            SendOrPostCallback d = delegate(object state)
            {
                this.OnUpdated((EventArgs<T>)state);
            };
            if (this._asyncOperation != null)
            {
                this._asyncOperation.Post(d, arg);
            }
            else
            {
                d(arg);
            }
        }

        public void RunWorkerAsync()
        {
            this.RunWorkerAsync(null);
        }

        public void RunWorkerAsync(object argument)
        {
            if (this._isRunning)
            {
                throw new InvalidOperationException("BackgroundWorker: Already running");
            }
            this._isRunning = true;
            this._cancellationPending = false;
            this._asyncOperation = AsyncOperationManager.CreateOperation(null);
            this._completed = false;
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnRun), argument);
        }

        // Properties
        public bool CancellationPending
        {
            get
            {
                return this._cancellationPending;
            }
        }

        public bool IsBusy
        {
            get
            {
                return this._isRunning;
            }
        }

        public bool WorkerReportsUpdates
        {
            get
            {
                return this._workerReportsUpdates;
            }
            set
            {
                this._workerReportsUpdates = value;
            }
        }

        public bool WorkerSupportsCancellation
        {
            get
            {
                return this._workerSupportsCancellation;
            }
            set
            {
                this._workerSupportsCancellation = value;
            }
        }
    }
}
