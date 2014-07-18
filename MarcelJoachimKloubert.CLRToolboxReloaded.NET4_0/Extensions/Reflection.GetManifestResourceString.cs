// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Returns a embedded resource inside an <see cref="Assembly" /> as an UTF-8 string.
        /// </summary>
        /// <typeparam name="T">The type that is used for the namespace of the resource.</typeparam>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The string or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static string GetManifestResourceString<T>(this Assembly asm, string name)
        {
            return GetManifestResourceString<T>(asm: asm,
                                                name: name,
                                                enc: Encoding.UTF8);
        }

        /// <summary>
        /// Returns a embedded resource inside an <see cref="Assembly" /> as a string.
        /// </summary>
        /// <typeparam name="T">The type that is used for the namespace of the resource.</typeparam>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <param name="enc">The charset to use.</param>
        /// <returns>The string or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public static string GetManifestResourceString<T>(this Assembly asm, string name, Encoding enc)
        {
            return GetManifestResourceString(asm: asm,
                                             type: typeof(T),
                                             name: name,
                                             enc: enc);
        }

        /// <summary>
        /// Returns a embedded resource inside an <see cref="Assembly" /> as an UTF-8 string.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The string or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static string GetManifestResourceString(this Assembly asm, string name)
        {
            return GetManifestResourceString(asm: asm,
                                             name: name,
                                             enc: Encoding.UTF8);
        }

        /// <summary>
        /// Returns a embedded resource inside an <see cref="Assembly" /> as a string.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <param name="enc">The charset to use.</param>
        /// <returns>The string or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public static string GetManifestResourceString(this Assembly asm, string name, Encoding enc)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            if (enc == null)
            {
                throw new ArgumentNullException("enc");
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

                    return enc.GetString(temp.ToArray(), 0, (int)temp.Length);
                }
            }
        }

        /// <summary>
        /// Returns a embedded resource inside an <see cref="Assembly" /> as an UTF-8 string.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="type">The type that is used for the namespace of the resource.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <returns>The string or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static string GetManifestResourceString(this Assembly asm, Type type, string name)
        {
            return GetManifestResourceString(asm: asm,
                                             type: type,
                                             name: name,
                                             enc: Encoding.UTF8);
        }

        /// <summary>
        /// Returns a embedded resource inside an <see cref="Assembly" /> as a string.
        /// </summary>
        /// <param name="asm">The assembly where the resource is stored.</param>
        /// <param name="type">The type that is used for the namespace of the resource.</param>
        /// <param name="name">The name / relative path of the resource.</param>
        /// <param name="enc">The charset to use.</param>
        /// <returns>The string or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" />, <paramref name="type" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public static string GetManifestResourceString(this Assembly asm, Type type, string name, Encoding enc)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var ns = type.Namespace;
            if (string.IsNullOrEmpty(ns) == false)
            {
                return GetManifestResourceString(asm: asm,
                                                 name: string.Format("{0}.{1}",
                                                                     ns,
                                                                     name),
                                                 enc: enc);
            }

            return GetManifestResourceString(asm: asm,
                                             name: name,
                                             enc: enc);
        }

        #endregion Methods (6)
    }
}