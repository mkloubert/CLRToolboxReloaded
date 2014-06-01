// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A basic logger that wraps another logger.
    /// </summary>
    public abstract class LoggerWrapperBase : LoggerBase
    {
        #region Fields (1)

        /// <summary>
        /// Stores the inner logger.
        /// </summary>
        protected readonly ILogger _INNER_LOGGER;

        #endregion Fields

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger to wrap.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> and/or <paramref name="sync" /> and/or are <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(ILogger innerLogger, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (innerLogger == null)
            {
                throw new ArgumentNullException("innerLogger");
            }

            this._INNER_LOGGER = innerLogger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger to wrap.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(ILogger innerLogger, bool isSynchronized)
            : this(innerLogger: innerLogger,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger to wrap.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> and/or <paramref name="sync" /> and/or are <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(ILogger innerLogger, object sync)
            : this(innerLogger: innerLogger,
                   sync: sync,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger to wrap.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerLogger" /> is <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(ILogger innerLogger)
            : this(innerLogger: innerLogger,
                   sync: new object())
        {
        }

        #endregion Constrcutors (4)
    }
}