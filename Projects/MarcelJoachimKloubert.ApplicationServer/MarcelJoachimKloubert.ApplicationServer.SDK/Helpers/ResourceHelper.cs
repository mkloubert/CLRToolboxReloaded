// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.ApplicationServer.Helpers
{
    /// <summary>
    /// Helper class for resource operations.
    /// </summary>
    public static class ResourceHelper
    {
        #region Methods (1)

        /// <summary>
        /// Tries to return a resource stream from an assembly.
        /// </summary>
        /// <param name="asm">The underlying assembly.</param>
        /// <param name="baseType">The main / base type.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>
        /// The stream or <see langword="null" /> if not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> and/or <see cref="baseType" /> are <see langword="null" />.
        /// </exception>
        public static Stream GetManifestResourceStream(Assembly asm, Type baseType, string resourceName)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }

            var attrib = asm.GetCustomAttribute<global::MarcelJoachimKloubert.CLRToolbox.Resources.ResourceRootNamespaceAttribute>();

            string ns;
            if (attrib != null)
            {
                ns = attrib.Namespace;
            }
            else
            {
                ns = string.Format("{0}{1}Resources",
                                   baseType.Namespace,
                                   string.IsNullOrWhiteSpace(baseType.Namespace) ? string.Empty : ".");
            }

            var fullName = string.Format("{0}{1}{2}",
                                         ns,
                                         string.IsNullOrWhiteSpace(ns) ? string.Empty : ".",
                                         resourceName);

            var existingResourceName = asm.GetManifestResourceNames()
                                          .FirstOrDefault(n => n == fullName);

            return existingResourceName != null ? asm.GetManifestResourceStream(existingResourceName) : null;
        }

        #endregion Methods (1)
    }
}