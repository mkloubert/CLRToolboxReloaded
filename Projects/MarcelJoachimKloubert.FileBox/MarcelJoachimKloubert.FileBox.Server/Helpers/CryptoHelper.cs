// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox.Server.Helpers
{
    internal static class CryptoHelper
    {
        #region Methods (1)

        internal static Rijndael CreateRijndael(byte[] pwd, byte[] salt)
        {
            var pdb = new Rfc2898DeriveBytes(pwd, salt,
                                             1000);

            var result = Rijndael.Create();
            result.Key = pdb.GetBytes(32);
            result.IV = pdb.GetBytes(16);

            return result;
        }

        #endregion Methods (1)
    }
}