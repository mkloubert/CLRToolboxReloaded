// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Monitoring
{
    /// <summary>
    /// A monitor that is based on a delegate.
    /// </summary>
    public sealed class DelegateMonitor : ObjectBase, IMonitor
    {
        #region Fields (2)

        private readonly Func<IEnumerable<IMonitorItem>> _GET_ITEMS_FUNC;
        private readonly ItemProvider _PROVIDER;

        #endregion Fields (2)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateMonitor" /> class.
        /// </summary>
        /// <param name="provider">The delegate that provides the monitor items.</param>
        /// <param name="isSynchronized">Handle thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public DelegateMonitor(ItemProvider provider,
                               bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;

            if (this._IS_SYNCHRONIZED)
            {
                this._GET_ITEMS_FUNC = this.GetItems_ThreadSafe;
            }
            else
            {
                this._GET_ITEMS_FUNC = this.GetItems_NonThreadSafe;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateMonitor" /> class.
        /// </summary>
        /// <param name="provider">The delegate that provides the monitor items.</param>
        /// <param name="isSynchronized">Handle thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateMonitor(ItemProvider provider,
                               bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateMonitor" /> class.
        /// </summary>
        /// <param name="provider">The delegate that provides the monitor items.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public DelegateMonitor(ItemProvider provider,
                               object sync)
            : this(provider: provider,
                   sync: sync, isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateMonitor" /> class.
        /// </summary>
        /// <param name="provider">The delegate that provides the monitor items.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateMonitor(ItemProvider provider)
            : this(provider: provider,
                   sync: new object())
        {
        }

        #endregion Constructors (4)

        #region Delegates (1)

        /// <summary>
        /// Describes a method or function that provides the monitor items for a <see cref="DelegateMonitor" /> instance.
        /// </summary>
        /// <param name="monitor">The undlerying monitor.</param>
        /// <returns>The current items for the monitor.</returns>
        public delegate IEnumerable<IMonitorItem> ItemProvider(DelegateMonitor monitor);

        #endregion Delegates (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying item provider.
        /// </summary>
        public ItemProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (1)

        #region Methods (7)

        /// <summary>
        /// Creates a new instance of that class.
        /// </summary>
        /// <param name="items">The items to return by <see cref="DelegateMonitor.GetItems()" /> method.</param>
        /// <param name="isSynchronized">Handle thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The craeted instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateMonitor Create(IEnumerable<IMonitorItem> items,
                                             bool isSynchronized, object sync)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            return new DelegateMonitor((m) => items,
                                       isSynchronized: isSynchronized, sync: sync);
        }

        /// <summary>
        /// Creates a new instance of that class.
        /// </summary>
        /// <param name="items">The items to return by <see cref="DelegateMonitor.GetItems()" /> method.</param>
        /// <param name="isSynchronized">Handle thread safe or not.</param>
        /// <returns>The craeted instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public static DelegateMonitor Create(IEnumerable<IMonitorItem> items,
                                             bool isSynchronized)
        {
            return Create(items: items,
                          isSynchronized: isSynchronized, sync: new object());
        }

        /// <summary>
        /// Creates a new instance of that class.
        /// </summary>
        /// <param name="items">The items to return by <see cref="DelegateMonitor.GetItems()" /> method.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The craeted instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateMonitor Create(IEnumerable<IMonitorItem> items,
                                             object sync)
        {
            return Create(items: items,
                          isSynchronized: false, sync: sync);
        }

        /// <summary>
        /// Creates a new instance of that class.
        /// </summary>
        /// <param name="items">The items to return by <see cref="DelegateMonitor.GetItems()" /> method.</param>
        /// <returns>The craeted instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public static DelegateMonitor Create(IEnumerable<IMonitorItem> items)
        {
            return Create(items: items,
                          isSynchronized: false, sync: new object());
        }

        /// <inheriteddoc />
        public IEnumerable<IMonitorItem> GetItems()
        {
            return this._GET_ITEMS_FUNC();
        }

        private IEnumerable<IMonitorItem> GetItems_NonThreadSafe()
        {
            return (this._PROVIDER(this) ?? Enumerable.Empty<IMonitorItem>()).Where(i => i != null);
        }

        private IEnumerable<IMonitorItem> GetItems_ThreadSafe()
        {
            IEnumerable<IMonitorItem> result;

            lock (this._SYNC)
            {
                result = this.GetItems_NonThreadSafe();
            }

            return result;
        }

        #endregion Methods (7)
    }
}