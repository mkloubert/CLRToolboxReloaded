// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.IO.Console;
using System;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that uses an <see cref="IConsole" /> instance for output.
    /// </summary>
    public class ConsoleLogger : LoggerBase
    {
        #region Fields (1)

        /// <summary>
        /// Stores the underlyinf provider.
        /// </summary>
        protected readonly ConsoleProvider _PROVIDER;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="LoggerWrapperBase.Provider" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public ConsoleLogger(ConsoleProvider provider, bool isSynchronized, object sync)
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
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="LoggerWrapperBase.Provider" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public ConsoleLogger(ConsoleProvider provider, bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="LoggerWrapperBase.Provider" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public ConsoleLogger(ConsoleProvider provider, object sync)
            : this(provider: provider,
                   sync: sync,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="LoggerWrapperBase.Provider" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public ConsoleLogger(ConsoleProvider provider)
            : this(provider: provider,
                   sync: new object())
        {
        }

        #endregion Constrcutors (4)

        #region Delegates and events

        /// <summary>
        /// A function that provides the <see cref="IConsole" /> instance for an instance of that class.
        /// </summary>
        /// <param name="logger">The base logger.</param>
        /// <returns>The console to use.</returns>
        public delegate IConsole ConsoleProvider(ConsoleLogger logger);

        #endregion Delegates and events

        #region Properties (1)

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public ConsoleProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (1)

        #region Methods (9)

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <remarks>
        /// The new instance will always use the instance of <see cref="GlobalConsole.Current" /> property.
        /// </remarks>
        public static ConsoleLogger Create()
        {
            return Create(sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The new instance will always use the instance of <see cref="GlobalConsole.Current" /> property.
        /// </remarks>
        public static ConsoleLogger Create(object sync)
        {
            return Create(isSynchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <returns>The new instance.</returns>
        /// <remarks>
        /// The new instance will always use the instance of <see cref="GlobalConsole.Current" /> property.
        /// </remarks>
        public static ConsoleLogger Create(bool isSynchronized)
        {
            return Create(isSynchronized: isSynchronized,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The new instance will always use the instance of <see cref="GlobalConsole.Current" /> property.
        /// </remarks>
        public static ConsoleLogger Create(bool isSynchronized, object sync)
        {
            return new ConsoleLogger(provider: (l) => GlobalConsole.Current,
                                     isSynchronized: isSynchronized,
                                     sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="console">The console to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="console" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static ConsoleLogger Create(IConsole console, bool isSynchronized, object sync)
        {
            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            return new ConsoleLogger(provider: (l) => console,
                                     isSynchronized: isSynchronized,
                                     sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="console">The console to use.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="console" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static ConsoleLogger Create(IConsole console, object sync)
        {
            return Create(console: console,
                          isSynchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="console">The console to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="console" /> is <see langword="null" />.
        /// </exception>
        public static ConsoleLogger Create(IConsole console, bool isSynchronized)
        {
            return Create(console: console,
                          isSynchronized: isSynchronized,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleLogger" /> class.
        /// </summary>
        /// <param name="console">The console to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="console" /> is <see langword="null" />.
        /// </exception>
        public static ConsoleLogger Create(IConsole console)
        {
            return Create(console: console,
                          sync: new object());
        }

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msgObj, ref bool succeeded)
        {
            var console = this._PROVIDER(this);
            if (console == null)
            {
                return;
            }

            ConsoleColor? foreColor = ConsoleColor.Gray;
            ConsoleColor? bgColor = ConsoleColor.Black;
            ConsoleColor? oldForeColor = console.ForegroundColor;
            ConsoleColor? oldBgColor = console.BackgroundColor;
            try
            {
                if (msgObj.GetCategoryFlags().Contains(LogCategories.FatalErrors))
                {
                    foreColor = ConsoleColor.Yellow;
                    bgColor = ConsoleColor.Red;
                }
                else if (msgObj.GetCategoryFlags().Contains(LogCategories.Errors))
                {
                    foreColor = ConsoleColor.Red;
                    bgColor = ConsoleColor.Black;
                }
                else if (msgObj.GetCategoryFlags().Contains(LogCategories.Warnings))
                {
                    foreColor = ConsoleColor.Yellow;
                    bgColor = ConsoleColor.Black;
                }
                else if (msgObj.GetCategoryFlags().Contains(LogCategories.Information))
                {
                    foreColor = ConsoleColor.White;
                    bgColor = ConsoleColor.Blue;
                }

                console.BackgroundColor = bgColor;
                console.ForegroundColor = foreColor;

                console.WriteLine("[{0} :: {1}]",
                                  msgObj.Time.ToString("yyyy-MM-dd, HH:mm:ss.fffff"),
                                  msgObj.LogTag)
                       .WriteLine(msgObj.Message)
                       .WriteLine();
            }
            finally
            {
                console.BackgroundColor = oldBgColor;
                console.ForegroundColor = oldForeColor;
            }
        }

        #endregion Methods (9)
    }
}