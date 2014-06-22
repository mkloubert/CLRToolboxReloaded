// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;
using System.Security;

namespace MarcelJoachimKloubert.FileBox
{
    internal sealed class ExecutionContext
    {
        #region Properties (6)

        internal List<string> Arguments { get; set; }

        internal string Host { get; set; }

        internal bool IsSecure { get; set; }

        internal string Password { get; set; }

        internal int Port { get; set; }

        internal string Username { get; set; }

        #endregion Properties (6)

        #region Methods (1)

        internal FileBoxConnection CreateConnection()
        {
            var result = new FileBoxConnection();
            result.Host = this.Host;
            result.Port = Port;
            result.IsSecure = this.IsSecure;

            SecureString pwd = null;
            if (string.IsNullOrEmpty(this.Password) == false)
            {
                pwd = new SecureString();
                foreach (var c in this.Password)
                {
                    pwd.AppendChar(c);
                }

                pwd.MakeReadOnly();
            }

            result.User = this.Username;
            result.Password = pwd;

            return result;
        }

        #endregion Methods (1)
    }
}