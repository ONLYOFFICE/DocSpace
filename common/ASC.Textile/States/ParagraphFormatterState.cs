namespace Textile.States;

/// <summary>
/// Formatting state for a standard text (i.e. just paragraphs).
/// </summary>
[FormatterState(SimpleBlockFormatterState.PatternBegin + @"p" + SimpleBlockFormatterState.PatternEnd)]
public class ParagraphFormatterState : SimpleBlockFormatterState
{
    public ParagraphFormatterState(TextileFormatter f)
        : base(f)
    {
    }

    public override void Enter()
    {
        Formatter.Output.Write("<p" + FormattedStylesAndAlignment("p") + ">");
    }

    public override void Exit()
    {
        Formatter.Output.WriteLine("</p>");
    }

    public override void FormatLine(string input)
    {
        Formatter.Output.Write(input);
    }

    public override bool ShouldExit(string input)
    {
        if (Regex.IsMatch(input, @"^\s*$"))
            return true;
        Formatter.Output.WriteLine("<br />");
        return false;
    }

    public override bool ShouldNestState(FormatterState other)
    {
        return false;
    }
}
