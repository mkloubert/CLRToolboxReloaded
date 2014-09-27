using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Collections.ObjectModel;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace MarcelJoachimKloubert.JobScheduler
{
    /// <summary>
    /// A job host.
    /// </summary>
    public sealed class Host : NotifiableBase, IDisposableObject, IRunnable
    {
        #region Fields (1)

        private readonly bool _OWNS_SCHEDULER;

        #endregion Fields (1)

        #region Constuctors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="Host" /> class.
        /// </summary>
        public Host()
            : this(new SynchronizedObservableCollection<IJob>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host" /> class.
        /// </summary>
        /// <param name="scheduler">The value for the <see cref="Host.Scheduler" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="scheduler" /> is <see langword="null" /> references.
        /// </exception>
        public Host(IJobScheduler scheduler)
            : this(scheduler,
                   new SynchronizedObservableCollection<IJob>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host" /> class.
        /// </summary>
        /// <param name="jobs">The value for the <see cref="Host.Jobs" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="jobs" /> is <see langword="null" /> references.
        /// </exception>
        public Host(IList<IJob> jobs)
            : this(new global::MarcelJoachimKloubert.CLRToolbox.Execution.Jobs.JobScheduler(CreateJobProvider(jobs)),
                   jobs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host" /> class.
        /// </summary>
        /// <param name="scheduler">The value for the <see cref="Host.Scheduler" /> property.</param>
        /// <param name="jobs">The value for the <see cref="Host.Jobs" /> property.</param>
        /// <param name="ownsScheduler">Gets if underlying scheduler should be disposed or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="scheduler" /> and/or <paramref name="jobs" /> are <see langword="null" /> references.
        /// </exception>
        public Host(IJobScheduler scheduler, IList<IJob> jobs, bool ownsScheduler = true)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            if (jobs == null)
            {
                throw new ArgumentNullException("jobs");
            }

            this.Jobs = jobs;
            this.Scheduler = scheduler;
            this._OWNS_SCHEDULER = ownsScheduler;
        }

        #endregion Constuctors (4)

        #region Events (2)

        /// <inheriteddoc />
        public event EventHandler Disposed;

        /// <inheriteddoc />
        public event EventHandler Disposing;

        #endregion Events (2)

        #region Properties (6)

        /// <inheriteddoc />
        public bool CanRestart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public bool CanStart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public bool CanStop
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public bool IsDisposed
        {
            get { return this.Get(() => this.IsDisposed); }

            private set { this.Set(value, () => this.IsDisposed); }
        }

        /// <summary>
        /// Gets if the server is host or not.
        /// </summary>
        public bool IsRunning
        {
            get { return this.Get(() => this.IsRunning); }

            private set { this.Set(value, () => this.IsRunning); }
        }

        /// <summary>
        /// Gets the list of jobs.
        /// </summary>
        public IList<IJob> Jobs
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying scheduler.
        /// </summary>
        public IJobScheduler Scheduler
        {
            get;
            private set;
        }

        #endregion Properties (6)

        #region Methods (7)

        private static JobProvider CreateJobProvider(IEnumerable<IJob> jobs)
        {
            return (scheduler) => jobs;
        }

        /// <inheriteddoc />
        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lock (this._SYNC)
            {
                if (disposing)
                {
                    this.RaiseEventHandler(this.Disposing);
                }

                this.Scheduler.Stop();

                if (disposing)
                {
                    if (this._OWNS_SCHEDULER)
                    {
                        if (this.Scheduler.IsDisposed == false)
                        {
                            this.Scheduler.Dispose();
                        }
                    }

                    this.RaiseEventHandler(this.Disposed);
                }
            }
        }
        
        /// <summary>
        /// Restarts the host.
        /// </summary>
        public void Restart()
        {
            lock (this._SYNC)
            {
                this.Scheduler.Restart();
            }
        }

        /// <summary>
        /// Starts the host.
        /// </summary>
        public void Start()
        {
            lock (this._SYNC)
            {
                this.Scheduler.Start();
            }
        }

        /// <summary>
        /// Stops the host.
        /// </summary>
        public void Stop()
        {
            lock (this._SYNC)
            {
                this.Scheduler.Stop();
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(objectName: this.GetType().FullName);
            }
        }

        #endregion Methods (7)
    }
}