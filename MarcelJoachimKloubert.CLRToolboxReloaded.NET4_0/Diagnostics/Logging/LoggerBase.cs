// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_DO_REMOTING
#define CAN_GET_ASSEMBLY_BY_METHOD
#define CAN_HANDLE_THREADS
#define CAN_SERIALIZE
#define KNOWS_DBNULL
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A basic logger.
    /// </summary>
    public abstract class LoggerBase : ObjectBase, ILogger
    {
        #region Fields (1)

        private readonly Func<ILogMessage, bool> _LOG_METHOD;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class..
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected LoggerBase(bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
            this._LOG_METHOD = this._SYNCHRONIZED ? new Func<ILogMessage, bool>(this.OnLog_ThreadSafe)
                                                  : new Func<ILogMessage, bool>(this.OnLog_NonThreadSafe);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class..
        /// </summary>
        /// <param name="synchronized">The value for the <see cref="ObjectBase.Synchronized" /> property.</param>
        protected LoggerBase(bool synchronized)
            : this(synchronized: synchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class..
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected LoggerBase(object sync)
            : this(sync: sync,
                   synchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class..
        /// </summary>
        protected LoggerBase()
            : this(sync: new object())
        {
        }

        #endregion Constrcutors (4)

        #region Methods (5)

        /// <inheriteddoc />
        public bool Log(ILogMessage msgObj)
        {
            if (msgObj == null)
            {
                throw new ArgumentNullException("msgObj");
            }

            try
            {
                return this._LOG_METHOD(msgObj);
            }
            catch
            {
                return false;
            }
        }

        /// <inheriteddoc />
        public bool Log(object msg,
                        IEnumerable<char> tag = null,
                        LoggerFacadeCategories categories = LoggerFacadeCategories.None)
        {
            var msgObj = new LogMessage();
            msgObj.Time = DateTimeOffset.Now;
#if CAN_GET_ASSEMBLY_BY_METHOD
            msgObj.Assembly = global::System.Reflection.Assembly.GetCallingAssembly();
#endif
            msgObj.Categories = categories;
            msgObj.LogTag = tag.AsString();

            MemberInfo member = null;

#if CAN_HANDLE_THREADS

            msgObj.Thread = global::System.Threading.Thread.CurrentThread;

            try
            {
#pragma warning disable 618
                member = new global::System.Diagnostics.StackTrace(msgObj.Thread, false).GetFrame(2)
                                                                                        .GetMethod();
#pragma warning restore 618
            }
            catch
            {
                // ignore errors here
            }

#if CAN_DO_REMOTING

            try
            {
                msgObj.Context = global::System.Threading.Thread.CurrentContext;
            }
            catch
            {
                // ignore errors here
            }

#endif

            try
            {
                msgObj.Principal = Thread.CurrentPrincipal;
            }
            catch
            {
                // ignore errors here
            }

#endif

            // normalize log tag
            msgObj.LogTag = string.IsNullOrWhiteSpace(msgObj.LogTag) ? null
                                                                     : msgObj.LogTag.ToUpper().Trim();

#if KNOWS_DBNULL

            if (global::System.DBNull.Value.Equals(msg))
            {
                msg = null;
            }

#endif

            if (msg is IEnumerable<char>)
            {
                msg = msg.AsString();
            }
            else if (msg is IEnumerable<byte>)
            {
                msg = ((IEnumerable<byte>)msg).AsArray();
            }

            msgObj.Member = member;
            msgObj.Message = msg;

            return this.Log(msgObj);
        }

        /// <summary>
        /// The logic for the <see cref="LoggerBase.Log(ILogMessage)" /> method.
        /// </summary>
        /// <param name="msgObj">The message data.</param>
        /// <param name="succeeded">
        /// The result for <see cref="LoggerBase.Log(ILogMessage)" /> method.
        /// Is <see langword="true" /> by default.
        /// </param>
        protected abstract void OnLog(ILogMessage msgObj, ref bool succeeded);

        private bool OnLog_NonThreadSafe(ILogMessage msgObj)
        {
            var result = true;
            this.OnLog(msgObj, ref result);

            return result;
        }

        private bool OnLog_ThreadSafe(ILogMessage msgObj)
        {
            bool result;

            lock (this._SYNC)
            {
                result = this.OnLog_NonThreadSafe(msgObj);
            }

            return result;
        }

        #endregion Methods (5)
    }
}