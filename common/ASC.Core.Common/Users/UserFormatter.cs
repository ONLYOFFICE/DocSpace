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

namespace ASC.Core.Users;

[Singletone]
public class UserFormatter : IComparer<UserInfo>
{
    private readonly DisplayUserNameFormat _format;
    private static bool _forceFormatChecked;
    private static string _forceFormat;

    public UserFormatter(IConfiguration configuration)
    {
        _format = DisplayUserNameFormat.Default;
        _configuration = configuration;
        UserNameRegex = new Regex(_configuration["core:username:regex"] ?? "");
    }

    public string GetUserName(UserInfo userInfo, DisplayUserNameFormat format)
    {
        ArgumentNullException.ThrowIfNull(userInfo);

        return string.Format(GetUserDisplayFormat(format), userInfo.FirstName, userInfo.LastName);
    }

    public string GetUserName(string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException();
        }

        return string.Format(GetUserDisplayFormat(DisplayUserNameFormat.Default), firstName, lastName);
    }

    public string GetUserName(UserInfo userInfo)
    {
        return GetUserName(userInfo, DisplayUserNameFormat.Default);
    }

    int IComparer<UserInfo>.Compare(UserInfo x, UserInfo y)
    {
        return Compare(x, y, _format);
    }

    public static int Compare(UserInfo x, UserInfo y)
    {
        return Compare(x, y, DisplayUserNameFormat.Default);
    }

    public static int Compare(UserInfo x, UserInfo y, DisplayUserNameFormat format)
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null && y != null)
        {
            return -1;
        }
        if (x != null && y == null)
        {
            return +1;
        }

        if (format == DisplayUserNameFormat.Default)
        {
            format = GetUserDisplayDefaultOrder();
        }

        int result;
        if (format == DisplayUserNameFormat.FirstLast)
        {
            result = string.Compare(x.FirstName, y.FirstName, true);
            if (result == 0)
            {
                result = string.Compare(x.LastName, y.LastName, true);
            }
        }
        else
        {
            result = string.Compare(x.LastName, y.LastName, true);
            if (result == 0)
            {
                result = string.Compare(x.FirstName, y.FirstName, true);
            }
        }

        return result;
    }

    private static readonly Dictionary<string, Dictionary<DisplayUserNameFormat, string>> _displayFormats = new Dictionary<string, Dictionary<DisplayUserNameFormat, string>>
        {
            { "ru", new Dictionary<DisplayUserNameFormat, string>{ { DisplayUserNameFormat.Default, "{1} {0}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1} {0}" } } },
            { "default", new Dictionary<DisplayUserNameFormat, string>{ {DisplayUserNameFormat.Default, "{0} {1}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1}, {0}" } } },
        };


    private string GetUserDisplayFormat(DisplayUserNameFormat format)
    {
        if (!_forceFormatChecked)
        {
            _forceFormat = _configuration["core:user-display-format"];
            if (string.IsNullOrEmpty(_forceFormat))
            {
                _forceFormat = null;
            }

            _forceFormatChecked = true;
        }
        if (_forceFormat != null)
        {
            return _forceFormat;
        }

        var culture = CultureInfo.CurrentCulture.Name;
        if (!_displayFormats.TryGetValue(culture, out var formats))
        {
            var twoletter = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (!_displayFormats.TryGetValue(twoletter, out formats))
            {
                formats = _displayFormats["default"];
            }
        }

        return formats[format];
    }

    public static DisplayUserNameFormat GetUserDisplayDefaultOrder()
    {
        var culture = CultureInfo.CurrentCulture.Name;
        if (!_displayFormats.TryGetValue(culture, out var formats))
        {
            var twoletter = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (!_displayFormats.TryGetValue(twoletter, out formats))
            {
                formats = _displayFormats["default"];
            }
        }
        var format = formats[DisplayUserNameFormat.Default];

        return format.IndexOf("{0}") < format.IndexOf("{1}") ? DisplayUserNameFormat.FirstLast : DisplayUserNameFormat.LastFirst;
    }

    public Regex UserNameRegex { get; set; }

    private readonly IConfiguration _configuration;

    public bool IsValidUserName(string firstName, string lastName)
    {
        return UserNameRegex.IsMatch(firstName + lastName);
    }
}
