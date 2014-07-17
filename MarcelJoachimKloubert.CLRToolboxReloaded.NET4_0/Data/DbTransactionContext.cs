// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Data
{
    #region CLASS: DbTransactionContext

    /// <summary>
    /// Simple implementation of <see cref="IDbTransactionContext" /> interface.
    /// </summary>
    public class DbTransactionContext : ObjectBase, IDbTransactionContext
    {
        #region Properties (4)
        
        /// <inheriteddoc />
        public bool Commit
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool Rollback
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool RollbackOnFailure
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IDbTransaction Transaction
        {
            get;
            set;
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <inheriteddoc />
        public TTrans GetTransaction<TTrans>()
        {
            return GlobalConverter.Current
                                  .ChangeType<TTrans>(value: this.Transaction);
        }

        #endregion Methods (1)
    }

    #endregion CLASS: DbTransactionContext

    #region CLASS: DbTransactionContext<TState>

    /// <summary>
    /// Simple implementation of <see cref="IDbTransactionContext{TState}" /> interface.
    /// </summary>
    public sealed class DbTransactionContext<TState> : DbTransactionContext, IDbTransactionContext<TState>
    {
        #region Properties (1)

        /// <inheriteddoc />
        public TState State
        {
            get;
            set;
        }

        #endregion Properties (1)
    }

    #endregion CLASS: DbTransactionContext<TState>
}