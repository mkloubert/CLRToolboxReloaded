// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Windows.Forms
{
    static partial class ClrToolboxWinFormsExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Invokes an action for a control thread safe.
        /// </summary>
        /// <typeparam name="TCtrl">Type of the control.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static void InvokeSafe<TCtrl>(this TCtrl ctrl, Action<TCtrl> action)
            where TCtrl : global::System.Windows.Forms.Control
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            InvokeSafe<TCtrl, Action<TCtrl>>(ctrl: ctrl,
                                             action: (c, a) => a(c),
                                             actionState: action);
        }

        /// <summary>
        /// Invokes an action for a control thread safe.
        /// </summary>
        /// <typeparam name="TCtrl">Type of the control.</typeparam>
        /// <typeparam name="TState">Type of the additional state object for <paramref name="action" />.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The additional state object for <paramref name="action" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static void InvokeSafe<TCtrl, TState>(this TCtrl ctrl,
                                                     Action<TCtrl, TState> action,
                                                     TState actionState)
            where TCtrl : global::System.Windows.Forms.Control
        {
            InvokeSafe<TCtrl, TState>(ctrl: ctrl,
                                      action: action,
                                      actionStateProvider: (c) => actionState);
        }

        /// <summary>
        /// Invokes an action for a control thread safe.
        /// </summary>
        /// <typeparam name="TCtrl">Type of the control.</typeparam>
        /// <typeparam name="TState">Type of the additional state object for <paramref name="action" />.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The action that provides the additional state object for <paramref name="action" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" />, <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        public static void InvokeSafe<TCtrl, TState>(this TCtrl ctrl,
                                                     Action<TCtrl, TState> action,
                                                     Func<TCtrl, TState> actionStateProvider)
            where TCtrl : global::System.Windows.Forms.Control
        {
            if (ctrl == null)
            {
                throw new ArgumentNullException("ctrl");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            InvokeSafe(ctrl: ctrl,
                       func: (c, s) =>
                           {
                               s.Action(c,
                                        s.StateProvider(c));

                               return (object)null;
                           },
                       funcState: new
                           {
                               Action = action,
                               StateProvider = actionStateProvider,
                           });
        }

        /// <summary>
        /// Invokes a function for a control thread safe.
        /// </summary>
        /// <typeparam name="TCtrl">Type of the control.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="func" />.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult InvokeSafe<TCtrl, TResult>(this TCtrl ctrl,
                                                         Func<TCtrl, TResult> func)
            where TCtrl : global::System.Windows.Forms.Control
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return InvokeSafe<TCtrl, Func<TCtrl, TResult>, TResult>(ctrl: ctrl,
                                                                    func: (c, f) => f(c),
                                                                    funcState: func);
        }

        /// <summary>
        /// Invokes a function for a control thread safe.
        /// </summary>
        /// <typeparam name="TCtrl">Type of the control.</typeparam>
        /// <typeparam name="TState">Type of the additional state object for <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="func" />.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">
        /// The additional state object for <paramref name="func" />.
        /// </param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult InvokeSafe<TCtrl, TState, TResult>(this TCtrl ctrl,
                                                                 Func<TCtrl, TState, TResult> func,
                                                                 TState funcState)
            where TCtrl : global::System.Windows.Forms.Control
        {
            return InvokeSafe<TCtrl, TState, TResult>(ctrl: ctrl,
                                                      func: func,
                                                      funcStateProvider: (c) => funcState);
        }

        /// <summary>
        /// Invokes a function for a control thread safe.
        /// </summary>
        /// <typeparam name="TCtrl">Type of the control.</typeparam>
        /// <typeparam name="TState">Type of the additional state object for <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">The result of <paramref name="func" />.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateProvider">
        /// The function that provides the additional state object for <paramref name="func" />.
        /// </param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" />, <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are <see langword="null" />.
        /// </exception>
        public static TResult InvokeSafe<TCtrl, TState, TResult>(this TCtrl ctrl,
                                                                 Func<TCtrl, TState, TResult> func,
                                                                 Func<TCtrl, TState> funcStateProvider)
            where TCtrl : global::System.Windows.Forms.Control
        {
            if (ctrl == null)
            {
                throw new ArgumentNullException("ctrl");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateProvider");
            }

            if (ctrl.InvokeRequired)
            {
                return (TResult)ctrl.Invoke(new Func<TCtrl, Func<TCtrl, TState, TResult>, Func<TCtrl, TState>, TResult>(InvokeSafe<TCtrl, TState, TResult>),
                                            ctrl, func, funcStateProvider);
            }

            return func(ctrl,
                        funcStateProvider(ctrl));
        }

        #endregion Methods (6)
    }
}