// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Web;
using System.IO;
using System.Net;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// A basic HTTP handler for receiving a file from a box.
    /// </summary>
    public abstract class ReceiveHttpHandlerBase : FileBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <inheriteddoc />
        protected ReceiveHttpHandlerBase(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (2)

        /// <summary>
        /// Returns the full path of the underlying box.
        /// </summary>
        /// <param name="context">The underlying request context.</param>
        /// <returns>The full path of the box.</returns>
        protected abstract string GetBoxPath(IHttpRequestContext context);

        /// <inheriteddoc />
        protected override sealed void OnProcessRequest_Authorized(IHttpRequestContext context)
        {
            var fileName = (context.Http.Request.Headers["X-FileBox-File"] ?? string.Empty).Trim();

            ulong index;
            if (ulong.TryParse(fileName, out index))
            {
                var fileFound = false;

                var boxDir = new DirectoryInfo(this.GetBoxPath(context));
                if (boxDir.Exists)
                {
                    var dataFile = new FileInfo(Path.Combine(boxDir.FullName, index.ToString() + ".bin"));
                    var metaFile = new FileInfo(Path.Combine(boxDir.FullName, index.ToString() + ".dat"));
                    var metaPwdFile = new FileInfo(Path.Combine(boxDir.FullName, index.ToString() + ".asc"));

                    if (dataFile.Exists &&
                        metaFile.Exists &&
                        metaPwdFile.Exists)
                    {
                        fileFound = true;

                        using (var stream = dataFile.OpenRead())
                        {
                            context.Http.Response.BufferOutput = false;

                            stream.CopyTo(context.Http.Response.OutputStream);
                        }
                    }
                }

                if (fileFound == false)
                {
                    context.Http.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
            }
        }

        #endregion Methods (2)
    }
}