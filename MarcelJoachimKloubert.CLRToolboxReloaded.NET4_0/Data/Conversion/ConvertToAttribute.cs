// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Conversion
{
    /// <summary>
    /// The attributes marks a member to convert an object to a target type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property,
                    AllowMultiple = true,
                    Inherited = false)]
    public sealed class ConvertToAttribute : Attribute
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes an instance of the <see cref="ConvertToAttribute" /> class.
        /// </summary>
        /// <param name="targetType">The value for the <see cref="ConvertToAttribute.TargetType" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="targetType" /> is <see langword="null" />.
        /// </exception>
        public ConvertToAttribute(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.TargetType = targetType;
        }

        #endregion Constructors (1)

        #region Properties (1)

        /// <summary>
        /// Gets the target type.
        /// </summary>
        public Type TargetType
        {
            get;
            private set;
        }

        #endregion Properties (1)
    }
}