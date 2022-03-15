namespace ASC.Notify.Patterns;

public abstract class PatternFormatter : IPatternFormatter
{
    private readonly bool _doformat;
    private readonly string _tagSearchPattern;

    protected Regex RegEx { get; private set; }

    protected PatternFormatter() { }

    protected PatternFormatter(string tagSearchRegExp)
        : this(tagSearchRegExp, false)
    {
    }

    protected PatternFormatter(string tagSearchRegExp, bool formatMessage)
    {
        if (string.IsNullOrEmpty(tagSearchRegExp))
        {
            throw new ArgumentException(nameof(tagSearchRegExp));
        }

        _tagSearchPattern = tagSearchRegExp;
        RegEx = new Regex(_tagSearchPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        _doformat = formatMessage;
    }

    public string[] GetTags(IPattern pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var findedTags = new List<string>(SearchTags(pattern.Body));
        Array.ForEach(SearchTags(pattern.Subject), tag => { if (!findedTags.Contains(tag)) findedTags.Add(tag); });
        return findedTags.ToArray();
    }

    public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(message.Pattern);
        ArgumentNullException.ThrowIfNull(tagsValues);

        BeforeFormat(message, tagsValues);

        message.Subject = FormatText(_doformat ? message.Subject : message.Pattern.Subject, tagsValues);
        message.Body = FormatText(_doformat ? message.Body : message.Pattern.Body, tagsValues);

        AfterFormat(message);
    }

    protected abstract string FormatText(string text, ITagValue[] tagsValues);

    protected virtual void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues) { }

    protected virtual void AfterFormat(INoticeMessage message) { }

    protected virtual string[] SearchTags(string text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(_tagSearchPattern))
        {
            return Array.Empty<string>();
        }

        var maches = RegEx.Matches(text);
        var findedTags = new List<string>(maches.Count);
        foreach (Match mach in maches)
        {
            var tag = mach.Groups["tagName"].Value;
            if (!findedTags.Contains(tag))
            {
                findedTags.Add(tag);
            }
        }

        return findedTags.ToArray();
    }
}
