// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FinancePlaner.Classes
{
    /// <summary>
    /// Describes a booking of an account.
    /// </summary>
    public sealed class Booking
    {
        #region Constructors (1)

        internal Booking(Account account)
        {
            this.Account = account;
        }

        #endregion Constructors (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying account.
        /// </summary>
        public Account Account
        {
            get;
            private set;
        }

        #endregion Properties (1)
    }
}