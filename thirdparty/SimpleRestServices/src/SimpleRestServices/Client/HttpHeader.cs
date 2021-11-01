using System;
using System.Diagnostics;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// Represents a single header included with an HTTP response.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Key,nq} = {Value,nq}")]
    public class HttpHeader
    {
        /// <summary>
        /// The HTTP header key.
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// The HTTP header value.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Gets the HTTP header key.
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Gets the HTTP header value.
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeader"/> class using the specified
        /// key and value.
        /// </summary>
        /// <param name="key">The HTTP header key.</param>
        /// <param name="value">The HTTP header value.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="value"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="key"/> is empty.</exception>
        public HttpHeader(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty");

            _key = key;
            _value = value;
        }
    }
}