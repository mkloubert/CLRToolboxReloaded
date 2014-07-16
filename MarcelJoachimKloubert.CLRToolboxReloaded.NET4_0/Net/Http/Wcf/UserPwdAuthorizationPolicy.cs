// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security;
using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    internal class UserPwdAuthorizationPolicy : DisposableObjectBase, IAuthorizationPolicy
    {
        #region Fields (1)

        private const string _IDENTITIES_KEYS = "Identities";

        #endregion Fields (1)

        #region Constructors (1)

        internal UserPwdAuthorizationPolicy(string userName, string password)
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.UserName = userName;

            if (password != null)
            {
                var secPwd = new SecureString();

                password.ForEach(ctx =>
                    {
                        ctx.State
                           .Password.AppendChar(ctx.Item);
                    }, new
                    {
                        Password = secPwd,
                    });

                this.Password = secPwd;
            }
        }

        #endregion Constructors

        #region Properties (4)

        public string Id
        {
            get;
            private set;
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        internal SecureString Password
        {
            get;
            private set;
        }

        internal string UserName
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods (2)

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            // Check if the properties of the context has the identities list
            if ((evaluationContext.Properties.Count == 0) ||
                (evaluationContext.Properties.ContainsKey(_IDENTITIES_KEYS) == false) ||
                (evaluationContext.Properties[_IDENTITIES_KEYS] == null))
            {
                return false;
            }

            // Get the identities list
            var identities = evaluationContext.Properties[_IDENTITIES_KEYS] as List<IIdentity>;

            // Validate that the identities list is valid
            if (identities == null)
            {
                return false;
            }

            // Get the current identity
            var currentIdentity = identities.Find(identityMatch => (identityMatch is GenericIdentity) &&
                                                                   string.Equals(identityMatch.Name, UserName,
                                                                                 StringComparison.OrdinalIgnoreCase));

            // Check if an identity was found
            if (currentIdentity == null)
            {
                return false;
            }

            //// Create new identity
            //PasswordIdentity newIdentity = new PasswordIdentity(
            //    UserName, Password, currentIdentity.IsAuthenticated, currentIdentity.AuthenticationType);
            //const String PrimaryIdentityKey = "PrimaryIdentity";

            //// Update the list and the context with the new identity
            //identities.Remove(currentIdentity);
            //identities.Add(newIdentity);
            //evaluationContext.Properties[PrimaryIdentityKey] = newIdentity;

            //// Create a new principal for this identity
            //PasswordPrincipal newPrincipal = new PasswordPrincipal(newIdentity, null);
            //const String PrincipalKey = "Principal";

            // Store the new principal in the context
            // evaluationContext.Properties[PrincipalKey] = newPrincipal;

            // This policy has successfully been evaluated and doesn't need to be called again
            return true;
        }

        protected override void OnDispose(DisposeContext ctx)
        {
            using (var pwd = this.Password)
            {
                this.Password = null;
            }
        }

        #endregion Methods
    }
}