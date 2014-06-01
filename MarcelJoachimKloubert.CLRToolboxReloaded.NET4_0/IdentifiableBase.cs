// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// A basic implementation if <see cref="IIdentifiable" />.
    /// </summary>
    public abstract class IdentifiableBase : ObjectBase, IIdentifiable
    {
        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected IdentifiableBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        protected IdentifiableBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableBase" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected IdentifiableBase(object sync)
            : base(sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableBase" /> class.
        /// </summary>
        protected IdentifiableBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public abstract Guid Id
        {
            get;
        }

        #endregion Properties (1)

        #region Methods (4)

        /// <inheriteddoc />
        public bool Equals(Guid other)
        {
            return this.Id == other;
        }

        /// <inheriteddoc />
        public bool Equals(IIdentifiable other)
        {
            return other != null ? this.Equals(other.Id) : false;
        }

        /// <inheriteddoc />
        public sealed override bool Equals(object other)
        {
            if (other is IIdentifiable)
            {
                return this.Equals((IIdentifiable)other);
            }

            if (other is Guid)
            {
                return this.Equals((Guid)other);
            }

            return base.Equals(other);
        }

        /// <inheriteddoc />
        public sealed override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #endregion Methods (4)
    }
}