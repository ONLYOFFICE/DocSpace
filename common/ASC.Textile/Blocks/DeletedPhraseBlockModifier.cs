namespace Textile.Blocks;

public class DeletedPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"\-", "del");
    }
}
