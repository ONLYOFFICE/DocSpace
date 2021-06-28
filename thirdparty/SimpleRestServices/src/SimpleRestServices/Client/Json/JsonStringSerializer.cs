using System;
using Newtonsoft.Json;

namespace JSIStudios.SimpleRESTServices.Client.Json
{
    /// <summary>
    /// Provides a default implementation of <see cref="IStringSerializer"/> using JSON for
    /// the underlying serialized notation.
    /// </summary>
    public class JsonStringSerializer : IStringSerializer
    {
        /// <inheritdoc />
        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrEmpty(content))
                return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(content,
                                                        new JsonSerializerSettings
                                                            {
                                                                NullValueHandling = NullValueHandling.Ignore
                                                            });
            }
            catch (JsonReaderException ex)
            {
                throw new StringSerializationException(ex);
            }
            catch (JsonSerializationException ex)
            {
                throw new StringSerializationException(ex);
            }
        }

        /// <inheritdoc />
        public string Serialize<T>(T obj)
        {
            if (Equals(obj, default(T)))
                return null;

            try
            {
                return JsonConvert.SerializeObject(obj,
                                                   new JsonSerializerSettings
                                                       {
                                                           NullValueHandling = NullValueHandling.Ignore
                                                       });
            }
            catch (JsonReaderException ex)
            {
                throw new StringSerializationException(ex);
            }
            catch (JsonSerializationException ex)
            {
                throw new StringSerializationException(ex);
            }
        }
    }

    /// <summary>
    /// The exception thrown when string serialization or deserialization fails.
    /// </summary>
    public class StringSerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSerializationException"/> class
        /// with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.
        /// If the <paramref name="innerException"/> parameter is not a null reference, the current
        /// exception is raised in a <b>catch</b> block that handles the inner exception.</param>
        public StringSerializationException(Exception innerException)
            : base(GetMessageFromInnerException(innerException, "An error occurred during string serialization."), innerException)
        {
        }

        private static string GetMessageFromInnerException(Exception innerException, string defaultMessage)
        {
            if (innerException == null)
                return defaultMessage;

            return innerException.Message ?? defaultMessage;
        }
    }
}
