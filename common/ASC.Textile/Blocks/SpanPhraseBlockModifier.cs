namespace Textile.Blocks;

public class SpanPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"%", "span");
    }
}
