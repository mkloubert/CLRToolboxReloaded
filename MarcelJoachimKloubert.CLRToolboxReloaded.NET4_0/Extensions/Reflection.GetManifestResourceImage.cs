// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions.Drawing;
using System;
using System.Drawing;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Returns an embedded resource inside an <see cref="Assembly" /> as a bitmap.
        /// </summary>
        /// <typeparam name="T">The type that is used for the namespace of the resource.</typeparam>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The image or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static Bitmap GetManifestResourceImage<T>(this Assembly asm, string name)
        {
            return GetManifestResourceImage(asm: asm,
                                            type: typeof(T),
                                            name: name);
        }

        /// <summary>
        /// Returns an embedded resource inside an <see cref="Assembly" /> as a bitmap.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The image or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static Bitmap GetManifestResourceImage(this Assembly asm, string name)
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
                return stream.LoadBitmap();
            }
        }

        /// <summary>
        /// Returns an embedded resource inside an <see cref="Assembly" /> as a bitmap.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="type">The type that is used for the namespace of the resource.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The image or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> and/or <paramref name="type" /> are <see langword="null" />.
        /// </exception>
        public static Bitmap GetManifestResourceImage(this Assembly asm, Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return GetManifestResourceImage(asm: asm,
                                            name: GetFullResourceName(type, name));
        }

        #endregion Methods
    }
}