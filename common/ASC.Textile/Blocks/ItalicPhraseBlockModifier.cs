namespace Textile.Blocks;

public class ItalicPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"__", "i");
    }
}
