// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace MarcelJoachimKloubert.ApplicationServer.Execution.Functions
{
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Execution.Functions.IServerFunctionLocator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class ServerFunctionLocator : ObjectBase, IServerFunctionLocator
    {
        #region Fields (1)

        private readonly IApplicationServer _SERVER;

        #endregion Fields (1)

        #region Constructors (1)

        [ImportingConstructor]
        internal ServerFunctionLocator(IApplicationServer server)
        {
            this._SERVER = server;
        }

        #endregion Constructors (1)

        #region Methods (1)

        public IEnumerable<IServerFunction> GetFunctions()
        {
            var serverFuncs = this._SERVER
                                  .Context
                                  .GetAllInstances<IServerFunction>();

            var serviceFuncs = this._SERVER
                                   .GetServiceModules()
                                   .SelectMany(m => m.Context
                                                     .GetAllInstances<IServerFunction>());

            return serverFuncs.Concat(serviceFuncs);
        }

        #endregion Methods (1)
    }
}