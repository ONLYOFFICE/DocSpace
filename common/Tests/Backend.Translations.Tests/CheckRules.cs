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

using System.Globalization;
using System.Text.RegularExpressions;

namespace Backend.Translations.Tests;
public static class CheckRules
{
    public static bool CompliesToRulePunctuationLead(string? neutralValue, string? value)
    {
        var reference = GetPunctuationSequence(neutralValue).ToArray();

        var array = GetPunctuationSequence(value);
        return !reference.SequenceEqual(array);
    }

    public static bool CompliesToRulePunctuationTail(string? neutralValue, string? value)
    {
        var reference = GetPunctuationSequence(neutralValue, true).ToArray();

        var array = GetPunctuationSequence(value, true);
        return !reference.SequenceEqual(array);
    }

    public static bool CompliesToRuleWhiteSpaceLead(string? neutralValue, string? value)
    {
        var reference = GetWhiteSpaceSequence(neutralValue);

        var array = GetWhiteSpaceSequence(value);
        return !reference.SequenceEqual(array);
    }

    public static bool CompliesToRuleWhiteSpaceTail(string? neutralValue, string? value)
    {
        var reference = GetWhiteSpaceSequence(neutralValue, true);

        var array = GetWhiteSpaceSequence(value, true);
        return !reference.SequenceEqual(array);
    }

    public static bool CompliesToRuleStringFormat(string? neutralValue, string? value)
    {
        var allValues = new[] { neutralValue, value }.ToList();

        var indexedComply = GetStringFormatByIndexFlags(neutralValue) == GetStringFormatByIndexFlags(value);

        var namedComply = GetStringFormatByPlaceholdersFingerprint(neutralValue) == GetStringFormatByPlaceholdersFingerprint(value);

        return !(indexedComply && namedComply);
    }

    private static string GetStringFormatByPlaceholdersFingerprint(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return string.Join("|", ExtractPlaceholders(value).OrderBy(item => item));
    }

    private static readonly Regex _formatPlaceholderExpression = new(@"\$\{\s*(\w[.\w\d_]*)\s*\}");
    public static IEnumerable<string> ExtractPlaceholders(string text)
    {
        var placeholders = _formatPlaceholderExpression.Matches(text)
            .OfType<Match>()
            .Select(m => m.Groups[1].Value)
            .Distinct();

        return placeholders;
    }

    private static readonly Regex _getStringFormatByIndexExpression = new(@"\{([0-9]+)(?:,-?[0-9]+)?(?::[^\}]+)?\}", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static long GetStringFormatByIndexFlags(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        return _getStringFormatByIndexExpression.Matches(value)
            .Cast<Match>()
            .Where(m => m.Success)
            .Aggregate(0L, (a, match) => a | ParseMatch(match));
    }

    private static long ParseMatch(Match match)
    {
        if (int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return 1L << value;

        return 0;
    }

    private static IEnumerable<char> GetWhiteSpaceSequence(string? value, bool revers = false)
    {
        return GetCharIterator(value, revers).TakeWhile(char.IsWhiteSpace);
    }

    private static IEnumerable<char> GetCharIterator(string value, bool revers) => revers ? value.Reverse() : value;

    private static IEnumerable<char> GetPunctuationSequence(string? value, bool revers = false)
    {
        return GetCharIterator(NormalizeUnicode(value), revers)
            .SkipWhile(char.IsWhiteSpace).
            TakeWhile(IsPunctuation).
            Select(NormalizePunctuation);
    }

    private static char NormalizePunctuation(char value)
    {
        switch ((int)value)
        {
            case 0x055C: return '!'; // ARMENIAN EXCLAMATION MARK
            case 0x055D: return ','; // ARMENIAN COMMA
            case 0x055E: return '?'; // ARMENIAN QUESTION MARK
            case 0x0589: return '.'; // ARMENIAN FULL STOP
            case 0x07F8: return ','; // NKO COMMA
            case 0x07F9: return '!'; // NKO EXCLAMATION MARK
            case 0x1944: return '!'; // LIMBU EXCLAMATION MARK
            case 0x1945: return '?'; // LIMBU QUESTION MARK
            case 0x3001: return ','; // IDEOGRAPHIC COMMA
            case 0x3002: return '.'; // IDEOGRAPHIC FULL STOP
            case 0xFF01: return '!'; // FULLWIDTH EXCLAMATION MARK
            case 0xFF0C: return ','; // FULLWIDTH COMMA
            case 0xFF0E: return '.'; // FULLWIDTH FULL STOP
            case 0xFF1A: return ':'; // FULLWIDTH COLON
            case 0xFF1B: return ';'; // FULLWIDTH SEMICOLON
            case 0xFF1F: return '?'; // FULLWIDTH QUESTION MARK
            case 0x061F: return '?'; // ARABIC QUESTION MARK
            default: return value;
        }
    }

    private static string NormalizeUnicode(string? value) => value?.Normalize() ?? string.Empty;

    private static bool IsPunctuation(char value)
    {
        // exclude quotes, special chars (\#), hot-key prefixes (&_), language specifics with no common equivalent (¡¿).
        const string excluded = "'\"\\#&_¡¿";

        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (char.GetUnicodeCategory(value))
        {
            case UnicodeCategory.OtherPunctuation:
                return !excluded.Contains(value, StringComparison.Ordinal);
            case UnicodeCategory.DashPunctuation:
                return true;
            default:
                return false;
        }
    }
}
