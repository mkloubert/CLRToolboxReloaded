// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections
{
    /// <summary>
    /// Describes an object that builds collections.
    /// </summary>
    public interface ICollectionBuilder : IObject
    {
        #region Methods (3)

        /// <summary>
        /// Creates new empty dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="isSynchronized">List should be thread safe or not.</param>
        /// <returns>The new dictionary.</returns>
        IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(bool isSynchronized = false);

        /// <summary>
        /// Creates new empty dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="keyComparer">The key comparer to use.</param>
        /// <param name="isSynchronized">List should be thread safe or not.</param>
        /// <returns>The new dictionary.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyComparer" /> is <see langword="null" />.
        /// </exception>
        IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> keyComparer, bool isSynchronized = false);

        /// <summary>
        /// Creates new empty list.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="isSynchronized">List should be thread safe or not.</param>
        /// <returns>The new list.</returns>
        IList<T> CreateList<T>(bool isSynchronized = false);

        #endregion Methods (3)
    }
}