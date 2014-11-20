// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

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
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="options">The task creation options.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           CancellationToken token,
                           TaskCreationOptions options,
                           TaskScheduler scheduler)
            : base(provider: provider,
                   isSynchronized: false)
        {
            this._TOKEN = token;
            this._OPTIONS = options;
            this._SCHEDULER = scheduler ?? TaskScheduler.Current;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="options">The task creation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           CancellationToken token,
                           TaskCreationOptions options)
            : this(provider: provider,
                   token: token,
                   options: options,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           CancellationToken token,
                           TaskScheduler scheduler)
            : this(provider: provider,
                   token: token,
                   options: Task.Factory.CreationOptions,
                   scheduler: scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="options">The task creation options.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           TaskCreationOptions options,
                           TaskScheduler scheduler)
            : this(provider: provider,
                   token: Task.Factory.CancellationToken,
                   options: options,
                   scheduler: scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           CancellationToken token)
            : this(provider: provider,
                   token: token,
                   options: Task.Factory.CreationOptions,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="options">The task creation options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           TaskCreationOptions options)
            : this(provider: provider,
                   token: Task.Factory.CancellationToken,
                   options: options,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider,
                           TaskScheduler scheduler)
            : this(provider: provider,
                   token: Task.Factory.CancellationToken,
                   options: Task.Factory.CreationOptions,
                   scheduler: scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AsyncLogger(LoggerProvider provider)
            : this(provider: provider,
                   token: Task.Factory.CancellationToken,
                   options: Task.Factory.CreationOptions,
                   scheduler: Task.Factory.Scheduler)
        {
        }

        #endregion Constructors

        #region Methods (11)

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="options">The task creation options.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         CancellationToken token,
                                         TaskCreationOptions options,
                                         TaskScheduler scheduler)
        {
            return new AsyncLogger(ToProvider(logger),
                                   token: token,
                                   options: options,
                                   scheduler: scheduler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="options">The task creation options.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         CancellationToken token,
                                         TaskCreationOptions options)
        {
            return new AsyncLogger(ToProvider(logger),
                                   token: token,
                                   options: options);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         CancellationToken token,
                                         TaskScheduler scheduler)
        {
            return new AsyncLogger(ToProvider(logger),
                                   token: token,
                                   scheduler: scheduler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="options">The task creation options.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         TaskCreationOptions options,
                                         TaskScheduler scheduler)
        {
            return new AsyncLogger(ToProvider(logger),
                                   options: options,
                                   scheduler: scheduler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         CancellationToken token)
        {
            return new AsyncLogger(ToProvider(logger),
                                   token: token);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="options">The task creation options.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         TaskCreationOptions options)
        {
            return new AsyncLogger(provider: ToProvider(logger),
                                   options: options);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <param name="scheduler">
        /// The scheduler to use. <see langword="null" /> means to use the instance from <see cref="TaskScheduler.Current" />.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger,
                                         TaskScheduler scheduler)
        {
            return new AsyncLogger(provider: ToProvider(logger),
                                   scheduler: scheduler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsyncLogger" /> class.
        /// </summary>
        /// <param name="logger">The inner logger.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public static AsyncLogger Create(ILogger logger)
        {
            return new AsyncLogger(provider: ToProvider(logger));
        }

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
                var logger = this._PROVIDER(this);
                if (logger != null)
                {
                    logger.Log((ILogMessage)state);
                }
            }
            catch
            {
                // ignore errors here
            }
        }

        private static LoggerProvider ToProvider(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            return new LoggerProvider((l) => logger);
        }

        #endregion Methods
    }
}