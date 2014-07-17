// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Loads a <see cref="Bitmap" /> from binary data.
        /// </summary>
        /// <param name="data">The data from where to load the bitmap from.</param>
        /// <returns>The loaded bitmap.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        public static Bitmap LoadBitmap(this IEnumerable<byte> data)
        {
            using (var stream = new MemoryStream(data.AsArray(), false))
            {
                return LoadBitmap(stream);
            }
        }

        /// <summary>
        /// Loads a <see cref="Bitmap" /> from a stream.
        /// </summary>
        /// <param name="stream">The stream from where to load the bitmap from.</param>
        /// <returns>The loaded bitmap.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> cannot be read.
        /// </exception>
        public static Bitmap LoadBitmap(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.CanRead == false)
            {
                throw new IOException();
            }

            FIBITMAP? dib = null;

            try
            {
                dib = FreeImage.LoadFromStream(stream);

                return FreeImage.GetBitmap(dib.Value);
            }
            finally
            {
                if (dib.HasValue)
                {
                    FreeImage.Unload(dib.Value);
                }
            }
        }

        #endregion Methods (2)
    }
}