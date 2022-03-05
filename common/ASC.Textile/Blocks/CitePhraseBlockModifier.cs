namespace Textile.Blocks;

public class CitePhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"\?\?", "cite");
    }
}
