// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="HashAlgorithm.ComputeHash(Stream)" />
        public static void ComputeHash(this HashAlgorithm algo, Stream inputStream, Stream outputStream)
        {
            if (algo == null)
            {
                throw new ArgumentNullException("algo");
            }

            Write(outputStream,
                  algo.ComputeHash(inputStream));
        }

        #endregion Methods (1)
    }
}