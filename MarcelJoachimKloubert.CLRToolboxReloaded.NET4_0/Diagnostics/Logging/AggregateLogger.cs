// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that invokes an internal list of stored loggers step-by-step.
    /// </summary>
    public sealed class AggregateLogger : LoggerBase
    {
        #region Fields (1)

        private readonly List<ILogger> _LOGGERS = new List<ILogger>();

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLogger" /> class.
        /// </summary>
        /// <remarks>
        /// Logging is thread safe.
        /// </remarks>
        public AggregateLogger()
            : base(synchronized: true)
        {
        }

        #endregion Constructors

        #region Methods (8)

        // Public Methods (7) 

        /// <summary>
        /// Adds a logger.
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
                this._LOGGERS
                    .Add(logger);
            }
        }

        /// <summary>
        /// Clears the list of loggers.
        /// </summary>
        public void Clear()
        {
            lock (this._SYNC)
            {
                this._LOGGERS
                    .Clear();
            }
        }

        /// <summary>
        /// Creates a new instance from an inital list of loggers.
        /// </summary>
        /// <param name="loggers">The initial list of loggers to add to the new instance.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(IEnumerable<ILogger> loggers)
        {
            if (loggers == null)
            {
                throw new ArgumentNullException("loggers");
            }

            var result = new AggregateLogger();

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
        /// <param name="loggers">The initial list of loggers to add to the new instance.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateLogger Create(params ILogger[] loggers)
        {
            return Create((IEnumerable<ILogger>)loggers);
        }

        /// <summary>
        /// Returns a flatten list of loggers and sub loggers that are part of this instance and its children.
        /// </summary>
        /// <returns>The flatten list.</returns>
        public IEnumerable<ILogger> Flatten()
        {
            var aggLogList = new List<AggregateLogger>();
            aggLogList.Add(this);

            int num = 0;
            while (aggLogList.Count > num)
            {
                var innerLoggers = aggLogList[num++].GetLoggers();

                for (var i = 0; i < innerLoggers.Count; i++)
                {
                    var logger = innerLoggers[i];

                    var aggLog = logger as AggregateLogger;
                    if (aggLog != null)
                    {
                        aggLogList.Add(aggLog);
                    }
                    else
                    {
                        yield return logger;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a new list of current stored loggers.
        /// </summary>
        /// <returns>A list of current loggers.</returns>
        public List<ILogger> GetLoggers()
        {
            List<ILogger> result;

            lock (this._SYNC)
            {
                result = new List<ILogger>(this._LOGGERS);
            }

            return result;
        }

        /// <summary>
        /// Removes a logger.
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
                result = this._LOGGERS.Remove(logger);
            }

            return result;
        }

        // Protected Methods (1) 

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            bool? allFailed = null;

            this._LOGGERS
                .ForEach(ctx =>
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