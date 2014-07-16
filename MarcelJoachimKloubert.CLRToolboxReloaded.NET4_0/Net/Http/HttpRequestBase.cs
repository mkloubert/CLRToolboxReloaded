// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// A basic HTTP request context.
    /// </summary>
    public abstract class HttpRequestBase : ObjectBase, IHttpRequest
    {
        #region Fields (1)

        private readonly Func<Stream> _ON_GET_BODY_FUNC;

        #endregion Fields (1)

        #region Constructors (4)

        /// <inheriteddoc />
        protected HttpRequestBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._ON_GET_BODY_FUNC = this.OnGetBody_ThreadSafe;
            }
            else
            {
                this._ON_GET_BODY_FUNC = this.OnGetBody;
            }
        }

        /// <inheriteddoc />
        protected HttpRequestBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected HttpRequestBase(object sync)
            : this(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HttpRequestBase()
            : this(isSynchronized: true)
        {
        }

        #endregion Constructors

        #region Properties (10)

        /// <inheriteddoc />
        public abstract Uri Address
        {
            get;
        }

        /// <inheriteddoc />
        public abstract string ContentType
        {
            get;
        }

        /// <inheriteddoc />
        public abstract IReadOnlyDictionary<string, string> GET
        {
            get;
        }

        /// <inheriteddoc />
        public abstract IReadOnlyDictionary<string, string> Headers
        {
            get;
        }

        /// <inheriteddoc />
        public abstract string Method
        {
            get;
        }

        /// <inheriteddoc />
        public abstract IReadOnlyDictionary<string, string> POST
        {
            get;
        }

        /// <inheriteddoc />
        public abstract ITcpAddress RemoteAddress
        {
            get;
        }

        /// <inheriteddoc />
        public abstract IReadOnlyDictionary<string, string> REQUEST
        {
            get;
        }

        /// <inheriteddoc />
        public abstract DateTimeOffset Time
        {
            get;
        }

        /// <inheriteddoc />
        public abstract IPrincipal User
        {
            get;
        }

        #endregion Properties

        #region Methods (7)

        /// <summary>
        /// Creates a value for the <see cref="HttpRequestBase.REQUEST" /> property.
        /// </summary>
        /// <returns>The value for the <see cref="HttpRequestBase.REQUEST" /> property.</returns>
        protected virtual IReadOnlyDictionary<string, string> CreateRequestVarsDictionary()
        {
            var result = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                              emptyIsNull: true));

            var getVars = this.GET;
            if (getVars != null)
            {
                getVars.ForAll(throwExceptions: false,
                               action: ctx =>
                                   {
                                       var key = ctx.Item.Key;

                                       SetVar(vars: ctx.State.Vars,
                                              key: key,
                                              value: ctx.Item.Value);
                                   },
                               actionState: new
                                   {
                                       Vars = result,
                                   });
            }

            var postVars = this.POST;
            if (postVars != null)
            {
                postVars.ForAll(throwExceptions: false,
                                action: ctx =>
                                    {
                                        var key = ctx.Item.Key;

                                        SetVar(vars: ctx.State.Vars,
                                               key: key,
                                               value: ctx.Item.Value);
                                    },
                               actionState: new
                                    {
                                        Vars = result,
                                    });
            }

            return new ReadOnlyDictionaryWrapper<string, string>(dict: result);
        }

        /// <inheriteddoc />
        public Stream GetBody()
        {
            return this._ON_GET_BODY_FUNC();
        }

        /// <inheriteddoc />
        public byte[] GetBodyData()
        {
            using (var stream = this.GetBody())
            {
                if (stream != null)
                {
                    using (var temp = new MemoryStream())
                    {
                        stream.CopyTo(temp);

                        return temp.ToArray();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The logic for the <see cref="HttpRequestBase.GetBody()" /> method.
        /// </summary>
        /// <returns>The stream with the body data.</returns>
        protected abstract Stream OnGetBody();

        private Stream OnGetBody_ThreadSafe()
        {
            Stream result;

            lock (this._SYNC)
            {
                result = this.OnGetBody();
            }

            return result;
        }

        /// <summary>
        /// Sets the value of a variable.
        /// </summary>
        /// <param name="vars">The variable container.</param>
        /// <param name="key">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="vars" /> is <see langword="null" />.
        /// </exception>
        protected static void SetVar(IDictionary<string, string> vars,
                                     string key, string value)
        {
            if (vars == null)
            {
                throw new ArgumentNullException("vars");
            }

            if (string.IsNullOrEmpty(value))
            {
                value = null;
            }

            vars[key ?? string.Empty] = value;
        }

        /// <inheriteddoc />
        public HttpMethod? TryGetKnownMethod()
        {
            string method = this.Method;
            if (string.IsNullOrWhiteSpace(method))
            {
                return HttpMethod.GET;
            }

            global::MarcelJoachimKloubert.CLRToolbox.Net.Http.HttpMethod result;
            if (Enum.TryParse<HttpMethod>(value: this.Method, ignoreCase: true, result: out result))
            {
                return result;
            }

            return null;
        }

        #endregion Methods
    }
}