// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

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
            if ((returnType == null) ||
                typeof(void).Equals(returnType))
            {
                // action

                ForEach(_KNOWN_ACTION_TYPES,
                        (ctx) =>
                        {
                            var type = ctx.Item;

                            if (type.GetGenericArguments().Length == ctx.State.Parameters.Length)
                            {
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
                // function
                delegateReturnType = returnType;

                ForEach(_KNOWN_ACTION_TYPES,
                        (ctx) =>
                        {
                            var type = ctx.Item;

                            if (type.GetGenericArguments().Length ==
                                (ctx.State.Parameters.Length + 1))
                            {
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
                var delegateTypesForGenericArgs = new List<Type>();
                ForEach(@params,
                        (ctx) =>
                        {
                            ctx.State
                               .DelegateTypesForGenericArgs.Add(ctx.Item.ParameterType);
                        }, actionState: new
                        {
                            DelegateTypesForGenericArgs = delegateTypesForGenericArgs,
                        });

                if (delegateReturnType != null)
                {
                    delegateTypesForGenericArgs.Add(delegateReturnType);
                }

                result = delegateType.MakeGenericType(delegateTypesForGenericArgs.ToArray());
            }

            return result;
        }

        #endregion Methods
    }
}