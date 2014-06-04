// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Text
{
    /// <summary>
    /// A basic template.
    /// </summary>
    public abstract class TemplateBase : ObjectBase, ITemplate
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected TemplateBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected TemplateBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected TemplateBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected TemplateBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Methods (2)

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

        #endregion Methods (2)
    }
}