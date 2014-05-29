// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    partial class NotifiableBase
    {
        #region CLASS: ReceiveValueFromArgs

        private sealed class ReceiveValueFromArgs : ObjectBase, IReceiveValueFromArgs
        {
            #region Properties (7)
            
            public bool AreDifferent
            {
                get
                {
                    return object.Equals(this.OldValue,
                                         this.NewValue) == false;
                }
            }

            public object NewValue
            {
                get;
                internal set;
            }

            public object OldValue
            {
                get;
                internal set;
            }

            public object Sender
            {
                get;
                internal set;
            }

            public string SenderName
            {
                get;
                internal set;
            }

            public int SenderType
            {
                get;
                internal set;
            }

            public Type TargetType
            {
                get;
                internal set;
            }

            #endregion Properties (7)

            #region Methods (3)

            public T GetNewValue<T>()
            {
                return GlobalConverter.Current
                                      .ChangeType<T>(value: this.NewValue);
            }

            public T GetOldValue<T>()
            {
                return GlobalConverter.Current
                                      .ChangeType<T>(value: this.OldValue);
            }

            public T GetSender<T>()
            {
                return GlobalConverter.Current
                                      .ChangeType<T>(value: this.Sender);
            }

            #endregion Methods (3)
        }

        #endregion CLASS: ReceiveValueFromArgs
    }
}