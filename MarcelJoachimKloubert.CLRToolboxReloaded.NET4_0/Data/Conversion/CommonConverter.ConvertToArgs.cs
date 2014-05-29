// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Conversion
{
    partial class CommonConverter
    {
        #region CLASS: ConvertToArgs

        private sealed class ConvertToArgs : ObjectBase, IConvertToArgs
        {
            #region Properts (3)

            public object CurrentValue
            {
                get;
                internal set;
            }

            public IFormatProvider FormatProvider
            {
                get;
                internal set;
            }

            public Type TargetType
            {
                get;
                internal set;
            }

            #endregion Properts (3)

            #region Methods (1)

            public T GetCurrentValue<T>()
            {
                return GlobalConverter.Current
                                      .ChangeType<T>(this.CurrentValue);
            }

            #endregion Methods (1)
        }

        #endregion CLASS: ConvertToArgs
    }
}