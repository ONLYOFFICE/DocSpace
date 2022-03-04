namespace Textile.States;

[FormatterState(@"^\s*<code" + Globals.HtmlAttributesPattern + ">")]
public class CodeFormatterState : FormatterState
{
    private bool _shouldExitNextTime = false;
    private bool _shouldFixHtmlEntities = false;

    public CodeFormatterState(TextileFormatter f)
        : base(f)
    {
    }

    public override string Consume(string input, Match m)
    {
        if (!Regex.IsMatch(input, "</code>"))
        {
            this.Formatter.ChangeState(this);
        }
        else
        {
            this.Formatter.ChangeState(new PassthroughFormatterState(this.Formatter));
        }
        return input;
    }

    public override bool ShouldNestState(FormatterState other)
    {
        return true;
    }

    public override void Enter()
    {
        _shouldFixHtmlEntities = false;
    }

    public override void Exit()
    {
    }

    public override void FormatLine(string input)
    {
        if (_shouldFixHtmlEntities)
            input = FixEntities(input);
        Formatter.Output.WriteLine(input);
        _shouldFixHtmlEntities = true;
    }

    public override bool ShouldExit(string input)
    {
        if (_shouldExitNextTime)
            return true;
        _shouldExitNextTime = Regex.IsMatch(input, @"</code>");
        _shouldFixHtmlEntities = !_shouldExitNextTime;
        return false;
    }

    public override bool ShouldFormatBlocks(string input)
    {
        return false;
    }

    public override bool ShouldParseForNewFormatterState(string input)
    {
        return false;
    }

    private string FixEntities(string text)
    {
        // de-entify any remaining angle brackets or ampersands
        text = text.Replace("&", "&amp;");
        text = text.Replace(">", "&gt;");
        text = text.Replace("<", "&lt;");
        //Regex.Replace(text, @"\b&([#a-z0-9]+;)", "x%x%");
        return text;
    }
}