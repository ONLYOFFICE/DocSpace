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

namespace Textile;

/// <summary>
/// A utility class for global things used by the TextileFormatter.
/// </summary>
static class Globals
{
    #region Global Regex Patterns

    public const string HorizontalAlignPattern = @"(?:[()]*(\<(?!>)|(?<!<)\>|\<\>|=)[()]*)";
    public const string VerticalAlignPattern = @"[\-^~]";
    public const string CssClassPattern = @"(?:\([^)]+\))";
    public const string LanguagePattern = @"(?:\[[^]]+\])";
    public const string CssStylePattern = @"(?:\{[^}]+\})";
    public const string ColumnSpanPattern = @"(?:\\\d+)";
    public const string RowSpanPattern = @"(?:/\d+)";

    public const string AlignPattern = "(?<align>" + HorizontalAlignPattern + "?" + VerticalAlignPattern + "?|" + VerticalAlignPattern + "?" + HorizontalAlignPattern + "?)";
    public const string SpanPattern = @"(?<span>" + ColumnSpanPattern + "?" + RowSpanPattern + "?|" + RowSpanPattern + "?" + ColumnSpanPattern + "?)";
    public const string BlockModifiersPattern = @"(?<atts>" + CssClassPattern + "?" + CssStylePattern + "?" + LanguagePattern + "?|" +
                                                    CssStylePattern + "?" + LanguagePattern + "?" + CssClassPattern + "?|" +
                                                    LanguagePattern + "?" + CssStylePattern + "?" + CssClassPattern + "?)";

    public const string PunctuationPattern = @"[\!""#\$%&'()\*\+,\-\./:;<=>\?@\[\\\]\^_`{}~]";

    public const string HtmlAttributesPattern = @"(\s+\w+=((""[^""]+"")|('[^']+')))*";

    #endregion

    /// <summary>
    /// Image alignment tags, mapped to their HTML meanings.
    /// </summary>
    public static Dictionary<string, string> ImageAlign { get; set; }
    /// <summary>
    /// Horizontal text alignment tags, mapped to their HTML meanings.
    /// </summary>
    public static Dictionary<string, string> HorizontalAlign { get; set; }
    /// <summary>
    /// Vertical text alignment tags, mapped to their HTML meanings.
    /// </summary>
    public static Dictionary<string, string> VerticalAlign { get; set; }

    static Globals()
    {
        ImageAlign = new Dictionary<string, string>
        {
            ["<"] = "left",
            ["="] = "center",
            [">"] = "right"
        };

        HorizontalAlign = new Dictionary<string, string>
        {
            ["<"] = "left",
            ["="] = "center",
            [">"] = "right",
            ["<>"] = "justify"
        };

        VerticalAlign = new Dictionary<string, string>
        {
            ["^"] = "top",
            ["-"] = "middle",
            ["~"] = "bottom"
        };
    }

    public static string EncodeHTMLLink(string url)
    {
        url = url.Replace("&amp;", "&#38;");
        url = Regex.Replace(url, "&(?=[^#])", "&#38;");
        return url;
    }
}
