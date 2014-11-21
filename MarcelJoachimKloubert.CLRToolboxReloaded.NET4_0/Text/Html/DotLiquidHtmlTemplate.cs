// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using DotLiquid;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
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

        #region Constrcutors (1)

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

        #endregion Constrcutors (1)

        #region Delegates and events (1)

        /// <summary>
        /// Describes a function or method that provides the source for an instance of that class.
        /// </summary>
        /// <param name="tpl">The underlying instance.</param>
        /// <param name="target">The builder where to write the template source to.</param>
        public delegate void SourceProvider(DotLiquidHtmlTemplate tpl,
                                            StringBuilder target);

        #endregion Delegates and events (1)

        #region Methods (5)

        /// <summary>
        /// Creates a new instance based on a template source string.
        /// </summary>
        /// <param name="src">The template source.</param>
        /// <returns>The new instance.</returns>
        public static DotLiquidHtmlTemplate Create(string src)
        {
            return new DotLiquidHtmlTemplate((tpl, sb) => sb.Append(src));
        }

        /// <summary>
        /// Reads the data of a template from a UTF-8 stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The created template.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> cannot be read.
        /// </exception>
        public static DotLiquidHtmlTemplate Create(Stream stream)
        {
            return Create(stream,
                          Encoding.UTF8);
        }

        /// <summary>
        /// Reads the data of a template from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="enc">The string encoding.</param>
        /// <returns>The created template.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> cannot be read.
        /// </exception>
        public static DotLiquidHtmlTemplate Create(Stream stream, Encoding enc)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            if (stream.CanRead == false)
            {
                throw new IOException();
            }

            return new DotLiquidHtmlTemplate((tpl, sb) =>
                {
                    using (var temp = new MemoryStream())
                    {
                        stream.CopyTo(temp);

                        sb.Append(enc.GetString(temp.ToArray()));
                    }
                });
        }

        /// <inheriteddoc />
        protected override void OnRender(TextWriter writer)
        {
            // get template source
            var src = new StringBuilder();
            this._PROVIDER(this, src);

            // create template and assign variables
            var tpl = Template.Parse(src.ToString());
            foreach (var item in this)
            {
                tpl.Assigns
                   .Add(item.Key,
                        ToLiquidObject(item.Value));
            }

            // render
            writer.Write(tpl.Render());
        }

        private static object ToLiquidObject(object input)
        {
            var result = input;

            if (DBNull.Value.Equals(result))
            {
                result = null;
            }
            else
            {
                result = result.AsString();
            }

            return result;
        }

        #endregion Methods (5)
    }
}