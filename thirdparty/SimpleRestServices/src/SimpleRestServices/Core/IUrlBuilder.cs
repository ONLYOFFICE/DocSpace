using System;
using System.Collections.Generic;

namespace JSIStudios.SimpleRESTServices.Core
{
    /// <summary>
    /// Represents a builder which can construct a complete URI for a GET or HEAD request
    /// from a base URI and a collection of query parameters.
    /// </summary>
    public interface IUrlBuilder
    {
        /// <summary>
        /// Constructs a complete URI for an HTTP request using a base URI and a
        /// collection of query string parameters.
        /// </summary>
        /// <remarks>
        /// If <paramref name="baseUrl"/> already contains a query string, the specified
        /// <paramref name="queryStringParameters"/> are appended to the existing query string.
        /// This method does not perform substitution for any template parameters
        /// which may exist in <paramref name="baseUrl"/>. If <paramref name="queryStringParameters"/>
        /// is <c>null</c> or empty, <paramref name="baseUrl"/> is returned unchanged.
        /// </remarks>
        /// <param name="baseUrl">The base URI.</param>
        /// <param name="queryStringParameters">A collection of parameters to place in the URI query string,
        /// or <c>null</c> if there are no parameters.</param>
        /// <returns>A <see cref="Uri"/> constructed from <paramref name="baseUrl"/> and the specified
        /// <paramref name="queryStringParameters"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="baseUrl"/> is <c>null</c>.</exception>
        Uri Build(Uri baseUrl, Dictionary<string, string> queryStringParameters);
    }
}