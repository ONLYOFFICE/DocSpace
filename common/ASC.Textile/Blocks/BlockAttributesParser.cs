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

public static class BlockAttributesParser
{
    public static StyleReader Styler { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ParseBlockAttributes(string input)
    {
        return ParseBlockAttributes(input, "");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static string ParseBlockAttributes(string input, string element)
    {
        var style = string.Empty;
        var cssClass = string.Empty;
        var lang = string.Empty;
        var colspan = string.Empty;
        var rowspan = string.Empty;
        var id = string.Empty;

        if (Styler != null)
        {
            style = GetStyle(element, style);
        }

        if (input.Length == 0)
        {
            return style.Length > 0 ? " style=\"" + style + "\"" : "";
        }

        Match m;
        var matched = input;
        if (element == "td")
        {
            // column span
            m = Regex.Match(matched, @"\\(\d+)");
            if (m.Success)
            {
                colspan = m.Groups[1].Value;
            }
            // row span
            m = Regex.Match(matched, @"/(\d+)");
            if (m.Success)
            {
                rowspan = m.Groups[1].Value;
            }
            // vertical align
            m = Regex.Match(matched, @"(" + Globals.VerticalAlignPattern + @")");
            if (m.Success)
            {
                style += "vertical-align:" + Globals.VerticalAlign[m.Captures[0].Value] + ";";
            }
        }

        // First, match custom styles
        m = Regex.Match(matched, @"\{([^}]*)\}");
        if (m.Success)
        {
            style += m.Groups[1].Value + ";";
            matched = matched.Replace(m.ToString(), "");
        }

        // Then match the language
        m = Regex.Match(matched, @"\[([^()]+)\]");
        if (m.Success)
        {
            lang = m.Groups[1].Value;
            matched = matched.Replace(m.ToString(), "");
        }

        // Match classes and IDs after that
        m = Regex.Match(matched, @"\(([^()]+)\)");
        if (m.Success)
        {
            cssClass = m.Groups[1].Value;
            matched = matched.Replace(m.ToString(), "");

            // Separate the public class and the ID
            m = Regex.Match(cssClass, @"^(.*)#(.*)$");
            if (m.Success)
            {
                cssClass = m.Groups[1].Value;
                id = m.Groups[2].Value;
            }
            if (Styler != null && !string.IsNullOrEmpty(cssClass))
            {
                style = GetStyle("." + cssClass, style);
            }

        }

        // Get the padding on the left
        m = Regex.Match(matched, @"([(]+)");
        if (m.Success)
        {
            style += "padding-left:" + m.Groups[1].Length + "em;";
            matched = matched.Replace(m.ToString(), "");
        }

        // Get the padding on the right
        m = Regex.Match(matched, @"([)]+)");
        if (m.Success)
        {
            style += "padding-right:" + m.Groups[1].Length + "em;";
            matched = matched.Replace(m.ToString(), "");
        }

        // Get the text alignment
        m = Regex.Match(matched, "(" + Globals.HorizontalAlignPattern + ")");
        if (m.Success)
        {
            style += "text-align:" + Globals.HorizontalAlign[m.Groups[1].Value] + ";";
        }

        return
                (style.Length > 0 ? " style=\"" + style + "\"" : "") +
                (cssClass.Length > 0 ? " class=\"" + cssClass + "\"" : "") +
                (lang.Length > 0 ? " lang=\"" + lang + "\"" : "") +
                (id.Length > 0 ? " id=\"" + id + "\"" : "") +
                (colspan.Length > 0 ? " colspan=\"" + colspan + "\"" : "") +
                (rowspan.Length > 0 ? " rowspan=\"" + rowspan + "\"" : "")
                ;
    }

    private static string GetStyle(string element, string style)
    {
        var styled = Styler.GetStyle(element);
        if (!string.IsNullOrEmpty(styled))
        {
            style += styled;
        }
        return style;
    }
}
