// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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
        {
            atts += " title=\"" + m.Groups["title"].Value + "\"";
        }

        var linkText = m.Groups["text"].Value.Trim(' ');

        var str = m.Groups["pre"].Value + "<a ";
        if (!string.IsNullOrEmpty(_rel))
        {
            str += "ref=\"" + _rel + "\" ";
        }

        str += "href=\"" +
                m.Groups["url"].Value + m.Groups["slash"].Value + "\"" +
                atts +
                ">" + linkText + "</a>" + m.Groups["post"].Value;
        return str;
    }
}
