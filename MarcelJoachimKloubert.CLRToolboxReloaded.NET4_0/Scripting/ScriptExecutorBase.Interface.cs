// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting
{
    partial class ScriptExecutorBase
    {
        #region Methods (4)

        IScriptExecutor IScriptExecutor.ExposeType<T>(string alias)
        {
            return this.ExposeType<T>(alias);
        }

        IScriptExecutor IScriptExecutor.ExposeType(Type type, string alias)
        {
            return this.ExposeType(type, alias);
        }

        IScriptExecutor IScriptExecutor.SetFunction(string funcName, Delegate func)
        {
            return this.SetFunction(funcName, func);
        }

        IScriptExecutor IScriptExecutor.SetVariable(string varName, object value)
        {
            return this.SetVariable(varName, value);
        }

        #endregion Methods
    }
}