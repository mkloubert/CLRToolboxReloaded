// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    /// <summary>
    /// A basic workflow.
    /// </summary>
    public abstract class WorkflowBase : ObjectBase, IWorkflow
    {
        #region Fields (3)

        private readonly Func<object[], object> _EXECUTE_FUNC;
        private readonly Func<IEnumerator<WorkflowFunc>> _GET_ENUMERATOR_FUNC;
        private readonly IDictionary<string, object> _VARS;

        #endregion Fields (3)

        #region Constructors (4)

        /// <inheriteddoc />
        public WorkflowBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._EXECUTE_FUNC = this.Execute_ThreadSafe;
                this._GET_ENUMERATOR_FUNC = this.GetEnumerator_ThreadSafe;
            }
            else
            {
                this._EXECUTE_FUNC = this.Execute_NonThreadSafe;
                this._GET_ENUMERATOR_FUNC = this.GetEnumerator_NonThreadSafe;
            }

            this._VARS = this.CreateVarStorage() ?? new Dictionary<string, object>();
        }

        /// <inheriteddoc />
        public WorkflowBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        public WorkflowBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public WorkflowBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors (4)

        #region Properties (3)

        /// <summary>
        /// Gets the converter to use to cast/convert objects.
        /// </summary>
        public virtual IConverter Converter
        {
            get { return GlobalConverter.Current; }
        }

        /// <summary>
        /// Gets or sets a value of the variables of that workflow.
        /// </summary>
        /// <param name="name">The name of the var.</param>
        /// <returns>The value of the var.</returns>
        /// <remarks>
        /// The variable names are NOT case sensitive.
        /// </remarks>
        public object this[string name]
        {
            get { return this.Vars[WorkflowExecutionContext.ParseVarName(name)]; }

            set { this.Vars[WorkflowExecutionContext.ParseVarName(name)] = value; }
        }

        /// <summary>
        /// Gets the variables of that workflow as dictionary.
        /// </summary>
        /// <remarks>
        /// The variable names are NOT case sensitive.
        /// </remarks>
        public IDictionary<string, object> Vars
        {
            get { return this._VARS; }
        }

        #endregion Properties (3)

        #region Methods (15)

        /// <summary>
        /// Converts / casts an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="input">The input value.</param>
        /// <returns>The output value.</returns>
        protected virtual T ChangeType<T>(object input)
        {
            var converter = this.Converter;

            return converter != null ? converter.ChangeType<T>(value: input)
                                     : (T)input;
        }

        /// <summary>
        /// Creates an empty storage for variables.
        /// </summary>
        /// <returns>The created storage.</returns>
        protected virtual IDictionary<string, object> CreateVarStorage()
        {
            return new Dictionary<string, object>(EqualityComparerFactory.CreateCaseInsensitiveStringComparer(true, true));
        }

        /// <inheriteddoc />
        public object Execute(IEnumerable<object> args)
        {
            return this.Execute(args.AsArray());
        }

        /// <inheriteddoc />
        public object Execute(params object[] args)
        {
            return this._EXECUTE_FUNC(args);
        }

        private object Execute_NonThreadSafe(object[] args)
        {
            IWorkflowExecutionContext result = null;
            this.ForEach(ctx => result = ctx.Item(ctx.State.Arguments),
                         actionState: new
                         {
                             Arguments = args,
                         });

            return result != null ? result.Result : null;
        }

        private object Execute_ThreadSafe(object[] args)
        {
            object result = null;

            lock (this._SYNC)
            {
                result = this.Execute_NonThreadSafe(args);
            }

            return result;
        }

        /// <summary>
        /// Returns the iterator for <see cref="WorkflowBase.GetEnumerator()" />.
        /// </summary>
        /// <returns>The iterator.</returns>
        protected virtual IEnumerable<WorkflowFunc> GetFunctionIterator()
        {
            yield break;
        }

        /// <inheriteddoc />
        public IEnumerator<WorkflowFunc> GetEnumerator()
        {
            return this._GET_ENUMERATOR_FUNC();
        }

        private IEnumerator<WorkflowFunc> GetEnumerator_NonThreadSafe()
        {
            return this.GetFunctionIterator()
                       .GetEnumerator();
        }

        private IEnumerator<WorkflowFunc> GetEnumerator_ThreadSafe()
        {
            IEnumerator<WorkflowFunc> result;

            lock (this._SYNC)
            {
                result = this.GetEnumerator_NonThreadSafe();
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns a value of <see cref="WorkflowBase.Vars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <returns>The strong typed version of var.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="name" /> does not exist.</exception>
        public T GetVar<T>(string name)
        {
            T result;
            if (this.TryGetVar<T>(name, out result))
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        /// <summary>
        /// Tries to return a value of <see cref="WorkflowBase.Vars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <returns>Var exists or not.</returns>
        public bool TryGetVar<T>(string name, out T value)
        {
            return this.TryGetVar<T>(name, out value, default(T));
        }

        /// <summary>
        /// Tries to return a value of <see cref="WorkflowBase.Vars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValue">
        /// The default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        public bool TryGetVar<T>(string name, out T value, T defaultValue)
        {
            return this.TryGetVar<T>(name, out value,
                                     (vn, tt) => defaultValue);
        }

        /// <summary>
        /// Tries to return a value of <see cref="WorkflowBase.Vars" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="name">The name of the var.</param>
        /// <param name="value">The field where to write the found value to.</param>
        /// <param name="defaultValueProvider">
        /// The logic that produces the default value for <paramref name="name" />
        /// if <paramref name="value" /> does not exist.
        /// </param>
        /// <returns>Var exists or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> is <see langword="null" />.
        /// </exception>
        public bool TryGetVar<T>(string name, out T value, Func<string, Type, object> defaultValueProvider)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            lock (this.SyncRoot)
            {
                var dict = this.Vars;
                if (dict != null)
                {
                    object foundValue;
                    if (dict.TryGetValue(WorkflowExecutionContext.ParseVarName(name), out foundValue))
                    {
                        value = this.ChangeType<T>(foundValue);

                        return true;
                    }
                }
            }

            value = this.ChangeType<T>(defaultValueProvider(name, typeof(T)));
            return false;
        }

        #endregion Methods (15)
    }
}