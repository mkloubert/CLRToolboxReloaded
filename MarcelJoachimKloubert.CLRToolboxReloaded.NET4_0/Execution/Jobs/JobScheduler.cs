// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    /// <summary>
    /// A class for handling scheduled jobs.
    /// </summary>
    public partial class JobScheduler : NotifiableBase, IJobScheduler
    {
        #region Fields (2)

        /// <summary>
        /// Stores the function / method that provides available jobs.
        /// </summary>
        protected readonly JobProvider _PROVIDER;

        private Timer _timer;

        #endregion Fields

        #region Constructors (3)

        /// <summary>
        /// Initializes a new instance of the <see cref="JobScheduler" /> class.
        /// </summary>
        /// <param name="provider">The job provider.</param>
        /// <param name="syncRoot">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="syncRoot" /> are <see langword="null" />.
        /// </exception>
        public JobScheduler(JobProvider provider, object syncRoot)
            : base(syncRoot)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobScheduler" /> class.
        /// </summary>
        /// <param name="provider">The job provider.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public JobScheduler(JobProvider provider)
            : this(provider, new object())
        {
        }

        /// <summary>
        /// Finalizes the current instance of the <see cref="JobScheduler" /> class.
        /// </summary>
        ~JobScheduler()
        {
            this.DisposeInner(false);
        }

        #endregion Constructors

        #region Events and delegates (6)

        /// <inheriteddoc />
        public event EventHandler Disposed;

        /// <inheriteddoc />
        public event EventHandler Disposing;

        /// <inheriteddoc />
        public event EventHandler<JobExecutionResultEventArgs> Executed;

        /// <inheriteddoc />
        public event EventHandler Initialized;

        /// <summary>
        /// Is invoked when that scheduler has been started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Is invoked when that scheduler has been stopped.
        /// </summary>
        public event EventHandler Stopped;

        #endregion Events and delegates

        #region Methods (19)

        private static void AppendErrors(Exception ex, IList<JobException> occuredErrors, JobExceptionContext context)
        {
            if (ex == null)
            {
                return;
            }

            occuredErrors.Add(new JobException(ex, context));
        }

        /// <inheriteddoc />
        public void Dispose()
        {
            this.DisposeInner(true);
            GC.SuppressFinalize(this);
        }

        private void DisposeInner(bool disposing)
        {
            lock (this._SYNC)
            {
                if (disposing && this.IsDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    this.RaiseEventHandler(this.Disposing);
                }

                this.OnDispose(disposing);

                if (disposing)
                {
                    this.RaiseEventHandler(this.Disposed);
                    this.IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// Disposes the underlying timer.
        /// </summary>
        protected virtual void DisposeTimer()
        {
            using (var t = this._timer)
            {
                this._timer = null;
            }
        }

        /// <summary>
        /// Returns all jobs that should be executed at a specific time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The jobs.</returns>
        protected IEnumerable<IJob> GetJobsToExecute(DateTimeOffset time)
        {
            var allJobs = this._PROVIDER(this) ?? Enumerable.Empty<IJob>();

            return allJobs.OfType<IJob>()
                          .Where(j => this.IsRunning &&
                                      j.CanExecute(time));
        }

        /// <summary>
        /// Handles a job item.
        /// </summary>
        /// <param name="ctx">The underlying item context.</param>
        protected virtual void HandleJobItem(IForAllItemContext<IJob, DateTimeOffset> ctx)
        {
            var isCancelling = this.IsRunning == false;

            var job = ctx.Item;
            var occuredErrors = new List<JobException>();

            DateTimeOffset completedAt;
            var execCtx = new JobExecutionContext();
            execCtx.State = JobExecutionState.NotRunning;

            try
            {
                execCtx.Job = job;
                execCtx.ResultVars = new Dictionary<string, object>(EqualityComparerFactory.CreateCaseInsensitiveStringComparer(true, true));

                // execute
                execCtx.Time = ctx.State;

                execCtx.IsCancellingPredicate = (c) =>
                    {
                        if ((isCancelling == false) &&
                            (this.IsRunning == false))
                        {
                            isCancelling = true;
                        }

                        if (isCancelling)
                        {
                            switch (c.State)
                            {
                                case JobExecutionState.NotRunning:
                                case JobExecutionState.Running:
                                    c.State = JobExecutionState.Cancelling;
                                    break;
                            }
                        }

                        return c.State == JobExecutionState.Cancelling;
                    };

                if (this.IsRunning)
                {
                    execCtx.State = JobExecutionState.Running;

                    execCtx.Job
                           .Execute(execCtx);

                    execCtx.State = isCancelling ? JobExecutionState.Canceled
                                                 : JobExecutionState.Finished;
                }
                else
                {
                    execCtx.State = JobExecutionState.Canceled;
                }

                completedAt = AppTime.Now;
            }
            catch (Exception ex)
            {
                completedAt = AppTime.Now;

                execCtx.State = JobExecutionState.Faulted;

                AppendErrors(ex, occuredErrors, JobExceptionContext.Execute);

                // error callback
                try
                {
                    job.Error(execCtx, ex);
                }
                catch (Exception ex2)
                {
                    AppendErrors(ex2, occuredErrors, JobExceptionContext.Error);
                }
            }
            finally
            {
                // completed callback
                try
                {
                    job.Completed(execCtx);
                }
                catch (Exception ex)
                {
                    AppendErrors(ex, occuredErrors, JobExceptionContext.Completed);
                }
            }

            var result = new JobExecutionResult();
            result.Context = execCtx;
            result.Errors = occuredErrors.ToArray();
            result.Vars = new ReadOnlyDictionaryWrapper<string, object>(execCtx.ResultVars);
            result.Time = completedAt;

            this.RaiseEventHandler(this.Executed,
                                   new JobExecutionResultEventArgs(result));
        }

        /// <summary>
        /// Handles jobs that should be executed at a specific time.
        /// </summary>
        /// <param name="time">The time.</param>
        protected virtual void HandleJobs(DateTimeOffset time)
        {
            this.GetJobsToExecute(time)
                .ForAll(this.HandleJobItem,
                        time);
        }

        /// <inheriteddoc />
        public void Initialize()
        {
            lock (this._SYNC)
            {
                if (this.IsInitialized)
                {
                    throw new InvalidOperationException();
                }

                this.OnInitialize();
                this.IsInitialized = true;

                this.RaiseEventHandler(this.Initialized);
            }
        }

        /// <inheriteddoc />
        public void Restart()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                if (this.CanRestart == false)
                {
                    throw new InvalidOperationException();
                }

                this.OnStop();
                this.OnStart();
            }
        }

        /// <inheriteddoc />
        public void Start()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                if (this.CanStart == false)
                {
                    throw new InvalidOperationException();
                }

                this.OnStart();
            }
        }

        /// <inheriteddoc />
        public void Stop()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                if (this.CanStop == false)
                {
                    throw new InvalidOperationException();
                }

                this.OnStop();
            }
        }

        /// <summary>
        /// The logic for the <see cref="JobScheduler.Dispose()" /> method
        /// and the finalizer.
        /// </summary>
        /// <param name="disposing">
        /// Is called from <see cref="JobScheduler.Dispose()" /> method (<see langword="true" />)
        /// or the finalizer (<see langword="false" />).
        /// </param>
        protected virtual void OnDispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeTimer();
            }
            else
            {
                this.StopTimer();
            }

            this.Session = null;
        }

        /// <summary>
        /// The logic for the <see cref="JobScheduler.Initialize()" /> method.
        /// </summary>
        protected virtual void OnInitialize()
        {
            // dummy
        }

        private void OnStart()
        {
            if (this.IsRunning)
            {
                return;
            }

            try
            {
                var newSession = new Session<IJobScheduler>();
                newSession.SetId(Guid.NewGuid());
                newSession.Parent = this;
                newSession.Time = AppTime.Now;

                this.Session = newSession;
                this.StartTimer();

                this.RaiseEventHandler(this.Started);
            }
            catch
            {
                this.Session = null;
                this.RaiseEventHandler(this.Stopped);

                throw;
            }
        }

        private void OnStop()
        {
            if (this.IsRunning == false)
            {
                return;
            }

            this.StopTimer();
            this.Session = null;

            this.RaiseEventHandler(this.Stopped);
        }

        /// <summary>
        /// Starts the underlying timer.
        /// </summary>
        protected virtual void StartTimer()
        {
            this._timer = new Timer(this.Timer_Elapsed, null,
                                    (int)this.TimerInterval.TotalMilliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Stops the underlying timer.
        /// </summary>
        protected virtual void StopTimer()
        {
            this.DisposeTimer();
        }

        /// <summary>
        /// Throws an exception if that object has already been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">That object has already been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Throws an exception if that object has not been initiaized yet.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// That object has not been initiaized yet.
        /// </exception>
        protected void ThrowIfNotInitialized()
        {
            if (this.IsInitialized == false)
            {
                throw new InvalidOperationException();
            }
        }

        private void Timer_Elapsed(object state)
        {
            lock (this._SYNC)
            {
                try
                {
                    var now = AppTime.Now;

                    this.HandleJobs(now);
                }
                catch (Exception ex)
                {
                    this.OnErrorsReceived(ex);
                }
                finally
                {
                    if (this.IsRunning)
                    {
                        this.StartTimer();
                    }
                }
            }
        }

        #endregion Methods

        #region Properties (9)

        /// <inheriteddoc />
        public virtual bool CanRestart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanStart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanStop
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public bool IsDisposed
        {
            get { return this.Get<bool>(() => this.IsDisposed); }

            private set { this.Set(value, () => this.IsDisposed); }
        }

        /// <inheriteddoc />
        public bool IsInitialized
        {
            get { return this.Get<bool>(() => this.IsInitialized); }

            private set { this.Set(value, () => this.IsInitialized); }
        }

        /// <inheriteddoc />
        [ReceiveNotificationFrom("Session")]
        public bool IsRunning
        {
            get { return this.Session != null; }
        }

        /// <inheriteddoc />
        public ISession<IJobScheduler> Session
        {
            get { return this.Get<ISession<IJobScheduler>>(() => this.Session); }

            private set { this.Set(value, () => this.Session); }
        }

        /// <summary>
        /// Gets the start time or <see langword="null" /> if not running.
        /// </summary>
        [ReceiveNotificationFrom("Session")]
        public DateTimeOffset? StartTime
        {
            get
            {
                ISession<IJobScheduler> session = this.Session;

                return session == null ? (DateTimeOffset?)null : session.Time;
            }
        }

        /// <summary>
        /// Gets the check interval for an underlying timer.
        /// </summary>
        protected virtual TimeSpan TimerInterval
        {
            get { return TimeSpan.FromMilliseconds(750); }
        }

        #endregion Properties
    }
}