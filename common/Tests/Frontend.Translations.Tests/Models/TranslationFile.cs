using System.Collections.Generic;

namespace Frontend.Translations.Tests
{
    public class TranslationFile
    {
        public TranslationFile(string path)
        {
            Path = path.Replace("/", "\\");

            Language = Path.Substring(Path.IndexOf("locales\\") + 8, 2);
        }

        public string Path { get; }

        public string Language { get; private set; }

        public List<TranslationItem> Translations { get; set; }
    }
}
