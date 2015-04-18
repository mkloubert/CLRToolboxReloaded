// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if (PORTABLE45)
#define ACTIONS_AND_FUNCS_FROM_ONE_ASSEMBLY
#endif

using MarcelJoachimKloubert.CLRToolbox.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    /// <summary>
    /// Common extension methods.
    /// </summary>
    public static partial class ClrToolboxExtensionMethods
    {
        #region Fields (2)

        private static Type[] _KNOWN_ACTION_TYPES;
        private static Type[] _KNOWN_FUNC_TYPES;

        #endregion Fields (2)

        #region Constructors (1)

        static ClrToolboxExtensionMethods()
        {
            var asms = new HashSet<Assembly>();
            asms.Add(ReflectionHelper.GetAssembly(typeof(Action)));
#if !ACTIONS_AND_FUNCS_FROM_ONE_ASSEMBLY
            asms.Add(ReflectionHelper.GetAssembly(typeof(Action<,,,,,,,,>)));
#endif

            // known actions
            _KNOWN_ACTION_TYPES = asms.SelectMany(a => ReflectionHelper.GetTypes(a))
                                      .Where(t => t.FullName.StartsWith("System.Action"))
                                      .Distinct()
                                      .OrderBy(t => GetGenericTypeArguments(t).Count())
                                      .ThenBy(t => t.FullName)
                                      .ToArray();

            // known functions
            _KNOWN_FUNC_TYPES = asms.SelectMany(a => ReflectionHelper.GetTypes(a))
                                    .Where(t => t.FullName.StartsWith("System.Func"))
                                    .Distinct()
                                    .OrderBy(t => GetGenericTypeArguments(t).Count())
                                    .ThenBy(t => t.FullName)
                                    .ToArray();
        }

        #endregion
    }
}