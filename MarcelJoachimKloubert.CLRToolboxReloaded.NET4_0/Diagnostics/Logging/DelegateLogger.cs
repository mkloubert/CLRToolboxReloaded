// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that invokes a list of delegates that handle <see cref="ILogMessage" />s step-by-step.
    /// </summary>
    public sealed class DelegateLogger : LoggerBase, IEnumerable<global::MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.DelegateLogger.LogMessageHandler>
    {
        #region Fields (1)

        private readonly LogMessageHandlerProvider _PROVIDER;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateLogger.Provider" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public DelegateLogger(LogMessageHandlerProvider provider, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
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
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateLogger(LogMessageHandlerProvider provider, bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized,
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
                   isSynchronized: false)
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
        /// <param name="succeeded">
        /// Defines if handling <paramref name="msg" /> was succeeded or not.
        /// That value is or should be <see langword="true" /> at the beginning.
        /// </param>
        public delegate void LogMessageHandler(ILogMessage msg, ref bool succeeded);

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

        #region Methods (9)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(IEnumerable<LogMessageHandler> handlers, bool isSynchronized, object sync)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            return new DelegateLogger((l) => handlers,
                                      isSynchronized: isSynchronized,
                                      sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="handlers">The handlers to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(IEnumerable<LogMessageHandler> handlers, bool isSynchronized)
        {
            return Create(handlers,
                          isSynchronized: isSynchronized,
                          sync: new object());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogger" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="handlers">The handlers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlers" /> is <see langword="null" />.
        /// </exception>
        public static DelegateLogger Create(bool isSynchronized, params LogMessageHandler[] handlers)
        {
            return Create(isSynchronized: isSynchronized,
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
                          isSynchronized: false);
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

        /// <inheriteddoc />
        public IEnumerator<DelegateLogger.LogMessageHandler> GetEnumerator()
        {
            return this.GetHandlers()
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
                                var s = true;
                                ctx.Item(CloneLogMessage(msg), ref s);

                                if (s == false)
                                {
                                    throw new Exception();
                                }

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