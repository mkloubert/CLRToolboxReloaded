﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Checks if a sequence is NOT empty.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="seq">The sequence to check.</param>
        /// <returns>Sequence is empty (<see langword="false" />) or not (<see langword="true" />).</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public static bool IsNotEmpty<T>(this IEnumerable<T> seq)
        {
            return IsEmpty<T>(seq) == false;
        }

        /// <summary>
        /// Checks if a sequence is NOT empty.
        /// </summary>
        /// <param name="seq">The sequence to check.</param>
        /// <returns>Sequence is empty (<see langword="false" />) or not (<see langword="true" />).</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public static bool IsNotEmpty(this IEnumerable seq)
        {
            return IsEmpty(seq) == false;
        }

        #endregion Methods (2)
    }
}