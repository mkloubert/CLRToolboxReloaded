﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region INTERFACE: ForAllItemContext<T>

    /// <summary>
    /// Simple implementation of the <see cref="IForAllItemContext{T}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    public class ForAllItemContext<T> : ObjectBase, IForAllItemContext<T>
    {
        #region Properties (2)

        /// <inheriteddoc />
        public long Index
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public T Item
        {
            get;
            set;
        }

        #endregion Properties (2)
    }

    #endregion INTERFACE: ForAllItemContext<T>

    #region INTERFACE: ForAllItemContext<T, TState>

    /// <summary>
    /// Simple implementation of the <see cref="IForAllItemContext{T, TState}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    /// <typeparam name="TState">Type of the underlying state object.</typeparam>
    public class ForAllItemContext<T, TState> : ForAllItemContext<T>, IForAllItemContext<T, TState>
    {
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