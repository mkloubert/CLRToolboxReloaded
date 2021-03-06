﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    /// <summary>
    /// An extension of <see cref="AttributeWorkflowBase" /> that uses a delegate for the workflow start logic.
    /// </summary>
    public sealed class DelegateWorkflow : AttributeWorkflowBase
    {
        #region Fields (1)

        private readonly WorkflowActionProvider _PROVIDER;

        #endregion Fields (1)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateWorkflow.Provider" /> property.</param>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public DelegateWorkflow(WorkflowActionProvider provider, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateWorkflow.Provider" /> property.</param>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateWorkflow(WorkflowActionProvider provider, bool isSynchronized)
            : this(provider: provider,
                   sync: new object(),
                   isSynchronized: isSynchronized)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateWorkflow.Provider" /> property.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        /// <remarks>Object will NOT work thread safe.</remarks>
        public DelegateWorkflow(WorkflowActionProvider provider, object sync)
            : this(provider: provider,
                   sync: sync,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="DelegateWorkflow.Provider" /> property.</param>
        /// <remarks>Object will NOT work thread safe.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DelegateWorkflow(WorkflowActionProvider provider)
            : this(provider: provider,
                   isSynchronized: false)
        {
        }

        #endregion CLASS: DelegateWorkflow

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function or method that provides the start action for an instance of that class.
        /// </summary>
        /// <param name="workflow">The underlying instance.</param>
        /// <param name="contract">The contract name.</param>
        /// <returns>The inital workflow action.</returns>
        public delegate WorkflowAction WorkflowActionProvider(DelegateWorkflow workflow, string contract);

        #endregion Events and delegates (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public WorkflowActionProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (1)

        #region Methods (5)

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="action">The start action to use.</param>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateWorkflow Create(WorkflowAction action, bool isSynchronized, object sync)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return new DelegateWorkflow(provider: (wf, c) => action,
                                        isSynchronized: isSynchronized,
                                        sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="action">The start action to use.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateWorkflow Create(WorkflowAction action, object sync)
        {
            return Create(action: action,
                          isSynchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="action">The start action to use.</param>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        public static DelegateWorkflow Create(WorkflowAction action, bool isSynchronized)
        {
            return Create(action: action,
                          isSynchronized: isSynchronized,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateWorkflow" /> class.
        /// </summary>
        /// <param name="action">The start action to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        public static DelegateWorkflow Create(WorkflowAction action)
        {
            return Create(action: action,
                          isSynchronized: false);
        }

        [WorkflowStart]
        private void StartWorkflow(IWorkflowExecutionContext ctx)
        {
            var action = this._PROVIDER(this, this.ContractName);
            if (action != null)
            {
                action(ctx);
            }
        }

        #endregion Methods (5)
    }
}