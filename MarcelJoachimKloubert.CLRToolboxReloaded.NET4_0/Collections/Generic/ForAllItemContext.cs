// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region CLASS: ForAllItemContext<T>

    /// <summary>
    /// Simple implementation of the <see cref="IForAllItemContext{T}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    public class ForAllItemContext<T> : ObjectBase, IForAllItemContext<T>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public ForAllItemContext(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForAllItemContext(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public ForAllItemContext(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForAllItemContext()
            : base()
        {
        }

        #endregion Constrcutors (4)

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

    #endregion CLASS: ForAllItemContext<T>

    #region CLASS: ForAllItemContext<T, TState>

    /// <summary>
    /// Simple implementation of the <see cref="IForAllItemContext{T, TState}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    /// <typeparam name="TState">Type of the underlying state object.</typeparam>
    public class ForAllItemContext<T, TState> : ForAllItemContext<T>, IForAllItemContext<T, TState>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public ForAllItemContext(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForAllItemContext(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public ForAllItemContext(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public ForAllItemContext()
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

    #endregion CLASS: ForAllItemContext<T, TState>
}