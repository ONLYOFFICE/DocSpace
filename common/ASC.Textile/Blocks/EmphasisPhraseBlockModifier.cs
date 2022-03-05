namespace Textile.Blocks;

public class EmphasisPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"_", "em");
    }
}
