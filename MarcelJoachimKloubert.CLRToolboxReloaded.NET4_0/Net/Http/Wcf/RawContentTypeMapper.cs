// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.ServiceModel.Channels;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    internal sealed class RawContentTypeMapper : WebContentTypeMapper
    {
        #region Methods (1)

        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            return WebContentFormat.Raw;
        }

        #endregion Methods (1)
    }
}