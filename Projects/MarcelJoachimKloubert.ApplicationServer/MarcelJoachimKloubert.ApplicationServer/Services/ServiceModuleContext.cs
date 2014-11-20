// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarcelJoachimKloubert.ApplicationServer.Services
{
    internal sealed class ServiceModuleContext : ObjectBase, IServiceModuleContext
    {
        #region Properties (6)

        public Assembly Assembly
        {
            get;
            internal set;
        }

        public byte[] AssemblyHash
        {
            get;
            internal set;
        }

        public string AssemblyLocation
        {
            get;
            internal set;
        }

        internal Func<IEnumerable<IServiceModule>> GetOtherModulesFunc
        {
            get;
            set;
        }

        public IServiceModule Module
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the inner service locator.
        /// </summary>
        internal IServiceLocator ServiceLocator
        {
            get;
            set;
        }

        #endregion Properties (6)

        #region Methods (10)

        public IEnumerable<S> GetAllInstances<S>()
        {
            return this.ServiceLocator.GetAllInstances<S>();
        }

        public IEnumerable<S> GetAllInstances<S>(object key)
        {
            return this.ServiceLocator.GetAllInstances<S>(key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.ServiceLocator.GetAllInstances(serviceType);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType, object key)
        {
            return this.ServiceLocator.GetAllInstances(serviceType, key);
        }

        public S GetInstance<S>()
        {
            return this.ServiceLocator.GetInstance<S>();
        }

        public S GetInstance<S>(object key)
        {
            return this.ServiceLocator.GetInstance<S>(key);
        }

        public object GetInstance(Type serviceType)
        {
            return this.ServiceLocator.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, object key)
        {
            return this.ServiceLocator.GetInstance(serviceType, key);
        }

        public IEnumerable<IServiceModule> GetOtherModules()
        {
            return this.GetOtherModulesFunc();
        }

        public object GetService(Type serviceType)
        {
            return this.ServiceLocator.GetService(serviceType);
        }

        #endregion Methods (10)
    }
}