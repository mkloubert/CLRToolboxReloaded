﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;

namespace MarcelJoachimKloubert.CLRToolbox.Values
{
    /// <summary>
    /// Describes a state.
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    public abstract class StateBase<T> : NotifiableBase, IState<T>
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected StateBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected StateBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected StateBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected StateBase()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public virtual T Value
        {
            get { return this.Get<T>(() => this.Value); }

            set { this.Set<T>(value, () => this.Value); }
        }

        #endregion Properties (1)
    }
}