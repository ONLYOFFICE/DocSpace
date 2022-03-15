namespace Textile.Blocks;

public class NoTextileBlockModifier : BlockModifier
{
    public override string ModifyLine(string line)
    {
        line = NoTextileEncoder.EncodeNoTextileZones(line, @"(?<=^|\s)<notextile>", @"</notextile>(?=(\s|$)?)");
        line = NoTextileEncoder.EncodeNoTextileZones(line, @"==", @"==");
        return line;
    }

    public override string Conclude(string line)
    {
        line = NoTextileEncoder.DecodeNoTextileZones(line, @"(?<=^|\s)<notextile>", @"</notextile>(?=(\s|$)?)");
        line = NoTextileEncoder.DecodeNoTextileZones(line, @"==", @"==");
        return line;
    }
}
