// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that invokes list of delegates that handle <see cref="ILogMessage" />s step-by-step.
    /// </summary>
    public sealed class DelegateLogger : LoggerBase
    {
        #region Fields (1)

        private readonly LogMessageHandlerProvider _PROVIDER;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateLogger.Provider" /> property.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public DelegateLogger(LogMessageHandlerProvider provider, bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateLogger.Provider" /> property.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateLogger(LogMessageHandlerProvider provider, bool synchronized)
            : this(provider: provider,
                   synchronized: synchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateLogger.Provider" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public DelegateLogger(LogMessageHandlerProvider provider, object sync)
            : this(provider: provider,
                   sync: sync,
                   synchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateLogger.Provider" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateLogger(LogMessageHandlerProvider provider)
            : this(provider: provider,
                   sync: new object())
        {
        }

        #endregion Constrcutors (4)

        #region Delegates and Events (2)

        /// <summary>
        /// Describes a function / methods that provides the handlers for an instance that class.
        /// </summary>
        /// <param name="logger">The underlying instance.</param>
        /// <returns>The handlers for that class.</returns>
        public delegate IEnumerable<LogMessageHandler> LogMessageHandlerProvider(DelegateLogger logger);

        /// <summary>
        /// Describes a function or method that handles a log message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        public delegate void LogMessageHandler(ILogMessage msg);

        #endregion Delegates and Events

        #region Properties (1)

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public LogMessageHandlerProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (1)

        #region Methods (10)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(IEnumerable<LogMessageHandler> handlers, bool synchronized, object sync)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            return new DelegateLogger((l) => handlers,
                                      synchronized: synchronized,
                                      sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="handlers">The handlers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(bool synchronized, object sync, params LogMessageHandler[] handlers)
        {
            return Create(synchronized: synchronized,
                          sync: sync,
                          handlers: (IEnumerable<LogMessageHandler>)handlers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(IEnumerable<LogMessageHandler> handlers, object sync)
        {
            return Create(handlers,
                          synchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="handlers">The handlers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(object sync, params LogMessageHandler[] handlers)
        {
            return Create(sync: sync,
                          handlers: (IEnumerable<LogMessageHandler>)handlers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(IEnumerable<LogMessageHandler> handlers, bool synchronized)
        {
            return Create(handlers,
                          synchronized: false,
                          sync: new object());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="handlers">The handlers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(bool synchronized, params LogMessageHandler[] handlers)
        {
            return Create(synchronized: synchronized,
                          handlers: (IEnumerable<LogMessageHandler>)handlers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(IEnumerable<LogMessageHandler> handlers)
        {
            return Create(handlers,
                          synchronized: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(params LogMessageHandler[] handlers)
        {
            return Create(handlers: (IEnumerable<LogMessageHandler>)handlers);
        }

        /// <summary>
        /// Returns a normalized list of handlers that are provides by <see cref="DelegateLogger.Provider" /> property.
        /// </summary>
        /// <returns>The list of handlers.</returns>
        public IEnumerable<LogMessageHandler> GetHandlers()
        {
            return (this._PROVIDER(this) ?? Enumerable.Empty<LogMessageHandler>()).Where(h => h != null);
        }

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            bool? allFailed = null;

            this.GetHandlers()
                .ForAll(ctx =>
                        {
                            try
                            {
                                ctx.Item(CloneLogMessage(msg));

                                allFailed = false;
                            }
                            catch
                            {
                                if (allFailed.IsNull())
                                {
                                    allFailed = true;
                                }
                            }
                        });

            if (allFailed.IsTrue())
            {
                succeeded = false;
            }
        }

        #endregion Methods
    }
}