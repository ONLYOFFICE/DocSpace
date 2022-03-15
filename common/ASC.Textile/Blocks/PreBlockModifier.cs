namespace Textile.Blocks;

public class PreBlockModifier : BlockModifier
{
    public override string ModifyLine(string line)
    {
        // Encode the contents of the "<pre>" tags so that we don't
        // generate formatting out of it.
        line = NoTextileEncoder.EncodeNoTextileZones(line,
                                @"(?<=(^|\s)<pre(" + Globals.HtmlAttributesPattern + @")>)",
                                @"(?=</pre>)");
        return line;
    }

    public override string Conclude(string line)
    {
        // Recode everything.
        line = NoTextileEncoder.DecodeNoTextileZones(line,
                                @"(?<=(^|\s)<pre(" + Globals.HtmlAttributesPattern + @")>)",
                                @"(?=</pre>)",
                                new string[] { "<", ">" });
        return line;
    }
}
