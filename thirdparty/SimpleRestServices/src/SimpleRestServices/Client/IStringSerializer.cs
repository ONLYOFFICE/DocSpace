using JSIStudios.SimpleRESTServices.Client.Json;

namespace JSIStudios.SimpleRESTServices.Client
{
    /// <summary>
    /// This interface provides simple support for serializing and deserializing generic objects to and from strings.
    /// </summary>
    public interface IStringSerializer
    {
        /// <summary>
        /// Deserializes an object from a string.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="content">The serialized representation of the object.</param>
        /// <returns>An instance of <typeparamref name="T"/> which <paramref name="content"/> describes.</returns>
        /// <exception cref="StringSerializationException">If <paramref name="content"/> could not be deserialized to an object of type <typeparamref name="T"/>.</exception>
        T Deserialize<T>(string content);

        /// <summary>
        /// Serializes an object to a string.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is suitable for deserialization using <see cref="Deserialize{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>A serialized string representation of <paramref name="obj"/>.</returns>
        /// <exception cref="StringSerializationException">If <paramref name="obj"/> could not be serialized to a string.</exception>
        string Serialize<T>(T obj);
    }
}
