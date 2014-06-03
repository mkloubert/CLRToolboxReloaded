// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.ServiceLocation
{
    partial class DelegateServiceLocator
    {
        #region Nested Classes (1)

        private sealed class InstanceProvider
        {
            #region Fields (2)

            internal readonly Delegate PROVIDER;
            internal readonly Type TYPE;

            #endregion Fields

            #region Constructors (1)

            internal InstanceProvider(Type type, Delegate provider)
            {
                this.TYPE = type;
                this.PROVIDER = provider;
            }

            #endregion Constructors

            #region Methods (1)

            internal T Invoke<T>(IServiceLocator baseLocator, object key)
            {
                return GlobalConverter.Current
                                      .ChangeType<T>(value: this.PROVIDER
                                                                .Method
                                                                .Invoke(this.PROVIDER.Target,
                                                                        new object[] { baseLocator, key }));
            }

            #endregion Methods
        }

        #endregion Nested Classes
    }
}