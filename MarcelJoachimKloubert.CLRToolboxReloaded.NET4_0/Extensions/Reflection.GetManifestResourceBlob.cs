// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Returns an embedded resource inside an <see cref="Assembly" /> as a byte array.
        /// </summary>
        /// <typeparam name="T">The type that is used for the namespace of the resource.</typeparam>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The binary data or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static byte[] GetManifestResourceBlob<T>(this Assembly asm, string name)
        {
            return GetManifestResourceBlob(asm: asm,
                                           type: typeof(T),
                                           name: name);
        }

        /// <summary>
        /// Returns an embedded resource inside an <see cref="Assembly" /> as a byte array.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The binary data or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static byte[] GetManifestResourceBlob(this Assembly asm, string name)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            var stream = asm.GetManifestResourceStream(name);
            if (stream == null)
            {
                return null;
            }

            using (stream)
            {
                using (var temp = new MemoryStream())
                {
                    stream.CopyTo(temp);

                    return temp.ToArray();
                }
            }
        }

        /// <summary>
        /// Returns an embedded resource inside an <see cref="Assembly" /> as a byte array.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="type">The type that is used for the namespace of the resource.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The binary data or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> and/or <paramref name="type" /> are <see langword="null" />.
        /// </exception>
        public static byte[] GetManifestResourceBlob(this Assembly asm, Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return GetManifestResourceBlob(asm: asm,
                                           name: GetFullResourceName(type, name));
        }

        #endregion Methods
    }
}