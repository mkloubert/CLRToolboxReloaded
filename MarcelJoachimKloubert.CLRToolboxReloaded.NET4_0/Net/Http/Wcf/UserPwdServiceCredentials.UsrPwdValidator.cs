// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    partial class UserPwdServiceCredentials
    {
        #region Nested classes (1)

        private sealed class UserPwdValidator : UserNamePasswordValidator
        {
            #region Fields (1)

            private readonly UsernamePasswordValidator _VALIDATOR;

            #endregion Fields (1)

            #region Constructors (1)

            internal UserPwdValidator(UsernamePasswordValidator validator)
            {
                this._VALIDATOR = validator;
            }

            #endregion Constructors (1)

            #region Methods (1)

            public override void Validate(string username, string password)
            {
                var failed = true;

                try
                {
                    if (this._VALIDATOR(username, password))
                    {
                        failed = false;
                    }
                }
                catch
                {
                }

                if (failed)
                {
                    throw new SecurityTokenValidationException();
                }
            }

            #endregion Methods (1)
        }

        #endregion Nested classes (1)
    }
}