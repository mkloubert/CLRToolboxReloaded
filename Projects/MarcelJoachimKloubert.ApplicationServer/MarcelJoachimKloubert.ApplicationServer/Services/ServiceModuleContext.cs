// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Helpers;
using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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

        internal IServiceLocator ServiceLocator
        {
            get;
            set;
        }

        #endregion Properties (6)

        #region Methods (13)

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

        public byte[] GetHash()
        {
            using (var temp = new MemoryStream())
            {
                // assembly name
                temp.Write(Encoding.UTF8
                                   .GetBytes(this.Assembly.FullName));

                // assembly public key
                var asmPubKey = this.Assembly.GetName().GetPublicKey();
                if (asmPubKey != null)
                {
                    temp.Write(asmPubKey);
                }

                // assembly hash
                temp.Write(this.AssemblyHash);

                // assembly location
                temp.Write(Encoding.UTF8
                                   .GetBytes(this.AssemblyLocation));

                // full name of service module
                temp.Write(Encoding.UTF8
                                   .GetBytes(this.Module.GetType().FullName));

                // ID of the module
                temp.Write(this.Module.Id.ToByteArray());

                // calculate hash and return...
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    temp.Position = 0;
                    return md5.ComputeHash(temp);
                }
            }
        }

        public string GetHashAsString()
        {
            return string.Concat(this.GetHash()
                                     .Select(b => b.ToString("x2")));
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

        public Stream TryGetResourceStream(string resourceName)
        {
            return ResourceHelper.GetManifestResourceStream(this.Assembly,
                                                            this.Module.GetType(),
                                                            resourceName);
        }

        #endregion Methods (13)
    }
}