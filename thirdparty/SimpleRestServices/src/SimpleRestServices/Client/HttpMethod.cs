namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Represents the types of HTTP protocol methods that can be used with a REST request.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// Represents an HTTP GET protocol method.
        /// </summary>
        GET,

        /// <summary>
        /// Represents an HTTP POST protocol method that is used to post a new entity
        /// as an addition to a URI.
        /// </summary>
        POST,

        /// <summary>
        /// Represents an HTTP PUT protocol method that is used to replace an entity
        /// identified by a URI.
        /// </summary>
        PUT,

        /// <summary>
        /// Represents an HTTP DELETE protocol method.
        /// </summary>
        DELETE,

        /// <summary>
        /// Represents an HTTP HEAD protocol method. The <see cref="HEAD"/> method is identical to
        /// <see cref="GET"/> except that the server only returns message-headers in the response,
        /// without a message-body.
        /// </summary>
        HEAD,

        /// <summary>
        /// Represents an HTTP PATCH protocol method.
        /// </summary>
        PATCH,

        /// <summary>
        /// Represents an HTTP OPTIONS protocol method.
        /// </summary>
        OPTIONS,

        /// <summary>
        /// Represents an HTTP TRACE protocol method.
        /// </summary>
        TRACE,

        /// <summary>
        /// Represents the HTTP CONNECT protocol method that is used with a proxy that
        /// can dynamically switch to tunneling, as in the case of SSL tunneling.
        /// </summary>
        CONNECT,

        /// <summary>
        /// Represents an HTTP COPY protocol method.
        /// </summary>
        COPY,

        /// <summary>
        /// Represents an HTTP MOVE protocol method.
        /// </summary>
        MOVE,
    }
}
