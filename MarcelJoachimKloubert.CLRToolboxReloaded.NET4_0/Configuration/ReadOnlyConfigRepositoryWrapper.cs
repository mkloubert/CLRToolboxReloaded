// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    #region CLASS: ReadOnlyConfigRepositoryWrapper<TConf>

    /// <summary>
    /// A read-only wrapper for an <see cref="IConfigRepository" />.
    /// </summary>
    public class ReadOnlyConfigRepositoryWrapper<TConf> : ConfigRepositoryWrapper<TConf> where TConf : global::MarcelJoachimKloubert.CLRToolbox.Configuration.IConfigRepository
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyConfigRepositoryWrapper{TConf}" /> class.
        /// </summary>
        /// <param name="innerConf">The base config repository.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerConf" /> is <see langword="null" />.
        /// </exception>
        public ReadOnlyConfigRepositoryWrapper(TConf innerConf)
            : base(innerConf: innerConf,
                   prefix: null)
        {
        }

        #endregion CLASS: ReadOnlyConfigRepositoryWrapper<TConf>

        #region Properties (2)

        /// <inheriteddoc />
        public sealed override bool CanRead
        {
            get { return this._INNER_CONF.CanRead; }
        }

        /// <inheriteddoc />
        public sealed override bool CanWrite
        {
            get { return false; }
        }

        #endregion Properties
    }

    #endregion

    #region CLASS: ReadOnlyConfigRepositoryWrapper

    /// <summary>
    /// A simple implementation of <see cref="ReadOnlyConfigRepositoryWrapper{TConf}" /> class.
    /// </summary>
    public sealed class ReadOnlyConfigRepositoryWrapper : ReadOnlyConfigRepositoryWrapper<global::MarcelJoachimKloubert.CLRToolbox.Configuration.IConfigRepository>
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyConfigRepositoryWrapper"/> class.
        /// </summary>
        /// <param name="innerConf">The base config repository.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerConf" /> is <see langword="null" />.
        /// </exception>
        public ReadOnlyConfigRepositoryWrapper(IConfigRepository innerConf)
            : base(innerConf: innerConf)
        {
        }

        #endregion Constructors
    }

    #endregion
}