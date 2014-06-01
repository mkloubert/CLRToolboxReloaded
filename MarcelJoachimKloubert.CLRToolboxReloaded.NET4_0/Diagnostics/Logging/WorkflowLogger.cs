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

        private readonly ArgumentProvider _PROVIDER_OF_ARGUMENTS;
        private readonly IWorkflow _WORKFLOW;

        #endregion Fields

        #region Constructors (7)

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="argProvider">The value for the <see cref="WorkflowLogger.ProviderOfArguments" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" />, <paramref name="argProvider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow, ArgumentProvider argProvider, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (workflow == null)
            {
                throw new ArgumentNullException("workflow");
            }

            if (argProvider == null)
            {
                throw new ArgumentNullException("argProvider");
            }

            this._WORKFLOW = workflow;
            this._PROVIDER_OF_ARGUMENTS = argProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow, bool isSynchronized, object sync)
            : this(workflow: workflow,
                   argProvider: GetEmptyArguments,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="argProvider">The value for the <see cref="WorkflowLogger.ProviderOfArguments" /> property.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" />, <paramref name="argProvider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow, ArgumentProvider argProvider, object sync)
            : this(workflow: workflow,
                   argProvider: argProvider,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="argProvider">The value for the <see cref="WorkflowLogger.ProviderOfArguments" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="argProvider" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow, ArgumentProvider argProvider, bool isSynchronized)
            : this(workflow: workflow,
                   argProvider: argProvider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow, object sync)
            : this(workflow: workflow,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> is <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow, bool isSynchronized)
            : this(workflow: workflow,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> is <see langword="null" />.
        /// </exception>
        public WorkflowLogger(IWorkflow workflow)
            : this(workflow: workflow,
                   isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Properties (2)

        /// <summary>
        /// Gets function that provides the the arguments for each function of <see cref="WorkflowLogger.Workflow" />.
        /// </summary>
        public ArgumentProvider ProviderOfArguments
        {
            get { return this._PROVIDER_OF_ARGUMENTS; }
        }

        /// <summary>
        /// Gets the underlying workflow.
        /// </summary>
        public IWorkflow Workflow
        {
            get { return this._WORKFLOW; }
        }

        #endregion Properties

        #region Delegates and Events (1)

        // Delegates (1) 

        /// <summary>
        /// Describes a function or method that provides the instance of <see cref="WorkflowLogger.Workflow" />.
        /// </summary>
        /// <param name="logger">The underlying logger instance.</param>
        /// <returns>The argument to use.</returns>
        public delegate IEnumerable ArgumentProvider(WorkflowLogger logger);

        #endregion Delegates and Events

        #region Methods (5)

        // Public Methods (3) 

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowLogger" /> class.
        /// </summary>
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
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
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
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
        /// <param name="workflow">The value for the <see cref="WorkflowLogger.Workflow" /> property.</param>
        /// <param name="args">The arguments to use for each invokation.</param>
        /// <param name="isSynchronized">Object is thread safe or not.</param>
        /// <param name="sync">The unique object for sync operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflow" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static WorkflowLogger Create(IWorkflow workflow, IEnumerable args, bool isSynchronized, object sync)
        {
            return new WorkflowLogger(workflow: workflow,
                                      argProvider: (l) => args,
                                      isSynchronized: isSynchronized,
                                      sync: sync);
        }

        // Protected Methods (1) 

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
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

            this._WORKFLOW
                .ForEach(ctx =>
                {
                    var res = ctx.Item(ctx.State.Arguments);

                    if (res.HasBeenCanceled)
                    {
                        ctx.Cancel = true;
                    }
                }, actionState: new
                {
                    Arguments = allArgs.ToArray(),
                });
        }

        // Private Methods (1)

        private static object[] GetEmptyArguments(WorkflowLogger logger)
        {
            return new object[0];
        }

        #endregion Methods
    }
}