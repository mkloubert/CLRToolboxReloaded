﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// Logs a message asnychron.
    /// </summary>
    public sealed partial class AsyncLogger : LoggerWrapperBase
    {
        #region Fields (3)

        private readonly TaskCreationOptions _OPTIONS;
        private readonly TaskScheduler _SCHEDULER;
        private readonly CancellationToken _TOKEN;

        #endregion Fields

        #region Constructors (8)

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="options">The task creation options.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           CancellationToken token,
                           TaskCreationOptions options,
                           TaskScheduler scheduler)
            : base(innerLogger: innerLogger,
                   isSynchronized: false)
        {
            this._TOKEN = token;
            this._OPTIONS = options;
            this._SCHEDULER = scheduler ?? TaskScheduler.Current;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="options">The task creation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           CancellationToken token,
                           TaskCreationOptions options)
            : this(innerLogger: innerLogger,
                   token: token,
                   options: options,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           CancellationToken token,
                           TaskScheduler scheduler)
            : this(innerLogger: innerLogger,
                   token: token,
                   options: Task.Factory.CreationOptions,
                   scheduler: scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="options">The task creation options.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           TaskCreationOptions options,
                           TaskScheduler scheduler)
            : this(innerLogger: innerLogger,
                   token: Task.Factory.CancellationToken,
                   options: options,
                   scheduler: scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           CancellationToken token)
            : this(innerLogger: innerLogger,
                   token: token,
                   options: Task.Factory.CreationOptions,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="options">The task creation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           TaskCreationOptions options)
            : this(innerLogger: innerLogger,
                   token: Task.Factory.CancellationToken,
                   options: options,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger,
                           TaskScheduler scheduler)
            : this(innerLogger: innerLogger,
                   token: Task.Factory.CancellationToken,
                   options: Task.Factory.CreationOptions,
                   scheduler: scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(ILogger innerLogger)
            : this(innerLogger: innerLogger,
                   token: Task.Factory.CancellationToken,
                   options: Task.Factory.CreationOptions,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        #endregion Constructors

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            Task.Factory
                .StartNew(action: this.OnLogAsync,
                          state: msg,
                          cancellationToken: this._TOKEN,
                          creationOptions: this._OPTIONS,
                          scheduler: this._SCHEDULER);
        }

        private void OnLogAsync(object state)
        {
            try
            {
                this._INNER_LOGGER
                    .Log((ILogMessage)state);
            }
            catch
            {
                // ignore errors here
            }
        }

        #endregion Methods
    }
}