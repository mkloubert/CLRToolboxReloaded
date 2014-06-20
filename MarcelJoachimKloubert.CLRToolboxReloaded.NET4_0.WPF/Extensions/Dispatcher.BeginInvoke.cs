// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Windows
{
    static partial class ClrToolboxWpfExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Invoke the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method of the
        /// dispatcher of a <see cref="DispatcherObject" /> directly.
        /// </summary>
        /// <typeparam name="TDisp">Specific type of the dispatcher object.</typeparam>
        /// <param name="obj">The object that has an own <see cref="Dispatcher" />.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="prio">The priority.</param>
        /// <returns>The dispatcher operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherOperation BeginInvoke<TDisp>(this TDisp obj, Action<TDisp> action,
                                                             DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return BeginInvoke<TDisp, Action<TDisp>>(obj: obj,
                                                     action: (o, a) => a(o),
                                                     actionState: action,
                                                     prio: prio);
        }

        /// <summary>
        /// Invoke the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method of the
        /// dispatcher of a <see cref="DispatcherObject" /> directly.
        /// </summary>
        /// <typeparam name="TDisp">Specific type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the second parameter of <paramref name="action" />.</typeparam>
        /// <param name="obj">The object that has an own <see cref="Dispatcher" />.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The second parameter for <paramref name="action" />.
        /// </param>
        /// <param name="prio">The priority.</param>
        /// <returns>The dispatcher operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> and/or <paramref name="action" />
        /// are <see langword="null" />.
        /// </exception>
        public static DispatcherOperation BeginInvoke<TDisp, TState>(this TDisp obj, Action<TDisp, TState> action, TState actionState,
                                                                     DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            return BeginInvoke<TDisp, TState>(obj,
                                              action, (o) => actionState,
                                              prio);
        }

        /// <summary>
        /// Invoke the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method of the
        /// dispatcher of a <see cref="DispatcherObject" /> directly.
        /// </summary>
        /// <typeparam name="TDisp">Specific type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the second parameter of <paramref name="action" />.</typeparam>
        /// <param name="obj">The object that has an own <see cref="Dispatcher" />.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateFactory">
        /// The factory that creates the second parameter for <paramref name="action" />.
        /// </param>
        /// <param name="prio">The priority.</param>
        /// <returns>The dispatcher operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" />, <paramref name="action" /> and/or <paramref name="actionStateFactory" />
        /// are <see langword="null" />.
        /// </exception>
        public static DispatcherOperation BeginInvoke<TDisp, TState>(this TDisp obj, Action<TDisp, TState> action, Func<TDisp, TState> actionStateFactory,
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

            return BeginInvoke<TDisp, TState, object>(obj: obj,
                                                      func: (o, s) =>
                                                      {
                                                          action(o, s);
                                                          return null;
                                                      }, funcStateFactory: actionStateFactory
                                                       , prio: prio);
        }


        /// <summary>
        /// Invoke the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method of the
        /// dispatcher of a <see cref="DispatcherObject" /> directly.
        /// </summary>
        /// <typeparam name="TDisp">Specific type of the dispatcher object.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="obj">The object that has an own <see cref="Dispatcher" />.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="prio">The priority.</param>
        /// <returns>The dispatcher operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherOperation BeginInvoke<TDisp, TResult>(this TDisp obj, Func<TDisp, TResult> func,
                                                                      DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return BeginInvoke<TDisp, Func<TDisp, TResult>, TResult>(obj: obj,
                                                                     func: (o, f) => f(o),
                                                                     funcState: func,
                                                                     prio: prio);
        }

        /// <summary>
        /// Invoke the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method of the
        /// dispatcher of a <see cref="DispatcherObject" /> directly.
        /// </summary>
        /// <typeparam name="TDisp">Specific type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the second parameter of <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="obj">The object that has an own <see cref="Dispatcher" />.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">
        /// The second parameter for <paramref name="func" />.
        /// </param>
        /// <param name="prio">The priority.</param>
        /// <returns>The dispatcher operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> and/or <paramref name="func" />
        /// are <see langword="null" />.
        /// </exception>
        public static DispatcherOperation BeginInvoke<TDisp, TState, TResult>(this TDisp obj, Func<TDisp, TState, TResult> func, TState funcState,
                                                                              DispatcherPriority prio = DispatcherPriority.Normal)
            where TDisp : global::System.Windows.Threading.DispatcherObject
        {
            return BeginInvoke<TDisp, TState, TResult>(obj: obj,
                                                       func: func,
                                                       funcStateFactory: (o) => funcState,
                                                       prio: prio);
        }

        /// <summary>
        /// Invoke the <see cref="Dispatcher.BeginInvoke(Delegate, DispatcherPriority, object[])" /> method of the
        /// dispatcher of a <see cref="DispatcherObject" /> directly.
        /// </summary>
        /// <typeparam name="TDisp">Specific type of the dispatcher object.</typeparam>
        /// <typeparam name="TState">Type of the second parameter of <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="obj">The object that has an own <see cref="Dispatcher" />.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateFactory">
        /// The factory that creates the second parameter for <paramref name="func" />.
        /// </param>
        /// <param name="prio">The priority.</param>
        /// <returns>The dispatcher operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" />, <paramref name="func" /> and/or <paramref name="funcStateFactory" />
        /// are <see langword="null" />.
        /// </exception>
        public static DispatcherOperation BeginInvoke<TDisp, TState, TResult>(this TDisp obj, Func<TDisp, TState, TResult> func, Func<TDisp, TState> funcStateFactory,
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

            return obj.Dispatcher
                      .BeginInvoke(func,
                                   prio,
                                   obj, funcStateFactory(obj));
        }

        #endregion Methods
    }
}