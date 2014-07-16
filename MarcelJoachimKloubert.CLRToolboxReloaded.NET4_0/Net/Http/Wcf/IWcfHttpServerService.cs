// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    [ServiceContract]
    internal interface IWcfHttpServerService
    {
        #region Methods (1)

        [OperationContract(Action = "*", ReplyAction = "*")]
        Message Request(Message message);

        #endregion Methods (1)
    }
}