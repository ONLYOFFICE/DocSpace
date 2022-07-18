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

public static class NoTextileEncoder
{
    private static readonly string[,] _textileModifiers = {
                            { "\"", "&#34;" },
                            { "%", "&#37;" },
                            { "*", "&#42;" },
                            { "+", "&#43;" },
                            { "-", "&#45;" },
                            { "<", "&lt;" },   // or "&#60;"
            				{ "=", "&#61;" },
                            { ">", "&gt;" },   // or "&#62;"
            				{ "?", "&#63;" },
                            { "^", "&#94;" },
                            { "_", "&#95;" },
                            { "~", "&#126;" },
                            { "@", "&#64;" },
                            { "'", "&#39;" },
                            { "|", "&#124;" },
                            { "!", "&#33;" },
                            { "(", "&#40;" },
                            { ")", "&#41;" },
                            { ".", "&#46;" },
                            { "x", "&#120;" }
                        };


    public static string EncodeNoTextileZones(string tmp, string patternPrefix, string patternSuffix)
    {
        return EncodeNoTextileZones(tmp, patternPrefix, patternSuffix, null);
    }

    public static string EncodeNoTextileZones(string tmp, string patternPrefix, string patternSuffix, string[] exceptions)
    {
        string evaluator(Match m)
        {
            var toEncode = m.Groups["notex"].Value;
            if (toEncode.Length == 0)
            {
                return string.Empty;
            }
            for (var i = 0; i < _textileModifiers.GetLength(0); ++i)
            {
                if (exceptions == null || Array.IndexOf(exceptions, _textileModifiers[i, 0]) < 0)
                {
                    toEncode = toEncode.Replace(_textileModifiers[i, 0], _textileModifiers[i, 1]);
                }
            }
            return patternPrefix + toEncode + patternSuffix;
        }
        tmp = Regex.Replace(tmp, "(" + patternPrefix + "(?<notex>.+?)" + patternSuffix + ")*", new MatchEvaluator(evaluator));
        return tmp;
    }

    public static string DecodeNoTextileZones(string tmp, string patternPrefix, string patternSuffix)
    {
        return DecodeNoTextileZones(tmp, patternPrefix, patternSuffix, null);
    }

    public static string DecodeNoTextileZones(string tmp, string patternPrefix, string patternSuffix, string[] exceptions)
    {
        string evaluator(Match m)
        {
            var toEncode = m.Groups["notex"].Value;
            for (var i = 0; i < _textileModifiers.GetLength(0); ++i)
            {
                if (exceptions == null || Array.IndexOf(exceptions, _textileModifiers[i, 0]) < 0)
                {
                    toEncode = toEncode.Replace(_textileModifiers[i, 1], _textileModifiers[i, 0]);
                }
            }
            return toEncode;
        }
        tmp = Regex.Replace(tmp, "(" + patternPrefix + "(?<notex>.+?)" + patternSuffix + ")*", new MatchEvaluator(evaluator));
        return tmp;
    }
}
