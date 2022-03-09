namespace Textile.Blocks;

public class SubScriptPhraseBlockModifier : PhraseBlockModifier
{
    public override string ModifyLine(string line)
    {
        return PhraseModifierFormat(line, @"~", "sub");
    }
}