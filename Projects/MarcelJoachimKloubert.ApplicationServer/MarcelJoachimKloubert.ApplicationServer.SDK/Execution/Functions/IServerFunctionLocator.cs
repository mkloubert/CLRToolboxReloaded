// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.ApplicationServer.Execution.Functions
{
    /// <summary>
    /// Describes an object that locates server functions.
    /// </summary>
    public interface IServerFunctionLocator : IObject
    {
        #region Methods (1)

        /// <summary>
        /// Returns all available server functions.
        /// </summary>
        /// <returns>The list of all available functions.</returns>
        IEnumerable<IServerFunction> GetFunctions();

        #endregion Methods (1)
    }
}