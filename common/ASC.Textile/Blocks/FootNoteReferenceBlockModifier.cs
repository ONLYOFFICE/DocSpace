namespace Textile.Blocks;

public class FootNoteReferenceBlockModifier : BlockModifier
{
    public override string ModifyLine(string line)
    {
        return Regex.Replace(line, @"\b\[([0-9]+)\](\W)", "<sup><a href=\"#fn$1\">$1</a></sup>$2");
    }
}
