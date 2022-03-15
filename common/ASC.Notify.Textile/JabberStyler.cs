namespace ASC.Notify.Textile;

[Scope]
public class JabberStyler : IPatternStyler
{
    private static readonly Regex _velocityArguments
        = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix,
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex _linkReplacer
        = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""",
            RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex _textileReplacer
        = new Regex(@"(h1\.|h2\.|\*|h3\.|\^)",
            RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex _brReplacer
        = new Regex(@"<br\s*\/*>",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _closedTagsReplacer
        = new Regex(@"</(p|div)>",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _tagReplacer
        = new Regex(@"<(.|\n)*?>",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _multiLineBreaksReplacer
        = new Regex(@"(?:\r\n|\r(?!\n)|(?!<\r)\n){3,}",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public void ApplyFormating(NoticeMessage message)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(message.Subject))
        {
            sb.AppendLine(_velocityArguments.Replace(message.Subject, ArgMatchReplace));
            message.Subject = string.Empty;
        }
        if (string.IsNullOrEmpty(message.Body))
        {
            return;
        }

        var lines = message.Body.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

        for (var i = 0; i < lines.Length - 1; i++)
        {
            ref var line = ref lines[i];
            if (string.IsNullOrEmpty(line))
            {
                sb.AppendLine();
                continue;
            }

            line = _velocityArguments.Replace(line, ArgMatchReplace);
            sb.AppendLine(_linkReplacer.Replace(line, EvalLink));
        }

        ref var lastLine = ref lines[^1];
        lastLine = _velocityArguments.Replace(lastLine, ArgMatchReplace);
        sb.Append(_linkReplacer.Replace(lastLine, EvalLink));
        var body = sb.ToString();
        body = _textileReplacer.Replace(HttpUtility.HtmlDecode(body), ""); //Kill textile markup
        body = _brReplacer.Replace(body, Environment.NewLine);
        body = _closedTagsReplacer.Replace(body, Environment.NewLine);
        body = _tagReplacer.Replace(body, "");
        body = _multiLineBreaksReplacer.Replace(body, Environment.NewLine);
        message.Body = body;
    }

    private string EvalLink(Match match)
    {
        if (match.Success)
        {
            if (match.Groups["text"].Success && match.Groups["link"].Success)
            {
                if (match.Groups["text"].Value.Equals(match.Groups["link"].Value, StringComparison.OrdinalIgnoreCase))
                {
                    return " " + match.Groups["text"].Value + " ";
                }

                return match.Groups["text"].Value + $" ( {match.Groups["link"].Value} )";
            }
        }

        return match.Value;
    }

    private string ArgMatchReplace(Match match)
    {
        return match.Result("${arg}");
    }
}
