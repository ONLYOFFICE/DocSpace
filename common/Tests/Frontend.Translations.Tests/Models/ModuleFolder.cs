using System.Collections.Generic;

namespace Frontend.Translations.Tests
{
    public class ModuleFolder
    {
        public string Path { get; set; }
        public List<string> AppliedJsTranslationKeys { get; set; }
        public List<LanguageItem> AvailableLanguages { get; set; }
    }
}
