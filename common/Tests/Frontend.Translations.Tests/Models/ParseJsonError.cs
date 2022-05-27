using System;

namespace Frontend.Translations.Tests
{
    public class ParseJsonError
    {
        public Exception Exception { get; }

        public string Path { get; }

        public ParseJsonError(string path, Exception ex)
        {
            Path = path;
            Exception = ex;
        }
    }
}
