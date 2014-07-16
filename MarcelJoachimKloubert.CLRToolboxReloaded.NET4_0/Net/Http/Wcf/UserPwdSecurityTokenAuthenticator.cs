// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    internal class UserPwdSecurityTokenAuthenticator : CustomUserNameSecurityTokenAuthenticator
    {
        #region Constructors (1)

        internal UserPwdSecurityTokenAuthenticator(UserNamePasswordValidator validator)
            : base(validator)
        {
        }

        #endregion Constructors

        #region Methods  (1)

        protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateUserNamePasswordCore(string userName, string password)
        {
            var currentPolicies = base.ValidateUserNamePasswordCore(userName, password);

            var newPolicies = new List<IAuthorizationPolicy>();
            if (currentPolicies != null)
            {
                newPolicies.AddRange(currentPolicies.OfType<IAuthorizationPolicy>());
            }

            newPolicies.Add(new UserPwdAuthorizationPolicy(userName, password));

            return newPolicies.AsReadOnly();
        }

        #endregion Methods
    }
}