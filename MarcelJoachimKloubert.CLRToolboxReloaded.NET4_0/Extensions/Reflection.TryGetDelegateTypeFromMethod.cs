// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Tries to return the delegate type for a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The type or <see langword="null" /> if not found.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method" /> is <see langword="null" />.
        /// </exception>
        public static Type TryGetDelegateTypeFromMethod(this MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            Type result = null;

            var @params = method.GetParameters();

            Type delegateType = null;
            Type delegateReturnType = null;

            var returnType = method.ReturnType;
            if (typeof(void).Equals(returnType) ||
                (returnType == null))
            {
                // search for action type

                ForEach(_KNOWN_ACTION_TYPES,
                        (ctx) =>
                        {
                            var type = ctx.Item;

                            if (GetGenericTypeArguments(type).Count() ==
                                ctx.State.Parameters.Length)
                            {
                                // found

                                delegateType = type;
                                ctx.Cancel = true;
                            }
                        }, actionState: new
                        {
                            Parameters = @params,
                        });
            }
            else
            {
                // search for func type
                delegateReturnType = returnType;

                ForEach(_KNOWN_FUNC_TYPES,
                        (ctx) =>
                        {
                            var type = ctx.Item;

                            if (GetGenericTypeArguments(type).Count() ==
                                (ctx.State.Parameters.Length + 1))
                            {
                                // found

                                delegateType = type;
                                ctx.Cancel = true;
                            }
                        }, actionState: new
                        {
                            Parameters = @params,
                        });
            }

            if (delegateType != null)
            {
                if (GetGenericTypeArguments(delegateType).Count() > 0)
                {
                    var delegateTypesForGenericArgs = new List<Type>(@params.Select(p => p.ParameterType));
                    if (delegateReturnType != null)
                    {
                        delegateTypesForGenericArgs.Add(delegateReturnType);
                    }

                    result = delegateType.MakeGenericType(delegateTypesForGenericArgs.ToArray());
                }
                else
                {
                    result = delegateType;
                }
            }

            return result;
        }

        #endregion Methods (1)
    }
}