using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JSIStudios.SimpleRESTServices.Client.Json;
using JSIStudios.SimpleRESTServices.Core;
#if !NET35
using System.Diagnostics.Contracts;
#endif

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Implements basic support for <see cref="IRestService"/> in terms of an implementation
    /// of <see cref="IRetryLogic{T, T2}"/>, <see cref="IRequestLogger"/>,
    /// <see cref="IUrlBuilder"/>, and <see cref="IStringSerializer"/>.
    /// </summary>
    public abstract class RestServiceBase : IRestService
    {
        private readonly IRetryLogic<Response, HttpStatusCode> _retryLogic;
        private readonly IRequestLogger _logger;
        private readonly IUrlBuilder _urlBuilder;
        private readonly IStringSerializer _stringSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceBase"/> class with the specified string serializer
        /// and the default retry logic and URL builder.
        /// </summary>
        /// <param name="stringSerializer">The string serializer to use for requests from this service.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="stringSerializer"/> is <c>null</c>.</exception>
        protected RestServiceBase(IStringSerializer stringSerializer) : this(stringSerializer, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceBase"/> class with the specified string serializer
        /// and logger, and the default retry logic and URL builder.
        /// </summary>
        /// <param name="stringSerializer">The string serializer to use for requests from this service.</param>
        /// <param name="requestLogger">The logger to use for requests. Specify <c>null</c> if requests do not need to be logged.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="stringSerializer"/> is <c>null</c>.</exception>
        protected RestServiceBase(IStringSerializer stringSerializer, IRequestLogger requestLogger) : this(stringSerializer, requestLogger, new RequestRetryLogic(), new UrlBuilder()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceBase"/> class with the specified string serializer,
        /// logger, retry logic, and URI builder.
        /// </summary>
        /// <param name="stringSerializer">The string serializer to use for requests from this service.</param>
        /// <param name="logger">The logger to use for requests. Specify <c>null</c> if requests do not need to be logged.</param>
        /// <param name="retryLogic">The retry logic to use for REST operations.</param>
        /// <param name="urlBuilder">The URL builder to use for constructing URLs with query parameters.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="stringSerializer"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="retryLogic"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="urlBuilder"/> is <c>null</c>.</para>
        /// </exception>
        protected RestServiceBase(IStringSerializer stringSerializer, IRequestLogger logger, IRetryLogic<Response, HttpStatusCode> retryLogic, IUrlBuilder urlBuilder)
        {
            if (stringSerializer == null)
                throw new ArgumentNullException("stringSerializer");
            if (retryLogic == null)
                throw new ArgumentNullException("retryLogic");
            if (urlBuilder == null)
                throw new ArgumentNullException("urlBuilder");

            _retryLogic = retryLogic;
            _logger = logger;
            _urlBuilder = urlBuilder;
            _stringSerializer = stringSerializer;
        }

        /// <summary>
        /// Gets the default <see cref="RequestSettings"/> to use for requests sent from this service.
        /// </summary>
        protected virtual RequestSettings DefaultRequestSettings
        {
            get
            {
                return new RequestSettings();
            }
        }

        /// <inheritdoc/>
        public virtual Response<T> Execute<T, TBody>(string url, HttpMethod method, TBody body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url cannot be empty");

            return Execute<T, TBody>(new Uri(url), method, body, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response<T> Execute<T, TBody>(Uri url, HttpMethod method, TBody body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            var rawBody = _stringSerializer.Serialize(body);
            return Execute<T>(url, method, rawBody, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response<T> Execute<T>(string url, HttpMethod method, string body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url cannot be empty");

            return Execute<T>(new Uri(url), method, body, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response<T> Execute<T>(Uri url, HttpMethod method, string body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return Execute(url, method, BuildWebResponse<T>, body, headers, queryStringParameters, settings) as Response<T>;
        }

        /// <inheritdoc/>
        public virtual Response Execute<TBody>(string url, HttpMethod method, TBody body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url cannot be empty");

            return Execute(new Uri(url), method, body, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response Execute<TBody>(Uri url, HttpMethod method, TBody body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            var rawBody = _stringSerializer.Serialize(body);
            return Execute(url, method, rawBody, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response Execute(string url, HttpMethod method, string body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url cannot be empty");

            return Execute(new Uri(url), method, body, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response Execute(Uri url, HttpMethod method, string body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return Execute(url, method, null, body, headers, queryStringParameters, settings);
        }

        /// <inheritdoc/>
        public virtual Response Execute(Uri url, HttpMethod method, Func<HttpWebResponse, bool, Response> responseBuilderCallback, string body, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            return ExecuteRequest(url, method, responseBuilderCallback, headers, queryStringParameters, settings, (req) =>
            {
                // Encode the parameters as form data:
                if (!string.IsNullOrEmpty(body))
                {
                    byte[] formData = UTF8Encoding.UTF8.GetBytes(body);
                    req.ContentLength = formData.Length;

                    // Send the request:
                    using (Stream post = req.GetRequestStream())
                    {
                        post.Write(formData, 0, formData.Length);
                    }
                }

                return body;
            });     
        }

        /// <inheritdoc/>
        public virtual Response<T> Stream<T>(string url, HttpMethod method, Stream content, int bufferSize, long maxReadLength, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings, Action<long> progressUpdated)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (content == null)
                throw new ArgumentNullException("content");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url cannot be empty");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");
            if (maxReadLength < 0)
                throw new ArgumentOutOfRangeException("maxReadLength");

            return Stream<T>(new Uri(url), method, content, bufferSize, maxReadLength, headers, queryStringParameters, settings, progressUpdated)  as Response<T>;
        }

        /// <inheritdoc/>
        public virtual Response Stream(string url, HttpMethod method, Stream content, int bufferSize, long maxReadLength, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings, Action<long> progressUpdated)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (content == null)
                throw new ArgumentNullException("content");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url cannot be empty");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");
            if (maxReadLength < 0)
                throw new ArgumentOutOfRangeException("maxReadLength");

            return Stream(new Uri(url), method, content, bufferSize, maxReadLength, headers, queryStringParameters, settings, progressUpdated);
        }

        /// <inheritdoc/>
        public virtual Response<T> Stream<T>(Uri url, HttpMethod method, Stream content, int bufferSize, long maxReadLength, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings, Action<long> progressUpdated)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (content == null)
                throw new ArgumentNullException("content");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");
            if (maxReadLength < 0)
                throw new ArgumentOutOfRangeException("maxReadLength");

            return Stream(url, method, BuildWebResponse<T>, content, bufferSize, maxReadLength, headers, queryStringParameters, settings, progressUpdated) as Response<T>;
        }

        /// <inheritdoc/>
        public virtual Response Stream(Uri url, HttpMethod method, Stream content, int bufferSize, long maxReadLength, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings, Action<long> progressUpdated)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (content == null)
                throw new ArgumentNullException("content");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");
            if (maxReadLength < 0)
                throw new ArgumentOutOfRangeException("maxReadLength");

            return Stream(url, method, null, content, bufferSize, maxReadLength, headers, queryStringParameters, settings, progressUpdated);
        }

        /// <summary>
        /// Executes a REST request with a <see cref="System.IO.Stream"/> <paramref name="content"/>
        /// and user-defined callback function for constructing the resulting <see cref="Response"/>
        /// object.
        /// </summary>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="responseBuilderCallback">A user-specified function used to construct the resulting <see cref="Response"/>
        /// object from the <see cref="HttpWebResponse"/> and a Boolean value specifying whether or not a <see cref="WebException"/>
        /// was thrown during the request. If this value is <c>null</c>, this method is equivalent to calling
        /// <see cref="Stream(Uri, HttpMethod, Stream, int, long, Dictionary{string, string}, Dictionary{string, string}, RequestSettings, Action{long})"/>.</param>
        /// <param name="content">A stream providing the body of the request.</param>
        /// <param name="bufferSize">
        /// The size of the buffer used for copying data from <paramref name="content"/> to the
        /// HTTP request stream.
        /// </param>
        /// <param name="maxReadLength">
        /// The maximum number of bytes to send with the request. This parameter is optional.
        /// If the value is 0, the request will include all data from <paramref name="content"/>.
        /// </param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. This parameter is
        /// optional. If the value is <c>null</c>, no custom headers are added to the HTTP request.
        /// </param>
        /// <param name="queryStringParameters">
        /// A collection of parameters to add to the query string portion of the request URI.
        /// This parameter is optional. If the value is <c>null</c>, no parameters are added
        /// to the query string.
        /// </param>
        /// <param name="settings">
        /// The settings to use for the request. This parameters is optional. If the value is
        /// <c>null</c>, an implementation-specific set of default settings will be used for the request.
        /// </param>
        /// <param name="progressUpdated">
        /// A user-defined callback function for reporting progress of the send operation.
        /// This parameter is optional. If the value is <c>null</c>, the method does not report
        /// progress updates to the caller.
        /// </param>
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="url"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="content"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="bufferSize"/> is less than or equal to zero.
        /// <para>-or-</para>
        /// <para>If <paramref name="maxReadLength"/> is less than zero.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        public virtual Response Stream(Uri url, HttpMethod method, Func<HttpWebResponse, bool, Response> responseBuilderCallback, Stream content, int bufferSize, long maxReadLength, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings, Action<long> progressUpdated)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (content == null)
                throw new ArgumentNullException("content");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");
            if (maxReadLength < 0)
                throw new ArgumentOutOfRangeException("maxReadLength");

            return ExecuteRequest(url, method, responseBuilderCallback, headers, queryStringParameters, settings, (req) =>
            {
                long bytesWritten = 0;

                if (settings.ChunkRequest || maxReadLength > 0 )
                {
                    req.SendChunked = settings.ChunkRequest;
                    req.AllowWriteStreamBuffering = false;

                    req.ContentLength = content.Length > maxReadLength ? maxReadLength : content.Length;
                }

                using (Stream stream = req.GetRequestStream())
                {
                    var buffer = new byte[bufferSize];
                    int count;
                    while ((count = content.Read(buffer, 0, maxReadLength > 0 ? (int)Math.Min(bufferSize, maxReadLength - bytesWritten) : bufferSize)) > 0)
                    {
                        bytesWritten += count;
                        stream.Write(buffer, 0, count);

                        if (progressUpdated != null)
                            progressUpdated(bytesWritten);

                        if (maxReadLength > 0 && bytesWritten >= maxReadLength)
                            break;
                    }
                }

                return "[STREAM CONTENT]";
            });
        }

        /// <summary>
        /// Executes a REST request indirectly via a callback function <paramref name="executeCallback"/>,
        /// and using a user-defined callback function <paramref name="responseBuilderCallback"/> for
        /// constructing the resulting <see cref="Response"/> object.
        /// </summary>
        /// <remarks>
        /// The callback method <paramref name="executeCallback"/> is responsible for setting the body
        /// of the request, if any, before executing the request. The callback method returns a string
        /// representation of the body of the final request when available, otherwise returns a string
        /// indicating the body is no longer available (e.g. was sent as a stream, or is binary). The
        /// result is only required for passing as an argument to <see cref="IRequestLogger.Log"/>.
        ///
        /// <para>The Boolean argument to <paramref name="responseBuilderCallback"/> indicates whether
        /// or not an exception was thrown while executing the request. The value is <c>true</c>
        /// if an exception occurred, otherwise <c>false</c>.</para>
        /// </remarks>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="responseBuilderCallback">A user-specified function used to construct the resulting <see cref="Response"/>
        /// object from the <see cref="HttpWebResponse"/> and a Boolean value specifying whether or not a <see cref="WebException"/>
        /// was thrown during the request. If this value is <c>null</c>, a default method is used to construct
        /// the resulting <see cref="Response"/> object.</param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. If the value is
        /// <c>null</c>, no custom headers are added to the HTTP request.
        /// </param>
        /// <param name="queryStringParameters">
        /// A collection of parameters to add to the query string portion of the request URI.
        /// If the value is <c>null</c>, no parameters are added to the query string.
        /// </param>
        /// <param name="settings">
        /// The settings to use for the request. If the value is <c>null</c>, the default settings returned
        /// by <see cref="DefaultRequestSettings"/> will be used for the request.
        /// </param>
        /// <param name="executeCallback"></param>
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="url"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="executeCallback"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        public virtual Response ExecuteRequest(Uri url, HttpMethod method, Func<HttpWebResponse, bool, Response> responseBuilderCallback, Dictionary<string, string> headers, Dictionary<string, string> queryStringParameters, RequestSettings settings, Func<HttpWebRequest, string> executeCallback)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (executeCallback == null)
                throw new ArgumentNullException("executeCallback");

            url = _urlBuilder.Build(url, queryStringParameters);

            if (settings == null)
                settings = DefaultRequestSettings;

            return _retryLogic.Execute(() =>
            {
                Response response;

                var startTime = DateTimeOffset.UtcNow;

                string requestBodyText = null;
                try
                {
                    var req = WebRequest.Create(url) as HttpWebRequest;
                    req.Method = method.ToString();
                    req.ContentType = settings.ContentType;
                    req.Accept = settings.Accept;
                    req.AllowAutoRedirect = settings.AllowAutoRedirect;
                    if(settings.ContentLength > 0 || settings.AllowZeroContentLength)
                        req.ContentLength = settings.ContentLength;

                    if (settings.ConnectionLimit != null)
                        req.ServicePoint.ConnectionLimit = settings.ConnectionLimit.Value;

                    req.Timeout = (int)settings.Timeout.TotalMilliseconds;

                    if (!string.IsNullOrEmpty(settings.UserAgent))
                        req.UserAgent = settings.UserAgent;

                    if (settings.Credentials != null)
                        req.Credentials = settings.Credentials;

                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            req.Headers.Add(header.Key, header.Value);
                        }
                    }

                    requestBodyText = executeCallback(req);

                    using (var resp = req.GetResponse() as HttpWebResponse)
                    {
                        if (responseBuilderCallback != null)
                            response = responseBuilderCallback(resp, false);
                        else
                            response = BuildWebResponse(resp);
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response == null)
                        throw;

                    using (var resp = ex.Response as HttpWebResponse)
                    {
                        if (responseBuilderCallback != null)
                            response = responseBuilderCallback(resp, true);
                        else
                            response = BuildWebResponse(resp);
                    }
                }
                var endTime = DateTimeOffset.UtcNow;

                // Log the request
                if (_logger != null)
                    _logger.Log(method, url.OriginalString, headers, requestBodyText, response, startTime, endTime, settings.ExtendedLoggingData);

                if (response != null && settings.ResponseActions != null && settings.ResponseActions.ContainsKey(response.StatusCode))
                {
                    var action = settings.ResponseActions[response.StatusCode];
                    if (action != null)
                        action(response);
                }

                return response;
            }, settings.Non200SuccessCodes, settings.RetryCount, settings.RetryDelay);
        }

        /// <summary>
        /// Build a <see cref="Response"/> for a given <see cref="HttpWebResponse"/>.
        /// </summary>
        /// <param name="resp">The response from the REST request.</param>
        /// <returns>A <see cref="Response"/> object representing the result of the REST API call.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="resp"/> is <c>null</c>.</exception>
        private Response BuildWebResponse(HttpWebResponse resp)
        {
            if (resp == null)
                throw new ArgumentNullException("resp");

            string respBody;
            using (var reader = new StreamReader(resp.GetResponseStream(), GetEncoding(resp)))
            {
                respBody = reader.ReadToEnd();
            }

            var respHeaders =
                resp.Headers.AllKeys.Select(key => new HttpHeader(key, resp.GetResponseHeader(key)))
                .ToList();
            return new Response(resp.StatusCode, respHeaders, respBody);
        }

        /// <summary>
        /// Determines the <see cref="Encoding"/> to use for reading an <see cref="HttpWebResponse"/>
        /// body as text based on the response headers.
        /// </summary>
        /// <remarks>
        /// If the response provides the <c>Content-Encoding</c> header, then it is used.
        /// Otherwise, if the optional <c>charset</c> parameter to the <c>Content-Type</c> header
        /// is provided, then it is used. If no encoding is specified in the headers, or if the
        /// encoding specified in the headers is not valid, <see cref="Encoding.Default"/> is
        /// used.
        /// </remarks>
        /// <param name="response">The response to examine</param>
        /// <returns>The <see cref="Encoding"/> to use when reading the response stream as text.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        private Encoding GetEncoding(HttpWebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");
#if !NET35
            Contract.Ensures(Contract.Result<Encoding>() != null);
            Contract.EndContractBlock();
#endif

            string contentEncoding = response.ContentEncoding;
            if (!string.IsNullOrEmpty(contentEncoding))
            {
                try
                {
                    return Encoding.GetEncoding(contentEncoding);
                }
                catch (ArgumentException)
                {
                    // continue below
                }
            }

            string characterSet = response.CharacterSet;
            if (string.IsNullOrEmpty(characterSet))
                return Encoding.Default;

            try
            {
                return Encoding.GetEncoding(characterSet) ?? Encoding.Default;
            }
            catch (ArgumentException)
            {
                return Encoding.Default;
            }
        }

        /// <summary>
        /// Builds a <see cref="Response{T}"/> for a given <see cref="HttpWebResponse"/>
        /// containing a serialized representation of strongly-typed data in the body of
        /// the response.
        /// </summary>
        /// <typeparam name="T">The object model type for the data contained in the body of <paramref name="resp"/>.</typeparam>
        /// <param name="resp">The response from the REST request.</param>
        /// <param name="isError">Indicates whether the response is an error response. If the value is <c>true</c> the response 
        /// will not be deserialized to <typeparamref name="T"/></param>
        /// <returns>A <see cref="Response{T}"/> instance representing the response from the REST API call.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="resp"/> is <c>null</c>.</exception>
        /// <exception cref="StringSerializationException">
        /// If the body of <paramref name="resp"/> could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        private Response<T> BuildWebResponse<T>(HttpWebResponse resp, bool isError = false)
        {
            var baseReponse = BuildWebResponse(resp);
            T data = default(T);

            if (!isError)
            {
                if (baseReponse != null && !string.IsNullOrEmpty(baseReponse.RawBody))
                    data = _stringSerializer.Deserialize<T>(baseReponse.RawBody);
            }

            return new Response<T>(baseReponse, data);
        }
    }
}
