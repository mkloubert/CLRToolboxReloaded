// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_REFLECTION
#endif

using System;
using System.Collections.Generic;
using System.Linq;

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

        #region Constrcutors(1)

        static ClrToolboxExtensionMethods()
        {
            // known actions and functions
            {
                var actionTypeList = new HashSet<Type>();
                var funcTypeList = new HashSet<Type>();

#if CAN_REFLECTION

                var actionType = typeof(global::System.Action);
                var actionAsm = actionType.Assembly;

                ForEach(actionAsm.GetTypes(),
                        (ctx) =>
                        {
                            var type = ctx.Item;

                            if (type.FullName.StartsWith("System.Action"))
                            {
                                ctx.State
                                   .ActionTypes.Add(type);
                            }
                        }, actionState: new
                        {
                            ActionTypes = actionTypeList,
                        });

                var funcType = typeof(global::System.Func<>);
                var funcAsm = funcType.Assembly;

                ForEach(funcAsm.GetTypes(),
                        (ctx) =>
                        {
                            var type = ctx.Item;

                            if (type.FullName.StartsWith("System.Func"))
                            {
                                ctx.State
                                   .FuncTypes.Add(type);
                            }
                        }, actionState: new
                        {
                            FuncTypes = funcTypeList,
                        });

#endif

                _KNOWN_ACTION_TYPES = actionTypeList.ToArray();
                _KNOWN_FUNC_TYPES = funcTypeList.ToArray();
            }
        }

        #endregion
    }
}