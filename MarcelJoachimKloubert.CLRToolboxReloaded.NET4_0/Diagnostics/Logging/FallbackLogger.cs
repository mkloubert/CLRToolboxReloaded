// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that defines a main logger and fallbacks that are called if
    /// the main logger fails.
    /// </summary>
    public sealed class FallbackLogger : LoggerWrapperBase
    {
        #region Fields (1)

        private readonly List<ILogger> _FALLBACK_LOGGERS = new List<ILogger>();

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="mainLogger">The value for the <see cref="FallbackLogger.MainLogger" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Logging is thread safe.
        /// </remarks>
        public FallbackLogger(ILogger mainLogger)
            : base(innerLogger: mainLogger,
                   synchronized: true)
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the main logger.
        /// </summary>
        public ILogger MainLogger
        {
            get { return this._INNER_LOGGER; }
        }

        #endregion Properties

        #region Methods (8)

        /// <summary>
        /// Adds a fallback logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public void Add(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            lock (this._SYNC)
            {
                this._FALLBACK_LOGGERS.Add(logger);
            }
        }

        /// <summary>
        /// Clears the list of fallback loggers.
        /// </summary>
        public void Clear()
        {
            lock (this._SYNC)
            {
                this._FALLBACK_LOGGERS.Clear();
            }
        }

        /// <summary>
        /// Creates a new instance from an inital list of loggers.
        /// </summary>
        /// <param name="mainLogger">The value for the <see cref="FallbackLogger.MainLogger" /> property.</param>
        /// <param name="loggers">The initial list of loggers to add to the new instance.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, IEnumerable<ILogger> loggers)
        {
            if (loggers == null)
            {
                throw new ArgumentNullException("loggers");
            }

            FallbackLogger result = new FallbackLogger(mainLogger);

            loggers.ForEach(ctx => ctx.State
                                      .Logger.Add(ctx.Item),
                            actionState: new
                            {
                                Logger = result,
                            });

            return result;
        }

        /// <summary>
        /// Creates a new instance from an inital list of loggers.
        /// </summary>
        /// <param name="mainLogger">The value for the <see cref="FallbackLogger.MainLogger" /> property.</param>
        /// <param name="loggers">The initial list of loggers to add to the new instance.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mainLogger" /> and/or <paramref name="loggers" /> are <see langword="null" />.
        /// </exception>
        public static FallbackLogger Create(ILogger mainLogger, params ILogger[] loggers)
        {
            return Create(mainLogger,
                          (IEnumerable<ILogger>)loggers);
        }

        /// <summary>
        /// Gets a new list of current stored fallback loggers.
        /// </summary>
        /// <returns>A list of current fallback loggers.</returns>
        public List<ILogger> GetFallbacks()
        {
            List<ILogger> result;

            lock (this._SYNC)
            {
                result = new List<ILogger>(this._FALLBACK_LOGGERS);
            }

            return result;
        }

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            var failed = false;
            var index = 0;

            using (var e = this._FALLBACK_LOGGERS.GetEnumerator())
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

        /// <summary>
        /// Removes a fallback logger.
        /// </summary>
        /// <param name="logger">The logger to remove.</param>
        /// <returns>Logger was removed or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public bool Remove(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            bool result;

            lock (this._SYNC)
            {
                result = this._FALLBACK_LOGGERS.Remove(logger);
            }

            return result;
        }

        #endregion Methods
    }
}