using System;
using System.Collections.Generic;
using System.Net;
using JSIStudios.SimpleRESTServices.Client.Json;
using JSIStudios.SimpleRESTServices.Core;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Specifies the settings for an HTTP REST request.
    /// </summary>
    /// <remarks>
    /// The base implementation does not specify values for the <see cref="RequestSettings.ContentType"/>
    /// or <see cref="RequestSettings.Accept"/> properties. In most cases, these should be properly
    /// set for the particular application either manually or in the constructor of a derived class
    /// (e.g. <see cref="JsonRequestSettings"/>).
    /// </remarks>
    public class RequestSettings
    {
        /// <summary>
        /// Gets or sets the value of the Content-Type HTTP header. If this value is
        /// <c>null</c>, the Content-Type header is omitted from the request.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <c>null</c>.
        /// </remarks>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the number of times this request should be retried if it fails.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is 0.
        /// </remarks>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the delay before retrying a failed request.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <see cref="TimeSpan.Zero"/>.
        /// </remarks>
        public TimeSpan RetryDelay { get; set; }

        /// <summary>
        /// Gets or sets the set of HTTP status codes greater than or equal to 300 which
        /// should be considered a successful result for this request. A value of
        /// <c>null</c> is allowed, and should be treated as an empty collection.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <c>null</c>.
        /// </remarks>
        public IEnumerable<HttpStatusCode> Non200SuccessCodes { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Accept header, which specifies the MIME types that are
        /// acceptable for the response. If this value is <c>null</c>, the Accept header
        /// is omitted from the request.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <c>null</c>.
        /// </remarks>
        public virtual string Accept { get; set; }

        /// <summary>
        /// Gets or sets a map of user-defined actions to execute in response to specific
        /// HTTP status codes. A value of <c>null</c> is allowed, and should be treated as
        /// an empty collection.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <c>null</c>.
        ///
        /// <para>If this value is non-null, and the request results in a status code not
        /// present in this collection, no custom action is executed for the response.</para>
        /// </remarks>
        public Dictionary<HttpStatusCode, Action<Response>> ResponseActions { get; set; }

        /// <summary>
        /// Gets or sets the value of the User-Agent HTTP header. If this value is <c>null</c>,
        /// the User-Agent header is omitted from the request.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <c>null</c>.
        /// </remarks>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the credentials to use for this request. This value can be
        /// <c>null</c> if credentials are not specified.
        /// </summary>
        /// <remarks>
        /// In the base implementation, the default value is <c>null</c>.
        /// </remarks>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        /// <value>
        /// The time to wait before the request times out. In the base
        /// implementation, the default value is 100,000 milliseconds (100 seconds).
        /// </value>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send data in segments to the
        /// Internet resource.
        /// </summary>
        /// <value>
        /// <c>true</c> to send data to the Internet resource in segments; otherwise,
        /// <c>false</c>. In the base implementation, the default value is <c>false</c>.
        /// </value>
        public bool ChunkRequest { get; set; }

        /// <summary>
        /// Gets or sets a user-defined collection to pass as the final argument to the
        /// <see cref="IRequestLogger.Log"/> callback method.
        /// </summary>
        /// <remarks>
        /// This value not used directly within SimpleRESTServices.
        /// </remarks>
        public Dictionary<string, string> ExtendedLoggingData { get; set; }

        /// <summary>
        /// Gets or sets the value of the Content-Length HTTP header.
        /// </summary>
        /// <remarks>
        /// When this value is 0, the <see cref="AllowZeroContentLength"/> property controls
        /// whether or not the Content-Length header is included with the request.
        ///
        /// <para>In the base implementation, the default value is 0.</para>
        /// </remarks>
        public long ContentLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not 0 is a valid value for the Content-Length HTTP header
        /// for this request.
        /// </summary>
        /// <remarks>
        /// When <see cref="ContentLength"/> is non-zero, this value is ignored.
        /// <para>When <see cref="ContentLength"/> is zero and this is <c>true</c>, the
        /// Content-Length HTTP header is added to the request with an explicit value of 0.
        /// Otherwise, when <see cref="ContentLength"/> is 0 the Content-Length HTTP header
        /// is omitted from the request.</para>
        ///
        /// <para>In the base implementation, the default value is <c>false</c>.</para>
        /// </remarks>
        public bool AllowZeroContentLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of connections allowed on the <see cref="ServicePoint"/> object
        /// used for the request. If the value is <c>null</c>, the connection limit value for the
        /// <see cref="ServicePoint"/> object is not altered.
        /// </summary>
        public int? ConnectionLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the request should follow redirection responses.
        /// </summary>
        /// <remarks>
        /// <para>The default value is <see langword="true"/>.</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if the request should follow redirection responses; otherwise,
        /// <see langword="false"/>.
        /// </value>
        /// <seealso cref="HttpWebRequest.AllowAutoRedirect"/>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSettings"/> class with the default values.
        /// </summary>
        public RequestSettings()
        {
            RetryCount = 0;
            RetryDelay = TimeSpan.Zero;
            Non200SuccessCodes = null;
            Timeout = TimeSpan.FromMilliseconds(100000);
            AllowAutoRedirect = true;
        }
    }
}
