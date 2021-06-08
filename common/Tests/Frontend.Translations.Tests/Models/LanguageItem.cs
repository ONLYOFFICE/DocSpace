using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Translations.Tests
{
    public class LanguageItem
    {
        public string Language { get; set; }

        public string Path { get; set; }

        public List<TranslationItem> Translations { get; set; }
    }
}
