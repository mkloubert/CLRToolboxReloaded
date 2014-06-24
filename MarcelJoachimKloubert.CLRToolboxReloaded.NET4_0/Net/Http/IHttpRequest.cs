﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// Describes a HTTP request.
    /// </summary>
    public interface IHttpRequest : IObject
    {
        #region Data Members (10)

        /// <summary>
        /// Gets the request address.
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// Gets the MIME type (lower case) of the data of the request body.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the list of GET variables.
        /// </summary>
        IReadOnlyDictionary<string, string> GET { get; }

        /// <summary>
        /// Gets the list of request headers.
        /// </summary>
        IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the upper case name of the HTTP method if available.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the list of POST variables.
        /// </summary>
        IReadOnlyDictionary<string, string> POST { get; }

        /// <summary>
        /// Gets the endpoint of the requesting client.
        /// </summary>
        ITcpAddress RemoteAddress { get; }

        /// <summary>
        /// Gets the merged list of POST and GET variables.
        /// </summary>
        IReadOnlyDictionary<string, string> REQUEST { get; }

        /// <summary>
        /// Gets the request time.
        /// </summary>
        DateTimeOffset Time { get; }

        /// <summary>
        /// Gets the requesting user if available.
        /// </summary>
        IPrincipal User { get; }

        #endregion Data Members

        #region Operations (3)

        /// <summary>
        /// Returns a new stream with the body data.
        /// </summary>
        /// <returns>The stream with the body data.</returns>
        Stream GetBody();

        /// <summary>
        /// Returns the result of <see cref="IHttpRequest.GetBody()" /> as byte array.
        /// </summary>
        /// <returns>The body data as byte array.</returns>
        byte[] GetBodyData();

        /// <summary>
        /// Tries to return the value of <see cref="IHttpRequest.Method" /> as enum.
        /// </summary>
        /// <returns><see cref="IHttpRequest.Method" /> as enum or <see langword="null" /> if their is no representation for the string value.</returns>
        HttpMethod? TryGetKnownMethod();

        #endregion Operations
    }
}