// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.CLRToolbox.Text.Html;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    /// <summary>
    /// Describes a web interface request context.
    /// </summary>
    public interface IWebExecutionContext : IObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets the underlying HTTP request context.
        /// </summary>
        IHttpRequest Request { get; }

        /// <summary>
        /// Gets the underlying HTTP response context.
        /// </summary>
        IHttpResponse Response { get; }

        #endregion Properties (2)

        #region Methods (3)

        /// <summary>
        /// Tries to return a HTML template.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <returns>The template or <see langword="null" /> if NOT found.</returns>
        IHtmlTemplate TryGetHtmlTemplate(string name);

        /// <summary>
        /// Tries to load javascript code.
        /// </summary>
        /// <param name="name">The name of the script container.</param>
        /// <returns>The code or <see langword="null" /> if NOT found.</returns>
        string TryLoadJavascript(string name);

        /// <summary>
        /// Tries to load CSS code.
        /// </summary>
        /// <param name="name">The name of the CSS container.</param>
        /// <returns>The code or <see langword="null" /> if NOT found.</returns>
        string TryLoadStylesheets(string name);

        #endregion Methods (3)
    }
}