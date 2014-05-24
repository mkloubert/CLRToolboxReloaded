// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_DO_REMOTING
#define CAN_HANDLE_THREADS
#define CAN_SERIALIZE
#endif

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// Simple implementation of <see cref="ILogMessage" /> interface.
    /// </summary>
#if CAN_SERIALIZE
    [global::System.Serializable]
#endif
    public class LogMessage : IdentifiableBase, ILogMessage
    {
        #region Fields (1)

        private Guid _id;

        #endregion Fields (1)

        #region Constrcutors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage" /> class..
        /// </summary>
        /// <param name="id">The (initial) value for the <see cref="LogMessage.Id" /> property.</param>
        public LogMessage(Guid id)
            : base(synchronized: false)
        {
            this._id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage" /> class..
        /// </summary>
        public LogMessage()
            : this(id: Guid.NewGuid())
        {
        }

        #endregion Constrcutors (2)

        #region Properties (10)

        /// <inheriteddoc />
        public Assembly Assembly
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public LoggerFacadeCategories Categories
        {
            get;
            set;
        }

#if CAN_DO_REMOTING

        /// <inheriteddoc />
        public global::System.Runtime.Remoting.Contexts.Context Context
        {
            get;
            set;
        }

#endif

        /// <inheriteddoc />
        public sealed override Guid Id
        {
            get { return this._id; }
        }

        /// <inheriteddoc />
        public string LogTag
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public MemberInfo Member
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public object Message
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IPrincipal Principal
        {
            get;
            set;
        }

#if CAN_HANDLE_THREADS

        /// <inheriteddoc />
        public global::System.Threading.Thread Thread
        {
            get;
            set;
        }

#endif

        /// <inheriteddoc />
        public DateTimeOffset Time
        {
            get;
            set;
        }

        #endregion Properties (10)

        #region Methods (4)

        /// <inheriteddoc />
        public virtual T GetMessage<T>()
        {
            IFormatProvider provider = null;
#if CAN_HANDLE_THREADS

            try
            {
                var t = this.Thread;
                if (t != null)
                {
                    provider = t.CurrentCulture;
                }
            }
            catch
            {
            }

#endif

            return GlobalConverter.Current
                                  .ChangeType<T>(value: this.Message,
                                                 provider: provider);
        }

        /// <inheriteddoc />
        public bool HasAllCategories(IEnumerable<LoggerFacadeCategories> categories)
        {
            return categories.All(c => this.Categories
                                           .HasFlag(c));
        }

        /// <inheriteddoc />
        public bool HasAllCategories(params LoggerFacadeCategories[] categories)
        {
            return this.HasAllCategories((IEnumerable<LoggerFacadeCategories>)categories);
        }

        /// <summary>
        /// Sets the value for the <see cref="LogMessage.Id" /> property.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public void SetId(Guid newValue)
        {
            this._id = newValue;
        }

        #endregion Methods (4)
    }
}