// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Text.Html.Builders
{
    /// <summary>
    /// A basic object that builds HTML code.
    /// </summary>
    public abstract class HtmlBuilderBase : ObjectBase, IHtmlBuilder
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected HtmlBuilderBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HtmlBuilderBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected HtmlBuilderBase(object sync)
            : this(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HtmlBuilderBase()
            : base(isSynchronized: true)
        {
        }

        #endregion Constrcutors (4)

        #region Methods (7)

        /// <inheriteddoc />
        public string ToHtml()
        {
            var html = new StringBuilder();
            this.WriteTo(html);

            return html.ToString();
        }

        /// <inheriteddoc />
        public override sealed string ToString()
        {
            return this.ToHtml() ?? string.Empty;
        }

        /// <summary>
        /// The logic that generates the HTML code of that instance.
        /// </summary>
        /// <param name="writer">The target where to write the HTML code to.</param>
        protected abstract void OnToHtml(TextWriter writer);

        /// <inheriteddoc />
        public HtmlBuilderBase WriteTo(StringBuilder sb)
        {
            if (sb == null)
            {
                throw new ArgumentNullException("sb");
            }

            using (var writer = new StringWriter(sb))
            {
                this.WriteTo(writer);
            }

            return this;
        }

        IHtmlBuilder IHtmlBuilder.WriteTo(StringBuilder sb)
        {
            return this.WriteTo(sb);
        }

        /// <inheriteddoc />
        public HtmlBuilderBase WriteTo(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            this.OnToHtml(writer);
            return this;
        }

        IHtmlBuilder IHtmlBuilder.WriteTo(TextWriter writer)
        {
            return this.WriteTo(writer);
        }

        #endregion Methods (7)
    }
}