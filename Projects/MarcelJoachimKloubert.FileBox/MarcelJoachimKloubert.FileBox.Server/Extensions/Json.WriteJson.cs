// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.FileBox.Server.Extensions
{
    static partial class FileBoxServerExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Writes an object directly to a HTTP response.
        /// </summary>
        /// <typeparam name="TObj">Type of the object.</typeparam>
        /// <param name="resp">The response context.</param>
        /// <param name="obj">The object to serialize and write.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resp" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// UTF-8 is used as encoding.
        /// </remarks>
        public static void WriteJson<TObj>(this HttpResponse resp, TObj obj)
        {
            WriteJson<TObj>(resp, obj, new UTF8Encoding());
        }

        /// <summary>
        /// Writes an object directly to a HTTP response.
        /// </summary>
        /// <typeparam name="TObj">Type of the object.</typeparam>
        /// <param name="resp">The response context.</param>
        /// <param name="obj">The object to serialize and write.</param>
        /// <param name="enc">The encoding of the data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resp" /> is <see langword="null" />.
        /// </exception>
        public static void WriteJson<TObj>(this HttpResponse resp, TObj obj, Encoding enc)
        {
            resp.ContentType = "application/json; charset=" + enc.WebName;

            var json = obj.ToJson<TObj>();
            if (json != null)
            {
                var data = enc.GetBytes(json);

                resp.OutputStream.Write(data, 0, data.Length);
            }
        }

        #endregion Methods (1)
    }
}