// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IConverter.ChangeType{T}(object, IFormatProvider)" />
        public static T ChangeTo<T>(this object obj, IFormatProvider provider = null)
        {
            return GlobalConverter.Current
                                  .ChangeType<T>(obj, provider);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IConverter.ChangeType(Type, object, IFormatProvider)" />
        public static object ChangeTo(this object obj, Type targetType, IFormatProvider provider = null)
        {
            return GlobalConverter.Current
                                  .ChangeType(targetType, obj, provider);
        }

        #endregion Methods (2)
    }
}