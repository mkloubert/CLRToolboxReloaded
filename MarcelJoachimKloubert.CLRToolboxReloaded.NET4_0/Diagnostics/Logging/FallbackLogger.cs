// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that defines a main logger and uses fallbacks that are called if
    /// the main logger fails.
    /// </summary>
    public sealed class FallbackLogger : LoggerWrapperBase
    {
        #region Fields (1)

        private readonly LoggerProvider _PROVIDER;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public FallbackLogger(ILogger mainLogger, LoggerProvider provider, bool isSynchronized, object sync)
            : base(innerLogger: mainLogger,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public FallbackLogger(ILogger mainLogger, LoggerProvider provider, bool isSynchronized)
            : this(mainLogger: mainLogger,
                   provider: provider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public FallbackLogger(ILogger mainLogger, LoggerProvider provider, object sync)
            : this(mainLogger: mainLogger,
                   provider: provider,
                   sync: sync,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="provider">The value for the <see cref="FallbackLogger.Provider" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public FallbackLogger(ILogger mainLogger, LoggerProvider provider)
            : this(mainLogger: mainLogger,
                   provider: provider,
                   sync: new object())
        {
        }

        #endregion Constrcutors (4)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function / methods that provides the fallback loggers for an instance that class.
        /// </summary>
        /// <param name="logger">The underlying instance.</param>
        /// <returns>The fallback for that class.</returns>
        public delegate IEnumerable<ILogger> LoggerProvider(FallbackLogger logger);

        #endregion Events and delegates (1)

        #region Properties (2)

        /// <summary>
        /// Gets the main logger.
        /// </summary>
        public ILogger MainLogger
        {
            get { return this._INNER_LOGGER; }
        }

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public LoggerProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (2)

        #region Methods (8)

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" />, <paramref name="sync" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, IEnumerable<ILogger> loggers, bool isSynchronized, object sync)
        {
            if (loggers == null)
            {
                throw new ArgumentNullException("loggers");
            }

            return new FallbackLogger(mainLogger: mainLogger,
                                      provider: (l) => loggers,
                                      isSynchronized: isSynchronized,
                                      sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" />, <paramref name="sync" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, bool isSynchronized, object sync, params ILogger[] loggers)
        {
            return Create(mainLogger: mainLogger,
                          loggers: (IEnumerable<ILogger>)loggers,
                          isSynchronized: isSynchronized,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" />, <paramref name="sync" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, IEnumerable<ILogger> loggers, object sync)
        {
            return Create(mainLogger: mainLogger,
                          loggers: loggers,
                          isSynchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" />, <paramref name="sync" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, object sync, params ILogger[] loggers)
        {
            return Create(mainLogger: mainLogger,
                          loggers: (IEnumerable<ILogger>)loggers,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, IEnumerable<ILogger> loggers, bool isSynchronized)
        {
            return Create(mainLogger: mainLogger,
                          loggers: loggers,
                          isSynchronized: false,
                          sync: new object());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, bool isSynchronized, params ILogger[] loggers)
        {
            return Create(mainLogger: mainLogger,
                          loggers: (IEnumerable<ILogger>)loggers,
                          isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, IEnumerable<ILogger> loggers)
        {
            return Create(mainLogger: mainLogger,
                          loggers: loggers,
                          isSynchronized: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The main logger.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, params ILogger[] loggers)
        {
            return Create(mainLogger: mainLogger,
                          loggers: (IEnumerable<ILogger>)loggers);
        }

        /// <summary>
        /// Returns a normalized list of fallback loggers that are provides by <see cref="FallbackLogger.Provider" /> property.
        /// </summary>
        /// <returns>The list of fallback loggers.</returns>
        public IEnumerable<ILogger> GetFallbacks()
        {
            return (this._PROVIDER(this) ?? Enumerable.Empty<ILogger>()).Where(l => l != null);
        }

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            var failed = false;
            var index = 0;

            using (var e = this.GetFallbacks().GetEnumerator())
            {
                var currentLogger = this._INNER_LOGGER;
                while (currentLogger != null)
                {
                    var searchForNextFallback = false;
                    failed = false;

                    try
                    {
                        if (currentLogger.Log(msg) == false)
                        {
                            searchForNextFallback = true;
                            failed = true;
                        }
                    }
                    catch
                    {
                        searchForNextFallback = true;
                        failed = true;
                    }
                    finally
                    {
                        currentLogger = null;
                    }

                    if (searchForNextFallback)
                    {
                        if (e.MoveNext())
                        {
                            currentLogger = e.Current;
                            ++index;
                        }
                    }
                }
            }

            if (failed)
            {
                succeeded = false;
            }
        }

        #endregion Methods
    }
}