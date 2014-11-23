// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Execution.Functions;
using System;

namespace MarcelJoachimKloubert.ApplicationServer.Services.Functions
{
    /// <summary>
    /// A basic application server function.
    /// </summary>
    public abstract class AppServerFunctionBase : ServerFunctionBase
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected AppServerFunctionBase(Guid id, bool isSynchronized, object sync)
            : base(id: id,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected AppServerFunctionBase(Guid id, bool isSynchronized)
            : base(id: id,
                   isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected AppServerFunctionBase(Guid id, object sync)
            : base(id: id,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected AppServerFunctionBase(Guid id)
            : base(id: id)
        {
        }

        #endregion Constrcutors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public override string Namespace
        {
            get { return "MarcelJoachimKloubert.ApplicationServer"; }
        }

        #endregion Properties (1)
    }
}