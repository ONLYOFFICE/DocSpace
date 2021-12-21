using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Frontend.Translations.Tests.Models
{
    public class SpellCheckResult
    {
        private static Regex wordRegex = new Regex(@"\p{L}+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static Regex regVariables = new Regex("\\{\\{([^\\{].?[^\\}]+)\\}\\}", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex htmlTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline);
        private static List<string> trademarks = new List<string>()
        {
            "onlyoffice.com", "onlyoffice.eu", "Office Open XML", "ONLYOFFICE Desktop Editors",
            "ONLYOFFICE Desktop", "ONLYOFFICE Documents", "Google Drive", "Twitter", "Facebook", "LinkedIn", "Google",
            "Yandex", "Yandex.Disk", "Dropbox","OneDrive","ONLYOFFICE", "DocuSign", "e-mail",
            "SharePoint", "Windows Phone", "Enterprise Edition"
        };
        private static List<string> exclusions = new List<string>()
        {
            "ok","doc","docx","xls","xlsx","ppt","pptx","xml","ooxml","jpg","png","mb","ip",
            "canvas","tag","Disk","Box","Dcs","zip","Android","Authenticator","iOS","Windows",
            "Web","oform","WebDAV","kDrive","AES", "Punycode","logo","sms","html","LDAP",
            "Portal","Favicon","URL","QR", "email", "app", "api"
        };

        public SpellCheckResult(string text)
        {
            Text = text;

            var sanitizedText = htmlTags.Replace(text, string.Empty);

            sanitizedText = regVariables.Replace(sanitizedText, string.Empty);

            foreach (var trademark in trademarks)
                sanitizedText = sanitizedText.Replace(trademark, string.Empty);

            Words = wordRegex.Matches(sanitizedText)
                    .Select(m => m.Value)
                    .Where(w => !string.IsNullOrEmpty(w) && !exclusions.Exists(t =>
                                t.Equals(w, System.StringComparison.InvariantCultureIgnoreCase)))
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
