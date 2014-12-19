// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.ServiceLocation
{
    /// <summary>
    /// A basic object that locates service instances.
    /// </summary>
    public abstract partial class ServiceLocatorBase : ObjectBase, IServiceLocator
    {
        #region Constructors (2)

        /// <inheriteddoc />
        protected ServiceLocatorBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ServiceLocatorBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected ServiceLocatorBase(object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ServiceLocatorBase()
            : base(isSynchronized: true)
        {
        }

        #endregion Constructors

        #region Methods (11)

        /// <inheriteddoc />
        public IEnumerable<S> GetAllInstances<S>()
        {
            return this.GetAllInstances<S>(key: null);
        }

        /// <inheriteddoc />
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.GetAllInstances(serviceType, null);
        }

        /// <inheriteddoc />
        public IEnumerable<S> GetAllInstances<S>(object key)
        {
            return this.GetAllInstances(typeof(S), key)
                       .Cast<S>();
        }

        /// <inheriteddoc />
        public IEnumerable<object> GetAllInstances(Type serviceType, object key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            IEnumerable<object> result = null;

            ServiceActivationException exceptionToThrow = null;
            try
            {
                result = this.OnGetAllInstances(serviceType, key);

                if (result == null)
                {
                    exceptionToThrow = new ServiceActivationException(serviceType,
                                                                      key);
                }
            }
            catch (Exception ex)
            {
                exceptionToThrow = new ServiceActivationException(serviceType,
                                                                  key,
                                                                  ex);
            }

            if (exceptionToThrow != null)
            {
                throw exceptionToThrow;
            }

            using (var e = result.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var obj = ParseValue(e.Current);
                    if (obj != null)
                    {
                        yield return obj;
                    }
                }
            }
        }

        /// <inheriteddoc />
        public S GetInstance<S>()
        {
            return (S)this.GetInstance(typeof(S));
        }

        /// <inheriteddoc />
        public S GetInstance<S>(object key)
        {
            return (S)this.GetInstance(typeof(S),
                                       key);
        }

        /// <inheriteddoc />
        public object GetInstance(Type serviceType)
        {
            return this.GetInstance(serviceType,
                                    null);
        }

        /// <inheriteddoc />
        public object GetInstance(Type serviceType, object key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            object result = null;

            ServiceActivationException exceptionToThrow = null;
            try
            {
                result = ParseValue(this.OnGetInstance(serviceType,
                                                       ParseValue(key)));

                if (result == null)
                {
                    exceptionToThrow = new ServiceActivationException(serviceType,
                                                                      key);
                }
            }
            catch (Exception ex)
            {
                exceptionToThrow = new ServiceActivationException(serviceType,
                                                                  key,
                                                                  ex);
            }

            if (exceptionToThrow != null)
            {
                throw exceptionToThrow;
            }

            return result;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            try
            {
                return this.GetInstance(serviceType);
            }
            catch (ServiceActivationException sae)
            {
                var innerEx = sae.InnerException;
                if (innerEx != null)
                {
                    throw innerEx;
                }

                return null;
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="ServiceLocatorBase.GetAllInstances(Type)" /> method.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">
        /// The key of the service.
        /// <see langword="null" /> indicates to locate the default service.
        /// </param>
        /// <returns>The list of service instances.</returns>
        /// <exception cref="ServiceActivationException">
        /// Error while locating service instance, e.g. not found.
        /// </exception>
        protected abstract IEnumerable<object> OnGetAllInstances(Type serviceType, object key);

        /// <summary>
        /// Stores the logic for the <see cref="ServiceLocatorBase.GetInstance(Type, object)" /> method.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">
        /// The key of the service.
        /// <see langword="null" /> indicates to locate the default service.
        /// </param>
        /// <returns>The located instance.</returns>
        /// <exception cref="ServiceActivationException">
        /// Error while locating service instance, e.g. not found.
        /// </exception>
        protected abstract object OnGetInstance(Type serviceType,
                                                object key);

        private static object ParseValue(object value)
        {
            var result = value;
            if (result.IsNull())
            {
                result = null;
            }

            return result;
        }

        #endregion Methods
    }
}