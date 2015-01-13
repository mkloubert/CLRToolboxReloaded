// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// Describes an object that handles temporary data.
    /// </summary>
    public interface ITempDataManager : IObject
    {
        #region Methods (2)

        /// <summary>
        /// Creates a new stream that should contain temporary data.
        /// </summary>
        /// <returns>The new temp stream.</returns>
        Stream CreateStream();

        /// <summary>
        /// Creates a new stream that should contain temporary data.
        /// </summary>
        /// <param name="initialData">The streams that contains the inital data.</param>
        /// <param name="bufferSize">
        /// The buffer size in bytes that should be used to read <paramref name="initialData" />.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <param name="startAtBeginning">
        /// Set position of new stream to beginning or not.
        /// </param>
        /// <returns>The new temp stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="initialData" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="initialData" /> cannot be read.
        /// </exception>
        Stream CreateStream(Stream initialData, int? bufferSize = null,
                            bool startAtBeginning = true);

        #endregion Methods (2)
    }
}