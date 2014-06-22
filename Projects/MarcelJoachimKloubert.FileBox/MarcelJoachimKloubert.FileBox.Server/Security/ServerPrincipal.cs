// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.FileBox.Server.IO;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    internal sealed class ServerPrincipal : ObjectBase, IServerPrincipal
    {
        #region Properties (7)

        public string Files
        {
            get;
            internal set;
        }

        public IServerIdentity Identity
        {
            get;
            internal set;
        }

        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        public string Inbox
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "i")); }
        }

        internal Func<string, bool> IsInRolePredicate
        {
            get;
            set;
        }
        
        public string Outbox
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "o")); }
        }

        public string Temp
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "t")); }
        }

        #endregion Properties (5)

        #region Methods (3)

        internal static ServerPrincipal FromUsername(string username)
        {
            var dirs = ServiceLocator.Current.GetInstance<IDirectories>();

            var dir = new DirectoryInfo(Path.Combine(dirs.Files, username.ToLower().Trim()));

            Guid id;
            using (var md5 = new MD5CryptoServiceProvider())
            {
                id = new Guid(md5.ComputeHash(new UTF8Encoding().GetBytes(username)));
            }

            return new ServerPrincipal()
                {
                    Files = Path.GetFullPath(dir.FullName),

                    Identity = new ServerIdentity(id: id)
                    {
                        AuthenticationType = "HttpBasicAuth",
                        IsAuthenticated = true,
                        Name = username.ToLower().Trim(),
                    },

                    IsInRolePredicate = (role) => false,
                };
        }

        public bool IsInRole(string role)
        {
            return this.IsInRolePredicate(role);
        }

        public RSACryptoServiceProvider TryGetRsaCrypter()
        {
            RSACryptoServiceProvider result = null;

            try
            {
                var keyFile = new FileInfo(Path.Combine(this.Files, "key.xml"));
                if (keyFile.Exists)
                {
                    result = new RSACryptoServiceProvider();
                    result.FromXmlString(File.ReadAllText(keyFile.FullName,
                                                          Encoding.UTF8));
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        #endregion Methods (2)
    }
}