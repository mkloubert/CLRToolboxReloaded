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
        #region Properties (6)

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

        internal Func<FileInfo, string> TryLoadJavascriptFunc
        {
            get;
            set;
        }

        internal Func<FileInfo, string> TryLoadStylesheetsFunc
        {
            get;
            set;
        }

        #endregion Properties (6)

        #region Methods (3)

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

        public string TryLoadJavascript(string name)
        {
            string result = null;

            var dir = new DirectoryInfo(Path.Combine(this.ServerContext.WebDirectory, "js"));
            if (dir.Exists)
            {
                var file = new FileInfo(Path.Combine(dir.FullName,
                                                     name.Trim() + ".js"));

                result = this.TryLoadJavascriptFunc(file);
            }

            return result;
        }

        public string TryLoadStylesheets(string name)
        {
            string result = null;

            var dir = new DirectoryInfo(Path.Combine(this.ServerContext.WebDirectory, "css"));
            if (dir.Exists)
            {
                var file = new FileInfo(Path.Combine(dir.FullName,
                                                     name.Trim() + ".css"));

                result = this.TryLoadStylesheetsFunc(file);
            }

            return result;
        }

        #endregion Methods (3)
    }
}