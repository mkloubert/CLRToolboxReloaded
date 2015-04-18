// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Text
{
    /// <summary>
    /// A basic template that produces string output.
    /// </summary>
    public abstract class StringTemplateBase : TemplateBase, IStringTemplate
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected StringTemplateBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected StringTemplateBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected StringTemplateBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected StringTemplateBase()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Methods (4)

        /// <inheriteddoc />
        protected sealed override void OnRender(ref object output)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                this.OnRender(writer: writer);
            }

            output = sb.ToString();
        }

        /// <summary>
        /// Renders the output of that template.
        /// </summary>
        /// <param name="writer">The target where to write the output to.</param>
        protected abstract void OnRender(TextWriter writer);

        /// <inheriteddoc />
        public new string Render()
        {
            return base.Render()
                       .AsString();
        }

        /// <inheriteddoc />
        public override sealed string ToString()
        {
            return this.Render() ?? string.Empty;
        }

        #endregion Methods (4)
    }
}