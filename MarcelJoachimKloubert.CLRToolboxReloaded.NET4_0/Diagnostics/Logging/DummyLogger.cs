// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that does nothing.
    /// </summary>
    public sealed class DummyLogger : LoggerBase
    {
        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyLogger" /> class.
        /// </summary>
        /// <param name="synchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public DummyLogger(bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyLogger" /> class.
        /// </summary>
        /// <param name="synchronized">Object is thread safe or not.</param>
        public DummyLogger(bool synchronized)
            : base(synchronized: synchronized)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyLogger" /> class.
        /// </summary>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public DummyLogger(object sync)
            : base(sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyLogger" /> class.
        /// </summary>
        public DummyLogger()
            : base()
        {
        }

        #endregion Constructors

        #region Methods (1)

        // Protected Methods (1) 

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            // do nothing
        }

        #endregion Methods
    }
}