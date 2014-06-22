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
    /// A basic HTTP handler.
    /// </summary>
    public abstract class BoxHttpHandlerBase : FileBoxHttpHandlerBase
    {
        #region Constrcutors (2)

        /// <inheriteddoc />
        protected BoxHttpHandlerBase(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (2)

        #region Methods (2)

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
                    boxDir.GetFiles(".dat")
                          .ForAll(throwExceptions: false,
                                  action: ctx =>
                                  {
                                      var metaFile = ctx.Item;

                                      ulong index;
                                      if (ulong.TryParse(metaFile.Name, out index) == false)
                                      {
                                          // must be a valid number
                                          return;
                                      }

                                      var dataFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString()));
                                      if (dataFile.Exists == false)
                                      {
                                          // no data file found
                                          return;
                                      }

                                      ctx.State.FileList.Add(new
                                          {
                                              metaFile = new
                                              {
                                                  data = File.ReadAllText(metaFile.FullName,
                                                                          ctx.State.MetaFileEncoding),
                                                  name = index.ToString(),
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