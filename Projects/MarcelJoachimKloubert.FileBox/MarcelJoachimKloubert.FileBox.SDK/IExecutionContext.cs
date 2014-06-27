// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.FileBox
{
    #region INTERFACE: IExecutionProgress

    /// <summary>
    /// Describes an execution context.
    /// </summary>
    public interface IExecutionContext : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Properties (11)

        /// <summary>
        /// Gets a value between 0 and 100 that describes the progress of the current step
        /// inside that operation in percentage (if defined).
        /// </summary>
        double? CurrentStepPercentage { get; }

        /// <summary>
        /// Gets the status text for the progress of the current step.
        /// </summary>
        string CurrentStepProgressStatus { get; }

        /// <summary>
        /// Gets a value that describes / categorizes the task inside the current step context.
        /// </summary>
        int? CurrentStepTaskId { get; }

        /// <summary>
        /// Gets the time the context needed to be executed.
        /// </summary>
        TimeSpan? Duration { get; }

        /// <summary>
        /// Gets the end time.
        /// </summary>
        DateTimeOffset? EndTime { get; }

        /// <summary>
        /// Gets the occured errors (if available).
        /// </summary>
        IList<Exception> Errors { get; }

        /// <summary>
        /// Gets if the exeuction has been failed or not.
        /// </summary>
        bool HasFailed { get; }

        /// <summary>
        /// Gets if the operation is cancelling or not.
        /// </summary>
        bool IsCancelling { get; }

        /// <summary>
        /// Gets if the context is currently running or not.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets if the operations runs thread safe or not.
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Gets a value between 0 and 100 that describes the progress of the whole operation
        /// in percentage (if defined).
        /// </summary>
        double? OverallPercentage { get; }

        /// <summary>
        /// Gets the status text for the progress of the whole operation.
        /// </summary>
        string OverallProgressStatus { get; }

        /// <summary>
        /// Gets a value that describes / categorizes the task inside the whole operation context.
        /// </summary>
        int? OverallTaskId { get; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        DateTimeOffset? StartTime { get; }

        /// <summary>
        /// Gets the object for thread safe operations.
        /// </summary>
        object SyncRoot { get; }

        #endregion Properties (11)

        #region Methods (8)

        /// <summary>
        /// Cancels the operation.
        /// </summary>
        /// <param name="wait">Wait until cancellation has been finish (with or without timeout).</param>
        /// <param name="timeout">
        /// The maximum time to wait.
        /// <see langword="null" /> stands for NO LIMIT.
        /// </param>
        /// <returns>Cancellation was successful or not (e.g. timeout).</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is invalid.
        /// </exception>
        bool Cancel(bool wait = true,
                    TimeSpan? timeout = null);

        /// <summary>
        /// Cancels the operation via an async task.
        /// </summary>
        /// <param name="completedAction">
        /// The optional action that should be invoked if operation has bee completed.
        /// </param>
        /// <param name="timeout">The optional maximum time to wait for cancellation operation.</param>
        /// <param name="startTask">Directly start task or return it only.</param>
        /// <returns>The underlying task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is invalid.
        /// </exception>
        Task CancelAsync(Action<IExecutionContext, bool> completedAction = null,
                         TimeSpan? timeout = null,
                         bool startTask = true);

        /// <summary>
        /// Cancels the operation via an async task.
        /// </summary>
        /// <typeparam name="T">Type of the second parameter for <paramref name="completedAction" />.</typeparam>
        /// <param name="completedAction">
        /// The optional action that should be invoked if operation has bee completed.
        /// </param>
        /// <param name="startTask">Directly start task or return it only.</param>
        /// <param name="timeout">The optional maximum time to wait for cancellation operation.</param>
        /// <param name="actionState">The second parameter for <paramref name="completedAction" />.</param>
        /// <returns>The underlying task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="completedAction" /> is <see langword="null" />.
        /// </exception>
        Task CancelAsync<T>(Action<IExecutionContext, T, bool> completedAction,
                            T actionState,
                            TimeSpan? timeout = null,
                            bool startTask = true);

        /// <summary>
        /// Cancels the operation via an async task.
        /// </summary>
        /// <typeparam name="T">Type of the second parameter for <paramref name="completedAction" />.</typeparam>
        /// <param name="completedAction">
        /// The optional action that should be invoked if operation has bee completed.
        /// </param>
        /// <param name="startTask">Directly start task or return it only.</param>
        /// <param name="actionStateProvider">
        /// The function that returns the second parameter for <paramref name="completedAction" />.
        /// </param>
        /// <param name="timeout">The optional maximum time to wait for cancellation operation.</param>
        /// <returns>The underlying task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="completedAction" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is invalid.
        /// </exception>
        Task CancelAsync<T>(Action<IExecutionContext, T, bool> completedAction,
                            Func<IExecutionContext, T> actionStateProvider,
                            TimeSpan? timeout = null,
                            bool startTask = true);

        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">Operation is currently running.</exception>
        void Start();

        /// <summary>
        /// Starts the operation via an async task.
        /// </summary>
        /// <param name="completedAction">
        /// The optional action that should be invoked if operation has bee completed.
        /// </param>
        /// <param name="startTask">Directly start task or return it only.</param>
        /// <returns>The underlying task.</returns>
        Task StartAsync(Action<IExecutionContext> completedAction = null,
                        bool startTask = true);

        /// <summary>
        /// Starts the operation via an async task.
        /// </summary>
        /// <typeparam name="T">Type of the second parameter for <paramref name="completedAction" />.</typeparam>
        /// <param name="completedAction">
        /// The optional action that should be invoked if operation has bee completed.
        /// </param>
        /// <param name="startTask">Directly start task or return it only.</param>
        /// <param name="actionState">The second parameter for <paramref name="completedAction" />.</param>
        /// <returns>The underlying task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="completedAction" /> is <see langword="null" />.
        /// </exception>
        Task StartAsync<T>(Action<IExecutionContext, T> completedAction,
                           T actionState,
                           bool startTask = true);

        /// <summary>
        /// Starts the operation via an async task.
        /// </summary>
        /// <typeparam name="T">Type of the second parameter for <paramref name="completedAction" />.</typeparam>
        /// <param name="completedAction">
        /// The optional action that should be invoked if operation has bee completed.
        /// </param>
        /// <param name="startTask">Directly start task or return it only.</param>
        /// <param name="actionStateProvider">
        /// The function that returns the second parameter for <paramref name="completedAction" />.
        /// </param>
        /// <returns>The underlying task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="completedAction" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        Task StartAsync<T>(Action<IExecutionContext, T> completedAction,
                           Func<IExecutionContext, T> actionStateProvider,
                           bool startTask = true);

        #endregion Methods (8)
    }

    #endregion INTERFACE: IExecutionProgress

    #region INTERFACE: IExecutionContext<TResult>

    /// <summary>
    /// Describes an execution context with a result.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IExecutionContext<out TResult> : IExecutionContext
    {
        #region Properties (1)

        /// <summary>
        /// Gets the result if execution was successful.
        /// </summary>
        TResult Result { get; }

        #endregion Properties (1)

        #region Methods (6)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IExecutionContext.CancelAsync(Action{IExecutionContext, bool}, TimeSpan?, bool)" />
        Task CancelAsync(Action<IExecutionContext<TResult>, bool> completedAction = null,
                         TimeSpan? timeout = null,
                         bool startTask = true);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IExecutionContext.CancelAsync{T}(Action{IExecutionContext, T, bool}, T, TimeSpan?, bool)" />
        Task CancelAsync<T>(Action<IExecutionContext<TResult>, T, bool> completedAction,
                            T actionState,
                            TimeSpan? timeout = null,
                            bool startTask = true);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IExecutionContext.CancelAsync{T}(Action{IExecutionContext, T, bool}, Func{IExecutionContext, T}, TimeSpan?, bool)" />
        Task CancelAsync<T>(Action<IExecutionContext<TResult>, T, bool> completedAction,
                            Func<IExecutionContext<TResult>, T> actionStateProvider,
                            TimeSpan? timeout = null,
                            bool startTask = true);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IExecutionContext.StartAsync(Action{IExecutionContext}, bool)" />
        Task StartAsync(Action<IExecutionContext<TResult>> completedAction = null,
                        bool startTask = true);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IExecutionContext.StartAsync{T}(Action{IExecutionContext, T}, T, bool)" />
        Task StartAsync<T>(Action<IExecutionContext<TResult>, T> completedAction,
                           T actionState,
                           bool startTask = true);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IExecutionContext.StartAsync{T}(Action{IExecutionContext, T}, Func{IExecutionContext, T}, bool)" />
        Task StartAsync<T>(Action<IExecutionContext<TResult>, T> completedAction,
                           Func<IExecutionContext<TResult>, T> actionStateProvider,
                           bool startTask = true);

        #endregion Methods (6)
    }

    #endregion INTERFACE: IExecutionContext<TResult>
}