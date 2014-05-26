// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that invokes a list of child loggers step-by-step.
    /// </summary>
    public class AggregateLogger : LoggerBase
    {
        #region Fields (1)

        private readonly LoggerProvider _PROVIDER;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public AggregateLogger(LoggerProvider provider, bool synchronized, object sync)
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
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AggregateLogger(LoggerProvider provider, bool synchronized)
            : this(provider: provider,
                   synchronized: synchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public AggregateLogger(LoggerProvider provider, object sync)
            : this(provider: provider,
                   sync: sync,
                   synchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateLogger.Provider" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AggregateLogger(LoggerProvider provider)
            : this(provider: provider,
                   sync: new object())
        {
        }

        #endregion Constrcutors (4)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function / methods that provides the child loggers for an instance that class.
        /// </summary>
        /// <param name="logger">The underlying instance.</param>
        /// <returns>The loggers for that class.</returns>
        public delegate IEnumerable<ILogger> LoggerProvider(AggregateLogger logger);

        #endregion Events and delegates (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public LoggerProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (1)

        #region Methods (10)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="loggers">The loggers to use.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(IEnumerable<ILogger> loggers, bool synchronized, object sync)
        {
            if (loggers == null)
            {
                throw new ArgumentNullException("loggers");
            }

            return new AggregateLogger((l) => loggers,
                                       synchronized: synchronized,
                                       sync: sync);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(bool synchronized, object sync, params ILogger[] loggers)
        {
            return Create(loggers: (IEnumerable<ILogger>)loggers,
                          synchronized: synchronized,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="loggers">The loggers to use.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(IEnumerable<ILogger> loggers, object sync)
        {
            return Create(loggers,
                          synchronized: false,
                          sync: sync);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(object sync, params ILogger[] loggers)
        {
            return Create(loggers: (IEnumerable<ILogger>)loggers,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="loggers">The loggers to use.</param>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(IEnumerable<ILogger> loggers, bool synchronized)
        {
            return Create(loggers,
                          synchronized: false,
                          sync: new object());
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(bool synchronized, params ILogger[] loggers)
        {
            return Create(loggers: (IEnumerable<ILogger>)loggers,
                          synchronized: synchronized);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(IEnumerable<ILogger> loggers)
        {
            return Create(loggers,
                          synchronized: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <param name="loggers">The loggers to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(params ILogger[] loggers)
        {
            return Create(loggers: (IEnumerable<ILogger>)loggers);
        }

        /// <summary>
        /// Returns a normalized list of loggers that are provides by <see cref="AggregateLogger.Provider" /> property.
        /// </summary>
        /// <returns>The list of loggers.</returns>
        public IEnumerable<ILogger> GetLoggers()
        {
            return (this._PROVIDER(this) ?? Enumerable.Empty<ILogger>()).Where(l => l != null);
        }

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            bool? allFailed = null;

            this.GetLoggers()
                .ForAll(ctx =>
                        {
                            try
                            {
                                ctx.Item
                                   .Log(CloneLogMessage(msg));

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