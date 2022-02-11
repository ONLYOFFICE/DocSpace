namespace Textile.States
{
    public class TableCellParser
    {
        readonly string m_lineFragment;

        public TableCellParser(string input)
        {
            m_lineFragment = input;
        }

        public string GetLineFragmentFormatting()
        {
            var htmlTag = "td";

            var m = Regex.Match(m_lineFragment,
                                   @"^((?<head>_?)" +
                                   Globals.SpanPattern +
                                   Globals.AlignPattern +
                                   Globals.BlockModifiersPattern +
                                   @"(?<dot>\.)\s?)?" +
                                   @"(?<content>.*)"
                                  );
            if (!m.Success)
                throw new Exception("Couldn't parse table cell.");

            if (m.Groups["head"].Value == "_")
                htmlTag = "th";
            //string opts = BlockAttributesParser.ParseBlockAttributes(m.Groups["span"].Value, "td") +
            //              BlockAttributesParser.ParseBlockAttributes(m.Groups["align"].Value, "td") +
            //              BlockAttributesParser.ParseBlockAttributes(m.Groups["atts"].Value, "td");
            var opts = Blocks.BlockAttributesParser.ParseBlockAttributes(m.Groups["span"].Value + m.Groups["align"].Value + m.Groups["atts"].Value, "td");

            var res = "<" + htmlTag + opts + ">";
            // It may be possible the user actually intended to have a dot at the beginning of
            // this cell's text, without any formatting (header tag or options).
            if (string.IsNullOrEmpty(opts) && htmlTag == "td" && !string.IsNullOrEmpty(m.Groups["dot"].Value))
                res += ".";
            res += m.Groups["content"].Value;
            res += "</" + htmlTag + ">";

            return res;
        }
    }
}
