// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if (PORTABLE45)
#define GET_GENERIC_TYPE_ARGUMENTS_FROM_PROPERTY
#define GET_TYPES_OF_ASSEMBLY_FROM_PROPERTY
#endif

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        private static string GetFullResourceName(Type type, string name)
        {
            var ns = type.Namespace;

            return string.IsNullOrEmpty(ns) ? name
                                            : string.Format("{0}.{1}",
                                                            ns, name);
        }

        private static IEnumerable<Type> GetGenericTypeArguments(Type type)
        {
#if GET_GENERIC_TYPE_ARGUMENTS_FROM_PROPERTY
            return type.GenericTypeArguments;
#else
            return type.GetGenericArguments();
#endif
        }

        #endregion Methods (2)
    }
}