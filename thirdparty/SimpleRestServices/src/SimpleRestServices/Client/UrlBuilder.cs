using System;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Core;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// A simple, default implementation of a URI builder.
    /// </summary>
    public class UrlBuilder : IUrlBuilder
    {
        /// <inheritdoc/>
        public Uri Build(Uri baseUrl, Dictionary<string, string> queryStringParameters)
        {
            if (baseUrl == null)
                throw new ArgumentNullException("baseUrl");

            return new Uri(Build(baseUrl.AbsoluteUri, queryStringParameters));
        }

        /// <summary>
        /// Constructs a complete URI for an HTTP request using a base URI and a
        /// collection of query string parameters.
        /// </summary>
        /// <remarks>
        /// If <paramref name="baseAbsoluteUrl"/> already contains a query string, the specified
        /// <paramref name="queryStringParameters"/> are appended to the existing query string.
        /// This method does not perform substitution for any template parameters
        /// which may exist in <paramref name="baseAbsoluteUrl"/>. If <paramref name="queryStringParameters"/>
        /// is <c>null</c> or empty, <paramref name="baseAbsoluteUrl"/> is returned unchanged.
        /// </remarks>
        /// <param name="baseAbsoluteUrl">The base URI.</param>
        /// <param name="queryStringParameters">A collection of parameters to place in the URI query string,
        /// or <c>null</c> if there are no parameters.</param>
        /// <returns>A <see cref="Uri"/> constructed from <paramref name="baseAbsoluteUrl"/> and the specified
        /// <paramref name="queryStringParameters"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="baseAbsoluteUrl"/> is <c>null</c>.</exception>
        public string Build(string baseAbsoluteUrl, Dictionary<string, string> queryStringParameters)
        {
            if (baseAbsoluteUrl == null)
                throw new ArgumentNullException("baseAbsoluteUrl");

            if (queryStringParameters != null && queryStringParameters.Count > 0)
            {
                var paramsCombinedList =
                    queryStringParameters.Select(
                        param =>
                        string.Format("{0}={1}", System.Web.HttpUtility.UrlEncode(param.Key),
                                      System.Web.HttpUtility.UrlEncode(param.Value)));
                var paramsCombined = string.Join("&", paramsCombinedList.ToArray());

                var separator = baseAbsoluteUrl.Contains("?") ? "&" : "?";
                return baseAbsoluteUrl + separator + paramsCombined;
            }

            return baseAbsoluteUrl;
        }
    }
}
