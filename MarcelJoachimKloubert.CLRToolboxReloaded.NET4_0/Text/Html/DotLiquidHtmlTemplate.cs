// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using DotLiquid;
using System;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Text.Html
{
    /// <summary>
    /// A HTML template that is based on DotLiquid (<see href="http://dotliquidmarkup.org/" />).
    /// </summary>
    public sealed class DotLiquidHtmlTemplate : HtmlTemplateBase
    {
        #region Fields (1)

        private readonly SourceProvider _PROVIDER;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidHtmlTemplate" /> class.
        /// </summary>
        /// <param name="provider">The logic that provides the source for the template.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public DotLiquidHtmlTemplate(SourceProvider provider)
            : base(isSynchronized: false)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
        }

        #endregion Constrcutors (4)

        #region Delegates and events (1)

        /// <summary>
        /// Describes a function or method that provides the source for an instance of that class.
        /// </summary>
        /// <param name="tpl">The underlying instance.</param>
        /// <param name="target">The builder where to write the template source to.</param>
        public delegate void SourceProvider(DotLiquidHtmlTemplate tpl,
                                            StringBuilder target);

        #endregion Delegates and events (1)

        #region Methods (2)

        /// <summary>
        /// Creates a new instance based on a template source string.
        /// </summary>
        /// <param name="src">The template source.</param>
        /// <returns>The new instance.</returns>
        public static DotLiquidHtmlTemplate Create(string src)
        {
            return new DotLiquidHtmlTemplate((tpl, sb) => sb.Append(src));
        }

        /// <inheriteddoc />
        protected override void OnRender(TextWriter writer)
        {
            // get template source
            var src = new StringBuilder();
            this._PROVIDER(this, src);

            // render
            var tpl = Template.Parse(src.ToString());
            writer.Write(tpl.Render());
        }

        #endregion Methods (2)
    }
}