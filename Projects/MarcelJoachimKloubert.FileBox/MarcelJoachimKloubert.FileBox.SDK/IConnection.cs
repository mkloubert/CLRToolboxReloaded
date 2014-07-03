// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Describes a (server) connection
    /// </summary>
    public interface IConnection : IObject
    {
        #region Methods (8)

        /// <summary>
        /// Creates a basic and setupped HTTP web request client.
        /// </summary>
        /// <param name="relativePath">The additional path for the request URI.</param>
        /// <returns>The created instance.</returns>
        HttpWebRequest CreateWebRequest(string relativePath);

        /// <summary>
        /// Returns the files from the INBOX folder.
        /// </summary>
        /// <param name="rsa">The crypter for encrypting the meta data of the files.</param>
        /// <param name="startAt">The zero based index of the first item.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rsa" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startAt" /> and/or <paramref name="maxItems" /> are invalid.
        /// </exception>
        IExecutionContext<IList<IFile>> GetInbox(RSACryptoServiceProvider rsa,
                                                 int startAt = 0, int? maxItems = null,
                                                 bool autoStart = true);

        /// <summary>
        /// Returns the files from the OUTBOX folder.
        /// </summary>
        /// <param name="rsa">The crypter for encrypting the meta data of the files.</param>
        /// <param name="startAt">The zero based index of the first item.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rsa" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startAt" /> and/or <paramref name="maxItems" /> are invalid.
        /// </exception>
        IExecutionContext<IList<IFile>> GetOutbox(RSACryptoServiceProvider rsa,
                                                  int startAt = 0, int? maxItems = null,
                                                  bool autoStart = true);

        /// <summary>
        /// Gets information about the server.
        /// </summary>
        /// <param name="autoStart">Start operation directly or not.</param>
        /// <returns>The underlying execution result.</returns>
        IExecutionContext<IServerInfo> GetServerInfo(bool autoStart = true);

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <returns>The underlying execution context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        /// <remarks>The operations directly starts.</remarks>
        IExecutionContext Send(string path, params string[] recipients);

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <returns>The underlying execution context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        IExecutionContext Send(string path, bool autoStart, params string[] recipients);

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        /// <returns>The underlying execution context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        IExecutionContext Send(string path, IEnumerable<string> recipients, bool autoStart = true);

        /// <summary>
        /// Updates the public RSA key on the server.
        /// </summary>
        /// <param name="xml">The RSA key as XML data.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        IExecutionContext UpdateKey(string xml, bool autoStart = true);

        #endregion Methods (8)
    }
}