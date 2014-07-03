// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Security;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Describes an object that stores the data of a file (item).
    /// </summary>
    public interface IFile : IConnectionChild
    {
        #region Properties (11)

        /// <summary>
        /// Gets the creation date.
        /// </summary>
        DateTimeOffset CreationDate { get; }

        /// <summary>
        /// Gets the crypted meta data of the item.
        /// </summary>
        byte[] CryptedMeta { get; }

        /// <summary>
        /// Gets the crypted meta data of the item as XML string.
        /// </summary>
        SecureString CryptedMetaXml { get; }

        /// <summary>
        /// Gets the ID of the file.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets if the data of that data is corrupted or not.
        /// </summary>
        bool IsCorrupted { get; }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        DateTimeOffset LastWriteTime { get; }

        /// <summary>
        /// Gets the location of the file.
        /// </summary>
        Location Location { get; }

        /// <summary>
        /// Gets the (machine) name of the server.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the (internal) name of that item on the server.
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// Gets time the file was send.
        /// </summary>
        DateTimeOffset SendTime { get; }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        long Size { get; }

        #endregion Properties (11)

        #region Methods (1)

        /// <summary>
        /// Opens that file for reading.
        /// </summary>
        /// <returns>The stream for reading the file.</returns>
        /// <exception cref="InvalidOperationException">
        /// File item contains corrupted data.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The operation is (currently) not supported.
        /// </exception>
        Stream Open();

        #endregion Methods (1)
    }
}