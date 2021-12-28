using UtfUnknown;

namespace Frontend.Translations.Tests
{
    public class JsonEncodingError
    {
        public DetectionDetail DetectionDetail { get; }

        public string Path { get; }

        public JsonEncodingError(string path, DetectionDetail detail)
        {
            Path = path;
            DetectionDetail = detail;
        }
    }
}
