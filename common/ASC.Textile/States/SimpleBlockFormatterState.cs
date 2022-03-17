namespace Textile.States;

public abstract class SimpleBlockFormatterState : FormatterState
{
    internal const string _patternBegin = @"^\s*(?<tag>";
    internal const string _patternEnd = @")" + Globals.AlignPattern + Globals.BlockModifiersPattern + @"\.(?:\s+)?(?<content>.*)$";

    public string Tag { get; private set; }

    public string AlignInfo { get; private set; }

    public string AttInfo { get; private set; }

    protected SimpleBlockFormatterState(TextileFormatter formatter)
        : base(formatter)
    {
    }

    public override string Consume(string input, Match m)
    {
        Tag = m.Groups["tag"].Value;
        AlignInfo = m.Groups["align"].Value;
        AttInfo = m.Groups["atts"].Value;
        input = m.Groups["content"].Value;

        OnContextAcquired();

        this.Formatter.ChangeState(this);

        return input;
    }

    public override bool ShouldNestState(FormatterState other)
    {
        var blockFormatterState = (SimpleBlockFormatterState)other;
        return blockFormatterState.Tag != Tag ||
                blockFormatterState.AlignInfo != AlignInfo ||
                blockFormatterState.AttInfo != AttInfo;
    }

    protected virtual void OnContextAcquired()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string FormattedAlignment()
    {
        return Blocks.BlockAttributesParser.ParseBlockAttributes(AlignInfo);
    }

    protected string FormattedStyles(string element)
    {
        return Blocks.BlockAttributesParser.ParseBlockAttributes(AttInfo, element);
    }

    protected string FormattedStylesAndAlignment(string element)
    {
        return Blocks.BlockAttributesParser.ParseBlockAttributes(AlignInfo + AttInfo, element);
    }
}