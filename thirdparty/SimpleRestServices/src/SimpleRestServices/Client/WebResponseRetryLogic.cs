using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using JSIStudios.SimpleRESTServices.Core;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Provides a simple default implementation of <see cref="IRetryLogic{T, T2}"/>
    /// for HTTP REST requests.
    /// </summary>
    /// <remarks>
    /// This implementation of <see cref="IRetryLogic{T, T2}"/> invokes the callback
    /// method until one of the following conditions is met.
    ///
    /// <list type="bullet">
    /// <item>The operation has been attempted <c>retryCount</c> times.</item>
    /// <item>The <see cref="Response"/> status code is less than 300.</item>
    /// <item>The <see cref="Response"/> status code is contained in the
    /// <c>non200SuccessCodes</c> collection.</item>
    /// </list>
    ///
    /// If the retry delay is greater than zero, <see cref="Thread.Sleep(int)"/> is called
    /// between retry attempts.
    /// </remarks>
    public class RequestRetryLogic : IRetryLogic<Response, HttpStatusCode>
    {
        /// <inheritdoc/>
        public Response Execute(Func<Response> callback, int retryCount = 0, TimeSpan? retryDelay = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException("retryCount");
            if (retryDelay.HasValue && retryDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("retryDelay");

            return Execute(callback, Enumerable.Empty<HttpStatusCode>(), retryCount, retryDelay);
        }

        /// <inheritdoc/>
        public Response Execute(Func<Response> callback, IEnumerable<HttpStatusCode> non200SuccessCodes, int retryCount = 0, TimeSpan? retryDelay = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException("retryCount");
            if (retryDelay.HasValue && retryDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("retryDelay");

            Response response;
            do
            {
                response = callback();
                if (IsRequestSuccessful(response, non200SuccessCodes))
                    return response;

                retryCount--;
                if (retryCount >= 0)
                    Thread.Sleep(retryDelay ?? TimeSpan.Zero);
            }
            while (retryCount >= 0);

            return response;
        }

        /// <summary>
        /// Checks if <paramref name="response"/> is considered a successful HTTP response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="non200SuccessCodes">The HTTP status codes to consider successful, in addition to codes
        /// below 300.</param>
        /// <returns><c>true</c> if <paramref name="response"/> is considered a successful HTTP
        /// response, otherwise <c>false</c>.</returns>
        private static bool IsRequestSuccessful(Response response, IEnumerable<HttpStatusCode> non200SuccessCodes)
        {
            if (response != null && response.StatusCode < (HttpStatusCode)300)
                return true;

            if (non200SuccessCodes == null)
                return false;

            return non200SuccessCodes.Contains(response.StatusCode);
        }
    }
}
