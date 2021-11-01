using System.Collections.Generic;

namespace Frontend.Translations.Tests
{
    public class JavaScriptFile
    {
        public JavaScriptFile(string path)
        {
            Path = path.Replace("/", "\\");
        }

        public string Path { get; }

        public List<string> TranslationKeys { get; set; }
    }
}
