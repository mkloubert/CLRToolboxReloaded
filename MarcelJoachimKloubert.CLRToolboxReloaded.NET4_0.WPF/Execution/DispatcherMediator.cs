﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Execution
{
    /// <summary>
    /// A mediator that uses a <see cref="Dispatcher" /> for invoking logic on the UI thread.
    /// </summary>
    public sealed class DispatcherMediator : Mediator
    {
        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherMediator" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the underlying dispatcher.</param>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public DispatcherMediator(DispatcherProvider provider,
                                  object sync,
                                  DispatcherPriority prio = DispatcherPriority.Normal,
                                  bool runInBackground = false)
            : base(uiAction: ToUIAction(provider, prio, runInBackground),
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherMediator" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the underlying dispatcher.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DispatcherMediator(DispatcherProvider provider,
                                  DispatcherPriority prio = DispatcherPriority.Normal, bool runInBackground = false)
            : this(provider: provider,
                   sync: new object(),
                   prio: prio,
                   runInBackground: runInBackground)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherMediator" /> class.
        /// </summary>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public DispatcherMediator(object sync,
                                  DispatcherPriority prio = DispatcherPriority.Normal,
                                  bool runInBackground = false)
            : this(provider: GetAppDispatcher,
                   sync: sync,
                   prio: prio,
                   runInBackground: runInBackground)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherMediator" /> class.
        /// </summary>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        public DispatcherMediator(DispatcherPriority prio = DispatcherPriority.Normal, bool runInBackground = false)
            : this(provider: GetAppDispatcher,
                   prio: prio,
                   runInBackground: runInBackground)
        {
        }

        #endregion Constructors (4)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function that provides the control for the UI operations.
        /// </summary>
        /// <param name="mediator">The underlying mediator instance.</param>
        /// <returns>The control for the thread safe UI operations.</returns>
        public delegate Dispatcher DispatcherProvider(DispatcherMediator mediator);

        #endregion Events and delegates (1)

        #region Methods (6)

        /// <summary>
        /// Creates a new instance for a specific dispatcher object.
        /// </summary>
        /// <param name="dispObj">The underlying object that manages a dispatcher.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispObj" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherMediator Create(DispatcherObject dispObj,
                                                DispatcherPriority prio = DispatcherPriority.Normal,
                                                bool runInBackground = false)
        {
            return Create(dispObj: dispObj,
                          sync: new object(),
                          prio: prio,
                          runInBackground: runInBackground);
        }

        /// <summary>
        /// Creates a new instance for a specific dispatcher object.
        /// </summary>
        /// <param name="dispObj">The underlying object that manages a dispatcher.</param>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispObj" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DispatcherMediator Create(DispatcherObject dispObj,
                                                object sync,
                                                DispatcherPriority prio = DispatcherPriority.Normal,
                                                bool runInBackground = false)
        {
            if (dispObj == null)
            {
                throw new ArgumentNullException("dispObj");
            }

            return new DispatcherMediator((m) => dispObj.Dispatcher,
                                          prio: prio,
                                          runInBackground: runInBackground,
                                          sync: sync);
        }

        /// <summary>
        /// Creates a new instance for a specific dispatcher.
        /// </summary>
        /// <param name="dispatcher">The underlying dispatcher.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher" /> is <see langword="null" />.
        /// </exception>
        public static DispatcherMediator Create(Dispatcher dispatcher,
                                                DispatcherPriority prio = DispatcherPriority.Normal,
                                                bool runInBackground = false)
        {
            return Create(dispatcher: dispatcher,
                          sync: new object(),
                          prio: prio,
                          runInBackground: runInBackground);
        }

        /// <summary>
        /// Creates a new instance for a specific dispatcher.
        /// </summary>
        /// <param name="dispatcher">The underlying dispatcher.</param>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <param name="prio">The dispatcher priority to use.</param>
        /// <param name="runInBackground">Run in background or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DispatcherMediator Create(Dispatcher dispatcher,
                                                object sync,
                                                DispatcherPriority prio = DispatcherPriority.Normal,
                                                bool runInBackground = false)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            return new DispatcherMediator((m) => dispatcher,
                                          prio: prio,
                                          runInBackground: runInBackground,
                                          sync: sync);
        }

        private static Dispatcher GetAppDispatcher(DispatcherMediator mediator)
        {
            return Application.Current
                              .Dispatcher;
        }

        private static MediatorUIAction ToUIAction(DispatcherProvider provider,
                                                   DispatcherPriority prio, bool runInBackground)
        {
            if (provider == null)
            {
                return null;
            }

            return (ctx) =>
                {
                    var disp = provider(ctx.GetMediator<DispatcherMediator>());

                    if (disp != null)
                    {
                        Func<DispatcherPriority, Delegate, object> funcToInvoke;
                        if (runInBackground)
                        {
                            funcToInvoke = disp.BeginInvoke;
                        }
                        else
                        {
                            funcToInvoke = disp.Invoke;
                        }

                        funcToInvoke(prio, new Action(ctx.Invoke));
                    }
                    else
                    {
                        ctx.Invoke();
                    }
                };
        }

        #endregion Methods (6)
    }
}