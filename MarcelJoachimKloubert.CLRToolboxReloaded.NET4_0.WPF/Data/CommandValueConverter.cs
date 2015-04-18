// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Data
{
    /// <summary>
    /// Simple implementation of <see cref="CommandValueConverterBase{TParam}" /> class.
    /// </summary>
    public sealed class CommandValueConverter : CommandValueConverterBase<object>
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public CommandValueConverter(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public CommandValueConverter(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public CommandValueConverter(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public CommandValueConverter()
            : base()
        {
        }

        #endregion Constructors (4)
    }
}