// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    internal sealed class ExecutionContext
    {
        #region Properties (7)

        internal List<string> Arguments { get; set; }

        internal string Host { get; set; }

        internal bool IsSecure { get; set; }

        internal string Password { get; set; }

        internal int Port { get; set; }

        internal RSACryptoServiceProvider RSA { get; set; }

        internal string Username { get; set; }

        #endregion Properties (7)

        #region Methods (1)

        internal FileBoxConnection CreateConnection()
        {
            var result = new FileBoxConnection();
            result.Host = this.Host;
            result.Port = Port;
            result.IsSecure = this.IsSecure;

            result.User = this.Username;
            result.SetPassword(this.Password);

            return result;
        }

        #endregion Methods (1)
    }
}