// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !PORTABLE45
#define CAN_HANDLE_THREADS
#endif

using System;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Threading.Tasks
{
    #region CLASS: TaskExecutionContext

    /// <summary>
    /// Simple implementation of the <see cref="ITaskExecutionContext" /> interface.
    /// </summary>
    public class TaskExecutionContext : ObjectBase, ITaskExecutionContext
    {
        #region Properties (7)

        /// <inheriteddoc />
        public CancellationToken CancellationToken { get; set; }

        /// <inheriteddoc />
        public int? Id { get; set; }

        /// <inheriteddoc />
        public bool IsCancelling
        {
            get { return this.CancellationToken.IsCancellationRequested; }
        }

        /// <inheriteddoc />
        public Action<ITaskExecutionContext> OnCompleted
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public TaskExecutionErrorHandler OnError
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool? RethrowErrors
        {
            get;
            set;
        }

#if CAN_HANDLE_THREADS

        /// <summary>
        /// Gets the underlying thread.
        /// </summary>
        public global::System.Threading.Thread Thread { get; set; }

#endif

        #endregion Properties (8)
    }

    #endregion CLASS: TaskExecutionContext

    #region CLASS: TaskExecutionContext<TState>

    /// <summary>
    /// Simple implementation of the <see cref="ITaskExecutionContext{TState}" /> interface.
    /// </summary>
    public class TaskExecutionContext<TState> : TaskExecutionContext, ITaskExecutionContext<TState>
    {
        #region Fields (2)

        private Action<ITaskExecutionContext<TState>> _onCompleted;
        private TaskExecutionErrorHandler<TState> _onError;

        #endregion Fields (3)

        #region Properties (3)

        /// <inheriteddoc />
        public new Action<ITaskExecutionContext<TState>> OnCompleted
        {
            get { return this._onCompleted; }

            set
            {
                this._onCompleted = value;
                base.OnCompleted = value != null ? new Action<ITaskExecutionContext>((ctx) =>
                    {
                        value((ITaskExecutionContext<TState>)ctx);
                    }) : null;
            }
        }

        /// <inheriteddoc />
        public new TaskExecutionErrorHandler<TState> OnError
        {
            get { return this._onError; }

            set
            {
                this._onError = value;
                base.OnError = value != null ? new TaskExecutionErrorHandler((ctx, ex) =>
                    {
                        value((ITaskExecutionContext<TState>)ctx, ex);
                    }) : null;
            }
        }

        /// <inheriteddoc />
        public TState State { get; set; }

        #endregion Properties (4)
    }

    #endregion CLASS: TaskExecutionContext<TState>
}