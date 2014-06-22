// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using System.Security.Cryptography;
using System.Security.Principal;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    /// <summary>
    /// Describes a server principal.
    /// </summary>
    public interface IServerPrincipal : IObject, IPrincipal
    {
        #region Properties (5)

        /// <summary>
        /// Gets the root directory of the user's files.
        /// </summary>
        string Files { get; }

        /// <inheriteddoc />
        new IServerIdentity Identity { get; }

        /// <summary>
        /// Gets the root directory of the user's inbox files.
        /// </summary>
        string Inbox { get; }
        
        /// <summary>
        /// Gets the root directory of the user's outbox files.
        /// </summary>
        string Outbox { get; }
        
        /// <summary>
        /// Gets the root directory of the user's temp files.
        /// </summary>
        string Temp { get; }

        #endregion Properties

        #region Methods (1)

        /// <summary>
        /// Tries to return the RSA crypter.
        /// </summary>
        /// <returns>
        /// The RSA crypter or <see langword="null" /> if no key is defined.
        /// </returns>
        RSACryptoServiceProvider TryGetRsaCrypter();

        #endregion Methods
    }
}