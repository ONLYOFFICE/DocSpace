/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


namespace ASC.Common.Utils
{
    public static class Wildcard
    {
        public static bool WildcardMatch(this string input, string pattern)
        {
            return WildcardMatch(input, pattern, true);
        }

        public static bool WildcardMatch(this string input, string pattern, bool ignoreCase)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return IsMatch(pattern, input, ignoreCase);
            }
            return false;
        }

        public static bool IsMatch(string pattern, string input)
        {
            return IsMatch(pattern, input, true);
        }

        public static bool IsMatch(string pattern, string input, bool ignoreCase)
        {
            var offsetInput = 0;
            var isAsterix = false;
            int i = 0;
            while (i < pattern.Length)
            {
                switch (pattern[i])
                {
                    case '?':
                        isAsterix = false;
                        offsetInput++;
                        break;
                    case '*':
                        isAsterix = true;
                        while (i < pattern.Length &&
                                pattern[i] == '*')
                        {
                            i++;
                        }
                        if (i >= pattern.Length)
                            return true;
                        continue;
                    default:
                        if (offsetInput >= input.Length)
                            return false;
                        if ((ignoreCase
                                    ? char.ToLower(input[offsetInput])
                                    : input[offsetInput])
                            !=
                            (ignoreCase
                                    ? char.ToLower(pattern[i])
                                    : pattern[i]))
                        {
                            if (!isAsterix)
                                return false;
                            offsetInput++;
                            continue;
                        }
                        offsetInput++;
                        break;
                }
                i++;
            }

            if (i > input.Length)
                return false;

            while (i < pattern.Length && pattern[i] == '*')
                ++i;

            return offsetInput == input.Length;
        }
    }
}