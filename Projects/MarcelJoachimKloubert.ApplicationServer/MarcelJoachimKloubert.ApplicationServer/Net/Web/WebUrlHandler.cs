// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Helpers;
using MarcelJoachimKloubert.ApplicationServer.Services;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.CLRToolbox.Text.Html;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    internal sealed partial class WebUrlHandler
    {
        #region Fields (3)

        private const string _COMMON_FILENAME = @"[A-Za-z0-9|/|,|\-|_|\.]+";
        private const string _COMMON_HASH = @"[A-Za-z0-9]{32}";
        private readonly ApplicationServer _SERVER;

        #endregion Fields (3)

        #region Constructors (1)

        internal WebUrlHandler(ApplicationServer server)
        {
            this._SERVER = server;

            this.Initialize();
        }

        #endregion Constructors (1)

        #region Properties (1)

        private HandlerItem[] Handlers
        {
            get;
            set;
        }

        #endregion Properties (1)

        #region Methods (18)

        private static void CopyHtmlTemplateTo(IHtmlTemplate tpl, Stream target)
        {
            var str = tpl.Render();
            if (str != null)
            {
                target.Write(Encoding.UTF8
                                     .GetBytes(str));
            }
        }

        private void FillFrontendTemplate(IHtmlTemplate tpl, HttpRequestEventArgs e)
        {
            tpl["year"] = e.Request.Time.Year;

            e.Response
             .FrontendVars
             .ForAll((ctx) =>
             {
                 var v = ctx.Item;

                 ctx.State
                    .Template[v.Key] = v.Value;
             }, actionState: new
             {
                 Template = tpl,
             });
        }

        internal void Handle_DefaultServerModule(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var module = TryFindDefaultWebInterfaceModule(this._SERVER
                                                              .Context);

            if (module != null)
            {
                found = true;

                this.HandleWebInterfaceModule(module, e);
            }
        }

        private void Handle_DefautServerServiceModule(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var hash = (match.Groups[2].Value ?? string.Empty).ToLower().Trim();

            var service = TryFindServiceModule(this._SERVER
                                                   .GetServiceModules(), hash);

            if (service != null)
            {
                var module = TryFindDefaultWebInterfaceModule(service.Context);

                if (module != null)
                {
                    found = true;

                    this.HandleWebInterfaceModule(module, e);
                }
            }
        }

        private void Handle_ServerCssFile(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var name = (match.Groups[2].Value ?? string.Empty).ToLower().Trim() + ".css";

            var dir = new DirectoryInfo(Path.Combine(this._SERVER.Context.WebDirectory, "css"));

            HandleFile(name: name,
                       dir: dir,
                       contentType: "text/css; charset=utf-8",
                       e: e,
                       found: ref found,
                       compress: true);
        }

        private void Handle_ServerFontFile(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var name = (match.Groups[2].Value ?? string.Empty).ToLower().Trim();
            var ext = (match.Groups[4].Value ?? string.Empty).ToLower().Trim();
            var fullName = name + "." + ext;

            var dir = new DirectoryInfo(Path.Combine(this._SERVER.Context.WebDirectory, "fonts"));

            HandleFile(name: fullName,
                       dir: dir,
                       contentType: WebHelper.GetContentTypeByFileExtension(ext),
                       e: e,
                       found: ref found);
        }

        private void Handle_ServerImageFile(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var name = (match.Groups[2].Value ?? string.Empty).ToLower().Trim();
            var ext = (match.Groups[4].Value ?? string.Empty).ToLower().Trim();
            var fullName = name + "." + ext;

            var dir = new DirectoryInfo(Path.Combine(this._SERVER.Context.WebDirectory, "img"));

            HandleFile(name: fullName,
                       dir: dir,
                       contentType: WebHelper.GetContentTypeByFileExtension(ext),
                       e: e,
                       found: ref found);
        }

        private void Handle_ServerJavascriptFile(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var name = (match.Groups[2].Value ?? string.Empty).ToLower().Trim() + ".js";

            var dir = new DirectoryInfo(Path.Combine(this._SERVER.Context.WebDirectory, "js"));

            HandleFile(name: name,
                       dir: dir,
                       contentType: "text/javascript; charset=utf-8",
                       e: e,
                       found: ref found,
                       compress: true);
        }

        private void Handle_ServerModule(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var name = (match.Groups[2].Value ?? string.Empty).ToLower().Trim();

            var module = TryFindWebInterfaceModule(this._SERVER
                                                       .Context, name);

            if (module != null)
            {
                found = true;

                this.HandleWebInterfaceModule(module, e);
            }
        }

        private void Handle_ServerServiceModule(Match match, HttpRequestEventArgs e, ref bool found)
        {
            var hash = (match.Groups[2].Value ?? string.Empty).ToLower().Trim();
            var name = (match.Groups[4].Value ?? string.Empty).ToLower().Trim();

            var service = TryFindServiceModule(this._SERVER
                                                   .GetServiceModules(), hash);

            if (service != null)
            {
                var module = TryFindWebInterfaceModule(service.Context, name);

                if (module != null)
                {
                    found = true;

                    this.HandleWebInterfaceModule(module, e);
                }
            }
        }

        private static void HandleFile(string name, DirectoryInfo dir, string contentType, HttpRequestEventArgs e, ref bool found,
                                       bool compress = false)
        {
            while (name.Contains("../"))
            {
                name = name.Replace("../", string.Empty);
            }

            while (name.Contains("..\\"))
            {
                name = name.Replace("..\\", string.Empty);
            }

            while (name.Contains(".."))
            {
                name = name.Replace("..", string.Empty);
            }

            var file = new FileInfo(Path.Combine(dir.FullName, name));
            if (file.Exists == false)
            {
                return;
            }

            found = true;

            using (var fs = OpenFileForReading(file))
            {
                fs.CopyTo(e.Response.Stream);

                e.Response.ContentType = contentType;
                e.Response.Compress = compress;
            }
        }

        private void HandleWebInterfaceModule(IWebInterfaceModule module, HttpRequestEventArgs e)
        {
            var ctx = new WebExecutionContext()
            {
                Request = e.Request,
                Response = e.Response,
                ServerContext = this._SERVER.Context,
            };

            // load HTML template
            ctx.TryGetHtmlTemplateFunc = (file) =>
                {
                    if (file.Exists)
                    {
                        return DotLiquidHtmlTemplate.Create(file);
                    }

                    return null;
                };

            // load javascript
            ctx.TryLoadJavascriptFunc = (file) =>
                {
                    if (file.Exists)
                    {
                        return File.ReadAllText(file.FullName,
                                                Encoding.UTF8);
                    }

                    return null;
                };

            // load CSS styles
            ctx.TryLoadStylesheetsFunc = (file) =>
                {
                    if (file.Exists)
                    {
                        return File.ReadAllText(file.FullName,
                                                Encoding.UTF8);
                    }

                    return null;
                };

            module.Handle(ctx);

            if (e.Response.DirectOutput == false)
            {
                var headerFile = new FileInfo(Path.Combine(this._SERVER.Context.WebDirectory,
                                                           "header.html"));

                var footerFile = new FileInfo(Path.Combine(this._SERVER.Context.WebDirectory,
                                                           "footer.html"));

                if (headerFile.Exists || footerFile.Exists)
                {
                    var shredderOldStream = false;

                    var oldStream = e.Response.Stream;
                    var newStream = this._SERVER.Context.CreateAndOpenTempFile();
                    try
                    {
                        ctx.Response.SetStream(newStream);
                        shredderOldStream = true;

                        // header
                        if (headerFile.Exists)
                        {
                            using (var header = OpenFileForReading(headerFile))
                            {
                                var tpl = DotLiquidHtmlTemplate.Create(header);
                                this.FillFrontendTemplate(tpl, e);

                                CopyHtmlTemplateTo(tpl,
                                                   newStream);
                            }
                        }

                        // content
                        oldStream.Position = 0;
                        oldStream.CopyTo(newStream);

                        // footer
                        if (footerFile.Exists)
                        {
                            using (var footer = OpenFileForReading(footerFile))
                            {
                                var tpl = DotLiquidHtmlTemplate.Create(footer);
                                this.FillFrontendTemplate(tpl, e);

                                CopyHtmlTemplateTo(tpl,
                                                   newStream);
                            }
                        }

                        ctx.Response.ContentType = "text/html; charset=utf-8";
                    }
                    catch
                    {
                        FileHelper.ShredderAndDeleteFile(newStream);

                        throw;
                    }
                    finally
                    {
                        if (shredderOldStream)
                        {
                            FileHelper.ShredderAndDeleteFile(oldStream as FileStream);
                        }
                    }
                }
            }
        }

        private void Initialize()
        {
            var handlers = new List<HandlerItem>();

            // server web interface module
            handlers.Add(new HandlerItem(regex: new Regex("^(/)(" + _COMMON_FILENAME + @")(\.html)",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_ServerModule));

            // server image file
            handlers.Add(new HandlerItem(regex: new Regex("^(/img/)(" + _COMMON_FILENAME + @")(\.)(bmp|gif|ico|icon|jpg|jpeg|png|svg)",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_ServerImageFile));

            // server javascript file
            handlers.Add(new HandlerItem(regex: new Regex("^(/js/)(" + _COMMON_FILENAME + @")(\.js)",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_ServerJavascriptFile));

            // server css file
            handlers.Add(new HandlerItem(regex: new Regex("^(/css/)(" + _COMMON_FILENAME + @")(\.css)",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_ServerCssFile));

            // default server service web interface module
            handlers.Add(new HandlerItem(regex: new Regex("^(/)(" + _COMMON_HASH + @")(/)?$",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_DefautServerServiceModule));

            // server service web interface module
            handlers.Add(new HandlerItem(regex: new Regex("^(/)(" + _COMMON_HASH + @")(/)(" + _COMMON_FILENAME + @")(\.html)",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_ServerServiceModule));

            // server font file
            handlers.Add(new HandlerItem(regex: new Regex("^(/fonts/)(" + _COMMON_FILENAME + @")(\.)(eot|svg|ttf|woff)",
                                                          RegexOptions.Compiled | RegexOptions.IgnoreCase),
                                         action: this.Handle_ServerFontFile));

            this.Handlers = handlers.ToArray();
        }

        private static FileStream OpenFileForReading(FileInfo file)
        {
            return file.Open(FileMode.Open,
                             FileAccess.Read,
                             FileShare.Read);
        }

        private static IWebInterfaceModule TryFindDefaultWebInterfaceModule(IServiceLocator locator)
        {
            return locator.GetAllInstances<IWebInterfaceModule>()
                          .SingleOrDefault((m) =>
                               {
                                   return m.GetType()
                                           .GetCustomAttributes(typeof(global::MarcelJoachimKloubert.ApplicationServer.Net.Web.DefaultWebInterfaceModuleAttribute),
                                                                false)
                                           .Any();
                               });
        }

        private static IServiceModule TryFindServiceModule(IEnumerable<IServiceModule> services, string hash)
        {
            return services.SingleOrDefault((m) =>
                                            {
                                                return m.Context.GetHashAsString() == hash;
                                            });
        }

        private static IWebInterfaceModule TryFindWebInterfaceModule(IServiceLocator locator, string name)
        {
            return locator.GetAllInstances<IWebInterfaceModule>()
                          .SingleOrDefault((m) =>
                           {
                               return (m.Name ?? string.Empty).ToLower().Trim() == name;
                           });
        }

        internal bool TryHandle(HttpRequestEventArgs e)
        {
            var path = string.Empty;
            if (e.Request.Address != null)
            {
                path = e.Request.Address.PathAndQuery;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = string.Empty;
                }
            }

            for (var i = 0; i < this.Handlers.Length; i++)
            {
                var handler = this.Handlers[i];

                var match = handler.REG_EX.Match(path);
                if (match.Success)
                {
                    var found = false;
                    handler.ACTION(match, e, ref found);

                    if (found == false)
                    {
                        e.Response
                         .DocumentNotFound = true;
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion Methods (18)
    }
}