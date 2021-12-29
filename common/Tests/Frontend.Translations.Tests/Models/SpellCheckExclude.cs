using System.Collections.Generic;

namespace Frontend.Translations.Tests.Models
{
    public class SpellCheckExclude
    {
        public List<string> Excludes { get; set; }
        public string Language { get; set; }
        public SpellCheckExclude(string language)
        {
            Language = language;
            Excludes = new List<string>();
        }
    }
}
