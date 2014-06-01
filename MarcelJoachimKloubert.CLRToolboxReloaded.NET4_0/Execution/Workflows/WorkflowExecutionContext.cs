// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    #region CLASS: WorkflowExecutionContext

    /// <summary>
    /// Simple implementation of <see cref="IWorkflowExecutionContext" /> interface.
    /// </summary>
    public class WorkflowExecutionContext : ObjectBase, IWorkflowExecutionContext
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public WorkflowExecutionContext(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public WorkflowExecutionContext(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public WorkflowExecutionContext(object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public WorkflowExecutionContext()
            : base(isSynchronized: true)
        {
        }

        #endregion Constrcutors (4)

        #region Properties (15)

        /// <inheriteddoc />
        public bool Cancel
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool ContinueOnError
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IReadOnlyList<object> ExecutionArguments
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IDictionary<string, object> ExecutionVars
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool HasBeenCanceled
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public long Index
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool IsFirst
        {
            get { return this.Index == 0; }
        }

        /// <inheriteddoc />
        public bool IsLast
        {
            get { return this.Next == null; }
        }

        /// <inheriteddoc />
        public virtual WorkflowAction Next
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IDictionary<string, object> NextVars
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IReadOnlyDictionary<string, object> PreviousVars
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public object Result
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool ThrowErrors
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IWorkflow Workflow
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IDictionary<string, object> WorkflowVars
        {
            get;
            set;
        }

        #endregion CLASS: WorkflowExecutionContext

        #region Methods (22)

        // Public Methods (22) 

        /// <inheriteddoc />
        public T GetExecutionArgument<T>(int index)
        {
            T result;
            if (this.TryGetExecutionArgument<T>(index, out result) == false)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetExecutionVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetExecutionVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetNextVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetNextVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetPreviousVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetPreviousVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <inheriteddoc />
        public T GetResult<T>()
        {
            return GlobalConverter.Current
                                  .ChangeType<T>(this.Result);
        }

        /// <inheriteddoc />
        public W GetWorkflow<W>() where W : global::MarcelJoachimKloubert.CLRToolbox.Execution.Workflows.IWorkflow
        {
            return GlobalConverter.Current
                                  .ChangeType<W>(this.Workflow);
        }

        /// <inheriteddoc />
        public T GetWorkflowVar<T>(IEnumerable<char> name)
        {
            T result;
            if (this.TryGetWorkflowVar<T>(name, out result) == false)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// Parses a char sequence to use it as variable name.
        /// </summary>
        /// <param name="name">The char sequence.</param>
        /// <returns>The parsed char sequence.</returns>
        public static string ParseVarName(IEnumerable<char> name)
        {
            return name.AsString() ?? string.Empty;
        }

        /// <inheriteddoc />
        public bool TryGetExecutionArgument<T>(int index, out T value)
        {
            return this.TryGetExecutionArgument<T>(index, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetExecutionArgument<T>(int index, out T value, T defaultValue)
        {
            return this.TryGetExecutionArgument<T>(index, out value,
                                                   delegate(int i)
                                                   {
                                                       return defaultValue;
                                                   });
        }

        /// <inheriteddoc />
        public bool TryGetExecutionArgument<T>(int index, out T value, Func<int, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IReadOnlyList<object> list = this.ExecutionArguments;
                if (list != null)
                {
                    if ((index > -1) && (index < list.Count))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(list[index]);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(index);
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetExecutionVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetExecutionVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetExecutionVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetExecutionVar<T>(name, out value,
                                              defaultValueProvider: (varName) => defaultValue);
        }

        /// <inheriteddoc />
        public bool TryGetExecutionVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IDictionary<string, object> dict = this.ExecutionVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(name.AsString());
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetNextVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetNextVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetNextVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetNextVar<T>(name, out value,
                                         defaultValueProvider: (varName) => defaultValue);
        }

        /// <inheriteddoc />
        public bool TryGetNextVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IDictionary<string, object> dict = this.NextVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(name.AsString());
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetPreviousVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetPreviousVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetPreviousVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetPreviousVar<T>(name, out value,
                                             defaultValueProvider: (varName) => defaultValue);
        }

        /// <inheriteddoc />
        public bool TryGetPreviousVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                IReadOnlyDictionary<string, object> dict = this.PreviousVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(name.AsString());
            return false;
        }

        /// <inheriteddoc />
        public bool TryGetWorkflowVar<T>(IEnumerable<char> name, out T value)
        {
            return this.TryGetWorkflowVar<T>(name, out value, default(T));
        }

        /// <inheriteddoc />
        public bool TryGetWorkflowVar<T>(IEnumerable<char> name, out T value, T defaultValue)
        {
            return this.TryGetWorkflowVar<T>(name, out value,
                                             defaultValueProvider: (varName) => defaultValue);
        }

        /// <inheriteddoc />
        public bool TryGetWorkflowVar<T>(IEnumerable<char> name, out T value, Func<string, T> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.Workflow.SyncRoot)
            {
                IDictionary<string, object> dict = this.WorkflowVars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(ParseVarName(name), out foundValue))
                    {
                        value = GlobalConverter.Current
                                               .ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = defaultValueProvider(name.AsString());
            return false;
        }

        #endregion Methods
    }

    #endregion

    #region CLASS: WorkflowExecutionContext<S>

    /// <summary>
    /// Simple implementation of <see cref="IWorkflowExecutionContext{S}" /> interface.
    /// </summary>
    /// <typeparam name="TState">Type of the underlying state objects.</typeparam>
    public sealed class WorkflowExecutionContext<TState> : WorkflowExecutionContext, IWorkflowExecutionContext<TState>
    {
        #region Fields (1)

        private WorkflowAction<TState> _nextWithState;

        #endregion Fields

        #region Constrcutors (4)

        /// <inheriteddoc />
        public WorkflowExecutionContext(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public WorkflowExecutionContext(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public WorkflowExecutionContext(object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public WorkflowExecutionContext()
            : base(isSynchronized: true)
        {
        }

        #endregion Constrcutors (5)

        #region Properties (4)

        /// <inheriteddoc />
        public override WorkflowAction Next
        {
            get { return base.Next; }

            set
            {
                base.Next = value;
                this._nextWithState = value != null ? new WorkflowAction<TState>((ctx) => value(ctx)) : null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IWorkflowExecutionContext{S}.Next" />
        public WorkflowAction<TState> NextWithState
        {
            get { return this._nextWithState; }

            set
            {
                this._nextWithState = value;
                base.Next = ToActionWithoutState(value);
            }
        }

        /// <inheriteddoc />
        public TState State
        {
            get;
            set;
        }

        WorkflowAction<TState> IWorkflowExecutionContext<TState>.Next
        {
            get { return this.NextWithState; }

            set { this.NextWithState = value; }
        }

        #endregion Properties

        #region Methods (3)

        /// <summary>
        /// Copies all property values that can be changed to a target <see cref="IWorkflowExecutionContext" /> object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target" /> is <see langword="null" />.
        /// </exception>
        public void CopyChanges(IWorkflowExecutionContext target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            target.Cancel = this.Cancel;
            target.ContinueOnError = this.ContinueOnError;
            target.Next = ToActionWithoutState(this.NextWithState);
            target.Result = this.Result;
            target.ThrowErrors = this.ThrowErrors;
        }

        /// <summary>
        /// Clones an <see cref="IWorkflowExecutionContext" /> object with a state object.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="state">The state object to set.</param>
        /// <returns>The clone of <paramref name="original" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="original" /> is <see langword="null" />.
        /// </exception>
        public static WorkflowExecutionContext<TState> Clone(IWorkflowExecutionContext original, TState state)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            var result = new WorkflowExecutionContext<TState>(isSynchronized: original.IsSynchronized,
                                                              sync: original.SyncRoot);
            result.Cancel = original.Cancel;
            result.ContinueOnError = original.Cancel;
            result.ExecutionArguments = original.ExecutionArguments;
            result.ExecutionVars = original.ExecutionVars;
            result.HasBeenCanceled = original.HasBeenCanceled;
            result.Index = original.Index;
            result.Next = original.Next;
            result.NextVars = original.NextVars;
            result.PreviousVars = original.PreviousVars;
            result.Result = original.Result;
            result.State = state;
            result.Tag = original.Tag;
            result.ThrowErrors = original.ThrowErrors;
            result.Workflow = original.Workflow;
            result.WorkflowVars = original.WorkflowVars;

            return result;
        }

        private static WorkflowAction ToActionWithoutState(WorkflowAction<TState> action)
        {
            if (action == null)
            {
                return null;
            }

            return ctx =>
                {
                    var clonedCtx = ctx as IWorkflowExecutionContext<TState>;
                    if (clonedCtx == null)
                    {
                        // create "wrapper"

                        clonedCtx = Clone(original: ctx,
                                          state: default(TState));
                    }

                    action(clonedCtx);

                    var clonedCtx2 = clonedCtx as WorkflowExecutionContext<TState>;
                    if (clonedCtx2 != null)
                    {
                        clonedCtx2.CopyChanges(ctx);
                    }
                };
        }

        #endregion
    }

    #endregion
}