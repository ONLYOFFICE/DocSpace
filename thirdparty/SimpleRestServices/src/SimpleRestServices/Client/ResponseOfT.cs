using System;
using System.Collections.Generic;
using System.Net;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Extends <see cref="Response"/> to include a strongly-typed return value
    /// from the response.
    /// </summary>
    /// <typeparam name="T">The type of the data included with the response.</typeparam>
    [Serializable]
    public class Response<T> : Response
    {
        /// <summary>
        /// Gets the strongly-typed representation of the value included with this response.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class with the given HTTP status code,
        /// status, strongly-type data, headers, and raw body.
        /// </summary>
        /// <param name="responseCode">The HTTP status code.</param>
        /// <param name="status">A string representation of the HTTP status code.</param>
        /// <param name="data">The strongly-typed data representation of the value returned with this response.</param>
        /// <param name="headers">A collection of all HTTP headers included with this response.</param>
        /// <param name="rawBody">
        /// The raw body of this HTTP response as a text string. When included in the response, this
        /// value should be loaded with the encoding specified in the Content-Encoding and/or
        /// Content-Type HTTP headers.
        /// </param>
        public Response(HttpStatusCode responseCode, string status, T data, IList<HttpHeader> headers, string rawBody)
            : base(responseCode, status, headers, rawBody)
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class with the given HTTP status code,
        /// strongly-type data, headers, and raw body.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="data">The strongly-typed data representation of the value returned with this response.</param>
        /// <param name="headers">A collection of all HTTP headers included with this response.</param>
        /// <param name="rawBody">
        /// The raw body of this HTTP response as a text string. When included in the response, this
        /// value should be loaded with the encoding specified in the Content-Encoding and/or
        /// Content-Type HTTP headers.
        /// </param>
        public Response(HttpStatusCode statusCode, T data, IList<HttpHeader> headers, string rawBody)
            : this(statusCode, statusCode.ToString(), data, headers, rawBody)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class by adding a strongly-typed
        /// data value to a base response.
        /// </summary>
        /// <param name="baseResponse">The base response.</param>
        /// <param name="data">The strongly-typed data representation of the value returned with this response.</param>
        public Response(Response baseResponse, T data)
            : this((baseResponse == null) ? default(int) : baseResponse.StatusCode,
                (baseResponse == null) ? null : baseResponse.Status, data,
                (baseResponse == null) ? null : baseResponse.Headers,
                (baseResponse == null) ? null : baseResponse.RawBody) { }
    }
}
