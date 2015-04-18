// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_DO_REMOTING
#define CAN_GET_ASSEMBLY_BY_METHOD
#define CAN_HANDLE_THREADS
#define KNOWS_DBNULL
#endif

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.Execution;
using MarcelJoachimKloubert.CLRToolbox.Execution.Commands;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

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

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected LoggerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this._LOG_METHOD = this._IS_SYNCHRONIZED ? new Func<ILogMessage, bool>(this.OnLog_ThreadSafe)
                                                     : new Func<ILogMessage, bool>(this.OnLog_NonThreadSafe);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        protected LoggerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class.
        /// </summary>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected LoggerBase(object sync)
            : this(sync: sync,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class.
        /// </summary>
        protected LoggerBase()
            : this(sync: new object())
        {
        }

        #endregion Constructors (4)

        #region Methods (7)

        /// <summary>
        /// Creates a clone of a message.
        /// </summary>
        /// <param name="src">The source object.</param>
        /// <returns>
        /// The cloned object or <see langword="null" />
        /// if <paramref name="src" /> is also <see langword="null" />.</returns>
        protected static LogMessage CloneLogMessage(ILogMessage src)
        {
            return CloneLogMessageInner(src);
        }

        private static LogMessage CloneLogMessageInner(ILogMessage src)
        {
            if (src == null)
            {
                return null;
            }

            var result = new LogMessage(id: src.Id)
            {
                Assembly = src.Assembly,
                Categories = src.Categories,
                LogTag = src.LogTag,
                Member = src.Member,
                Message = src.Message,
                Time = src.Time,
            };

#if CAN_HANDLE_THREADS

            result.Principal = src.Principal;
            result.Thread = src.Thread;

#if CAN_DO_REMOTING
            result.Context = src.Context;
#endif

#endif

            return result;
        }

        /// <summary>
        /// Creates a copy of a <see cref="ILogMessage" /> object with a new ID and a new value.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="msgVal">The value for the <see cref="ILogMessage.Message" /> property of the copy.</param>
        /// <returns>
        /// The copy of <paramref name="src" /> or <see langword="null" />
        /// if <paramref name="src" /> is also <see langword="null" />.
        /// </returns>
        protected static LogMessage CreateCopyOfLogMessage(ILogMessage src, object msgVal)
        {
            if (src == null)
            {
                return null;
            }

            var result = CloneLogMessageInner(src);
            result.SetId(newValue: Guid.NewGuid());
            result.Message = msgVal;

            return result;
        }

        /// <inheriteddoc />
        public bool Log(ILogMessage msgObj)
        {
            if (msgObj == null)
            {
                throw new ArgumentNullException("msgObj");
            }

            try
            {
                return this._LOG_METHOD(CloneLogMessage(msgObj));
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);

                return false;
            }
        }

        /// <inheriteddoc />
        public bool Log(object msg,
                        string tag = null,
                        LogCategories categories = LogCategories.None)
        {
            var msgObj = new LogMessage();
            msgObj.Time = AppTime.Now;
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
                msgObj.Principal = global::System.Threading.Thread.CurrentPrincipal;
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

            ILogMessage messageToLog = msgObj;

            // define message object to log
            {
                bool checkAgain;
                do
                {
                    checkAgain = false;

                    var orgMsgObj = messageToLog;
                    if (orgMsgObj == null)
                    {
                        break;
                    }

                    if (orgMsgObj.Message is ILogCommand)
                    {
                        var cmd = orgMsgObj.Message as ILogCommand;

                        messageToLog = null;
                        if (cmd.CanExecute(orgMsgObj))
                        {
                            var result = cmd.Execute(orgMsgObj);
                            if (result != null)
                            {
                                if (result.HasFailed)
                                {
                                    return false;
                                }

                                if (result.DoLogMessage)
                                {
                                    // send 'result.MessageValueToLog'
                                    // to "real" logger logic

                                    messageToLog = CreateCopyOfLogMessage(orgMsgObj,
                                                                          result.MessageValueToLog);
                                }
                            }
                        }

                        // maybe 'messageToLog.Message' can also be a log command
                        checkAgain = true;
                    }
                    else if (orgMsgObj.Message is ICommand<ILogMessage>)
                    {
                        var cmd = orgMsgObj.Message as ICommand<ILogMessage>;

                        messageToLog = null;
                        if (cmd.CanExecute(orgMsgObj))
                        {
                            if (cmd.Execute(orgMsgObj).IsFalse())
                            {
                                return false;
                            }
                        }
                    }
                }
                while (checkAgain);
            }

            if (messageToLog == null)
            {
                // dummy log
                return true;
            }

            return this.Log(messageToLog);
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