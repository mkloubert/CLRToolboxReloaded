// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE40 || PORTABLE45)
#define KNOWS_SECURE_STRING
#endif

using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Sets up a <see cref="WebRequest" /> for basic authorization.
        /// </summary>
        /// <param name="request">The underlying request.</param>
        /// <param name="userName">The username.</param>
        /// <param name="pwd">The password.</param>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="userName" /> contains invalid char(s).</exception>
        public static void SetBasicAuth(this WebRequest request, string userName, string pwd)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            userName = userName ?? string.Empty;
            if (userName.Contains(":"))
            {
                throw new FormatException();
            }

            string authInfo = string.Format("{0}:{1}",
                                            userName,
                                            pwd);

            request.Headers["Authorization"] = string.Format("Basic {0}",
                                                             Convert.ToBase64String(Encoding.GetEncoding("ASCII")
                                                                                            .GetBytes(authInfo)));
        }

        /// <summary>
        /// Sets up a <see cref="WebRequest" /> for basic authorization.
        /// </summary>
        /// <param name="request">The underlying request.</param>
        /// <param name="userName">The username.</param>
        /// <param name="pwd">The password as ASCII binary data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="userName" /> contains invalid char(s).</exception>
        public static void SetBasicAuth(this WebRequest request, string userName, IEnumerable<byte> pwd)
        {
            var pwdArray = AsArray(pwd);

            SetBasicAuth(request,
                         userName,
                         pwdArray != null ? Encoding.GetEncoding("ASCII")
                                                    .GetString(pwdArray, 0, pwdArray.Length) : null);
        }

#if KNOWS_SECURE_STRING

        /// <summary>
        /// Sets up a <see cref="global::System.Net.WebRequest" /> for basic authorization.
        /// </summary>
        /// <param name="request">The underlying request.</param>
        /// <param name="userName">The username.</param>
        /// <param name="pwd">The password.</param>
        /// <exception cref="global::System.ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="global::System.FormatException"><paramref name="userName" /> contains invalid char(s).</exception>
        public static void SetBasicAuth(this global::System.Net.WebRequest request, string userName, global::System.Security.SecureString pwd)
        {
            SetBasicAuth(request,
                         userName,
                         ToUnsecureString(pwd));
        }

#endif

        #endregion Methods (3)
    }
}