// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FinancePlaner.Classes
{
    /// <summary>
    /// A finance plan.
    /// </summary>
    public sealed class Plan
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="Plan" /> class.
        /// </summary>
        public Plan()
        {
            this.ApprovedBookings = new Account(this);
            this.PlannedBookings = new Account(this);
        }

        #endregion Constructors (1)

        #region Properties (2)

        /// <summary>
        /// Gets the account for the approved bookings.
        /// </summary>
        public Account ApprovedBookings
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the account for the planned bookings.
        /// </summary>
        public Account PlannedBookings
        {
            get;
            private set;
        }

        #endregion Properties (2)
    }
}