// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IdentityModel.Selectors;
using System.ServiceModel.Security;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    internal class UserPwdTokenManager : ServiceCredentialsSecurityTokenManager
    {
        #region Constructors (1)

        internal UserPwdTokenManager(UserPwdServiceCredentials credentials)
            : base(credentials)
        {
        }

        #endregion Constructors

        #region Methods  (1)

        public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement,
                                                                                    out SecurityTokenResolver outOfBandTokenResolver)
        {
            outOfBandTokenResolver = null;

            return new UserPwdSecurityTokenAuthenticator(this.ServiceCredentials
                                                             .UserNameAuthentication
                                                             .CustomUserNamePasswordValidator);
        }

        #endregion Methods
    }
}