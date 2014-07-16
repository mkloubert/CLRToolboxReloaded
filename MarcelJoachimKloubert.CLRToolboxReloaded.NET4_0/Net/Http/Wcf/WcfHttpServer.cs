// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using WcfTransferMode = System.ServiceModel.TransferMode;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    /// <summary>
    /// HTTP server implementation that uses <see cref="ServiceHost" /> from WCF framework.
    /// </summary>
    public partial class WcfHttpServer : HttpServerBase
    {
        #region Fields (1)

        private ServiceHost _host;

        #endregion Fields (1)

        #region Constructors (2)

        /// <inheriteddoc />
        public WcfHttpServer(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public WcfHttpServer()
            : this(sync: new object())
        {
        }

        #endregion Constructors

        #region Properties (3)

        /// <summary>
        /// Gets or sets the store location of the SSL certificate.
        /// <see langword="null" /> indicates to use the default one.
        /// </summary>
        public StoreLocation? SslStoreLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the store name of the SSL certificate.
        /// <see langword="null" /> indicates to use the default one.
        /// </summary>
        public StoreName? SslStoreName
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public override bool SupportsSecureHttp
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the thumbprint of the SSL certificate.
        /// <see langword="null" /> indicates to use the default one.
        /// </summary>
        public string Thumbprint
        {
            get;
            set;
        }

        #endregion

        #region Methods (5)

        private void DisposeOldHost()
        {
            try
            {
                using (var h = this._host)
                {
                    this._host = null;
                }
            }
            catch
            {
                // ignore here
            }
        }

        /// <inheriteddoc />
        protected override void OnSetSslCertificateByThumbprint(string thumbprint)
        {
            if (string.IsNullOrWhiteSpace(thumbprint))
            {
                thumbprint = null;
            }

            this.Thumbprint = thumbprint != null ? thumbprint.Trim() : null;
        }

        /// <inheriteddoc />
        protected override void OnStart(HttpServerBase.StartStopContext context, ref bool isRunning)
        {
            this.DisposeOldHost();

            var newHost = new ServiceHost(new WcfHttpServerService(this));
            try
            {
                var useHttps = this.UseSecureHttp;
                var credValidator = this.CredentialValidator;
                var port = this.Port;

                var baseUrl = new Uri(string.Format("http{0}://localhost:{1}/",
                                                    useHttps ? "s" : string.Empty,
                                                    port));

                HttpTransportBindingElement transport;
                if (useHttps)
                {
                    transport = new HttpsTransportBindingElement();

                    // setup SSL certificate
                    {
                        var storeLoc = this.SslStoreLocation ?? StoreLocation.CurrentUser;
                        var storeName = this.SslStoreName ?? StoreName.My;
                        var thumb = this.Thumbprint;

                        newHost.Credentials
                               .ClientCertificate
                               .SetCertificate(storeLoc,
                                               storeName,
                                               X509FindType.FindByThumbprint,
                                               thumb);
                    }
                }
                else
                {
                    transport = new HttpTransportBindingElement();
                }

                if (credValidator != null)
                {
                    transport.AuthenticationScheme = AuthenticationSchemes.Basic;

                    newHost.Description.Behaviors.Remove<ServiceCredentials>();
                    newHost.Description.Behaviors.Add(new UserPwdServiceCredentials(credValidator));
                }
                else
                {
                    transport.AuthenticationScheme = AuthenticationSchemes.Anonymous;
                }

                transport.TransferMode = this.ToWcfTransferMode();
                transport.MaxReceivedMessageSize = int.MaxValue;
                transport.MaxBufferPoolSize = int.MaxValue;
                transport.MaxBufferSize = int.MaxValue;

                var binding = new CustomBinding(WcfHttpServerService.CreateWebMessageBindingEncoder(),
                                                transport);

                newHost.AddServiceEndpoint(implementedContract: typeof(IWcfHttpServerService),
                                           binding: binding,
                                           address: baseUrl);

                newHost.Open();
                this._host = newHost;
            }
            catch
            {
                ((IDisposable)newHost).Dispose();

                throw;
            }
        }

        /// <inheriteddoc />
        protected override void OnStop(HttpServerBase.StartStopContext context, ref bool isRunning)
        {
            this.DisposeOldHost();
        }

        private WcfTransferMode ToWcfTransferMode()
        {
            switch (this.TransferMode)
            {
                case HttpTransferMode.Buffered:
                    return WcfTransferMode.Buffered;

                case HttpTransferMode.StreamedRequest:
                    return WcfTransferMode.StreamedRequest;

                case HttpTransferMode.StreamedResponse:
                    return WcfTransferMode.StreamedResponse;
            }

            return WcfTransferMode.Streamed;
        }

        #endregion
    }
}