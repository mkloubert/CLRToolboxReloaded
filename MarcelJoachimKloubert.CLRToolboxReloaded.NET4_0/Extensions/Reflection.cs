// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        private static string GetFullResourceName(Type type, string name)
        {
            var ns = type.Namespace;

            return string.IsNullOrEmpty(ns) ? name
                                            : string.Format("{0}.{1}",
                                                            ns, name);
        }

        #endregion Methods (1)
    }
}