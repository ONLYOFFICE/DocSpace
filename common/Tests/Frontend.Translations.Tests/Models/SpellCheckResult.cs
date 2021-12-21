using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Frontend.Translations.Tests.Models
{
    public class SpellCheckResult
    {
        private Regex wordRegex = new Regex(@"\p{L}+", RegexOptions.Multiline | RegexOptions.Compiled);

        public SpellCheckResult(string text)
        {
            Text = text;

            Words = wordRegex.Matches(text).Select(m => m.Value)
                    .Where(w => !string.IsNullOrEmpty(w))
                    .Where(w => !w.StartsWith("{{"))
                    .ToList();

            SpellIssues = new List<SpellIssue>();
        }

        public string Text { get; }
        public List<string> Words { get; }

        public List<SpellIssue> SpellIssues { get; set; }

        public bool HasProblems
        {
            get
            {
                return SpellIssues.Any(issue => issue.Suggestions.Any());
            }
        }
    }
}
