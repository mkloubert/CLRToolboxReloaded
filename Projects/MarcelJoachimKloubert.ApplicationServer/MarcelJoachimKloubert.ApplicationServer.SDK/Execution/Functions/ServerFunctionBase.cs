// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Functions;
using System;

namespace MarcelJoachimKloubert.ApplicationServer.Execution.Functions
{
    /// <summary>
    /// A basic server function.
    /// </summary>
    public abstract class ServerFunctionBase : FunctionBase, IServerFunction
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected ServerFunctionBase(Guid id, bool isSynchronized, object sync)
            : base(id: id,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ServerFunctionBase(Guid id, bool isSynchronized)
            : base(id: id,
                   isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected ServerFunctionBase(Guid id, object sync)
            : base(id: id,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ServerFunctionBase(Guid id)
            : base(id: id)
        {
        }

        #endregion Constrcutors (4)
    }
}