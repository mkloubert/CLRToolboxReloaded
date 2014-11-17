// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.IO.Console
{
    /// <summary>
    /// Access to global console.
    /// </summary>
    public static class GlobalConsole
    {
        #region Fields (1)

        private static ConsoleProvider _provider;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initializes the <see cref="GlobalConsole" /> class.
        /// </summary>
        static GlobalConsole()
        {
#if !(PORTABLE)
            SetConsole(new SystemConsole());
#else
            SetConsole(new DummyConsole());
#endif
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the global console instance.
        /// </summary>
        public static IConsole Current
        {
            get { return _provider(); }
        }

        #endregion Properties

        #region Delegates and Events (1)

        /// <summary>
        /// Describes the logic that returns the global console.
        /// </summary>
        /// <returns>The global console instance.</returns>
        public delegate IConsole ConsoleProvider();

        #endregion Delegates and Events

        #region Methods (2)

        /// <summary>
        /// Sets the logic that returns the value for <see cref="GlobalConsole.Current" />.
        /// </summary>
        /// <param name="newProvider">The new provider delegate.</param>
        public static void SetConsoleProvider(ConsoleProvider newProvider)
        {
            _provider = newProvider;
        }

        /// <summary>
        /// Sets the value for <see cref="GlobalConsole.Current" />.
        /// </summary>
        /// <param name="newConsole">The new console.</param>
        public static void SetConsole(IConsole newConsole)
        {
            SetConsoleProvider(newConsole == null ? null : new ConsoleProvider(() => newConsole));
        }

        #endregion Methods
    }
}