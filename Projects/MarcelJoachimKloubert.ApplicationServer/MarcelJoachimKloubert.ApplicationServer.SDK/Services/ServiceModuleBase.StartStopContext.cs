// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.ApplicationServer.Services
{
    partial class ServiceModuleBase
    {
        /// <summary>
        /// List of invokation contextes for <see cref="ServiceModuleBase.OnStart(StartStopContext, ref bool)" />
        /// and <see cref="ServiceModuleBase.OnStop(StartStopContext, ref bool)" /> methods.
        /// </summary>
        protected enum StartStopContext
        {
            /// <summary>
            /// Invoked from <see cref="ServiceModuleBase.Dispose()" /> method.
            /// </summary>
            Dispose,
            
            /// <summary>
            /// Invoked from finalizer of <see cref="ServiceModuleBase" /> class.
            /// </summary>
            Finalizer,

            /// <summary>
            /// Invoked from <see cref="ServiceModuleBase.Restart()" /> method.
            /// </summary>
            Restart,

            /// <summary>
            /// Invoked from <see cref="ServiceModuleBase.Start()" /> method.
            /// </summary>
            Start,

            /// <summary>
            /// Invoked from <see cref="ServiceModuleBase.Stop()" /> method.
            /// </summary>
            Stop,
        }
    }
}