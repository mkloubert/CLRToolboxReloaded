// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.ApplicationServer
{
    /// <summary>
    /// Simple implementation of the <see cref="IApplicationServerContext" /> interface.
    /// </summary>
    public class ApplicationServerContext : ObjectBase, IApplicationServerContext
    {
        #region Properties (4)

        /// <inheriteddoc />
        public ILogger Logger
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public string RootDirectory
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IApplicationServer Server
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inner service locator.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get;
            set;
        }

        #endregion Properties (4)

        #region Methods (9)

        /// <inheriteddoc />
        public IEnumerable<S> GetAllInstances<S>()
        {
            return this.ServiceLocator.GetAllInstances<S>();
        }

        /// <inheriteddoc />
        public IEnumerable<S> GetAllInstances<S>(object key)
        {
            return this.ServiceLocator.GetAllInstances<S>(key);
        }

        /// <inheriteddoc />
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.ServiceLocator.GetAllInstances(serviceType);
        }

        /// <inheriteddoc />
        public IEnumerable<object> GetAllInstances(Type serviceType, object key)
        {
            return this.ServiceLocator.GetAllInstances(serviceType, key);
        }

        /// <inheriteddoc />
        public S GetInstance<S>()
        {
            return this.ServiceLocator.GetInstance<S>();
        }

        /// <inheriteddoc />
        public S GetInstance<S>(object key)
        {
            return this.ServiceLocator.GetInstance<S>(key);
        }

        /// <inheriteddoc />
        public object GetInstance(Type serviceType)
        {
            return this.ServiceLocator.GetInstance(serviceType);
        }

        /// <inheriteddoc />
        public object GetInstance(Type serviceType, object key)
        {
            return this.ServiceLocator.GetInstance(serviceType, key);
        }

        /// <inheriteddoc />
        public object GetService(Type serviceType)
        {
            return this.ServiceLocator.GetService(serviceType);
        }

        #endregion Methods (9)
    }
}