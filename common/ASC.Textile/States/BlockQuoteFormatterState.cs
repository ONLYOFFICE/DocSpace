namespace Textile.States;

[FormatterState(SimpleBlockFormatterState.PatternBegin + @"bq" + SimpleBlockFormatterState.PatternEnd)]
public class BlockQuoteFormatterState : SimpleBlockFormatterState
{
    public BlockQuoteFormatterState(TextileFormatter f)
        : base(f)
    {
    }

    public override void Enter()
    {
        Formatter.Output.Write("<blockquote" + FormattedStylesAndAlignment("blockquote") + "><p>");
    }

    public override void Exit()
    {
        Formatter.Output.WriteLine("</p></blockquote>");
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

    public override Type FallbackFormattingState
    {
        get { return null; }
    }
}
