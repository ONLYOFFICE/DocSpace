namespace ASC.Notify.Patterns;

public sealed class NVelocityPatternFormatter : PatternFormatter
{
    public const string DefaultPattern = @"(^|[^\\])\$[\{]{0,1}(?<tagName>[a-zA-Z0-9_]+)";
    public const string NoStylePreffix = "==";
    public const string NoStyleSuffix = "==";

    private VelocityContext _nvelocityContext;

    public NVelocityPatternFormatter()
        : base(DefaultPattern)
    {
    }

    protected override void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues)
    {
        _nvelocityContext = new VelocityContext();
        _nvelocityContext.AttachEventCartridge(new EventCartridge());
        _nvelocityContext.EventCartridge.ReferenceInsertion += EventCartridgeReferenceInsertion;
        foreach (var tagValue in tagsValues)
        {
            _nvelocityContext.Put(tagValue.Tag, tagValue.Value);
        }

        base.BeforeFormat(message, tagsValues);
    }

    protected override string FormatText(string text, ITagValue[] tagsValues)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return VelocityFormatter.FormatText(text, _nvelocityContext);
    }

    protected override void AfterFormat(INoticeMessage message)
    {
        _nvelocityContext = null;
        base.AfterFormat(message);
    }

    private void EventCartridgeReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
    {
        if (!(e.OriginalValue is string originalString))
        {
            return;
        }

        var lines = originalString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
        {
            return;
        }

        e.NewValue = string.Empty;
        for (var i = 0; i < lines.Length - 1; i++)
        {
            e.NewValue += $"{NoStylePreffix}{lines[i]}{NoStyleSuffix}\n";
        }

        e.NewValue += $"{NoStylePreffix}{lines[^1]}{NoStyleSuffix}";
    }
}
