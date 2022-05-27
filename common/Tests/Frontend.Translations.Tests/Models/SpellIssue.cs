using System.Collections.Generic;

namespace Frontend.Translations.Tests.Models
{
    public class SpellIssue
    {
        public SpellIssue(string word, IEnumerable<string> suggestions)
        {
            Word = word;
            Suggestions = suggestions;
        }

        public string Word { get; }
        public IEnumerable<string> Suggestions { get; }
    }
}
