// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    partial class HttpServerBase
    {
        #region Nested C#classes (1)

        /// <summary>
        /// List of invokation contextes for <see cref="HttpServerBase.OnStart(StartStopContext, ref bool)" />
        /// and <see cref="HttpServerBase.OnStop(StartStopContext, ref bool)" /> methods.
        /// </summary>
        protected enum StartStopContext
        {
            /// <summary>
            /// Invoked from <see cref="DisposableObjectBase.Dispose()" /> method.
            /// </summary>
            Dispose,

            /// <summary>
            /// Invoked from <see cref="HttpServerBase.Restart()" /> method.
            /// </summary>
            Restart,

            /// <summary>
            /// Invoked from <see cref="HttpServerBase.Start()" /> method.
            /// </summary>
            Start,

            /// <summary>
            /// Invoked from <see cref="HttpServerBase.Stop()" /> method.
            /// </summary>
            Stop,
        }

        #endregion Nested C#classes (1)
    }
}