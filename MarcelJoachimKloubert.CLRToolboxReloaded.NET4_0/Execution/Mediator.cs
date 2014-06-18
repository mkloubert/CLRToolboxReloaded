// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox.Execution
{
    /// <summary>
    /// A simple mediator.
    /// </summary>
    public partial class Mediator : ObjectBase
    {
        #region Fields (2)

        private readonly IList<IMediatorActionItem> _ITEMS = new List<IMediatorActionItem>();
        private readonly UIAction _UI_ACTION;

        #endregion Fields

        #region Constructors (4)
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        public Mediator()
            : this(uiAction: InvokeOnCurrentThread)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="uiAction">The logic that invoked an action of the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="uiAction" /> is <see langword="null" />.
        /// </exception>
        public Mediator(UIAction uiAction)
            : this(uiAction: uiAction,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public Mediator(object sync)
            : this(uiAction: InvokeOnCurrentThread,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="uiAction">The logic that invoked an action of the UI thread.</param>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="uiAction" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public Mediator(UIAction uiAction, object sync)
            : base(isSynchronized: false,
                   sync: sync)
        {
            if (uiAction == null)
            {
                throw new ArgumentNullException("uiAction");
            }

            this._UI_ACTION = uiAction;
        }

        #endregion Constructors

        #region Events and delegates (2)

        /// <summary>
        /// Describes a mediator action.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload action.</typeparam>
        /// <param name="payload">The submitted payload.</param>
        public delegate void MediatorAction<TPayload>(TPayload payload);

        /// <summary>
        /// Describes logic that runs an action on the UI thread.
        /// </summary>
        /// <param name="mediator">The underlying mediator.</param>
        /// <param name="action">The action that should be invoked.</param>
        public delegate void UIAction(Mediator mediator, Action action);

        #endregion Events and delegates

        #region Methods (17)

        private static void AppendExceptionsIfPossible(AggregateException ex,
                                                       ICollection<Exception> exceptions, object syncExceptions)
        {
            if (ex == null)
            {
                return;
            }

            lock (syncExceptions)
            {
                exceptions.AddRange(ex.InnerExceptions
                                      .OfType<Exception>());
            }
        }

        private void CleanupList()
        {
            var livingItems = this._ITEMS
                                  .Where(i => i.IsAlive)
                                  .ToArray();

            this._ITEMS.Clear();
            this._ITEMS.AddRange(livingItems);
        }

        private static Task CreateTask<TPayload>(IMediatorActionItem<TPayload> item, TPayload payload,
                                                 ICollection<Exception> exceptions, object syncExceptions, bool saveExceptions)
        {
            return new Task(
                () =>
                {
                    try
                    {
                        item.Invoke(payload);
                    }
                    catch (Exception ex)
                    {
                        if (saveExceptions)
                        {
                            SaveException(ex,
                                          coll: exceptions, sync: syncExceptions);
                        }
                    }
                });
        }

        private static Action CreateAction<TPayload>(IMediatorActionItem<TPayload> item, TPayload payload,
                                                     ICollection<Exception> exceptions, object syncExceptions)
        {
            return new Action(() =>
                {
                    try
                    {
                        item.Invoke(payload);
                    }
                    catch (Exception ex)
                    {
                        SaveException(ex: ex,
                                      coll: exceptions, sync: syncExceptions);
                    }
                });
        }

        private void InvokeForItemList(Action<IList<IMediatorActionItem>> action)
        {
            this.InvokeForItemList<Action<IList<IMediatorActionItem>>>((l, a) => a(l),
                                                                       actionState: action);
        }

        private void InvokeForItemList<S>(Action<IList<IMediatorActionItem>, S> action,
                                          S actionState)
        {
            lock (this._SYNC)
            {
                action(this._ITEMS,
                       actionState);
            }
        }

        private static void InvokeOnCurrentThread(Mediator mediator, Action action)
        {
            action();
        }

        private static bool NonNullFilter<TPayload>(TPayload payload)
        {
            return payload != null;
        }

        /// <summary>
        /// Publishes a payload.
        /// </summary>
        /// <typeparam name="TPayload">Type of the payload.</typeparam>
        /// <param name="payload">The payload object.</param>
        /// <returns>All occured exceptions or <see langword="null" /> if no exception was thrown.</returns>
        public AggregateException Publish<TPayload>(TPayload payload)
        {
            // get actions
            IMediatorActionItem<TPayload>[] actionsBg = null;
            IMediatorActionItem<TPayload>[] actionsBgNoWait = null;
            IMediatorActionItem<TPayload>[] actionsCurrent = null;
            IMediatorActionItem<TPayload>[] actionsUI = null;
            this.InvokeForItemList((l, s) =>
                {
                    // cleasnup "dead" items
                    s.Mediator.CleanupList();

                    actionsCurrent = l.OfType<IMediatorActionItem<TPayload>>()
                                      .Where(i => (i.Option == ThreadOption.Current) &&
                                                  ((i.Filter == null) || (i.Filter(s.Payload))))
                                      .ToArray();

                    actionsBg = l.OfType<IMediatorActionItem<TPayload>>()
                                 .Where(i => (i.Option == ThreadOption.Background) &&
                                             ((i.Filter == null) || (i.Filter(s.Payload))))
                                 .ToArray();

                    actionsBgNoWait = l.OfType<IMediatorActionItem<TPayload>>()
                                       .Where(i => (i.Option == ThreadOption.BackgroundNoWait) &&
                                                   ((i.Filter == null) || (i.Filter(s.Payload))))
                                       .ToArray();

                    actionsUI = l.OfType<IMediatorActionItem<TPayload>>()
                                 .Where(i => (i.Option == ThreadOption.UserInterface) &&
                                             ((i.Filter == null) || (i.Filter(s.Payload))))
                                 .ToArray();
                }, actionState: new
                {
                    Mediator = this,
                    Payload = payload,
                });

            var exceptions = new List<Exception>();
            var sync = new object();

            try
            {
                // create tasks for background actions ...
                var tasksBgWait = actionsBg.Select(i => CreateTask(item: i,
                                                                   payload: payload,
                                                                   exceptions: exceptions,
                                                                   syncExceptions: sync,
                                                                   saveExceptions: true))
                                           .ToArray();

                // ... and start them
                AppendExceptionsIfPossible(ex: tasksBgWait.ForAll(throwExceptions: false,
                                                                  action: ctx => ctx.Item.Start()),
                                           exceptions: exceptions,
                                           syncExceptions: sync);
                
                // start tasks for actions
                // that run in background but that method
                // does not wait for them
                AppendExceptionsIfPossible(ex: actionsBgNoWait.ForAll(throwExceptions: false,
                                                                      action: ctx => CreateTask(item: ctx.Item,
                                                                                                payload: ctx.State.Payload,
                                                                                                exceptions: ctx.State.Exceptions,
                                                                                                syncExceptions: ctx.State.Sync,
                                                                                                saveExceptions: false).Start(),
                                                                      actionState: new
                                                                      {
                                                                          Exceptions = exceptions,
                                                                          Payload = payload,
                                                                          Sync = sync,
                                                                      }),
                                           exceptions: exceptions,
                                           syncExceptions: sync);

                // actions that are running in the same thread
                AppendExceptionsIfPossible(ex: actionsCurrent.ForAll(throwExceptions: false,
                                                                     action: ctx => ctx.Item.Invoke(ctx.State.Payload),
                                                                     actionState: new
                                                                     {
                                                                         Exceptions = exceptions,
                                                                         Payload = payload,
                                                                         Sync = sync,
                                                                     }),
                                           exceptions: exceptions,
                                           syncExceptions: sync);

                // run an UI thread
                AppendExceptionsIfPossible(ex: actionsUI.ForAll(throwExceptions: false,
                                                                action: ctx =>
                                                                {
                                                                    var m = ctx.State.Mediator;

                                                                    m._UI_ACTION(m, CreateAction<TPayload>(item: ctx.Item,
                                                                                                           payload: ctx.State.Payload,
                                                                                                           exceptions: ctx.State.Exceptions,
                                                                                                           syncExceptions: ctx.State.Sync));
                                                                },
                                                                actionState: new
                                                                {
                                                                    Exceptions = exceptions,
                                                                    Mediator = this,
                                                                    Payload = payload,
                                                                    Sync = sync,
                                                                }),
                                           exceptions: exceptions,
                                           syncExceptions: sync);

                // wait for background threads
                Task.WaitAll(tasksBgWait);
            }
            catch (Exception ex)
            {
                SaveException(ex,
                              coll: exceptions, sync: sync);
            }
            finally
            {
                actionsBg = null;
                actionsBgNoWait = null;
                actionsCurrent = null;
                actionsUI = null;
            }

            AggregateException result = null;

            lock (sync)
            {
                if (exceptions.Count > 0)
                {
                    result = new AggregateException(exceptions);
                }
            }

            return result;
        }

        /// <summary>
        /// Publishes a list of payload objects.
        /// </summary>
        /// <typeparam name="TPayload">Type of the payload objects.</typeparam>
        /// <param name="payloads">The objects to publish.</param>
        /// <returns>All occured exceptions or <see langword="null" /> if no exception was thrown.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="payloads" /> is <see langword="null" />.
        /// </exception>
        public AggregateException PublishAll<TPayload>(IEnumerable<TPayload> payloads)
        {
            if (payloads == null)
            {
                throw new ArgumentNullException("payloads");
            }

            var exceptions = new List<Exception>();

            payloads.ForAll(ctx =>
                {
                    var ex = ctx.State
                                .Mediator.Publish<TPayload>(ctx.Item);

                    if (ex != null)
                    {
                        ctx.State
                           .Exceptions.AddRange(ex.InnerExceptions);
                    }
                }, actionState: new
                {
                    Exceptions = exceptions,
                    Mediator = this,
                });

            return exceptions.Count < 1 ? null : new AggregateException(exceptions);
        }
        
        /// <summary>
        /// Publishes a list of payload objects.
        /// </summary>
        /// <typeparam name="TPayload">Type of the payload objects.</typeparam>
        /// <param name="payloads">The objects to publish.</param>
        /// <returns>All occured exceptions or <see langword="null" /> if no exception was thrown.</returns>
        public AggregateException PublishAll<TPayload>(params TPayload[] payloads)
        {
            IEnumerable<TPayload> npl = payloads ?? new TPayload[] { default(TPayload) };

            return this.PublishAll<TPayload>(npl);
        }

        private static void SaveException(Exception ex, ICollection<Exception> coll, object sync)
        {
            lock (sync)
            {
                coll.Add(ex);
            }
        }

        /// <summary>
        /// Subscribes an action for a specific payload.
        /// </summary>
        /// <typeparam name="TPayload">Type of the payload object.</typeparam>
        /// <param name="action">The action to subscribe.</param>
        /// <param name="filter">
        /// The payload filter predicate. This value can be <see langword="null" /> to use NO filter.
        /// </param>
        /// <param name="option">The thread handling.</param>
        /// <param name="keepReferenceAlive">
        /// Keep reference of <paramref name="action" /> alive or handle as weak reference.
        /// </param>
        /// <returns>That instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        public Mediator Subscribe<TPayload>(MediatorAction<TPayload> action,
                                            Func<TPayload, bool> filter,
                                            ThreadOption option = ThreadOption.Current,
                                            bool keepReferenceAlive = true)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeForItemList((l, s) =>
                {
                    IMediatorActionItem newItem;
                    if (s.KeepReferenceAlive)
                    {
                        newItem = new HardReferenceActionItem<TPayload>(action: s.Action,
                                                                        option: option,
                                                                        filter: filter);
                    }
                    else
                    {
                        newItem = new WeakReferenceActionItem<TPayload>(action: s.Action,
                                                                        option: option,
                                                                        filter: filter);
                    }

                    l.Add(newItem);
                }, new
                {
                    Action = action,
                    Filter = filter,
                    KeepReferenceAlive = keepReferenceAlive,
                    Options = option,
                });

            return this;
        }
        
        /// <summary>
        /// Subscribes an action for a specific payload.
        /// </summary>
        /// <typeparam name="TPayload">Type of the payload object.</typeparam>
        /// <param name="action">The action to subscribe.</param>
        /// <param name="option">The thread handling.</param>
        /// <param name="keepReferenceAlive">
        /// Keep reference of <paramref name="action" /> alive or handle as weak reference.
        /// </param>
        /// <returns>That instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// A payload filter that ignores all payloads that are <see langword="null" />.
        /// </remarks>
        public Mediator Subscribe<TPayload>(MediatorAction<TPayload> action,
                                            ThreadOption option = ThreadOption.Current,
                                            bool keepReferenceAlive = true)
        {
            return this.Subscribe<TPayload>(action: action,
                                            option: option,
                                            keepReferenceAlive: keepReferenceAlive,
                                            filter: NonNullFilter);
        }

        /// <summary>
        /// Unsubscribes an action.
        /// </summary>
        /// <param name="action">The action to unsubscribe.</param>
        /// <returns>That instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        public Mediator Unsubscribe<TPayload>(MediatorAction<TPayload> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeForItemList((l, s) =>
                {
                    s.Mediator.CleanupList();

                    var newItems = l.Where(i => i.Equals(s.Action) == false)
                                    .ToArray();

                    l.Clear();
                    l.AddRange(newItems);
                }, actionState: new
                {
                    Action = action,
                    Mediator = this,
                });

            return this;
        }

        /// <summary>
        /// Unsubscribes all actions.
        /// </summary>
        /// <returns>That instance.</returns>
        public Mediator UnsubscribeAll()
        {
            this.InvokeForItemList(l => l.Clear());

            return this;
        }

        /// <summary>
        /// Unsubscribes all actions for a specific payload type.
        /// </summary>
        /// <typeparam name="TPayload">Payload type.</typeparam>
        /// <returns>That instance.</returns>
        public Mediator UnsubscribeAll<TPayload>()
        {
            this.InvokeForItemList((l, s) =>
                {
                    s.Mediator.CleanupList();

                    var newItems = l.Where(i => (i is IMediatorActionItem<TPayload>) == false)
                                    .ToArray();

                    l.Clear();
                    l.AddRange(newItems);
                }, actionState: new
                {
                    Mediator = this,
                });

            return this;
        }

        #endregion Methods
    }
}