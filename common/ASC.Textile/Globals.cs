#region License Statement
// Copyright (c) L.A.B.Soft.  All rights reserved.
//
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.
#endregion

#region Using Statements
using System.Collections.Generic;
#endregion


namespace Textile
{
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
        public static Dictionary<string, string> VerticalAlign { get; set;}

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
            url = System.Text.RegularExpressions.Regex.Replace(url, "&(?=[^#])", "&#38;");
            return url;
        }
    }
}
