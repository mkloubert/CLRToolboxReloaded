// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Functions
{
    /// <summary>
    /// A basic dedicated function.
    /// </summary>
    public abstract class FunctionBase : IdentifiableBase, IFunction
    {
        #region Fields (2)

        private readonly Guid _ID;
        private readonly Action<IReadOnlyDictionary<string, object>, IDictionary<string, object>> _ON_EXECUTE_FUNC;

        #endregion Fields (2)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected FunctionBase(Guid id, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this._ID = id;

            this._ON_EXECUTE_FUNC = this._IS_SYNCHRONIZED ? new Action<IReadOnlyDictionary<string, object>, IDictionary<string, object>>(this.OnExecute_ThreadSafe)
                                                          : new Action<IReadOnlyDictionary<string, object>, IDictionary<string, object>>(this.OnExecute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        protected FunctionBase(Guid id, bool isSynchronized)
            : this(id: id,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected FunctionBase(Guid id, object sync)
            : this(id: id,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        protected FunctionBase(Guid id)
            : this(id: id,
                   isSynchronized: false)
        {
        }

        #endregion Constrcutors (4)

        #region Properties (4)

        /// <inheriteddoc />
        public string Fullname
        {
            get
            {
                var ns = this.Namespace;
                ns = string.IsNullOrWhiteSpace(ns) ? null : ns.Trim();

                var name = this.Name;
                name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();

                return string.Format("{0}{1}{2}",
                                     ns,
                                     ns != null ? "." : null,
                                     name);
            }
        }

        /// <inheriteddoc />
        public sealed override Guid Id
        {
            get { return this._ID; }
        }

        /// <inheriteddoc />
        public virtual string Name
        {
            get { return this.GetType().Name; }
        }

        /// <inheriteddoc />
        public virtual string Namespace
        {
            get { return this.GetType().Namespace; }
        }

        #endregion Properties (4)

        #region Methods (3)

        /// <inheriteddoc />
        public IDictionary<string, object> Execute(IEnumerable<KeyValuePair<string, object>> @params = null)
        {
            @params = @params ?? Enumerable.Empty<KeyValuePair<string, object>>();

            // input parameters
            IReadOnlyDictionary<string, object> input;
            {
                // keep sure to have a case insensitive dictionary
                var dict = new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                emptyIsNull: true));
                @params.ForEach(action: (ctx) => ctx.State
                                                    .Dictionary.Add(ctx.Item),
                                actionState: new
                                {
                                    Dictionary = (IDictionary<string, object>)dict,
                                });

                input = new ReadOnlyDictionaryWrapper<string, object>(dict);
            }

            // output parameters
            var output = new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                              emptyIsNull: true));

            this._ON_EXECUTE_FUNC(input, output);

            return output;
        }

        /// <summary>
        /// Stores the logic for the <see cref="FunctionBase.Execute(IEnumerable{KeyValuePair{string, object}})" /> method.
        /// </summary>
        /// <param name="input">The input parameters.</param>
        /// <param name="result">The result parameters.</param>
        protected abstract void OnExecute(IReadOnlyDictionary<string, object> input, IDictionary<string, object> result);

        private void OnExecute_ThreadSafe(IReadOnlyDictionary<string, object> input, IDictionary<string, object> result)
        {
            lock (this._SYNC)
            {
                this.OnExecute(input, result);
            }
        }

        #endregion Methods (3)
    }
}