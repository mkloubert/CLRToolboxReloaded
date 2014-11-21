// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if (PORTABLE45)
#define KNOWS_RUNTIME_REFLECTION_EXTENSIONS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Forces adding a header without validating it.
        /// </summary>
        /// <param name="coll">The target collection.</param>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="coll" /> is <see langword="null" />.
        /// </exception>
        public static void ForceAdd(this WebHeaderCollection coll, string name, string value)
        {
            if (coll == null)
            {
                throw new ArgumentNullException("coll");
            }

            IEnumerable<MethodBase> methods;
#if KNOWS_RUNTIME_REFLECTION_EXTENSIONS
            methods = coll.GetType()
                          .GetRuntimeMethods();
#else
            methods = coll.GetType()
                          .GetMethods(global::System.Reflection.BindingFlags.Instance |
                                      global::System.Reflection.BindingFlags.NonPublic);
#endif

            var method = methods.First(m =>
            {
                if (m.Name != "AddWithoutValidate")
                {
                    return false;
                }

                var p = m.GetParameters();
                return (p.Length == 2) &&
                       p[0].ParameterType.Equals(typeof(string)) &&
                       p[1].ParameterType.Equals(typeof(string));
            });

            method.Invoke(obj: coll,
                          parameters: new object[] { name, value });
        }

        #endregion Methods (1)
    }
}