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
        /// Stores the logger provider.
        /// </summary>
        protected readonly LoggerProvider _PROVIDER;

        #endregion Fields

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> and/or are <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(LoggerProvider provider, bool isSynchronized, object sync)
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
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(LoggerProvider provider, bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> and/or are <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(LoggerProvider provider, object sync)
            : this(provider: provider,
                   sync: sync,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapperBase" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the base logger.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        protected LoggerWrapperBase(LoggerProvider provider)
            : this(provider: provider,
                   sync: new object())
        {
        }

        #endregion Constructors (4)

        #region Delegates and events (1)

        /// <summary>
        /// Describes the function that provides the inner logger for an instance of that class.
        /// </summary>
        /// <param name="logger">The base logger.</param>
        /// <returns>The logger to use.</returns>
        public delegate ILogger LoggerProvider(LoggerWrapperBase logger);

        #endregion

        #region Properties (1)

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public LoggerProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion
    }
}