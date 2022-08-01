// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Frontend.Translations.Tests.Models;

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
