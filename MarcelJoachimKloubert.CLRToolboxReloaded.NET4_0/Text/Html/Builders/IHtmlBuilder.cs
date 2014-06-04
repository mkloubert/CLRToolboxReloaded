// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Text.Html.Builders
{
    /// <summary>
    /// Describes an object that builds HTML code.
    /// </summary>
    public interface IHtmlBuilder : IObject
    {
        #region Methods (3)

        /// <summary>
        /// Writes the current HTML code of that object to a <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="sb">The target string builder.</param>
        /// <returns>That instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sb" /> is <see langword="null" />).
        /// </exception>
        IHtmlBuilder WriteTo(StringBuilder sb);

        /// <summary>
        /// Writes the current HTML code of that object to a <see cref="TextWriter" />.
        /// </summary>
        /// <param name="writer">The target writer.</param>
        /// <returns>That instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer" /> is <see langword="null" />.
        /// </exception>
        IHtmlBuilder WriteTo(TextWriter writer);

        /// <summary>
        /// Generates an returns HTML code from the current state of that instance.
        /// </summary>
        /// <returns>The generated HTML code.</returns>
        string ToHtml();

        #endregion Methods (3)
    }
}