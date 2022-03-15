namespace ASC.Notify.Patterns;

public sealed class ReplacePatternFormatter : PatternFormatter
{
    public const string DefaultPattern = @"[[]%(?<tagName>[a-zA-Z0-9_\-.]+)%[]]";

    public ReplacePatternFormatter()
        : base(DefaultPattern)
    {
    }

    internal ReplacePatternFormatter(string tagPattern, bool formatMessage)
        : base(tagPattern, formatMessage)
    {
    }

    protected override string FormatText(string text, ITagValue[] tagsValues)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var formattedText = RegEx.Replace(text,
            match =>
            {
                var value = Array.Find(tagsValues, v => v.Tag == match.Groups["tagName"].Value);

                return value != null && value.Value != null ? Convert.ToString(value.Value) : match.Value;
            });

        return formattedText;
    }
}
