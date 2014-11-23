// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Execution.Functions;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Services.Functions
{
    /// <summary>
    /// A function that simply returns the input parameters.
    /// </summary>
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Execution.Functions.IServerFunction))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class Echo : AppServerFunctionBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="Echo" /> class.
        /// </summary>
        [ImportingConstructor]
        public Echo()
            : base(id: new Guid("{D83C3E14-F197-4A3A-8B1D-DFCCC01DCAFB}"))
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnExecute(IReadOnlyDictionary<string, object> input, IDictionary<string, object> result)
        {
            input.ForEach(action: ctx =>
                {
                    ctx.State.Result
                             .Add(ctx.Item);
                }, actionState: new
                {
                    Result = result,
                });
        }

        #endregion Methods (1)
    }
}