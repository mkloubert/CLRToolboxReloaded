// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox.Impl
{
    internal sealed class ServerInfo : ConnectionChildBase, IServerInfo
    {
        #region Properties (3)

        public bool HasKey
        {
            get { return this.Key != null; }
        }

        public string Key
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        #endregion Properties (3)

        #region Methods (1)

        public RSACryptoServiceProvider TryGetRsaCrypter()
        {
            RSACryptoServiceProvider result = null;

            try
            {
                if (this.HasKey)
                {
                    result = new RSACryptoServiceProvider();
                    result.FromXmlString(this.Key);
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        #endregion Methods (1)
    }
}