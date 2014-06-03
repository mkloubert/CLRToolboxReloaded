// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Data
{
    #region CLASS: DataConnectionBase

    /// <summary>
    /// Basic object that stores the connection string for a data connection.
    /// </summary>
    public abstract class DataConnectionBase : ObjectBase, IDataConnection
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this.ConnectionString = connStr.AsString();
        }

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr, bool isSynchronized)
            : this(connStr: connStr,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr, object sync)
            : this(connStr: connStr,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr)
            : this(connStr: connStr,
                   isSynchronized: false)
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public string ConnectionString
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public abstract Type Provider
        {
            get;
        }

        #endregion Properties
    }

    #endregion

    #region CLASS: DataConnectionBase<P>

    /// <summary>
    /// Basic object that stores the connection string for a data connection.
    /// </summary>
    /// <typeparam name="P">The value for <see cref="DataConnectionBase{P}.Provider" /> property.</typeparam>
    public abstract class DataConnectionBase<P> : DataConnectionBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr, bool isSynchronized, object sync)
            : base(connStr: connStr,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr, bool isSynchronized)
            : base(connStr: connStr,
                   isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr, object sync)
            : base(connStr: connStr,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DataConnectionBase(IEnumerable<char> connStr)
            : base(connStr: connStr)
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <inheriteddoc />
        public override sealed Type Provider
        {
            get { return typeof(P); }
        }

        #endregion Properties
    }

    #endregion
}