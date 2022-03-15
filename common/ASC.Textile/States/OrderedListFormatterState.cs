namespace Textile.States;

/// <summary>
/// Formatting state for a numbered list.
/// </summary>
[FormatterState(ListFormatterState.PatternBegin + @"#+" + ListFormatterState.PatternEnd)]
public class OrderedListFormatterState : ListFormatterState
{
    public OrderedListFormatterState(TextileFormatter formatter)
        : base(formatter)
    {
    }

    protected override void WriteIndent()
    {
        Formatter.Output.WriteLine("<ol" + FormattedStylesAndAlignment("ol") + ">");
    }

    protected override void WriteOutdent()
    {
        Formatter.Output.WriteLine("</ol>");
    }

    protected override bool IsMatchForMe(string input, int minNestingDepth, int maxNestingDepth)
    {
        return Regex.IsMatch(input, @"^\s*([\*#]{" + (minNestingDepth - 1) + @"," + (maxNestingDepth - 1) + @"})#" + Globals.BlockModifiersPattern + @"\s");
    }

    protected override bool IsMatchForOthers(string input, int minNestingDepth, int maxNestingDepth)
    {
        return Regex.IsMatch(input, @"^\s*([\*#]{" + (minNestingDepth - 1) + @"," + (maxNestingDepth - 1) + @"})\*" + Globals.BlockModifiersPattern + @"\s");
    }
}
