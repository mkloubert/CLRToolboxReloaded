// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Better disposing.
        /// </summary>
        /// <param name="obj">The object to dispose.</param>
        /// <returns>
        /// <see cref="IDisposable.Dispose()" /> method was called or not.
        /// <see langword="null" /> indicates that <paramref name="obj" /> is also <see langword="null" />.
        /// </returns>
        public static bool? DisposeEx(this IDisposable obj)
        {
            bool? result = null;

            if (obj != null)
            {
                result = false;

                var doDispose = true;
                if (obj is IDisposableObject)
                {
                    doDispose = ((IDisposableObject)obj).IsDisposed == false;
                }

                if (doDispose)
                {
                    obj.Dispose();
                    result = true;
                }
            }

            return result;
        }

        #endregion Methods
    }
}