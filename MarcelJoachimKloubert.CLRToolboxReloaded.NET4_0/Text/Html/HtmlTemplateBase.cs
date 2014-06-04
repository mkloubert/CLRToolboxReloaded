// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Text.Html
{
    /// <summary>
    /// A basic template that generates HTML output.
    /// </summary>
    public abstract class HtmlTemplateBase : StringTemplateBase, IHtmlTemplate
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected HtmlTemplateBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HtmlTemplateBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected HtmlTemplateBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HtmlTemplateBase()
            : base()
        {
        }

        #endregion Constrcutors (4)
    }
}