using System;
using System.Collections.Generic;
using System.Net;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Represents the basic response of an HTTP REST request, where the body of the response
    /// is stored as a text string.
    /// </summary>
    [Serializable]
    public class Response
    {
        /// <summary>
        /// Gets the HTTP status code for this response.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets a string representation of the HTTP status code for this response.
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Gets a collection of all HTTP headers included with this response.
        /// </summary>
        public IList<HttpHeader> Headers { get; private set; }

        /// <summary>
        /// Gets the raw body of this HTTP response as a text string.
        /// </summary>
        public string RawBody { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class with the given HTTP status code,
        /// status, headers, and raw body.
        /// </summary>
        /// <param name="responseCode">The HTTP status code.</param>
        /// <param name="status">A string representation of the HTTP status code.</param>
        /// <param name="headers">A collection of all HTTP headers included with this response.</param>
        /// <param name="rawBody">
        /// The raw body of this HTTP response as a text string. When included in the response, this
        /// value should be loaded with the encoding specified in the Content-Encoding and/or
        /// Content-Type HTTP headers.
        /// </param>
        public Response(HttpStatusCode responseCode, string status, IList<HttpHeader> headers, string rawBody)
        {
            StatusCode = responseCode;
            Status = status;
            Headers = headers;
            RawBody = rawBody;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class with the given HTTP status code,
        /// headers, and raw body.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="headers">A collection of all HTTP headers included with this response.</param>
        /// <param name="rawBody">
        /// The raw body of this HTTP response as a text string. When included in the response, this
        /// value should be loaded with the encoding specified in the Content-Encoding and/or
        /// Content-Type HTTP headers.
        /// </param>
        public Response(HttpStatusCode statusCode, IList<HttpHeader> headers, string rawBody)
            : this(statusCode, statusCode.ToString(), headers, rawBody)
        {
        }
    }
}
