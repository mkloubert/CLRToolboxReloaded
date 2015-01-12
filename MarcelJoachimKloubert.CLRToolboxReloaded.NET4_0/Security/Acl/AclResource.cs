// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// A simple resource for an access control list.
    /// </summary>
    public class AclResource : NotifiableBase, IAclResource
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public AclResource(string name)
            : this(name: name,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        public AclResource(string name, object sync)
            : base(sync: sync,
                   isSynchronized: true)
        {
            this.Name = ParseName(name);
        }

        #endregion Constructors (2)

        #region Properties (2)

        /// <inheriteddoc />
        public virtual bool IsAllowed
        {
            get { return this.Get<bool>(() => this.IsAllowed); }

            set { this.Set(value, () => this.IsAllowed); }
        }

        /// <inheriteddoc />
        public string Name
        {
            get;
            private set;
        }

        #endregion Properties (2)

        #region Methods (4)

        /// <inheriteddoc />
        public bool Equals(IAclResource other)
        {
            return (other != null) ? (this.Name == ParseName(other.Name))
                                   : false;
        }

        /// <inheriteddoc />
        public override bool Equals(object other)
        {
            if (other is IAclResource)
            {
                return this.Equals((IAclResource)other);
            }

            return base.Equals(other);
        }

        /// <inheriteddoc />
        public override int GetHashCode()
        {
            return this.Name != null ? this.Name.GetHashCode()
                                     : 0;
        }

        /// <summary>
        /// Parses a string for use as resource name.
        /// </summary>
        /// <param name="name">The string to convert/parse.</param>
        /// <returns>The converted/parsed string.</returns>
        public static string ParseName(string name)
        {
            return AclRole.ParseName(name);
        }

        #endregion Methods (4)

        #region Operators (1)

        /// <summary>
        /// Creates a new instance of <see cref="AclResource" /> from a string that represents the name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <returns>The new instance.</returns>
        public static implicit operator AclResource(string name)
        {
            return new AclResource(name: name);
        }

        #endregion Operators (1)
    }
}