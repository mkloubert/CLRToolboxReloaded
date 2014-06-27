// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.FileBox
{
    #region CLASS: ExecutionContext<TResult>

    internal class ExecutionContext<TResult> : IExecutionContext<TResult>
    {
        #region Fields (13)

        private double? _currentStepPercentage;
        private string _currentStepProgressStatus;
        private int? _currentStepTaskId;
        private DateTimeOffset? _endTime;
        private IList<Exception> _errors;
        private bool _isCancelling;
        private bool _isRunning;
        private double? _overallPercentage;
        private string _overallProgressStatus;
        private int? _overallTaskId;
        private TResult _result;
        private DateTimeOffset? _startTime;
        private readonly object _SYNC = new object();

        #endregion Fields (13)

        #region Events (2)

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion Events (2)

        #region Properties (18)

        internal Action<ExecutionContext<TResult>> Action
        {
            set
            {
                this.Func = value == null ? null : new Func<ExecutionContext<TResult>, TResult>((ctx) =>
                    {
                        value(ctx);
                        return default(TResult);
                    });
            }
        }

        public double? CurrentStepPercentage
        {
            get { return this._currentStepPercentage; }

            internal set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value > 100)
                {
                    value = 100;
                }

                this.OnPropertyChange(() => this.CurrentStepPercentage, ref this._currentStepPercentage, value);
            }
        }

        public string CurrentStepProgressStatus
        {
            get { return this._currentStepProgressStatus; }

            internal set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                else
                {
                    value = value.Trim();
                }

                this.OnPropertyChange(() => this.CurrentStepProgressStatus, ref this._currentStepProgressStatus, value);
            }
        }

        public int? CurrentStepTaskId
        {
            get { return this._currentStepTaskId; }

            internal set
            {
                this.OnPropertyChange(() => this.CurrentStepTaskId, ref this._currentStepTaskId, value);
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                var st = this.StartTime;
                var et = this.EndTime;

                if (st.HasValue && et.HasValue)
                {
                    return et.Value - st.Value;
                }

                return null;
            }
        }

        public DateTimeOffset? EndTime
        {
            get { return this._endTime; }

            private set
            {
                this.OnPropertyChange(() => this.EndTime, ref this._endTime, value);
            }
        }

        public IList<Exception> Errors
        {
            get { return this._errors; }

            private set
            {
                this.OnPropertyChange(() => this.Errors, ref this._errors, value);
            }
        }

        internal Func<ExecutionContext<TResult>, TResult> Func
        {
            get;
            set;
        }

        public bool IsCancelling
        {
            get { return this._isCancelling; }

            private set
            {
                this.OnPropertyChange(() => this.IsCancelling, ref this._isCancelling, value);
            }
        }

        public bool IsRunning
        {
            get { return this._isRunning; }

            private set
            {
                this.OnPropertyChange(() => this.IsRunning, ref this._isRunning, value);
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public bool HasFailed
        {
            get
            {
                var err = this.Errors;

                return (err != null) &&
                       (err.Count > 0);
            }
        }

        public double? OverallPercentage
        {
            get { return this._overallPercentage; }

            internal set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value > 100)
                {
                    value = 100;
                }

                this.OnPropertyChange(() => this.OverallPercentage, ref this._overallPercentage, value);
            }
        }

        public string OverallProgressStatus
        {
            get { return this._overallProgressStatus; }

            internal set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                else
                {
                    value = value.Trim();
                }

                this.OnPropertyChange(() => this.OverallProgressStatus, ref this._overallProgressStatus, value);
            }
        }

        public int? OverallTaskId
        {
            get { return this._overallTaskId; }

            internal set
            {
                this.OnPropertyChange(() => this.OverallTaskId, ref this._overallTaskId, value);
            }
        }

        public TResult Result
        {
            get { return this._result; }

            private set
            {
                this.OnPropertyChange(() => this.Result, ref this._result, value);
            }
        }

        public DateTimeOffset? StartTime
        {
            get { return this._startTime; }

            private set
            {
                this.OnPropertyChange(() => this.StartTime, ref this._startTime, value);
            }
        }

        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        #endregion Properties (18)

        #region Methods (16)

        public bool Cancel(bool wait = true,
                           TimeSpan? timeout = null)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            this.IsCancelling = true;
            var start = DateTimeOffset.Now;

            if (wait)
            {
                while (this.IsRunning)
                {
                    if (timeout.HasValue)
                    {
                        var duration = DateTimeOffset.Now - start;

                        if (duration > timeout.Value)
                        {
                            // timeout
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public Task CancelAsync(Action<IExecutionContext<TResult>, bool> completedAction = null,
                                TimeSpan? timeout = null,
                                bool startTask = true)
        {
            return this.CancelAsync(
                completedAction: (ctx, a, cancellationSucceeded) =>
                    {
                        if (a == null)
                        {
                            return;
                        }

                        a(ctx, cancellationSucceeded);
                    },
                actionState: completedAction,
                startTask: startTask);
        }

        Task IExecutionContext.CancelAsync(Action<IExecutionContext, bool> completedAction,
                                           TimeSpan? timeout,
                                           bool startTask)
        {
            return this.CancelAsync(
                completedAction: (ctx, s, cancellationSucceeded) =>
                    {
                        if (s.Action == null)
                        {
                            return;
                        }

                        s.Action(ctx,
                                 cancellationSucceeded);
                    },
                actionState: new
                    {
                        Action = completedAction,
                    },
                timeout: timeout,
                startTask: startTask);
        }

        public Task CancelAsync<T>(Action<IExecutionContext<TResult>, T, bool> completedAction,
                                   T actionState,
                                   TimeSpan? timeout = null,
                                   bool startTask = true)
        {
            return this.CancelAsync<T>(completedAction: completedAction,
                                       actionStateProvider: (ctx) => actionState,
                                       startTask: startTask);
        }

        Task IExecutionContext.CancelAsync<T>(Action<IExecutionContext, T, bool> completedAction,
                                              T actionState,
                                              TimeSpan? timeout,
                                              bool startTask)
        {
            if (completedAction == null)
            {
                throw new ArgumentNullException("completedAction");
            }

            return this.CancelAsync(
                completedAction: (ctx, s, cancellationSucceeded) =>
                    {
                        s.Action(ctx,
                                 s.State,
                                 cancellationSucceeded);
                    },
                actionState: new
                    {
                        Action = completedAction,
                        State = actionState,
                    },
                timeout: timeout,
                startTask: startTask);
        }

        public Task CancelAsync<T>(Action<IExecutionContext<TResult>, T, bool> completedAction,
                                   Func<IExecutionContext<TResult>, T> actionStateProvider,
                                   TimeSpan? timeout = null,
                                   bool startTask = true)
        {
            if (completedAction == null)
            {
                throw new ArgumentNullException("completedAction");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            var result = new Task((state) =>
                {
                    var t = (Tuple<ExecutionContext<TResult>,
                                   Action<IExecutionContext<TResult>, T, bool>,
                                   Func<IExecutionContext<TResult>, T>,
                                   TimeSpan?>)state;

                    var ctx = t.Item1;
                    var ca = t.Item2;
                    var provider = t.Item3;
                    var to = t.Item4;

                    var cancellationSucceeded = ctx.Cancel(wait: true,
                                                           timeout: to);

                    if (ca != null)
                    {
                        ca(ctx,
                           provider(ctx),
                           cancellationSucceeded);
                    }
                }, state: Tuple.Create(this,
                                       completedAction,
                                       actionStateProvider,
                                       timeout));

            if (startTask)
            {
                result.Start();
            }

            return result;
        }

        Task IExecutionContext.CancelAsync<T>(Action<IExecutionContext, T, bool> completedAction,
                                              Func<IExecutionContext, T> actionStateProvider,
                                              TimeSpan? timeout,
                                              bool startTask)
        {
            if (completedAction == null)
            {
                throw new ArgumentNullException("completedAction");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            return this.CancelAsync(
                completedAction: (ctx, s, cancellationSucceeded) =>
                    {
                        s.Action(ctx,
                                 s.Provider(ctx),
                                 cancellationSucceeded);
                    },
                actionState: new
                    {
                        Action = completedAction,
                        Provider = actionStateProvider,
                    },
                timeout: timeout,
                startTask: startTask);
        }

        private bool OnPropertyChange<T>(Expression<Func<T>> expr, ref T field, T newValue)
        {
            var propertyName = ((expr.Body as MemberExpression).Member as PropertyInfo).Name;

            if (object.Equals(field, newValue) == false)
            {
                var changingHandler = this.PropertyChanging;
                if (changingHandler != null)
                {
                    changingHandler(this, new PropertyChangingEventArgs(propertyName: propertyName));
                }

                field = newValue;

                var changedHandler = this.PropertyChanged;
                if (changedHandler != null)
                {
                    changedHandler(this, new PropertyChangedEventArgs(propertyName: propertyName));
                }

                return true;
            }

            return false;
        }

        internal void SetAction<T>(Action<ExecutionContext<TResult>, T> action, T actionState)
        {
            this.Action = action == null ? null : new Action<ExecutionContext<TResult>>((ctx) =>
                {
                    action(ctx, actionState);
                });
        }

        internal void SetFunc<T>(Func<ExecutionContext<TResult>, T, TResult> func, T funcState)
        {
            this.Func = func == null ? null : new Func<ExecutionContext<TResult>, TResult>((ctx) =>
                {
                    return func(ctx, funcState);
                });
        }

        public void Start()
        {
            lock (this._SYNC)
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException();
                }

                var errors = new List<Exception>();

                try
                {
                    this.Errors = null;
                    this.Result = default(TResult);

                    this.IsCancelling = false;

                    this.EndTime = null;
                    this.StartTime = DateTimeOffset.Now;

                    this.IsRunning = true;

                    this.Result = this.Func(this);
                }
                catch (Exception ex)
                {
                    var aggEx = ex as AggregateException;
                    if (aggEx != null)
                    {
                        var innerEx = aggEx.InnerExceptions;
                        if (innerEx != null)
                        {
                            errors.AddRange(innerEx.OfType<Exception>());
                        }
                    }
                }
                finally
                {
                    this.Errors = errors;

                    this.EndTime = DateTimeOffset.Now;
                    this.IsRunning = false;

                    this.IsCancelling = false;
                }
            }
        }

        public Task StartAsync(Action<IExecutionContext<TResult>> completedAction = null,
                               bool startTask = true)
        {
            return this.StartAsync(
                completedAction: (ctx, a) =>
                    {
                        if (a == null)
                        {
                            return;
                        }

                        a(ctx);
                    },
                actionState: completedAction,
                startTask: startTask);
        }

        Task IExecutionContext.StartAsync(Action<IExecutionContext> completedAction, bool startTask)
        {
            return this.StartAsync(
                completedAction: (ctx, s) =>
                    {
                        if (s.Action == null)
                        {
                            return;
                        }

                        s.Action(ctx);
                    },
                actionState: new
                    {
                        Action = completedAction,
                    },
                startTask: startTask);
        }

        public Task StartAsync<T>(Action<IExecutionContext<TResult>, T> completedAction,
                                  T actionState,
                                  bool startTask = true)
        {
            return this.StartAsync<T>(completedAction: completedAction,
                                      actionStateProvider: (ctx) => actionState,
                                      startTask: startTask);
        }

        Task IExecutionContext.StartAsync<T>(Action<IExecutionContext, T> completedAction,
                                             T actionState,
                                             bool startTask)
        {
            if (completedAction == null)
            {
                throw new ArgumentNullException("completedAction");
            }

            return this.StartAsync(
                completedAction: (ctx, s) =>
                    {
                        s.Action(ctx,
                                 s.State);
                    },
                actionState: new
                    {
                        Action = completedAction,
                        State = actionState,
                    },
                startTask: startTask);
        }

        public Task StartAsync<T>(Action<IExecutionContext<TResult>, T> completedAction,
                                  Func<IExecutionContext<TResult>, T> actionStateProvider,
                                  bool startTask = true)
        {
            if (completedAction == null)
            {
                throw new ArgumentNullException("completedAction");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            var result = new Task((state) =>
                {
                    var t = (Tuple<ExecutionContext<TResult>,
                                   Action<IExecutionContext<TResult>, T>,
                                   Func<IExecutionContext<TResult>, T>>)state;

                    var ctx = t.Item1;
                    var ca = t.Item2;
                    var provider = t.Item3;

                    ctx.Start();

                    if (ca != null)
                    {
                        ca(ctx,
                           provider(ctx));
                    }
                }, state: Tuple.Create(this,
                                       completedAction,
                                       actionStateProvider));

            if (startTask)
            {
                result.Start();
            }

            return result;
        }

        Task IExecutionContext.StartAsync<T>(Action<IExecutionContext, T> completedAction,
                                             Func<IExecutionContext, T> actionStateProvider,
                                             bool startTask)
        {
            if (completedAction == null)
            {
                throw new ArgumentNullException("completedAction");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            return this.StartAsync(
                completedAction: (ctx, s) =>
                    {
                        s.Action(ctx,
                                 s.Provider(ctx));
                    },
                actionState: new
                    {
                        Action = completedAction,
                        Provider = actionStateProvider,
                    },
                startTask: startTask);
        }

        #endregion Methods (16)
    }

    #endregion CLASS: ExecutionContext<TResult>

    #region CLASS: ExecutionContext

    internal class ExecutionContext : ExecutionContext<object>
    {
    }

    #endregion CLASS: ExecutionContext
}