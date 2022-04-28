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

public class GlyphBlockModifier : BlockModifier
{
    public override string ModifyLine(string line)
    {
        line = Regex.Replace(line, "\"\\z", "\" ");

        // fix: hackish
        string[,] glyphs = {
                            { @"([^\s[{(>_*])?\'(?(1)|(\s|s\b|" + Globals.PunctuationPattern + @"))", "$1&#8217;$2" },    //  single closing
                            { @"\'", "&#8216;" },                                                   //  single opening
                            { @"([^\s[{(>_*])?""(?(1)|(\s|" + Globals.PunctuationPattern + @"))", "$1&#8221;$2" },        //  double closing
                            { @"""", "&#8220;" },                                                   //  double opening
                            { @"\b( )?\.{3}", "$1&#8230;" },                                        //  ellipsis
                            { @"\b([A-Z][A-Z0-9]{2,})\b(?:[(]([^)]*)[)])", "<acronym title=\"$2\">$1</acronym>" },        //  3+ uppercase acronym
                            { @"(\s)?--(\s)?", "$1&#8212;$2" },                                     //  em dash
                            { @"\s-\s", " &#8211; " },                                              //  en dash
                            { @"(\d+)( )?x( )?(\d+)", "$1$2&#215;$3$4" },                           //  dimension sign
                            { @"\b ?[([](TM|tm)[])]", "&#8482;" },                                  //  trademark
                            { @"\b ?[([](R|r)[])]", "&#174;" },                                     //  registered
                            { @"\b ?[([](C|c)[])]", "&#169;" }                                      //  copyright
                            };

        var sb = new StringBuilder();

        if (!Regex.IsMatch(line, "<.*>"))
        {
            // If no HTML, do a simple search & replace.
            for (var i = 0; i < glyphs.GetLength(0); ++i)
            {
                line = Regex.Replace(line, glyphs[i, 0], glyphs[i, 1]);
            }
            sb.Append(line);
        }
        else
        {
            var splits = Regex.Split(line, "(<.*?>)");
            var offtags = "code|pre|notextile";
            var codepre = false;

            foreach (var split in splits)
            {
                var modifiedSplit = split;
                if (modifiedSplit.Length == 0)
                {
                    continue;
                }

                if (Regex.IsMatch(modifiedSplit, @"<(" + offtags + ")>"))
                {
                    codepre = true;
                }

                if (Regex.IsMatch(modifiedSplit, @"<\/(" + offtags + ")>"))
                {
                    codepre = false;
                }

                if (!Regex.IsMatch(modifiedSplit, "<.*>") && !codepre)
                {
                    for (var i = 0; i < glyphs.GetLength(0); ++i)
                    {
                        modifiedSplit = Regex.Replace(modifiedSplit, glyphs[i, 0], glyphs[i, 1]);
                    }
                }

                // do htmlspecial if between <code>
                if (codepre)
                {
                    //TODO: htmlspecialchars(line)
                    //line = Regex.Replace(line, @"&lt;(\/?" + offtags + ")&gt;", "<$1>");
                    //line = line.Replace("&amp;#", "&#");
                }

                sb.Append(modifiedSplit);
            }
        }

        return sb.ToString();
    }
}
