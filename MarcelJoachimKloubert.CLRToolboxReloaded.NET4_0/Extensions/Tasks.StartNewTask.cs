// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !PORTABLE45
#define CAN_HANDLE_THREADS
#endif

using MarcelJoachimKloubert.CLRToolbox.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Actions (9)

        /// <summary>
        /// Starts a new task from a task factory.
        /// </summary>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask(this TaskFactory factory,
                                        Action<ITaskExecutionContext> action,
                                        TaskScheduler scheduler = null,
                                        TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<object>(factory: factory,
                                        action: action, actionState: null,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task from a task factory.
        /// </summary>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask(this TaskFactory factory,
                                        Action<ITaskExecutionContext> action,
                                        out CancellationTokenSource tokenSrc,
                                        TaskScheduler scheduler = null,
                                        TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<object>(factory: factory,
                                        action: action, actionState: null,
                                        tokenSrc: out tokenSrc,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task from a task factory.
        /// </summary>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="action" /> and/or <paramref name="tokenSrc" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask(this TaskFactory factory,
                                        Action<ITaskExecutionContext> action,
                                        CancellationTokenSource tokenSrc,
                                        TaskScheduler scheduler = null,
                                        TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<object>(factory: factory,
                                        action: action, actionState: null,
                                        tokenSrc: tokenSrc,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The state object for <paramref name="action" />.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask<TState>(this TaskFactory factory,
                                                Action<ITaskExecutionContext<TState>> action, TState actionState,
                                                TaskScheduler scheduler = null,
                                                TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState>(factory: factory,
                                        action: action, actionStateFactory: () => actionState,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateFactory">The function that returns the state object for <paramref name="action" />.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="action" /> and/or <paramref name="actionStateFactory" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask<TState>(this TaskFactory factory,
                                                Action<ITaskExecutionContext<TState>> action, Func<TState> actionStateFactory,
                                                TaskScheduler scheduler = null,
                                                TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState>(factory: factory,
                                        action: action, actionStateFactory: actionStateFactory,
                                        tokenSrc: new CancellationTokenSource(),
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The state object for <paramref name="action" />.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask<TState>(this TaskFactory factory,
                                                Action<ITaskExecutionContext<TState>> action, TState actionState,
                                                out CancellationTokenSource tokenSrc,
                                                TaskScheduler scheduler = null,
                                                TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState>(factory: factory,
                                        action: action, actionStateFactory: () => actionState,
                                        tokenSrc: out tokenSrc,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateFactory">The function that returns the state object for <paramref name="action" />.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="action" /> and/or <paramref name="actionStateFactory" /> are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask<TState>(this TaskFactory factory,
                                                Action<ITaskExecutionContext<TState>> action, Func<TState> actionStateFactory,
                                                out CancellationTokenSource tokenSrc,
                                                TaskScheduler scheduler = null,
                                                TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            tokenSrc = new CancellationTokenSource();
            return StartNewTask<TState>(factory: factory,
                                        action: action, actionStateFactory: actionStateFactory,
                                        tokenSrc: tokenSrc,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The function that returns the state object for <paramref name="action" />.</param>
        /// <param name="tokenSrc">The object that manages the cancellation of the created task.</param>
        /// <param name="scheduler">The custom scheduler to use.</param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="action" /> and/or <paramref name="tokenSrc" />
        /// are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask<TState>(this TaskFactory factory,
                                                Action<ITaskExecutionContext<TState>> action, TState actionState,
                                                CancellationTokenSource tokenSrc,
                                                TaskScheduler scheduler = null,
                                                TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState>(factory: factory,
                                        action: action, actionStateFactory: () => actionState,
                                        tokenSrc: tokenSrc,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateFactory">The function that returns the state object for <paramref name="action" />.</param>
        /// <param name="tokenSrc">The object that manages the cancellation of the created task.</param>
        /// <param name="scheduler">The custom scheduler to use.</param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="action" />, <paramref name="actionStateFactory" /> and/or <paramref name="tokenSrc" />
        /// are <see langword="null" />.
        /// </exception>
        public static Task StartNewTask<TState>(this TaskFactory factory,
                                                Action<ITaskExecutionContext<TState>> action, Func<TState> actionStateFactory,
                                                CancellationTokenSource tokenSrc,
                                                TaskScheduler scheduler = null,
                                                TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return StartNewTask(factory: factory,
                                func: (ctx) =>
                                    {
                                        var ctx2 = new TaskExecutionContext<TState>()
                                            {
                                                CancellationToken = ctx.CancellationToken,
                                                Id = ctx.Id,
                                                State = ctx.State.StateFactory(),
                                            };

                                        ctx.State.Action(ctx2);
                                        return (object)null;
                                    },
                                funcState: new
                                    {
                                        Action = action,
                                        StateFactory = actionStateFactory,
                                    },
                                tokenSrc: tokenSrc,
                                scheduler: scheduler,
                                creationOptions: creationOptions);
        }

        #endregion Actions (9)

        #region Funcs (9)

        /// <summary>
        /// Starts a new task from a task factory.
        /// </summary>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TResult>(this TaskFactory factory,
                                                          Func<ITaskExecutionContext, TResult> func,
                                                          TaskScheduler scheduler = null,
                                                          TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<object, TResult>(factory: factory,
                                        func: func, funcState: null,
                                        scheduler: scheduler,
                                        creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task from a task factory.
        /// </summary>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TResult>(this TaskFactory factory,
                                                          Func<ITaskExecutionContext, TResult> func,
                                                          out CancellationTokenSource tokenSrc,
                                                          TaskScheduler scheduler = null,
                                                          TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<object, TResult>(factory: factory,
                                                 func: func, funcState: null,
                                                 tokenSrc: out tokenSrc,
                                                 scheduler: scheduler,
                                                 creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task from a task factory.
        /// </summary>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="func" /> and/or <paramref name="tokenSrc" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TResult>(this TaskFactory factory,
                                                          Func<ITaskExecutionContext, TResult> func,
                                                          CancellationTokenSource tokenSrc,
                                                          TaskScheduler scheduler = null,
                                                          TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<object, TResult>(factory: factory,
                                                func: func, funcState: null,
                                                tokenSrc: tokenSrc,
                                                scheduler: scheduler,
                                                creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The state object for <paramref name="func" />.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TState, TResult>(this TaskFactory factory,
                                                                  Func<ITaskExecutionContext<TState>, TResult> func, TState funcState,
                                                                  TaskScheduler scheduler = null,
                                                                  TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState, TResult>(factory: factory,
                                                 func: func, funcStateFactory: () => funcState,
                                                 scheduler: scheduler,
                                                 creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateFactory">The function that returns the state object for <paramref name="func" />.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="func" /> and/or <paramref name="funcStateFactory" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TState, TResult>(this TaskFactory factory,
                                                                  Func<ITaskExecutionContext<TState>, TResult> func, Func<TState> funcStateFactory,
                                                                  TaskScheduler scheduler = null,
                                                                  TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState, TResult>(factory: factory,
                                                 func: func, funcStateFactory: funcStateFactory,
                                                 tokenSrc: new CancellationTokenSource(),
                                                 scheduler: scheduler,
                                                 creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The state object for <paramref name="func" />.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TState, TResult>(this TaskFactory factory,
                                                                  Func<ITaskExecutionContext<TState>, TResult> func, TState funcState,
                                                                  out CancellationTokenSource tokenSrc,
                                                                  TaskScheduler scheduler = null,
                                                                  TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState, TResult>(factory: factory,
                                                 func: func, funcStateFactory: () => funcState,
                                                 tokenSrc: out tokenSrc,
                                                 scheduler: scheduler,
                                                 creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateFactory">The function that returns the state object for <paramref name="func" />.</param>
        /// <param name="tokenSrc">The variable where to write the object that manages the cancellation of the created task to.</param>
        /// <param name="scheduler">
        /// The custom scheduler to use. If <see langword="null" /> <see cref="TaskScheduler.Current" /> is used.
        /// </param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="func" /> and/or <paramref name="funcStateFactory" /> are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TState, TResult>(this TaskFactory factory,
                                                                  Func<ITaskExecutionContext<TState>, TResult> func, Func<TState> funcStateFactory,
                                                                  out CancellationTokenSource tokenSrc,
                                                                  TaskScheduler scheduler = null,
                                                                  TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            tokenSrc = new CancellationTokenSource();
            return StartNewTask<TState, TResult>(factory: factory,
                                                 func: func, funcStateFactory: funcStateFactory,
                                                 tokenSrc: tokenSrc,
                                                 scheduler: scheduler,
                                                 creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The function that returns the state object for <paramref name="func" />.</param>
        /// <param name="tokenSrc">The object that manages the cancellation of the created task.</param>
        /// <param name="scheduler">The custom scheduler to use.</param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="func" /> and/or <paramref name="tokenSrc" />
        /// are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TState, TResult>(this TaskFactory factory,
                                                                  Func<ITaskExecutionContext<TState>, TResult> func, TState funcState,
                                                                  CancellationTokenSource tokenSrc,
                                                                  TaskScheduler scheduler = null,
                                                                  TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            return StartNewTask<TState, TResult>(factory: factory,
                                                 func: func, funcStateFactory: () => funcState,
                                                 tokenSrc: tokenSrc,
                                                 scheduler: scheduler,
                                                 creationOptions: creationOptions);
        }

        /// <summary>
        /// Starts a new task with a state object from a task factory.
        /// </summary>
        /// <typeparam name="TState">Type of the state object.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="factory">The underlying factory.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateFactory">The function that returns the state object for <paramref name="func" />.</param>
        /// <param name="tokenSrc">The object that manages the cancellation of the created task.</param>
        /// <param name="scheduler">The custom scheduler to use.</param>
        /// <param name="creationOptions">The options for the task creation.</param>
        /// <returns>The created task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory" />, <paramref name="func" />, <paramref name="funcStateFactory" /> and/or <paramref name="tokenSrc" />
        /// are <see langword="null" />.
        /// </exception>
        public static Task<TResult> StartNewTask<TState, TResult>(this TaskFactory factory,
                                                                  Func<ITaskExecutionContext<TState>, TResult> func, Func<TState> funcStateFactory,
                                                                  CancellationTokenSource tokenSrc,
                                                                  TaskScheduler scheduler = null,
                                                                  TaskCreationOptions creationOptions = TaskCreationOptions.None)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateFactory == null)
            {
                throw new ArgumentNullException("funcStateFactory");
            }

            if (tokenSrc == null)
            {
                throw new ArgumentNullException("tokenSrc");
            }

            return factory.StartNew<TResult>(function: StartNewTask_Func<TState, TResult>,
                                             scheduler: scheduler ?? TaskScheduler.Current,
                                             cancellationToken: tokenSrc.Token,
                                             creationOptions: creationOptions,
                                             state: Tuple.Create(func, funcStateFactory,
                                                                 tokenSrc));
        }

        #endregion Funcs (9)

        #region Methods (1)

        private static TResult StartNewTask_Func<TState, TResult>(object state)
        {
            var tpl = (Tuple<Func<ITaskExecutionContext<TState>, TResult>, Func<TState>, CancellationTokenSource>)state;

            var action = tpl.Item1;
            var stateFactory = tpl.Item2;
            var tokenSrc = tpl.Item3;

            var ctx = new TaskExecutionContext<TState>()
                {
                    CancellationToken = tokenSrc.Token,
                    Id = Task.CurrentId,
                    RethrowErrors = null,
                    State = stateFactory(),
                };

#if CAN_HANDLE_THREADS
            ctx.Thread = global::System.Threading.Thread.CurrentThread;
#endif

            try
            {
                return action(ctx);
            }
            catch (Exception ex)
            {
                if (ctx.OnError != null)
                {
                    if (ctx.RethrowErrors.HasValue == false)
                    {
                        ctx.RethrowErrors = false;
                    }

                    ctx.OnError(ctx, ex);
                }
                else
                {
                    if (ctx.RethrowErrors.HasValue == false)
                    {
                        ctx.RethrowErrors = true;
                    }
                }

                if (IsTrue(ctx.RethrowErrors))
                {
                    throw ex;
                }
            }
            finally
            {
                if (ctx.OnCompleted != null)
                {
                    ctx.OnCompleted(ctx);
                }
            }

            return default(TResult);
        }

        #endregion Methods (1)
    }
}