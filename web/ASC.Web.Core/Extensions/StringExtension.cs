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

namespace System;

public static class StringExtension
{
    private static readonly Regex _reStrict = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
              + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
              + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");


    public static string HtmlEncode(this string str)
    {
        return !string.IsNullOrEmpty(str) ? HttpUtility.HtmlEncode(str) : str;
    }

    /// <summary>
    /// Replace ' on ′
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceSingleQuote(this string str)
    {
        return str?.Replace('\'', '′');
    }

    public static bool TestEmailRegex(this string emailAddress)
    {
        emailAddress = (emailAddress ?? "").Trim();
        return !string.IsNullOrEmpty(emailAddress) && _reStrict.IsMatch(emailAddress);
    }

    public static string GetMD5Hash(this string str)
    {
        var bytes = Encoding.Unicode.GetBytes(str);

        using var CSP = MD5.Create();

        var byteHash = CSP.ComputeHash(bytes);

        return byteHash.Aggregate(string.Empty, (current, b) => current + string.Format("{0:x2}", b));
    }

    public static int EnumerableComparer(this string x, string y)
    {
        var xIndex = 0;
        var yIndex = 0;

        while (xIndex < x.Length)
        {
            if (yIndex >= y.Length)
            {
                return 1;
            }

            if (char.IsDigit(x[xIndex]) && char.IsDigit(y[yIndex]))
            {
                var xBuilder = new StringBuilder();
                while (xIndex < x.Length && char.IsDigit(x[xIndex]))
                {
                    xBuilder.Append(x[xIndex++]);
                }

                var yBuilder = new StringBuilder();
                while (yIndex < y.Length && char.IsDigit(y[yIndex]))
                {
                    yBuilder.Append(y[yIndex++]);
                }

                long xValue;
                try
                {
                    xValue = Convert.ToInt64(xBuilder.ToString());
                }
                catch (OverflowException)
                {
                    xValue = long.MaxValue;
                }

                long yValue;
                try
                {
                    yValue = Convert.ToInt64(yBuilder.ToString());
                }
                catch (OverflowException)
                {
                    yValue = long.MaxValue;
                }

                int difference;
                if ((difference = xValue.CompareTo(yValue)) != 0)
                {
                    return difference;
                }
            }
            else
            {
                int difference;
                if ((difference = string.Compare(x[xIndex].ToString(CultureInfo.InvariantCulture), y[yIndex].ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)) != 0)
                {
                    return difference;
                }

                xIndex++;
                yIndex++;
            }
        }

        if (yIndex < y.Length)
        {
            return -1;
        }

        return 0;
    }
}
