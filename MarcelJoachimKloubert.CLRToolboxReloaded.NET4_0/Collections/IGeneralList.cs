// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections
{
    /// <summary>
    /// A general list with extended features.
    /// </summary>
    public interface IGeneralList : IObject, IList, IEnumerable<object>, ICloneable
    {
        #region Data Members (2)

        /// <summary>
        /// Gets if the list is empty or not.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets if the list is not empty or is empty.
        /// </summary>
        bool IsNotEmpty { get; }

        #endregion Data Members

        #region Methods (9)

        /// <summary>
        /// Adds a range of items.
        /// </summary>
        /// <param name="seq">The items to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        void AddRange(IEnumerable seq);
        
        /// <summary>
        /// Adds a list of items of a specific type which are not null.
        /// </summary>
        /// <typeparam name="T">type of items to filter from <paramref name="items" />.</typeparam>
        /// <param name="items">The items to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        void AddRangeOf<T>(IEnumerable items);

        /// <summary>
        /// Returns that list as a generic list of objects.
        /// </summary>
        /// <returns>That list as generic list of objects.</returns>
        /// <remarks>
        /// If that list is already a generic list of objects, it is simply casted.
        /// </remarks>
        IList<object> AsList();

        /// <summary>
        /// Returns that list as a generic list.
        /// </summary>
        /// <typeparam name="T">The target type of the items.</typeparam>
        /// <param name="ofType">
        /// Cast / convert all items (<see langword="false" />) or filter out (<see langword="true" />).
        /// </param>
        /// <param name="provider">
        /// The optional format provider that is used if items are casted / converted.
        /// </param>
        /// <returns>The new list.</returns>
        /// <remarks>
        /// If that list is already a generic list of the target type, it is simply casted.
        /// </remarks>
        IList<T> AsList<T>(bool ofType = false, IFormatProvider provider = null);

        /// <summary>
        /// Clones that list.
        /// </summary>
        /// <param name="copySyncRoot">
        /// Also apply object from <see cref="IObject.SyncRoot" /> or not.
        /// </param>
        /// <returns>The cloned list.</returns>
        IGeneralList Clone(bool copySyncRoot = false);

        /// <summary>
        /// Converts that list to an array of objects.
        /// </summary>
        /// <returns>The new array.</returns>
        object[] ToArray();

        /// <summary>
        /// Converts that list to an generic array.
        /// </summary>
        /// <typeparam name="T">The target type of the items.</typeparam>
        /// <param name="ofType">
        /// Cast / convert all items (<see langword="false" />) or filter out (<see langword="true" />).
        /// </param>
        /// <param name="provider">
        /// The optional format provider that is used if items are casted / converted.
        /// </param>
        /// <returns>The new array.</returns>
        T[] ToArray<T>(bool ofType = false, IFormatProvider provider = null);

        /// <summary>
        /// Converts that list to a new generic list.
        /// </summary>
        /// <returns>The new list.</returns>
        IList<object> ToList();

        /// <summary>
        /// Converts that list to a new generic list.
        /// </summary>
        /// <typeparam name="T">The target type of the items.</typeparam>
        /// <param name="ofType">
        /// Cast / convert all items (<see langword="false" />) or filter out (<see langword="true" />).
        /// </param>
        /// <param name="provider">
        /// The optional format provider that is used if items are casted / converted.
        /// </param>
        /// <returns>The new list.</returns>
        IList<T> ToList<T>(bool ofType = false, IFormatProvider provider = null);

        #endregion Methods (8)
    }
}