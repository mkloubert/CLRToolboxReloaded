// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Extensions;
using MarcelJoachimKloubert.FileBox.Server.IO;
using MarcelJoachimKloubert.FileBox.Server.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists a directory.
    /// </summary>
    public sealed class InboxHttpHandler : FileBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="" /> class.
        /// </summary>
        /// <param name="handler">
        /// The handler for the <see cref="FileBoxHttpHandlerBase.CheckLogin(string, SecureString, ref bool, ref IPrincipal)" /> method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler" /> is <see langword="null" />.
        /// </exception>
        public InboxHttpHandler(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest_Authorized(IHttpRequestContext context)
        {
            var result = new JsonResult();

            try
            {
                result.code = 0;

                var files = new List<object>();

                var dirs = ServiceLocator.Current.GetInstance<IDirectories>();

                var userDir = new DirectoryInfo(Path.Combine(dirs.Files, context.User.Identity.Name));
                if (userDir.Exists)
                {
                    int startAt = 0;
                    try
                    {
                        var str = context.Http.Request.Headers["X-FileBox-StartAt"];
                        if (string.IsNullOrWhiteSpace(str) == false)
                        {
                            startAt = int.Parse(str.Trim());
                        }
                    }
                    catch
                    {
                        startAt = 0;
                    }

                    int? maxItems = null;
                    try
                    {
                        var str = context.Http.Request.Headers["X-FileBox-MaxItems"];
                        if (string.IsNullOrWhiteSpace(str) == false)
                        {
                            maxItems = int.Parse(str.Trim());
                        }
                    }
                    catch
                    {
                        maxItems = null;
                    }

                    if (startAt < 0)
                    {
                        startAt = 0;
                    }

                    if (maxItems < 0)
                    {
                        maxItems = 0;
                    }

                    IEnumerable<FileInfo> usrFiles = userDir.GetFiles()
                                                            .Skip(startAt);

                    if (maxItems.HasValue)
                    {
                        usrFiles = usrFiles.Take(maxItems.Value);
                    }

                    foreach (var f in usrFiles)
                    {
                        files.Add(new
                            {
                                name = f.Name,
                                size = f.Length,
                            });
                    }
                }

                result.data = files.ToArray();
            }
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            context.Http.Response.WriteJson(result);
        }

        #endregion Methods (1)
    }
}