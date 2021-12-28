using System.Collections.Generic;
using System.IO;

namespace Frontend.Translations.Tests
{
    public class TranslationFile
    {
        public TranslationFile(string path, List<TranslationItem> translations, string md5hash = null)
        {
            FilePath = path.Replace("/", "\\");

            FileName = Path.GetFileName(FilePath);

            Language = Directory.GetParent(FilePath).Name; //FilePath.Substring(FilePath.IndexOf("locales\\") + 8, 2);

            Translations = translations;

            Md5Hash = md5hash;
        }

        public string FilePath { get; }

        public string FileName { get; }

        public string Language { get; }

        public List<TranslationItem> Translations { get; }

        public string Md5Hash { get; }
    }
}
