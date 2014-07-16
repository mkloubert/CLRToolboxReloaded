// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security;
using System.IdentityModel.Selectors;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    internal sealed partial class UserPwdServiceCredentials : ServiceCredentials
    {
        #region Constructors (3)

        internal UserPwdServiceCredentials(UsernamePasswordValidator validator)
        {
            this.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            this.UserNameAuthentication.CustomUserNamePasswordValidator = new UserPwdValidator(validator);
        }

        private UserPwdServiceCredentials(UserPwdServiceCredentials clone)
            : base(clone)
        {
        }

        internal UserPwdServiceCredentials()
        {
        }

        #endregion Constructors

        #region Methods (2)

        protected override ServiceCredentials CloneCore()
        {
            return new UserPwdServiceCredentials(this);
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            if (this.UserNameAuthentication.UserNamePasswordValidationMode == UserNamePasswordValidationMode.Custom)
            {
                return new UserPwdTokenManager(this);
            }

            return base.CreateSecurityTokenManager();
        }

        #endregion Methods
    }
}