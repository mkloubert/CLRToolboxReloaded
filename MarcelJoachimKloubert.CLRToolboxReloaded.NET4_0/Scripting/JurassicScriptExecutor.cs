// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de

using Jurassic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting
{
    /// <summary>
    /// Script executor based on Jurassic engine.
    /// </summary>
    public class JurassicScriptExecutor : ScriptExecutorBase
    {
        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnExecute(ScriptExecutorBase.OnExecuteContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Source))
            {
                return;
            }

            var engine = new ScriptEngine();
            engine.CompatibilityMode = CompatibilityMode.Latest;
            engine.EnableDebugging = context.IsDebug;
            engine.EnableExposedClrTypes = true;

            // global functions
            this._FUNCS
                .ForEach(ctx =>
                {
                    ctx.State
                       .Engine.SetGlobalFunction(functionName: ctx.Item.Key,
                                                 functionDelegate: ctx.Item.Value);
                }, actionState: new
                {
                    Engine = engine,
                });

            // variables
            this._VARS
                .ForEach(ctx =>
                {
                    ctx.State
                       .Engine.SetGlobalValue(variableName: ctx.Item.Key,
                                              value: ctx.Item.Value);
                }, actionState: new
                {
                    Engine = engine,
                });

            engine.Execute(code: context.Source);
        }

        #endregion Methods (1)
    }
}