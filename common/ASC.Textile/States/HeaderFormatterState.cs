namespace Textile.States;

[FormatterState(SimpleBlockFormatterState.PatternBegin + @"pad[0-9]+" + SimpleBlockFormatterState.PatternEnd)]
public class PaddingFormatterState : SimpleBlockFormatterState
{
    public PaddingFormatterState(TextileFormatter formatter)
        : base(formatter)
    {
    }

    public int HeaderLevel { get; private set; } = 0;


    public override void Enter()
    {
        for (var i = 0; i < HeaderLevel; i++)
        {
            Formatter.Output.Write($"<br {FormattedStylesAndAlignment("br")}/>");
        }
    }

    public override void Exit()
    {
    }

    protected override void OnContextAcquired()
    {
        var m = Regex.Match(Tag, @"^pad(?<lvl>[0-9]+)");
        HeaderLevel = int.Parse(m.Groups["lvl"].Value);
    }

    public override void FormatLine(string input)
    {
        Formatter.Output.Write(input);
    }

    public override bool ShouldExit(string intput)
    {
        return true;
    }

    public override bool ShouldNestState(FormatterState other)
    {
        return false;
    }
}

/// <summary>
/// Formatting state for headers and titles.
/// </summary>
[FormatterState(SimpleBlockFormatterState.PatternBegin + @"h[0-9]+" + SimpleBlockFormatterState.PatternEnd)]
public class HeaderFormatterState : SimpleBlockFormatterState
{
    public int HeaderLevel { get; private set; } = 0;

    public HeaderFormatterState(TextileFormatter f)
        : base(f)
    {
    }

    public override void Enter()
    {
        Formatter.Output.Write("<h" + HeaderLevel + FormattedStylesAndAlignment("h" + HeaderLevel) + ">");
    }

    public override void Exit()
    {
        Formatter.Output.WriteLine("</h" + HeaderLevel + ">");
    }

    protected override void OnContextAcquired()
    {
        var m = Regex.Match(Tag, @"^h(?<lvl>[0-9]+)");
        HeaderLevel = int.Parse(m.Groups["lvl"].Value);
    }

    public override void FormatLine(string input)
    {
        Formatter.Output.Write(input);
    }

    public override bool ShouldExit(string intput)
    {
        return true;
    }

    public override bool ShouldNestState(FormatterState other)
    {
        return false;
    }
}