// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Workflows;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that is based on a <see cref="IWorkflow" />.
    /// </summary>
    public sealed class WorkflowLogger : LoggerBase
    {
        #region Fields (2)

        private readonly WorkflowProvider _PROVIDER;
        private readonly ArgumentProvider _PROVIDER_OF_ARGUMENTS;

        #endregion Fields

        #region Constructors (7)

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <param name="argProvider">The value for the <see cref="WorkflowLogger.ProviderOfArguments" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" />, <paramref name="argProvider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider, ArgumentProvider argProvider, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (argProvider == null)
            {
                throw new ArgumentNullException("argProvider");
            }

            this._PROVIDER = provider;
            this._PROVIDER_OF_ARGUMENTS = argProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider, bool isSynchronized, object sync)
            : this(provider: provider,
                   argProvider: GetEmptyArguments,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <param name="argProvider">The value for the <see cref="WorkflowLogger.ProviderOfArguments" /> property.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" />, <paramref name="argProvider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider, ArgumentProvider argProvider, object sync)
            : this(provider: provider,
                   argProvider: argProvider,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <param name="argProvider">The value for the <see cref="WorkflowLogger.ProviderOfArguments" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="argProvider" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider, ArgumentProvider argProvider, bool isSynchronized)
            : this(provider: provider,
                   argProvider: argProvider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider, object sync)
            : this(provider: provider,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider, bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="WorkflowLogger.Provider" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public WorkflowLogger(WorkflowProvider provider)
            : this(provider: provider,
                   isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Properties (2)

        /// <summary>
        /// Gets the underlying workflow provider.
        /// </summary>
        public WorkflowProvider Provider
        {
            get { return this._PROVIDER; }
        }

        /// <summary>
        /// Gets function that provides the the arguments for each function of <see cref="WorkflowLogger.Provider" />.
        /// </summary>
        public ArgumentProvider ProviderOfArguments
        {
            get { return this._PROVIDER_OF_ARGUMENTS; }
        }

        #endregion Properties

        #region Delegates and Events (2)

        /// <summary>
        /// Describes a function or method that provides the instance of <see cref="WorkflowLogger.ProviderOfArguments" />.
        /// </summary>
        /// <param name="logger">The underlying logger instance.</param>
        /// <returns>The argument to use.</returns>
        public delegate IEnumerable ArgumentProvider(WorkflowLogger logger);

        /// <summary>
        /// Provides the <see cref="IWorkflow" /> for an instance of that class.
        /// </summary>
        /// <param name="logger">The underlying logger.</param>
        /// <returns>The provided workflow.</returns>
        public delegate IWorkflow WorkflowProvider(WorkflowLogger logger);

        #endregion Delegates and Events

        #region Methods (9)

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> is <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow)
        {
            return Create(workflow: workflow,
                          isSynchronized: false);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> is <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, bool isSynchronized)
        {
            return Create(workflow: workflow,
                          isSynchronized: isSynchronized,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <param name="args">The arguments to use for each invokation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, bool isSynchronized, object sync, params object[] args)
        {
            return Create(workflow: workflow,
                          args: (IEnumerable)args,
                          isSynchronized: isSynchronized,
                          sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <param name="args">The arguments to use for each invokation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> is <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, IEnumerable args)
        {
            return Create(workflow: workflow,
                          args: args,
                          isSynchronized: false);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <param name="args">The arguments to use for each invokation.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, IEnumerable args, object sync)
        {
            return Create(workflow: workflow,
                          args: args,
                          isSynchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <param name="args">The arguments to use for each invokation.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> is <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, IEnumerable args, bool isSynchronized)
        {
            return Create(workflow: workflow,
                          args: args,
                          isSynchronized: isSynchronized,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The workflow to use.</param>
        /// <param name="args">The arguments to use for each invokation.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, IEnumerable args, bool isSynchronized, object sync)
        {
            if (workflow == null)
            {
                throw new ArgumentNullException("workflow");
            }

            return new WorkflowLogger(provider: (l) => workflow,
                                      argProvider: (l) => args,
                                      isSynchronized: isSynchronized,
                                      sync: sync);
        }

        private static object[] GetEmptyArguments(WorkflowLogger logger)
        {
            return null;
        }

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            var workflow = this._PROVIDER(this);
            if (workflow == null)
            {
                return;
            }

            // use instance of 'msg'
            // as first argument for the workflow
            var basicArgs = new object[] { msg };

            // additional arguments from 'ProviderOfArguments' property
            var additionalArgs = this._PROVIDER_OF_ARGUMENTS(this)
                                     .AsSequence<object>();

            // concat both
            IEnumerable<object> allArgs = basicArgs;
            if (additionalArgs != null)
            {
                allArgs = allArgs.Concat(additionalArgs);
            }

            workflow.ForEach(action: ctx =>
                                 {
                                     var res = ctx.Item(ctx.State.Arguments);

                                     if (res.HasBeenCanceled)
                                     {
                                         ctx.Cancel = true;
                                     }
                                 },
                             actionState: new
                                 {
                                     Arguments = allArgs.AsArray(),
                                 });
        }

        #endregion Methods
    }
}