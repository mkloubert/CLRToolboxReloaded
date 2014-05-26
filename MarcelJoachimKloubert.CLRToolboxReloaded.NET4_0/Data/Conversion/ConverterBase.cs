// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Conversion
{
    /// <summary>
    /// A basic converter.
    /// </summary>
    public abstract class ConverterBase : ObjectBase, IConverter
    {
        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase" /> class.
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected ConverterBase(bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase" /> class.
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        protected ConverterBase(bool synchronized)
            : base(synchronized: synchronized)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected ConverterBase(object sync)
            : base(sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase" /> class.
        /// </summary>
        protected ConverterBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Methods (3)

        // Public Methods (2) 

        /// <inheriteddoc />
        public T ChangeType<T>(object value, IFormatProvider provider = null)
        {
            return (T)this.ChangeType(typeof(T), value, provider);
        }

        /// <inheriteddoc />
        public object ChangeType(Type type, object value, IFormatProvider provider = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

#if !(PORTABLE || PROTABLE40)

            if (provider == null)
            {
                provider = global::System.Threading.Thread.CurrentThread.CurrentCulture;
            }

#endif

            object result = value;
            this.OnChangeType(type, ref result, provider);

            return result;
        }

        // Protected Methods (1) 

        /// <summary>
        /// The logic for the <see cref="ConverterBase.ChangeType(Type, object, IFormatProvider)" /> method.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="targetValue">The value where to write the target value to.</param>
        /// <param name="provider">The optional format provider to use.</param>
        protected abstract void OnChangeType(Type targetType, ref object targetValue, IFormatProvider provider);

        #endregion Methods
    }
}