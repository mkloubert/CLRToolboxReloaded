// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !PORTABLE45
#define CAN_HANDLE_THREADS
#endif

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox.Threading.Tasks
{
    #region DELEGATE: TaskExecutionErrorHandler

    /// <summary>
    /// Describes logic that is invoked if an error occured while task execution.
    /// </summary>
    /// <param name="ctx">The underlying context.</param>
    /// <param name="ex">The occured error.</param>
    public delegate void TaskExecutionErrorHandler(ITaskExecutionContext ctx, Exception ex);

    #endregion DELEGATE: TaskExecutionErrorHandler

    #region DELEGATE: TaskExecutionErrorHandler<TState>

    /// <summary>
    /// Describes logic that is invoked if an error occured while task execution.
    /// </summary>
    /// <typeparam name="TState">Type of the underyling object of <paramref name="ctx" />.</typeparam>
    /// <param name="ctx">The underlying context.</param>
    /// <param name="ex">The occured error.</param>
    public delegate void TaskExecutionErrorHandler<TState>(ITaskExecutionContext<TState> ctx, Exception ex);

    #endregion DELEGATE: TaskExecutionErrorHandler<TState>

    #region INTERFACE: ITaskExecutionContext

    /// <summary>
    /// Describes a context of a <see cref="Task" /> execution.
    /// </summary>
    public interface ITaskExecutionContext : IObject
    {
        #region Properties (7)

        /// <summary>
        /// Gets the underlying cancellation token.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets the ID of the underlying task.
        /// </summary>
        int? Id { get; }

        /// <summary>
        /// Gets if cancellation request has been made from outside or not.
        /// </summary>
        bool IsCancelling { get; }

        /// <summary>
        /// Gets or sets the action that is invoked after task execution has been completed.
        /// This is ALWAYS invoked even if errors occured or not.
        /// </summary>
        Action<ITaskExecutionContext> OnCompleted { get; set; }

        /// <summary>
        /// Gets or sets the handler that is invoked when an error is thrown.
        /// </summary>
        TaskExecutionErrorHandler OnError { get; set; }
        
        /// <summary>
        /// Gets or sets if errors should be rethrown or not.
        /// </summary>
        bool? RethrowErrors { get; set; }

#if CAN_HANDLE_THREADS

        /// <summary>
        /// Gets the underlying thread.
        /// </summary>
        global::System.Threading.Thread Thread { get; }

#endif

        #endregion Properties (4)
    }

    #endregion INTERFACE: ITaskExecutionContext

    #region INTERFACE: ITaskExecutionContext<TState>

    /// <summary>
    /// Describes a context of a <see cref="Task" /> execution.
    /// </summary>
    /// <typeparam name="TState">Type of the underlying state object.</typeparam>
    public interface ITaskExecutionContext<TState> : ITaskExecutionContext
    {
        #region Properties (3)

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="ITaskExecutionContext.OnCompleted" />
        new Action<ITaskExecutionContext<TState>> OnCompleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="ITaskExecutionContext.OnError" />
        new TaskExecutionErrorHandler<TState> OnError { get; set; }

        /// <summary>
        /// Gets the underlying state object.
        /// </summary>
        TState State { get; }

        #endregion Properties (1)
    }

    #endregion INTERFACE: ITaskExecutionContext<TState>
}