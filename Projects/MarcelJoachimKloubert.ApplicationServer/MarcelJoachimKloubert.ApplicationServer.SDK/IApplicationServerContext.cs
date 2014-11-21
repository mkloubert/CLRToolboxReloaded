// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.IO;

namespace MarcelJoachimKloubert.ApplicationServer
{
    /// <summary>
    /// The context of an <see cref="IApplicationServer" />.
    /// </summary>
    public interface IApplicationServerContext : IServiceLocator
    {
        #region Properties (6)

        /// <summary>
        /// Gets the underlying logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the current server time.
        /// </summary>
        DateTimeOffset Now { get; }

        /// <summary>
        /// Gets the full path of the root directory.
        /// </summary>
        string RootDirectory { get; }

        /// <summary>
        /// Gets the underlying server.
        /// </summary>
        IApplicationServer Server { get; }

        /// <summary>
        /// Gets the root directory for temp files.
        /// </summary>
        string TempDirectory { get; }

        /// <summary>
        /// Gets the root directory for web files.
        /// </summary>
        string WebDirectory { get; }

        #endregion Properties (6)

        #region Methods (2)

        /// <summary>
        /// Creates an unique temp directory.
        /// </summary>
        /// <returns>The full path of the created directory.</returns>
        string CreateTempDirectory();

        /// <summary>
        /// Creates and opens an empty and unique temp file for read/write operations.
        /// </summary>
        /// <param name="tempDir">
        /// The custom directory where the file should be created.
        /// If that value is <see langword="null" /> or empty, the value from <see cref="IApplicationServerContext.TempDirectory" /> is used.
        /// </param>
        /// <param name="extension">The extension for the file.</param>
        /// <returns>The created and opened file.</returns>
        FileStream CreateAndOpenTempFile(string tempDir = null,
                                         string extension = "tmp");

        #endregion Methods (2)
    }
}