// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Text
{
    /// <summary>
    /// A basic template.
    /// </summary>
    public abstract class TemplateBase : ObjectBase, ITemplate
    {
        #region Fields (1)

        /// <summary>
        /// Stores the variables of that template.
        /// </summary>
        protected readonly IDictionary<string, object> _VARS;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <inheriteddoc />
        protected TemplateBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this._VARS = this.CreateVarStorage() ?? new Dictionary<string, object>();
        }

        /// <inheriteddoc />
        protected TemplateBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected TemplateBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected TemplateBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constrcutors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public object this[string varName]
        {
            get { return this._VARS[varName]; }

            set { this._VARS[varName] = value; }
        }

        #endregion Properties (1)

        #region Methods (5)

        /// <summary>
        /// Creates the instance for <see cref="TemplateBase._VARS" />.
        /// </summary>
        /// <returns>The created instance.</returns>
        protected virtual IDictionary<string, object> CreateVarStorage()
        {
            return null;
        }

        /// <inheriteddoc />
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this._VARS
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The logic that renders the output.
        /// </summary>
        /// <param name="output">
        /// The variable where to write the output to.
        /// Is <see langword="null" /> by default.
        /// </param>
        protected abstract void OnRender(ref object output);

        /// <inheriteddoc />
        public object Render()
        {
            object result = null;
            this.OnRender(ref result);

            return result;
        }

        #endregion Methods (5)
    }
}