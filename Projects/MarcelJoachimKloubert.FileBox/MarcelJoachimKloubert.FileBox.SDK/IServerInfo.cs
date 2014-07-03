// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Describes an object that stores information about a server.
    /// </summary>
    public interface IServerInfo : IConnectionChild
    {
        #region Methods (3)

        /// <summary>
        /// Gets if the server has stored the public key for the underlying user or not.
        /// </summary>
        bool HasKey { get; }

        /// <summary>
        /// Gets the public key of the connected user if available.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the (machine) name of the server.
        /// </summary>
        string Name { get; }

        #endregion Methods (3)

        #region Properties (1)

        /// <summary>
        /// Tries to return a RSA crypter based on the value of <see cref="ServerInfo.Key" />.
        /// </summary>
        /// <returns>The RSA crypter or <see langword="null" /> if no valid public key is available.</returns>
        RSACryptoServiceProvider TryGetRsaCrypter();

        #endregion Properties (1)
    }
}