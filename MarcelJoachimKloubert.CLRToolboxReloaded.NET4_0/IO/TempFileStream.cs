// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// An extension of <see cref="FileStream" /> that is handled
    /// as temporary file.
    /// </summary>
    public class TempFileStream : FileStream
    {
        #region Constructors (5)

        /// <summary>
        /// Initializes a new instance of the <see cref="TempFileStream" /> class.
        /// </summary>
        public TempFileStream()
            : base(path: CreateTempFileAndGetPath(),
                   mode: FileMode.Open,
                   access: FileAccess.ReadWrite)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempFileStream" /> class.
        /// </summary>
        /// <param name="share">Defines how the file is shared.</param>
        public TempFileStream(FileShare share)
            : base(path: CreateTempFileAndGetPath(),
                   mode: FileMode.Open,
                   access: FileAccess.ReadWrite,
                   share: share)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempFileStream" /> class.
        /// </summary>
        /// <param name="share">Defines how the file is shared.</param>
        /// <param name="bufferSize">Defines the buffer size in bytes.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is smaller than 1.
        /// </exception>
        public TempFileStream(FileShare share, int bufferSize)
            : base(path: CreateTempFileAndGetPath(),
                   mode: FileMode.Open,
                   access: FileAccess.ReadWrite,
                   share: share,
                   bufferSize: bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempFileStream" /> class.
        /// </summary>
        /// <param name="share">Defines how the file is shared.</param>
        /// <param name="bufferSize">Defines the buffer size in bytes.</param>
        /// <param name="useAsync">Use file asynchron or not.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is smaller than 1.
        /// </exception>
        public TempFileStream(FileShare share, int bufferSize, bool useAsync)
            : base(path: CreateTempFileAndGetPath(),
                   mode: FileMode.Open,
                   access: FileAccess.ReadWrite,
                   share: share,
                   bufferSize: bufferSize,
                   useAsync: useAsync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempFileStream" /> class.
        /// </summary>
        /// <param name="share">Defines how the file is shared.</param>
        /// <param name="bufferSize">Defines the buffer size in bytes.</param>
        /// <param name="options">The options to use.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is smaller than 1.
        /// </exception>
        public TempFileStream(FileShare share, int bufferSize, FileOptions options)
            : base(path: CreateTempFileAndGetPath(),
                   mode: FileMode.Open,
                   access: FileAccess.ReadWrite,
                   share: share,
                   bufferSize: bufferSize,
                   options: options)
        {
        }

        #endregion Constructors (5)

        #region Methods (3)

        /// <inheriteddoc />
        public override sealed void Close()
        {
            base.Close();

            this.OnClosed();
        }

        private static string CreateTempFileAndGetPath()
        {
            return Path.GetTempFileName();
        }

        /// <summary>
        /// Is invoked AFTER that stream has been closed.
        /// </summary>
        protected virtual void OnClosed()
        {
            try
            {
                File.Delete(this.Name);
            }
            catch
            {
                // ignore errors here
            }
        }

        #endregion Methods (3)
    }
}