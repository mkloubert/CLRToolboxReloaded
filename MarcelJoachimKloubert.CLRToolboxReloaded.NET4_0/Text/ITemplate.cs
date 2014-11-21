// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Text
{
    /// <summary>
    /// Describes a template.
    /// </summary>
    public interface ITemplate : IObject, IEnumerable<KeyValuePair<string, object>>
    {
        #region Properties (1)

        /// <summary>
        /// Gets or sets a variable.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        /// <returns>The value of the variable.</returns>
        object this[string varName] { get; set; }

        #endregion Properties (1)

        #region Methods (1)

        /// <summary>
        /// Renders output.
        /// </summary>
        /// <returns>The rendered output.</returns>
        object Render();

        #endregion Methods (1)
    }
}