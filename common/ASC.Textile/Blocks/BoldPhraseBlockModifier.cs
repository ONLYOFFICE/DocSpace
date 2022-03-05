namespace Textile.Blocks;

public class BoldPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"\*\*", "b");
    }
}
