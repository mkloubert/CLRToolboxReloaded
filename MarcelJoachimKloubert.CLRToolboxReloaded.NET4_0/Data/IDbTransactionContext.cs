// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Data
{
    #region INTERFACE: IDbTransactionContext

    /// <summary>
    /// Describes a execution context for an <see cref="IDbTransaction" /> object.
    /// </summary>
    public interface IDbTransactionContext : IObject
    {
        #region Properties (4)

        /// <summary>
        /// Gets or sets if transaction should be commited if execution was successful.
        /// </summary>
        /// <remarks>
        /// This should be <see langword="true" /> by default.
        /// </remarks>
        bool Commit { get; set; }
        
        /// <summary>
        /// Gets or sets if transaction should be rollbacked or not.
        /// </summary>
        /// <remarks>
        /// This should be <see langword="false" /> by default.
        /// </remarks>
        bool Rollback { get; set; }

        /// <summary>
        /// Gets or sets if transaction should be rollbacked if execution has been failed.
        /// </summary>
        /// <remarks>
        /// This should be <see langword="true" /> by default.
        /// </remarks>
        bool RollbackOnFailure { get; set; }

        /// <summary>
        /// Gets the underlying transaction.
        /// </summary>
        IDbTransaction Transaction { get; }

        #endregion Properties (3)

        #region Methods (1)

        /// <summary>
        /// Returns the value of <see cref="IDbTransactionContext.Transaction" /> property strong typed.
        /// </summary>
        /// <typeparam name="TTrans">Target type.</typeparam>
        /// <returns>Casted version of <see cref="IDbTransactionContext.Transaction" /> property.</returns>
        TTrans GetTransaction<TTrans>();

        #endregion Methods (1)
    }

    #endregion INTERFACE: IDbTransactionContext

    #region INTERFACE: IDbTransactionContext<TState>

    /// <summary>
    /// Describes a execution context for an <see cref="IDbTransaction" /> object.
    /// </summary>
    /// <typeparam name="TState">Type of the state object.</typeparam>
    public interface IDbTransactionContext<out TState> : IDbTransactionContext
    {
        #region Properties (1)

        /// <summary>
        /// Gets the underlying state object.
        /// </summary>
        TState State { get; }

        #endregion Properties (1)
    }

    #endregion INTERFACE: IDbTransactionContext<TState>
}