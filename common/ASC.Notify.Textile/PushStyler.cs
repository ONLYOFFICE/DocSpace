namespace ASC.Notify.Textile;

[Scope]
public class PushStyler : IPatternStyler
{
    private static readonly Regex _velocityArgumentsRegex
        = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?'arg'.*?)" + NVelocityPatternFormatter.NoStyleSuffix,
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    public void ApplyFormating(NoticeMessage message)
    {
        if (!string.IsNullOrEmpty(message.Subject))
        {
            message.Subject = _velocityArgumentsRegex.Replace(message.Subject, m => m.Groups["arg"].Value);
            message.Subject = message.Subject.Replace(Environment.NewLine, " ").Trim();
        }
        if (!string.IsNullOrEmpty(message.Body))
        {
            message.Body = _velocityArgumentsRegex.Replace(message.Body, m => m.Groups["arg"].Value);
            message.Body = message.Body.Replace(Environment.NewLine, " ").Trim();
        }
    }
}
