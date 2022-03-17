namespace Textile.States;

[FormatterState(@"^\s*<pre" + Globals.HtmlAttributesPattern + ">")]
public class PreFormatterState : FormatterState
{
    private bool _shouldExitNextTime;
    private int _fakeNestingDepth;

    public PreFormatterState(TextileFormatter f)
        : base(f)
    {
    }

    public override string Consume(string input, Match m)
    {
        if (!Regex.IsMatch(input, "</pre>"))
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
        return false;
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void FormatLine(string input)
    {
        if (Regex.IsMatch(input, "<pre>"))
        {
            _fakeNestingDepth++;
        }

        Formatter.Output.WriteLine(input);
    }

    public override bool ShouldExit(string input)
    {
        if (_shouldExitNextTime)
        {
            return true;
        }

        if (Regex.IsMatch(input, @"</pre>"))
        {
            _fakeNestingDepth--;
        }

        if (_fakeNestingDepth <= 0)
        {
            _shouldExitNextTime = true;
        }

        return false;
    }

    public override bool ShouldFormatBlocks(string input)
    {
        return false;
    }

    public override bool ShouldParseForNewFormatterState(string input)
    {
        // Only allow a child formatting state for <code> tag.
        return Regex.IsMatch(input, @"^\s*<code");
    }

    public override Type FallbackFormattingState
    {
        get { return null; }
    }
}