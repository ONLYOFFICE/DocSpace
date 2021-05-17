using System.Collections.Generic;

namespace Frontend.Translations.Tests
{
    public class TranslationFile
    {
        public TranslationFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public List<TranslationItem> Translations { get; set; }
    }
}
