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

namespace ASC.Notify.Textile;

public class MarkDownStyler : IPatternStyler
{
    static readonly Regex _velocityArguments = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
    static readonly Regex _linkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);
    static readonly Regex _divPTagReplacer = new Regex(@"(<(p|div).*?>)|(<\/(p|div)>)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Singleline);
    static readonly Regex _tagReplacer = new Regex(@"<(.|\n)*?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Singleline);
    static readonly Regex _multiLineBreaksReplacer = new Regex(@"(?:\r\n|\r(?!\n)|(?!<\r)\n){3,}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _symbolReplacer = new Regex(@"\[(.*?)]\(([^()]*)\)|[]\\[(){}*_|#+=.!~>`-]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _linkSymbolReplacer = new Regex(@"[]\\[(){}*_|#+=.!~>`-]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _boldReplacer = new Regex(@"<(strong|\/strong)\\>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _strikeThroughReplacer = new Regex(@"<(s|\/s)\\>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _underLineReplacer = new Regex(@"<(u|\/u)\\>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _italicReplacer = new Regex(@"<(em|\/em)\\>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static readonly Regex _hTMLLinkReplacer = new Regex(@"<a.*?href=""(.*?)"".*?>(.*?)<\/a>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public void ApplyFormating(NoticeMessage message)
    {
        var body = string.Empty;
        if (!string.IsNullOrEmpty(message.Subject))
        {
            body += _velocityArguments.Replace(message.Subject, ArgMatchReplace) + Environment.NewLine;
            message.Subject = string.Empty;
        }
        if (string.IsNullOrEmpty(message.Body)) return;
        var lines = message.Body.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
        for (var i = 0; i < lines.Length - 1; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) { body += Environment.NewLine; continue; }
            lines[i] = _velocityArguments.Replace(lines[i], ArgMatchReplace);
            body += _linkReplacer.Replace(lines[i], EvalLink) + Environment.NewLine;
        }
        lines[lines.Length - 1] = _velocityArguments.Replace(lines[lines.Length - 1], ArgMatchReplace);
        body += _linkReplacer.Replace(lines[lines.Length - 1], EvalLink);
        body = _divPTagReplacer.Replace(body, "");
        body = _hTMLLinkReplacer.Replace(body, @"[$2]($1)");
        body = HttpUtility.HtmlDecode(body);
        body = _symbolReplacer.Replace(body, m => m.Groups[1].Success ? $@"[{_linkSymbolReplacer.Replace(m.Groups[1].Value, @"\$&")}]({m.Groups[2].Value})" : $@"\{m.Value}");
        body = _boldReplacer.Replace(body, "*");
        body = _strikeThroughReplacer.Replace(body, "~");
        body = _underLineReplacer.Replace(body, "__");
        body = _italicReplacer.Replace(body, "_");
        body = _tagReplacer.Replace(body, "");
        body = _multiLineBreaksReplacer.Replace(body, Environment.NewLine);
        message.Body = body;
    }

    private static string EvalLink(Match match)
    {
        if (match.Success)
        {
            if (match.Groups["text"].Success && match.Groups["link"].Success)
            {
                if (match.Groups["text"].Value.Equals(match.Groups["link"].Value, StringComparison.OrdinalIgnoreCase))
                {
                    return " " + match.Groups["text"].Value + " ";
                }
                return match.Groups["text"].Value + string.Format(" ( {0} )", match.Groups["link"].Value);
            }
        }
        return match.Value;
    }

    private static string ArgMatchReplace(Match match)
    {
        return match.Result("${arg}");
    }
}
