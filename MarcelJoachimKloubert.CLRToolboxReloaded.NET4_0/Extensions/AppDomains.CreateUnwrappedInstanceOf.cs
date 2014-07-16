// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Creates a new instance of an object in a specific app domain for use in the current one.
        /// </summary>
        /// <typeparam name="TResult">The target type to return.</typeparam>
        /// <param name="domain">The application domain where the instance should be created.</param>
        /// <returns>The created (proxy) instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domain" /> is <see langword="null" />.
        /// </exception>
        public static TResult CreateUnwrappedInstanceOf<TResult>(this AppDomain domain)
        {
            return CreateUnwrappedInstanceOf<TResult, TResult>(domain: domain);
        }

        /// <summary>
        /// Creates a new instance of an object in a specific app domain for use in the current one.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <typeparam name="TResult">The target type to return.</typeparam>
        /// <param name="domain">The application domain where the instance should be created.</param>
        /// <returns>The created (proxy) instance of.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domain" /> is <see langword="null" />.
        /// </exception>
        public static TResult CreateUnwrappedInstanceOf<T, TResult>(this AppDomain domain)
            where TResult : T
        {
            return (TResult)CreateUnwrappedInstanceOf(domain, typeof(T));
        }

        /// <summary>
        /// Creates a new instance of an object in a specific app domain for use in the current one.
        /// </summary>
        /// <param name="domain">The application domain where the instance should be created.</param>
        /// <param name="type">The type that should be created.</param>
        /// <returns>The created (proxy) instance of <paramref name="type" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domain" /> and/or <paramref name="type" /> are <see langword="null" />.
        /// </exception>
        public static object CreateUnwrappedInstanceOf(this AppDomain domain, Type type)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return domain.CreateInstanceAndUnwrap(type.Assembly.FullName,
                                                  type.FullName);
        }

        #endregion Methods (3)
    }
}