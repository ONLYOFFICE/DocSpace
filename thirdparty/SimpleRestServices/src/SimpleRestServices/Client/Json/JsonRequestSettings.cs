namespace JSIStudios.SimpleRESTServices.Client.Json
{
    /// <summary>
    /// Extends <see cref="RequestSettings"/> by setting the default <see cref="RequestSettings.ContentType"/>
    /// and <see cref="RequestSettings.Accept"/> values to <c>application/json</c>.
    /// </summary>
    public class JsonRequestSettings : RequestSettings
    {
        /// <summary>
        /// The content type (MIME type) for a JSON request or response.
        /// </summary>
        /// <remarks>
        /// This value is equal to <c>application/json</c>.
        /// </remarks>
        public static readonly string JsonContentType = "application/json";

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRequestSettings"/> class with the default value
        /// <c>application/json</c> for <see cref="RequestSettings.ContentType"/> and <see cref="RequestSettings.Accept"/>.
        /// </summary>
        public JsonRequestSettings()
        {
            ContentType = JsonContentType;
            Accept = JsonContentType;
        }
    }
}
