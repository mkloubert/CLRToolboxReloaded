// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.ApplicationServer
{
    /// <summary>
    /// Simple implementation of the <see cref="IApplicationServerContext" /> interface.
    /// </summary>
    public class ApplicationServerContext : ObjectBase, IApplicationServerContext
    {
        #region Fields (1)

        /// <summary>
        /// Stores allowed chars for directory and file names.
        /// </summary>
        public const string ALLOWED_PATH_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion Fields (1)

        #region Properties (7)

        /// <inheriteddoc />
        public ILogger Logger
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public virtual DateTimeOffset Now
        {
            get { return AppTime.Now; }
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

        /// <inheriteddoc />
        public virtual string TempDirectory
        {
            get
            {
                return Path.Combine(this.RootDirectory,
                                    "temp");
            }
        }

        /// <inheriteddoc />
        public virtual string WebDirectory
        {
            get
            {
                return Path.Combine(this.RootDirectory,
                                    "web");
            }
        }

        #endregion Properties (7)

        #region Methods (11)

        /// <inheriteddoc />
        public virtual FileStream CreateAndOpenTempFile(string tempDir = null,
                                                        string extension = "tmp")
        {
            if (string.IsNullOrWhiteSpace(tempDir))
            {
                tempDir = this.TempDirectory;
            }

            if (Directory.Exists(tempDir) == false)
            {
                throw new DirectoryNotFoundException();
            }

            var rand = CryptoRandom.Create();

            string tempFile;
            do
            {
                tempFile = string.Empty;
                for (var i = 0; i < 8; i++)
                {
                    tempFile += ALLOWED_PATH_CHARS[rand.Next(0, ALLOWED_PATH_CHARS.Length)];
                }

                tempFile = Path.Combine(tempDir,
                                        tempFile + "." + extension);
            }
            while (File.Exists(tempFile));

            return new FileStream(path: tempFile,
                                  mode: FileMode.CreateNew,
                                  access: FileAccess.ReadWrite,
                                  share: FileShare.None);
        }

        /// <inheriteddoc />
        public virtual string CreateTempDirectory()
        {
            var rand = CryptoRandom.Create();

            string tempDir;
            do
            {
                tempDir = string.Empty;
                for (var i = 0; i < 8; i++)
                {
                    tempDir += ALLOWED_PATH_CHARS[rand.Next(0, ALLOWED_PATH_CHARS.Length)];
                }

                tempDir = Path.Combine(this.TempDirectory,
                                       tempDir);
            }
            while (Directory.Exists(tempDir));

            return Directory.CreateDirectory(tempDir)
                            .FullName;
        }

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

        #endregion Methods (11)
    }
}