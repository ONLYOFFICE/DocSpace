using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;

namespace JSIStudios.SimpleRESTServices.Core
{
    /// <summary>
    /// Represents custom logging behavior for a REST request.
    /// </summary>
    public interface IRequestLogger
    {
        /// <summary>
        /// Logs a REST request along with its response.
        /// </summary>
        /// <param name="httpMethod">The <see cref="HttpMethod"/> used for the request.</param>
        /// <param name="uri">The complete URI, including the query string (if any).</param>
        /// <param name="requestHeaders">The set of custom headers sent with the request. This may be <c>null</c> if no custom headers were specified.</param>
        /// <param name="requestBody">The body of the request. This is <c>null</c> or empty if the request did not include a body.</param>
        /// <param name="response">The response.</param>
        /// <param name="requestStartTime">The request start time.</param>
        /// <param name="requestEndTime">The request end time.</param>
        /// <param name="extendedData">The user-defined extended data specified in <see cref="RequestSettings.ExtendedLoggingData"/>.</param>
        void Log(HttpMethod httpMethod, string uri, Dictionary<string, string> requestHeaders, string requestBody, Response response, DateTimeOffset requestStartTime, DateTimeOffset requestEndTime, Dictionary<string, string> extendedData);
    }
}