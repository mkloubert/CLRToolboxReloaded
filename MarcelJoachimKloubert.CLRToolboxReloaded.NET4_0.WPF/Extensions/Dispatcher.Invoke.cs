// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Windows
{
    static partial class ClrToolboxWpfExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Directly invokes the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// of a <see cref="Dispatcher" /> instance that is provides by a <see cref="DispatcherObject.Dispatcher" /> property.
        /// </summary>
        /// <typeparam name="TDisp">Type of the dispatcher object.</typeparam>
        /// <param name="obj">The dispatcher object.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="prio">The priority to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static void Invoke<TDisp>(this TDisp obj, Action<TDisp> action,
                                         DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            Invoke<TDisp, Action<TDisp>>(obj: obj,
                                         action: (o, a) => a(o),
                                         actionState: action,
                                         prio: prio);
        }

        /// <summary>
        /// Directly invokes the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// of a <see cref="Dispatcher" /> instance that is provides by a <see cref="DispatcherObject.Dispatcher" /> property.
        /// </summary>
        /// <typeparam name="TDisp">Type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="obj">The dispatcher object.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The function that provides the state object for <paramref name="action" />.</param>
        /// <param name="prio">The priority to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static void Invoke<TDisp, TState>(this TDisp obj, Action<TDisp, TState> action, TState actionState,
                                                 DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            Invoke<TDisp, TState>(obj: obj,
                                  action: action,
                                  actionStateFactory: o => actionState,
                                  prio: prio);
        }

        /// <summary>
        /// Directly invokes the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// of a <see cref="Dispatcher" /> instance that is provides by a <see cref="DispatcherObject.Dispatcher" /> property.
        /// </summary>
        /// <typeparam name="TDisp">Type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="obj">The dispatcher object.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateFactory">The function that provides the state object for <paramref name="action" />.</param>
        /// <param name="prio">The priority to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" />, <paramref name="action" /> and/or <paramref name="actionStateFactory" />
        /// are <see langword="null" />.
        /// </exception>
        public static void Invoke<TDisp, TState>(this TDisp obj, Action<TDisp, TState> action, Func<TDisp, TState> actionStateFactory,
                                                 DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateFactory == null)
            {
                throw new ArgumentNullException("actionStateFactory");
            }

            Invoke<TDisp, TState, object>(obj: obj,
                                          func: (o, s) =>
                                          {
                                              action(o, s);
                                              return null;
                                          }, funcStateFactory: actionStateFactory
                                           , prio: prio);
        }

        /// <summary>
        /// Directly invokes the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// of a <see cref="Dispatcher" /> instance that is provides by a <see cref="DispatcherObject.Dispatcher" /> property.
        /// </summary>
        /// <typeparam name="TDisp">Type of the dispatcher object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="obj">The dispatcher object.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="prio">The priority to use.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult Invoke<TDisp, TResult>(this TDisp obj, Func<TDisp, TResult> func,
                                                     DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return Invoke<TDisp, Func<TDisp, TResult>, TResult>(obj: obj,
                                                                func: (o, f) => f(o),
                                                                funcState: func,
                                                                prio: prio);
        }

        /// <summary>
        /// Directly invokes the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// of a <see cref="Dispatcher" /> instance that is provides by a <see cref="DispatcherObject.Dispatcher" /> property.
        /// </summary>
        /// <typeparam name="TDisp">Type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="obj">The dispatcher object.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The state object for <paramref name="func" />.</param>
        /// <param name="prio">The priority to use.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult Invoke<TDisp, TState, TResult>(this TDisp obj, Func<TDisp, TState, TResult> func, TState funcState,
                                                             DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            return Invoke<TDisp, TState, TResult>(obj: obj,
                                                  func: func,
                                                  funcStateFactory: o => funcState,
                                                  prio: prio);
        }

        /// <summary>
        /// Directly invokes the <see cref="Dispatcher.Invoke(Delegate, DispatcherPriority, object[])" /> method
        /// of a <see cref="Dispatcher" /> instance that is provides by a <see cref="DispatcherObject.Dispatcher" /> property.
        /// </summary>
        /// <typeparam name="TDisp">Type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="obj">The dispatcher object.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateFactory">The function that provides the state object for <paramref name="func" />.</param>
        /// <param name="prio">The priority to use.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" />, <paramref name="func" /> and/or <paramref name="funcStateFactory" />
        /// are <see langword="null" />.
        /// </exception>
        public static TResult Invoke<TDisp, TState, TResult>(this TDisp obj, Func<TDisp, TState, TResult> func, Func<TDisp, TState> funcStateFactory,
                                                             DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateFactory == null)
            {
                throw new ArgumentNullException("funcStateFactory");
            }

            return (TResult)obj.Dispatcher
                               .Invoke(func,
                                       prio,
                                       obj, funcStateFactory(obj));
        }

        #endregion Methods
    }
}