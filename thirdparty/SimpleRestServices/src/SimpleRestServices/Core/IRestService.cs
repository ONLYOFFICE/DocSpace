using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using JSIStudios.SimpleRESTServices.Client.Json;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Represents a service for executing generic REST requests.
    /// </summary>
    public interface IRestService
    {
        /// <summary>
        /// Executes a REST request with a strongly-typed <paramref name="body"/> and result.
        /// </summary>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <typeparam name="TBody">The type of the data included in the body of the REST request.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The strongly-typed data to include in the body of the request. If the value is <c>null</c>,
        /// the behavior is implementation-defined.
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
        /// <returns>Returns a <see cref="Response{T}"/> object containing the HTTP status code, headers, body,
        /// and strongly-typed data from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException">If <paramref name="url"/> is not a valid base URI.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        /// <exception cref="StringSerializationException">
        /// If the body of the response could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        Response<T> Execute<T, TBody>(
            String url,
            HttpMethod method,
            TBody body,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a strongly-typed <paramref name="body"/> and result.
        /// </summary>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <typeparam name="TBody">The type of the data included in the body of the REST request.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The strongly-typed data to include in the body of the request. If the value is <c>null</c>,
        /// the behavior is implementation-defined.
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
        /// <returns>Returns a <see cref="Response{T}"/> object containing the HTTP status code, headers, body,
        /// and strongly-typed data from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        /// <exception cref="StringSerializationException">
        /// If the body of the response could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        Response<T> Execute<T, TBody>(
            Uri url,
            HttpMethod method,
            TBody body,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a string <paramref name="body"/> and strongly-typed result.
        /// </summary>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
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
        /// <returns>Returns a <see cref="Response{T}"/> object containing the HTTP status code, headers, body,
        /// and strongly-typed data from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException">If <paramref name="url"/> is not a valid base URI.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        /// <exception cref="StringSerializationException">
        /// If the body of the response could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        Response<T> Execute<T>(
            String url,
            HttpMethod method,
            string body = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a string <paramref name="body"/> and strongly-typed result.
        /// </summary>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
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
        /// <returns>Returns a <see cref="Response{T}"/> object containing the HTTP status code, headers, body,
        /// and strongly-typed data from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        /// <exception cref="StringSerializationException">
        /// If the body of the response could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        Response<T> Execute<T>(
            Uri url,
            HttpMethod method,
            string body = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a strongly-typed <paramref name="body"/> and basic result (text or no content).
        /// </summary>
        /// <typeparam name="TBody">The type of the data included in the body of the REST request.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The strongly-typed data to include in the body of the request. If the value is <c>null</c>,
        /// the behavior is implementation-defined.
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
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException">If <paramref name="url"/> is not a valid base URI.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        Response Execute<TBody>(
            String url,
            HttpMethod method,
            TBody body,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a strongly-typed <paramref name="body"/> and basic result (text or no content).
        /// </summary>
        /// <typeparam name="TBody">The type of the data included in the body of the REST request.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The strongly-typed data to include in the body of the request. If the value is <c>null</c>,
        /// the behavior is implementation-defined.
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
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        Response Execute<TBody>(
            Uri url,
            HttpMethod method,
            TBody body,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a string <paramref name="body"/> and basic result (text or no content).
        /// </summary>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
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
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException">If <paramref name="url"/> is not a valid base URI.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        Response Execute(
            String url,
            HttpMethod method,
            string body = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a string <paramref name="body"/> and basic result (text or no content).
        /// </summary>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="body">
        /// The body of the request. This parameter is optional. If the value is <c>null</c>,
        /// the request is sent without a body.
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
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        Response Execute(
            Uri url,
            HttpMethod method,
            string body = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null);

        /// <summary>
        /// Executes a REST request with a <see cref="System.IO.Stream"/> <paramref name="content"/> and strongly-typed result.
        /// </summary>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
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
        /// <returns>Returns a <see cref="Response{T}"/> object containing the HTTP status code, headers, body,
        /// and strongly-typed data from the REST response.</returns>
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
        /// <exception cref="StringSerializationException">
        /// If the body of the response could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        Response<T> Stream<T>(
            Uri url,
            HttpMethod method,
            Stream content,
            int bufferSize,
            long maxReadLength = 0,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null,
            Action<long> progressUpdated = null);

        /// <summary>
        /// Executes a REST request with a <see cref="System.IO.Stream"/> <paramref name="content"/> and basic result (text or no content).
        /// </summary>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
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
        Response Stream(
            Uri url,
            HttpMethod method,
            Stream content,
            int bufferSize,
            long maxReadLength = 0,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null,
            Action<long> progressUpdated = null);

        /// <summary>
        /// Executes a REST request with a <see cref="System.IO.Stream"/> <paramref name="content"/> and strongly-typed result.
        /// </summary>
        /// <typeparam name="T">The type of the data returned in the REST response.</typeparam>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
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
        /// <returns>Returns a <see cref="Response{T}"/> object containing the HTTP status code, headers, body,
        /// and strongly-typed data from the REST response.</returns>
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
        /// <exception cref="UriFormatException">If <paramref name="url"/> is not a valid base URI.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        /// <exception cref="StringSerializationException">
        /// If the body of the response could not be deserialized to an object of type <typeparamref name="T"/>.
        /// </exception>
        Response<T> Stream<T>(
            string url,
            HttpMethod method,
            Stream content,
            int bufferSize,
            long maxReadLength = 0,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null,
            Action<long> progressUpdated = null);

        /// <summary>
        /// Executes a REST request with a <see cref="System.IO.Stream"/> <paramref name="content"/> and basic result (text or no content).
        /// </summary>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
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
        /// <exception cref="UriFormatException">If <paramref name="url"/> is not a valid base URI.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        Response Stream(
            string url,
            HttpMethod method,
            Stream content,
            int bufferSize,
            long maxReadLength = 0,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryStringParameters = null,
            RequestSettings settings = null,
            Action<long> progressUpdated = null);

        /// <summary>
        /// Executes a REST request with a string <paramref name="body"/> and user-defined
        /// callback function for constructing the resulting <see cref="Response"/> object.
        /// </summary>
        /// <remarks>
        /// The Boolean argument to <paramref name="responseBuilderCallback"/> indicates whether
        /// or not an exception was thrown while executing the request. The value is <c>true</c>
        /// if an exception occurred, otherwise <c>false</c>.
        /// </remarks>
        /// <param name="url">The base URI.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="responseBuilderCallback">A user-specified function used to construct the resulting <see cref="Response"/>
        /// object from the <see cref="HttpWebResponse"/> and a Boolean value specifying whether or not a <see cref="WebException"/>
        /// was thrown during the request. If this value is <c>null</c>, this method is equivalent to calling
        /// <see cref="Execute(Uri, HttpMethod, string, Dictionary{string, string}, Dictionary{string, string}, RequestSettings)"/>.</param>
        /// <param name="body">The body of the request. If the value is <c>null</c>, the request is sent without a body.</param>
        /// <param name="headers">
        /// A collection of custom HTTP headers to include with the request. If the value is
        /// <c>null</c>, no custom headers are added to the HTTP request.
        /// </param>
        /// <param name="queryStringParameters">
        /// A collection of parameters to add to the query string portion of the request URI.
        /// If the value is <c>null</c>, no parameters are added to the query string.
        /// </param>
        /// <param name="settings">
        /// The settings to use for the request. If the value is <c>null</c>, an implementation-specific
        /// set of default settings will be used for the request.
        /// </param>
        /// <returns>Returns a <see cref="Response"/> object containing the HTTP status code, headers,
        /// and body from the REST response.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="url"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="method"/> is not supported by the service.</exception>
        Response Execute(
            Uri url,
            HttpMethod method,
            Func<HttpWebResponse, bool, Response> responseBuilderCallback,
            string body,
            Dictionary<string, string> headers,
            Dictionary<string, string> queryStringParameters,
            RequestSettings settings);
    }
}