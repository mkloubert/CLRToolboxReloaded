﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region INTERFACE: ForAllItemContext<T>

    /// <summary>
    /// Simple implementation of the <see cref="IForEachItemContext{T}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    public class ForEachItemContext<T> : ForAllItemContext<T>, IForEachItemContext<T>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public ForEachItemContext(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForEachItemContext(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public ForEachItemContext(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForEachItemContext()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public bool Cancel
        {
            get;
            set;
        }

        #endregion Properties (1)
    }

    #endregion INTERFACE: ForEachItemContext<T>

    #region INTERFACE: ForEachItemContext<T, TState>

    /// <summary>
    /// Simple implementation of the <see cref="IForEachItemContext{T, TState}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    /// <typeparam name="TState">Type of the underlying state object.</typeparam>
    public sealed class ForEachItemContext<T, TState> : ForEachItemContext<T>, IForEachItemContext<T, TState>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public ForEachItemContext(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForEachItemContext(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public ForEachItemContext(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForEachItemContext()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public TState State
        {
            get;
            set;
        }

        #endregion Properties (1)
    }

    #endregion INTERFACE: IForAllItemContext<T, TState>
}