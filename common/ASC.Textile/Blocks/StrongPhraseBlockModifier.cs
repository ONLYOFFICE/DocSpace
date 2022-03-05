namespace Textile.Blocks;

public class StrongPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"\*", "strong");
    }
}