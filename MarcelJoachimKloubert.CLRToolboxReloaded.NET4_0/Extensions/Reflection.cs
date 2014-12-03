// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if (PORTABLE45)
#define GET_GENERIC_TYPE_ARGUMENTS_FROM_PROPERTY
#define GET_TYPES_OF_ASSEMBLY_FROM_PROPERTY
#endif

#if !(PORTABLE45)
#define GET_ASSEMBLY_OF_TYPE_FROM_PROPERTY
#endif

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (4)

        private static Assembly GetAssembly(Type type)
        {
#if GET_ASSEMBLY_OF_TYPE_FROM_PROPERTY
            return type.Assembly;
#else
            return Assembly.Load(new AssemblyName(type.AssemblyQualifiedName));
#endif
        }

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

        private static IEnumerable<Type> GetTypes(Assembly asm)
        {
#if GET_TYPES_OF_ASSEMBLY_FROM_PROPERTY
            return asm.ExportedTypes;
#else
            return asm.GetTypes();
#endif
        }

        #endregion Methods (4)
    }
}