// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Functions
{
    /// <summary>
    /// Describes a dedicated function.
    /// </summary>
    public interface IFunction : IIdentifiable
    {
        #region Properties (3)

        /// <summary>
        /// Gets the fullname of the function.
        /// </summary>
        string Fullname { get; }

        /// <summary>
        /// Gets the short name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the namespace of the function.
        /// </summary>
        string Namespace { get; }

        #endregion Properties (3)

        #region Methods (1)

        /// <summary>
        /// Executes the function.
        /// </summary>
        /// <param name="params">The input parameters.</param>
        /// <returns>The output parameters.</returns>
        IDictionary<string, object> Execute(IEnumerable<KeyValuePair<string, object>> @params = null);

        #endregion Methods (1)
    }
}