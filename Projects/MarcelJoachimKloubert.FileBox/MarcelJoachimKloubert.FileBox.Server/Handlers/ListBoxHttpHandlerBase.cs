// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Extensions;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// A basic HTTP handler for listening the files of a box.
    /// </summary>
    public abstract class ListBoxHttpHandlerBase : FileBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <inheriteddoc />
        protected ListBoxHttpHandlerBase(CheckLoginHandler handler)
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
            var result = new JsonResult();

            try
            {
                result.code = 0;

                var files = new List<object>();
                var rsa = context.GetUser<IServerPrincipal>().TryGetRsaCrypter();

                var boxDir = new DirectoryInfo(this.GetBoxPath(context));
                if (boxDir.Exists)
                {
                    boxDir.GetFiles("*." + GlobalConstants.FileExtensions.META_FILE)
                          .ForAll(throwExceptions: false,
                                  action: ctx =>
                                  {
                                      var metaFile = ctx.Item;

                                      ulong index;
                                      if (ulong.TryParse(Path.GetFileNameWithoutExtension(metaFile.Name), out index) == false)
                                      {
                                          // must be a valid number
                                          return;
                                      }

                                      var metaPwdFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString() + "." + GlobalConstants.FileExtensions.META_PASSWORD_FILE));
                                      if (metaPwdFile.Exists == false)
                                      {
                                          // no password file for meta data found
                                          return;
                                      }

                                      var dataFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString() + "." + GlobalConstants.FileExtensions.DATA_FILE));
                                      if (dataFile.Exists == false)
                                      {
                                          // no data file found
                                          return;
                                      }

                                      ctx.State.FileList.Add(new
                                          {
                                              name = index.ToString(),

                                              meta = new
                                              {
                                                  dat = Convert.ToBase64String(File.ReadAllBytes(metaFile.FullName)),
                                                  sec = Convert.ToBase64String(File.ReadAllBytes(metaPwdFile.FullName)),
                                              },
                                          });
                                  }, actionState: new
                                  {
                                      MetaFileEncoding = new UTF8Encoding(),
                                      FileList = files,
                                  });
                }

                result.data = new
                    {
                        files = files.ToArray(),
                        key = rsa != null ? rsa.ToXmlString(includePrivateParameters: false) : null,
                    };
            }
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            context.Http.Response.WriteJson(result);
        }

        #endregion Methods (2)
    }
}