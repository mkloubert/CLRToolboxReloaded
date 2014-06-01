// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// A basic object.
    /// </summary>
    public abstract class ObjectBase :
#if !(PORTABLE || PORTABLE40)
        global::System.MarshalByRefObject,
#endif
 IObject
    {
        #region Fields (2)

        /// <summary>
        /// Stores the reference for the <see cref="ObjectBase.SyncRoot" /> property.
        /// </summary>
        protected readonly object _SYNC;

        /// <summary>
        /// Stores the value for the <see cref="ObjectBase.IsSynchronized" /> property.
        /// </summary>
        protected readonly bool _IS_SYNCHRONIZED;

        #endregion Fields (2)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected ObjectBase(bool isSynchronized, object sync)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._SYNC = sync;
            this._IS_SYNCHRONIZED = isSynchronized;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        protected ObjectBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected ObjectBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        protected ObjectBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constrcutors (4)

        #region Events (1)

        /// <inheriteddoc />
        public event EventHandler<ErrorEventArgs> ErrorsReceived;

        #endregion Events (1)

        #region Properties (3)

        /// <inheriteddoc />
        public bool IsSynchronized
        {
            get { return this._IS_SYNCHRONIZED; }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        /// <inheriteddoc />
        public virtual object Tag
        {
            get;
            set;
        }

        #endregion Properties (3)

        #region Methods (9)
        
        /// <summary>
        /// Invokes an action thread safe.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeThreadSafe<TResult>(Action<ObjectBase> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeThreadSafe<Action<ObjectBase>>(action: (obj, a) => a(obj),
                                                      actionState: action);
        }
        
        /// <summary>
        /// Invokes an action thread safe.
        /// </summary>
        /// <typeparam name="TState">Type of the optional and additional object / value for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The optional and additional object / value for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeThreadSafe<TState>(Action<ObjectBase, TState> action,
                                                TState actionState)
        {
            this.InvokeThreadSafe<TState>(action: action,
                                          actionStateProvider: (obj) => actionState);
        }

        /// <summary>
        /// Invokes an action thread safe.
        /// </summary>
        /// <typeparam name="TState">Type of the optional and additional object / value for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">The provider that creates / returns the optional and additional object / value for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected void InvokeThreadSafe<TState>(Action<ObjectBase, TState> action,
                                                Func<ObjectBase, TState> actionStateProvider)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            this.InvokeThreadSafe(func: (obj, s) =>
                                  {
                                      s.Action(s.Object,
                                               s.StateProvider(s.Object));

                                      return (object)null;
                                  },
                                  funcState: new
                                  {
                                      Action = action,
                                      Object = this,
                                      StateProvider = actionStateProvider,
                                  });
        }
        
        /// <summary>
        /// Invokes a function thread safe.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeThreadSafe<TResult>(Func<ObjectBase, TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return this.InvokeThreadSafe<Func<ObjectBase, TResult>, TResult>(func: (obj, f) => f(obj),
                                                                             funcState: func);
        }

        /// <summary>
        /// Invokes a function thread safe.
        /// </summary>
        /// <typeparam name="TState">Type of the optional and additional object / value for <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The optional and additional object / value for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeThreadSafe<TState, TResult>(Func<ObjectBase, TState, TResult> func,
                                                            TState funcState)
        {
            return this.InvokeThreadSafe<TState, TResult>(func: func,
                                                          funcStateProvider: (obj) => funcState);
        }

        /// <summary>
        /// Invokes a function thread safe.
        /// </summary>
        /// <typeparam name="TState">Type of the optional and additional object / value for <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateProvider">The provider that creates / returns the optional and additional object / value for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected TResult InvokeThreadSafe<TState, TResult>(Func<ObjectBase, TState, TResult> func,
                                                            Func<ObjectBase, TState> funcStateProvider)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateProvider");
            }

            TResult result;

            lock (this._SYNC)
            {
                result = func(this,
                              funcStateProvider(this));
            }

            return result;
        }

        /// <summary>
        /// Raises the <see cref="ObjectBase.ErrorsReceived" /> event.
        /// </summary>
        /// <param name="ex">The underlying exception.</param>
        /// <returns>Event was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ex" /> is <see langword="null" />.
        /// </exception>
        protected bool OnErrorsReceived(Exception ex)
        {
            return this.RaiseEventHandler(this.ErrorsReceived,
                                          new ErrorEventArgs(ex));
        }

        /// <summary>
        /// Raises an event handler (if defined).
        /// </summary>
        /// <param name="handler">The handler to raise.</param>
        /// <returns>Handler was raised or not.</returns>
        protected bool RaiseEventHandler(EventHandler handler)
        {
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises an event handler with specific arguments (if defined).
        /// </summary>
        /// <typeparam name="TEventArgs">Type of the arguments.</typeparam>
        /// <param name="handler">The handler to raise.</param>
        /// <param name="e">The arguments.</param>
        /// <returns>Handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e" /> is <see langword="null" />.
        /// </exception>
        protected bool RaiseEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs e) where TEventArgs : global::System.EventArgs
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (handler != null)
            {
                handler(this, e);
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}