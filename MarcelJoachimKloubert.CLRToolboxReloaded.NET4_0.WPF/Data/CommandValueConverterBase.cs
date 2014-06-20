// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Commands;
using MarcelJoachimKloubert.CLRToolbox.Windows.Input;
using System.Globalization;
using System.Windows.Input;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Data
{
    /// <summary>
    /// Basic value converter that converts <see cref="global::MarcelJoachimKloubert.CLRToolbox.Execution.Commands.ICommand{TParam}" /> to
    /// <see cref="global::System.Windows.Input.ICommand" /> and back.
    /// </summary>
    /// <typeparam name="TParam">Type of the command parameters.</typeparam>
    public abstract class CommandValueConverterBase<TParam> : ValueConverterBase<global::MarcelJoachimKloubert.CLRToolbox.Execution.Commands.ICommand<TParam>, global::System.Windows.Input.ICommand, TParam>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected CommandValueConverterBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CommandValueConverterBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected CommandValueConverterBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CommandValueConverterBase()
            : base()
        {
        }

        #endregion Constrcutors

        #region Methods (2)

        /// <inheriteddoc />
        protected override sealed ICommand OnConvert(ICommand<TParam> inputCmd, TParam parameter, CultureInfo culture)
        {
            ICommand result = null;

            if (inputCmd != null)
            {
                result = inputCmd as ICommand;
                if (result == null)
                {
                    // needs wrapper

                    result = new SimpleCommand<TParam>(
                        (p) =>
                        {
                            inputCmd.Execute(parameter);
                        },
                        (p) =>
                        {
                            return inputCmd.CanExecute(parameter);
                        });
                }
            }

            return result;
        }

        /// <inheriteddoc />
        protected override sealed ICommand<TParam> OnConvertBack(ICommand inputCmd, TParam parameter, CultureInfo culture)
        {
            ICommand<TParam> result = null;

            if (inputCmd != null)
            {
                result = inputCmd as ICommand<TParam>;
                if (result == null)
                {
                    // needs wrapper

                    result = new SimpleCommand<TParam>(
                        (p) =>
                        {
                            inputCmd.Execute(parameter);
                        },
                        (p) =>
                        {
                            return inputCmd.CanExecute(parameter);
                        });
                }
            }

            return result;
        }

        #endregion Methods
    }
}