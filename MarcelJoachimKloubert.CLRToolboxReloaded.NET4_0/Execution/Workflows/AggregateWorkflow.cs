// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    /// <summary>
    /// A workflow that manages a list of workflows that are called as flatten steps.
    /// </summary>
    public class AggregateWorkflow : WorkflowBase
    {
        #region Fields (1)

        private readonly WorkflowProvider _PROVIDER;

        #endregion Fields (1)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateWorkflow.Provider" /> property.</param>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public AggregateWorkflow(WorkflowProvider provider, bool isSynchronized, object sync)
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
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateWorkflow.Provider" /> property.</param>
        /// <param name="isSynchronized">Instance should work thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AggregateWorkflow(WorkflowProvider provider, bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateWorkflow.Provider" /> property.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        /// <remarks>Object will NOT work thread safe.</remarks>
        public AggregateWorkflow(WorkflowProvider provider, object sync)
            : this(provider: provider,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="provider">The value for the <see cref="AggregateWorkflow.Provider" /> property.</param>
        /// <remarks>Object will NOT work thread safe.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AggregateWorkflow(WorkflowProvider provider)
            : this(provider: provider,
                   isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function / methods that provides the child workflows for an instance that class.
        /// </summary>
        /// <param name="workflow">The underlying instance.</param>
        /// <returns>The workflows for that class.</returns>
        public delegate IEnumerable<IWorkflow> WorkflowProvider(AggregateWorkflow workflow);

        #endregion Events and delegates (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying provider.
        /// </summary>
        public WorkflowProvider Provider
        {
            get { return this._PROVIDER; }
        }

        #endregion Properties (1)

        #region Methods (10)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="workflows">The workflows to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(IEnumerable<IWorkflow> workflows, bool isSynchronized, object sync)
        {
            if (workflows == null)
            {
                throw new ArgumentNullException("workflows");
            }

            return new AggregateWorkflow((wf) => workflows,
                                         isSynchronized: isSynchronized,
                                         sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="workflows">The workflows to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(bool isSynchronized, object sync, params IWorkflow[] workflows)
        {
            return Create(workflows: (IEnumerable<IWorkflow>)workflows,
                          isSynchronized: isSynchronized,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="workflows">The workflows to use.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(IEnumerable<IWorkflow> workflows, object sync)
        {
            return Create(workflows,
                          isSynchronized: false,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <param name="workflows">The workflows to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> and/or <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(object sync, params IWorkflow[] workflows)
        {
            return Create(workflows: (IEnumerable<IWorkflow>)workflows,
                          sync: sync);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="workflows">The workflows to use.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(IEnumerable<IWorkflow> workflows, bool isSynchronized)
        {
            return Create(workflows,
                          isSynchronized: false,
                          sync: new object());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="workflows">The workflows to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(bool isSynchronized, params IWorkflow[] workflows)
        {
            return Create(workflows: (IEnumerable<IWorkflow>)workflows,
                          isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="workflows">The workflows to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(IEnumerable<IWorkflow> workflows)
        {
            return Create(workflows,
                          isSynchronized: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateWorkflow" /> class.
        /// </summary>
        /// <param name="workflows">The workflows to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="workflows" /> is <see langword="null" />.
        /// </exception>
        public static AggregateWorkflow Create(params IWorkflow[] workflows)
        {
            return Create(workflows: (IEnumerable<IWorkflow>)workflows);
        }

        /// <summary>
        /// Returns a normalized list of workflows that are provides by <see cref="AggregateWorkflow.Provider" /> property.
        /// </summary>
        /// <returns>The list of workflows.</returns>
        public IEnumerable<IWorkflow> GetWorkflows()
        {
            return (this._PROVIDER(this) ?? Enumerable.Empty<IWorkflow>()).Where(wf => wf != null);
        }

        /// <inheriteddoc />
        protected override IEnumerable<WorkflowFunc> GetFunctionIterator()
        {
            using (var e1 = this.GetWorkflows().GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    var wf = e1.Current;

                    using (var e2 = wf.GetEnumerator())
                    {
                        while (e2.MoveNext())
                        {
                            var func = e2.Current;
                            if (func != null)
                            {
                                yield return func;
                            }
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}