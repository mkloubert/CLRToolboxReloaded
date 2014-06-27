// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

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
        #region Properties (7)

        /// <summary>
        /// Gets if that principal can write messages or not.
        /// </summary>
        bool CanWriteMessages { get; }

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
        /// Gets the root directory of the user's message files.
        /// </summary>
        string Messages { get; }

        /// <summary>
        /// Gets the root directory of the user's outbox files.
        /// </summary>
        string Outbox { get; }

        /// <summary>
        /// Gets the root directory of the user's temp files.
        /// </summary>
        string Temp { get; }

        #endregion Properties (7)

        #region Methods (2)

        /// <summary>
        /// Tries to return the RSA crypter.
        /// </summary>
        /// <returns>
        /// The RSA crypter or <see langword="null" /> if no key is defined.
        /// </returns>
        RSACryptoServiceProvider TryGetRsaCrypter();

        /// <summary>
        /// Writes a message for the user.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="msg">The message in MarkDown format.</param>
        /// <exception cref="InvalidOperationException">Currently it is not possible to write a message.</exception>
        void WriteMessage(string subject,
                          string msg);

        #endregion Methods (2)
    }
}