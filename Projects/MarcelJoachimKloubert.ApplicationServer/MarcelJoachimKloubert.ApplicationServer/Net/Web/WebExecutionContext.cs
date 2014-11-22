// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.CLRToolbox.Text.Html;
using System;
using System.IO;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    internal sealed class WebExecutionContext : ObjectBase, IWebExecutionContext
    {
        #region Properties (4)

        public IHttpRequest Request
        {
            get;
            internal set;
        }

        public IHttpResponse Response
        {
            get;
            internal set;
        }

        internal IApplicationServerContext ServerContext
        {
            get;
            set;
        }

        internal Func<FileInfo, IHtmlTemplate> TryGetHtmlTemplateFunc
        {
            get;
            set;
        }

        #endregion Properties (4)

        #region Methods (1)

        /// <inheriteddoc />
        public IHtmlTemplate TryGetHtmlTemplate(string name)
        {
            IHtmlTemplate result = null;

            var dir = new DirectoryInfo(Path.Combine(this.ServerContext.WebDirectory, "tpl"));
            if (dir.Exists)
            {
                var file = new FileInfo(Path.Combine(dir.FullName,
                                                     name.Trim() + ".html"));

                result = this.TryGetHtmlTemplateFunc(file);
            }

            return result;
        }

        #endregion Methods (1)
    }
}