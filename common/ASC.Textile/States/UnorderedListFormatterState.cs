namespace Textile.States;

/// <summary>
/// Formatting state for a bulleted list.
/// </summary>
[FormatterState(ListFormatterState.PatternBegin + @"\*+" + ListFormatterState.PatternEnd)]
public class UnorderedListFormatterState : ListFormatterState
{
    public UnorderedListFormatterState(TextileFormatter formatter)
        : base(formatter)
    {
    }

    protected override void WriteIndent()
    {
        Formatter.Output.WriteLine("<ul" + FormattedStylesAndAlignment("ul") + ">");
    }

    protected override void WriteOutdent()
    {
        Formatter.Output.WriteLine("</ul>");
    }

    protected override bool IsMatchForMe(string input, int minNestingDepth, int maxNestingDepth)
    {
        return Regex.IsMatch(input, @"^\s*[\*]{" + minNestingDepth + @"," + maxNestingDepth + @"}" + Globals.BlockModifiersPattern + @"\s");
    }

    protected override bool IsMatchForOthers(string input, int minNestingDepth, int maxNestingDepth)
    {
        return Regex.IsMatch(input, @"^\s*[#]{" + minNestingDepth + @"," + maxNestingDepth + @"}" + Globals.BlockModifiersPattern + @"\s");
    }
}