// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.ObjectModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel
{
    #region CLASS: DispatcherObservableCollection<T>

    /// <summary>
    /// A thread safe observable collection that uses a <see cref="Dispatcher" /> for each operation.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public class DispatcherObservableCollection<T> : SynchronizedObservableCollection<T>
    {
        #region Fields (3)

        private readonly bool _IS_BACKGROUND;
        private readonly DispatcherPriority _PRIO;
        private readonly DispatcherProvider _PROVIDER;

        #endregion Fields

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DispatcherObservableCollection{T}.Provider" /> property.</param>
        /// <param name="syncRoot">The value for the <see cref="SynchronizedObservableCollection{T}._SYNC" /> field.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="syncRoot" /> are <see langword="null" />.
        /// </exception>
        public DispatcherObservableCollection(DispatcherProvider provider,
                                              object syncRoot,
                                              DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
            : base(syncRoot)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
            this._PRIO = prio;
            this._IS_BACKGROUND = isBackground;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DispatcherObservableCollection{T}.Provider" /> property.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DispatcherObservableCollection(DispatcherProvider provider,
                                              DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
            : this(provider: provider,
                   syncRoot: new object(),
                   prio: prio,
                   isBackground: isBackground)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <remarks>
        /// The dispatcher of <see cref="Application.Current" /> is used.
        /// </remarks>
        public DispatcherObservableCollection(DispatcherPriority prio = DispatcherPriority.Normal,
                                              bool isBackground = false)
            : this(provider: GetApplicationDispatcher,
                   prio: prio,
                   isBackground: isBackground)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="syncRoot">The value for the <see cref="SynchronizedObservableCollection{T}._SYNC" /> field.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syncRoot" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The dispatcher of <see cref="Application.Current" /> is used.
        /// </remarks>
        public DispatcherObservableCollection(object syncRoot,
                                              DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
            : this(provider: GetApplicationDispatcher,
                   syncRoot: syncRoot,
                   prio: prio, isBackground: isBackground)
        {
        }

        #endregion Constructors

        #region Properties (3)

        /// <summary>
        /// Gets if the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method should be invoked
        /// for each operation ((<see langword="true" />)) or the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// (<see langword="false" />).
        /// </summary>
        public bool IsBackground
        {
            get { return this._IS_BACKGROUND; }
        }

        /// <summary>
        /// Gets the priority to use.
        /// </summary>
        public DispatcherPriority Priority
        {
            get { return this._PRIO; }
        }

        /// <summary>
        /// Gets the handler that provides the <see cref="Dispatcher" /> to use.
        /// </summary>
        public DispatcherProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties

        #region Delegates and Events (1)

        // Delegates (1) 

        /// <summary>
        /// Describes a method or function that provides the <see cref="Dispatcher" /> to use.
        /// </summary>
        /// <param name="coll">The instance of the underlying collection.</param>
        /// <returns>The dispatcher to use.</returns>
        public delegate Dispatcher DispatcherProvider(DispatcherObservableCollection<T> coll);

        #endregion Delegates and Events

        #region Methods (2)

        private static Dispatcher GetApplicationDispatcher(DispatcherObservableCollection<T> coll)
        {
            return Application.Current.Dispatcher;
        }

        /// <inheriteddoc />
        protected override void InvokeForCollection<S>(Action<SynchronizedObservableCollection<T>, S> action, S actionState)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var disp = this._PROVIDER(this);

            Func<DispatcherPriority, Delegate, object> dispAction;
            if (this._IS_BACKGROUND)
            {
                dispAction = disp.BeginInvoke;
            }
            else
            {
                dispAction = disp.Invoke;
            }

            dispAction(this._PRIO,
                       this.CreateSyncAction<S>(action, actionState));
        }

        #endregion Methods
    }

    #endregion

    #region CLASS: DispatcherObservableCollection

    /// <summary>
    /// Factory class for <see cref="DispatcherObservableCollection{T}" />.
    /// </summary>
    public static class DispatcherObservableCollection
    {
        #region Methods (6)
        
        /// <summary>
        /// Creates new and empty instance of the <see cref="DispatcherObservableCollection{T}" /> class
        /// for a specific <see cref="Dispatcher" />.
        /// </summary>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        public static DispatcherObservableCollection<T> Create<T>(DispatcherPriority prio = DispatcherPriority.Normal,
                                                                  bool isBackground = false)
        {
            return Create<T>(syncRoot: new object(),
                             prio: prio,
                             isBackground: isBackground);
        }

        /// <summary>
        /// Creates new and empty instance of the <see cref="DispatcherObservableCollection{T}" /> class
        /// for a specific <see cref="Dispatcher" />.
        /// </summary>
        /// <param name="syncRoot">The value for the <see cref="SynchronizedObservableCollection{T}._SYNC" /> field.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syncRoot" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherObservableCollection<T> Create<T>(object syncRoot, 
                                                                  DispatcherPriority prio = DispatcherPriority.Normal,
                                                                  bool isBackground = false)
        {
            return new DispatcherObservableCollection<T>(syncRoot: syncRoot,
                                                         prio: prio,
                                                         isBackground: isBackground);
        }

        /// <summary>
        /// Creates new instance of the <see cref="DispatcherObservableCollection{T}" /> class
        /// for a specific <see cref="Dispatcher" />.
        /// </summary>
        /// <param name="disp">The dispatcher object from where to get the dispatcher from.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="disp" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherObservableCollection<T> Create<T>(Dispatcher disp,
                                                                  DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
        {
            return Create<T>(disp: disp,
                             syncRoot: new object(),
                             prio: prio,
                             isBackground: isBackground);
        }

        /// <summary>
        /// Creates new instance of the <see cref="DispatcherObservableCollection{T}" /> class
        /// for a specific <see cref="Dispatcher" />.
        /// </summary>
        /// <param name="disp">The dispatcher object from where to get the dispatcher from.</param>
        /// <param name="syncRoot">The value for the <see cref="SynchronizedObservableCollection{T}._SYNC" /> field.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="disp" /> and/or <paramref name="syncRoot" /> are <see langword="null" />.
        /// </exception>
        public static DispatcherObservableCollection<T> Create<T>(Dispatcher disp,
                                                                  object syncRoot,
                                                                  DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
        {
            if (disp == null)
            {
                throw new ArgumentNullException("disp");
            }

            return new DispatcherObservableCollection<T>(provider: (coll) => disp,
                                                         syncRoot: syncRoot,
                                                         prio: prio,
                                                         isBackground: isBackground);
        }

        /// <summary>
        /// Creates new instance of the <see cref="DispatcherObservableCollection{T}" /> class
        /// for a specific <see cref="DispatcherObject" />.
        /// </summary>
        /// <param name="dispObj">The dispatcher object from where to get the dispatcher from.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispObj" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherObservableCollection<T> Create<T>(DispatcherObject dispObj,
                                                                  DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
        {
            return Create<T>(dispObj: dispObj,
                             syncRoot: new object(),
                             prio: prio,
                             isBackground: isBackground);
        }

        /// <summary>
        /// Creates new instance of the <see cref="DispatcherObservableCollection{T}" /> class
        /// for a specific <see cref="DispatcherObject" />.
        /// </summary>
        /// <param name="dispObj">The dispatcher object from where to get the dispatcher from.</param>
        /// <param name="syncRoot">The value for the <see cref="SynchronizedObservableCollection{T}._SYNC" /> field.</param>
        /// <param name="prio">The value for the <see cref="DispatcherObservableCollection{T}.Priority" /> property.</param>
        /// <param name="isBackground">The value for the <see cref="DispatcherObservableCollection{T}.IsBackground" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispObj" /> and/or <paramref name="syncRoot" /> are <see langword="null" />.
        /// </exception>
        public static DispatcherObservableCollection<T> Create<T>(DispatcherObject dispObj,
                                                                  object syncRoot,
                                                                  DispatcherPriority prio = DispatcherPriority.Normal, bool isBackground = false)
        {
            if (dispObj == null)
            {
                throw new ArgumentNullException("dispObj");
            }

            return new DispatcherObservableCollection<T>(provider: (coll) => dispObj.Dispatcher,
                                                         syncRoot: syncRoot,
                                                         prio: prio,
                                                         isBackground: isBackground);
        }

        #endregion Methods
    }

    #endregion
}