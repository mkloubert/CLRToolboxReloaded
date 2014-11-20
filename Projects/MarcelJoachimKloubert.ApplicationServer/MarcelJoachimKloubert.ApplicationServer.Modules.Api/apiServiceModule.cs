using MarcelJoachimKloubert.ApplicationServer.Services;
using System;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Modules.Api
{
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Services.IServiceModule))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class ApiServiceModule : ServiceModuleBase
    {
        #region Constructors (1)

        internal ApiServiceModule()
            : base(new Guid("{F75BF7E0-1886-4528-83DD-4D6141D5CC4E}"))
        {
        }

        #endregion Constructors (1)
    }
}