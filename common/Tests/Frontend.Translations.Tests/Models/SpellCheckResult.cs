using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Frontend.Translations.Tests.Models
{
    public class SpellCheckResult
    {
        private static Regex wordRegex = new Regex(@"[\p{L}-]+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static Regex regVariables = new Regex("\\{\\{([^\\{].?[^\\}]+)\\}\\}", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex htmlTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline);
        private static List<string> trademarks = new List<string>()
        {
            "onlyoffice.com", "onlyoffice.eu", "Office Open XML", "ONLYOFFICE Desktop Editors",
            "ONLYOFFICE Desktop", "ONLYOFFICE Documents", "Google Drive", "Twitter", "Facebook", "LinkedIn", "Google",
            "Yandex", "Yandex.Disk", "Dropbox","OneDrive","ONLYOFFICE", "DocuSign", "e-mail",
            "SharePoint", "Windows Phone", "Enterprise Edition", "AES-256"
        };
        private static List<string> exclusions = new List<string>()
        {
            "ok","doc","docx","xls","xlsx","ppt","pptx","xml","ooxml","jpg","png","mb","ip",
            "canvas","tag","Disk","Box","Dcs","zip","Android","Authenticator","iOS","Windows",
            "Web","oform","WebDAV","kDrive", "Punycode","logo","sms","html","LDAP",
            "Portal","Favicon","URL","QR", "email", "app", "api"
        };

        private static List<SpellCheckExclude> excludes = File.Exists("../../../spellcheck-excludes.json")
            ? JsonConvert.DeserializeObject<List<SpellCheckExclude>>(File.ReadAllText("../../../spellcheck-excludes.json"))
            : new List<SpellCheckExclude>();

        public SpellCheckResult(string text, string language)
        {
            Text = text;
            Language = language;

            var sanitizedText = htmlTags.Replace(text, string.Empty);

            sanitizedText = regVariables.Replace(sanitizedText, string.Empty);

            foreach (var trademark in trademarks)
                sanitizedText = sanitizedText.Replace(trademark, string.Empty);

            var lngExcludes = excludes
                .Where(ex => ex.Language == language)
                .SelectMany(ex => ex.Excludes)
                .ToList();

            Words = wordRegex.Matches(sanitizedText)
                    .Select(m => m.Value.Trim('-'))
                    .Where(w => !string.IsNullOrEmpty(w)
                            && !exclusions.Exists(t =>
                                t.Equals(w, System.StringComparison.InvariantCultureIgnoreCase))
                            && !lngExcludes.Exists(t =>
                                t.Equals(w, System.StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

            SpellIssues = new List<SpellIssue>();
        }

        public string Text { get; }
        public string Language { get; }
        public List<string> Words { get; }

        public List<SpellIssue> SpellIssues { get; set; }

        public bool HasProblems
        {
            get
            {
                return SpellIssues.Any();
            }
        }
    }
}
