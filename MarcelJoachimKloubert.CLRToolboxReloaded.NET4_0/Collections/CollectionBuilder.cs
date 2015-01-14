// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using TMSynchronizedDictionary = MarcelJoachimKloubert.CLRToolbox.Collections.Generic.SynchronizedDictionary;
using TMSynchronizedList = MarcelJoachimKloubert.CLRToolbox.Collections.Generic.SynchronizedList;

namespace MarcelJoachimKloubert.CLRToolbox.Collections
{
    /// <summary>
    /// A common object that builds collections.
    /// </summary>
    public class CollectionBuilder : ObjectBase, ICollectionBuilder
    {
        #region Methods (3)

        /// <inheriteddoc />
        public virtual IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(bool isSynchronized = false)
        {
            IDictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            if (isSynchronized)
            {
                result = TMSynchronizedDictionary.Create<TKey, TValue>(items: result);
            }

            return result;
        }

        /// <inheriteddoc />
        public virtual IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> keyComparer, bool isSynchronized = false)
        {
            if (keyComparer == null)
            {
                throw new ArgumentNullException("keyComparer");
            }

            IDictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(keyComparer);
            if (isSynchronized)
            {
                result = TMSynchronizedDictionary.Create<TKey, TValue>(items: result);
            }

            return result;
        }

        /// <inheriteddoc />
        public virtual IList<T> CreateList<T>(bool isSynchronized = false)
        {
            IList<T> result = new List<T>();
            if (isSynchronized)
            {
                result = TMSynchronizedList.Create<T>(items: result);
            }

            return result;
        }

        #endregion Methods (3)
    }
}