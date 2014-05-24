﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Conversion
{
    /// <summary>
    /// Describes an object that converts values and objects.
    /// </summary>
    public interface IConverter : IObject
    {
        #region Operations (2)

        /// <summary>
        /// Changes a value to a target type if needed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The value to cast / convert.</param>
        /// <param name="provider">The format provider to use.</param>
        /// <returns>The converted / casted version of <paramref name="value" />.</returns>
        /// <exception cref="InvalidCastException">Cast operation failed.</exception>
        T ChangeType<T>(object value, IFormatProvider provider = null);

        /// <summary>
        /// Changes a value to a target type if needed.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="value">The value to cast / convert.</param>
        /// <param name="provider">The format provider to use.</param>
        /// <returns>The converted / casted version of <paramref name="value" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidCastException">Cast operation failed.</exception>
        object ChangeType(Type type, object value, IFormatProvider provider = null);

        #endregion Operations
    }
}