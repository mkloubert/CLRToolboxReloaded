// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FinancePlaner.Classes
{
    /// <summary>
    /// Describes an account of a plan.
    /// </summary>
    public sealed class Account
    {
        #region Constructors (1)

        internal Account(Plan plan)
        {
            this.Plan = plan;
        }

        #endregion Constructors (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying plan.
        /// </summary>
        public Plan Plan
        {
            get;
            private set;
        }

        #endregion Properties (1)
    }
}