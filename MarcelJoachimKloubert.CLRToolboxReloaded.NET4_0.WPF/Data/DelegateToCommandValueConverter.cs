// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Commands;
using MarcelJoachimKloubert.CLRToolbox.Windows.Input;
using System;
using System.Globalization;
using System.Windows.Input;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Data
{
    /// <summary>
    /// Converts a delegate to an WPF <see cref="ICommand" /> object.
    /// </summary>
    public class DelegateToCommandValueConverter : ValueConverterBase<Delegate, ICommand, object>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public DelegateToCommandValueConverter(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public DelegateToCommandValueConverter(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public DelegateToCommandValueConverter(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public DelegateToCommandValueConverter()
            : base()
        {
        }

        #endregion Constrcutors

        #region Methods (1)

        /// <inheriteddoc />
        protected override ICommand OnConvert(Delegate @delegate, object parameter, CultureInfo culture)
        {
            ICommand result = null;

            if (@delegate != null)
            {
                var method = @delegate.Method;
                if (method != null)
                {
                    var @params = method.GetParameters();
                    if (@params.Length < 2)
                    {
                        var obj = @delegate.Target;

                        if (@params.Length == 0)
                        {
                            result = new SimpleCommand(new DelegateCommand.ExecuteHandlerNoParameter(delegate()
                            {
                                method.Invoke(obj,
                                              new object[0]);
                            }));
                        }
                        else
                        {
                            // one parameter

                            result = new SimpleCommand(new DelegateCommand.ExecuteHandler(delegate(object cp)
                            {
                                method.Invoke(obj,
                                              new object[] { parameter });
                            }));
                        }
                    }
                    else
                    {
                        throw new InvalidCastException();
                    }
                }
            }

            return result;
        }

        #endregion Methods
    }
}