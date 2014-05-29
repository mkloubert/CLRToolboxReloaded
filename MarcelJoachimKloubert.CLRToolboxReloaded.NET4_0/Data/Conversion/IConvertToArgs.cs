// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Conversion
{
    /// <summary>
    /// Arguments for <see cref="ConvertToAttribute" />.
    /// </summary>
    public interface IConvertToArgs : IObject
    {
        #region Data members (3)

        /// <summary>
        /// Gets the current value.
        /// </summary>
        object CurrentValue { get; }

        /// <summary>
        /// Gets the format provider if defined.
        /// </summary>
        IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        Type TargetType { get; }

        #endregion Data members (3)

        #region Methods (1)

        /// <summary>
        /// returns the value of <see cref="IConvertToArgs.CurrentValue" /> strong typed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The casted value.</returns>
        T GetCurrentValue<T>();

        #endregion Methods (1)
    }
}