namespace Textile.Blocks;

public class HyperLinkBlockModifier : BlockModifier
{
    private readonly string _rel = string.Empty;

    public override string ModifyLine(string line)
    {
        line = Regex.Replace(line,
                                @"(?<pre>[\s[{(]|" + Globals.PunctuationPattern + @")?" +       // $pre
                                "\"" +									// start
                                Globals.BlockModifiersPattern +			// attributes
                                "(?<text>[\\w\\W]+?)" +					// text
                                @"\s?" +
                                @"(?:\((?<title>[^)]+)\)(?=""))?" +		// title
                                "\":" +
                                @"""(?<url>\S+[^""]+)""" +						// url
                                @"(?<slash>\/)?" +						// slash
                                @"(?<post>[^\w\/;]*)" +					// post
                                @"(?=\s|$)",
                                new MatchEvaluator(HyperLinksFormatMatchEvaluator));
        return line;
    }

    private string HyperLinksFormatMatchEvaluator(Match m)
    {
        //TODO: check the URL
        var atts = BlockAttributesParser.ParseBlockAttributes(m.Groups["atts"].Value, "a");
        if (m.Groups["title"].Length > 0)
            atts += " title=\"" + m.Groups["title"].Value + "\"";
        var linkText = m.Groups["text"].Value.Trim(' ');

        var str = m.Groups["pre"].Value + "<a ";
        if (!string.IsNullOrEmpty(_rel))
            str += "ref=\"" + _rel + "\" ";
        str += "href=\"" +
                m.Groups["url"].Value + m.Groups["slash"].Value + "\"" +
                atts +
                ">" + linkText + "</a>" + m.Groups["post"].Value;
        return str;
    }
}
