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

namespace ASC.Core.Users
{
    [Singletone]
    public class UserFormatter : IComparer<UserInfo>
    {
        public Regex UserNameRegex { get; set; }

        private readonly DisplayUserNameFormat _format;
        private readonly IConfiguration _configuration;
        private static bool s_forceFormatChecked;
        private static string s_forceFormat;
        
        private static readonly Dictionary<string, Dictionary<DisplayUserNameFormat, string>> s_displayFormats = new Dictionary<string, Dictionary<DisplayUserNameFormat, string>>
        {
            { "ru", new Dictionary<DisplayUserNameFormat, string>{ { DisplayUserNameFormat.Default, "{1} {0}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1} {0}" } } },
            { "default", new Dictionary<DisplayUserNameFormat, string>{ {DisplayUserNameFormat.Default, "{0} {1}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1}, {0}" } } },
        };

        public UserFormatter(IConfiguration configuration)
        {
            _format = DisplayUserNameFormat.Default;
            _configuration = configuration;
            UserNameRegex = new Regex(_configuration["core:username:regex"] ?? "");
        }

        public string GetUserName(UserInfo userInfo, DisplayUserNameFormat format)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException(nameof(userInfo));
            }

            return string.Format(GetUserDisplayFormat(format), userInfo.FirstName, userInfo.LastName);
        }

        public string GetUserName(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException(nameof(firstName));
            }

            return string.Format(GetUserDisplayFormat(DisplayUserNameFormat.Default), firstName, lastName);
        }

        public bool IsValidUserName(string firstName, string lastName)
        {
            return UserNameRegex.IsMatch(firstName + lastName);
        }

        public string GetUserName(UserInfo userInfo)
        {
            return GetUserName(userInfo, DisplayUserNameFormat.Default);
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

        public static DisplayUserNameFormat GetUserDisplayDefaultOrder()
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            if (!s_displayFormats.TryGetValue(culture, out var formats))
            {
                var twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!s_displayFormats.TryGetValue(twoletter, out formats))
                {
                    formats = s_displayFormats["default"];
                }
            }
            var format = formats[DisplayUserNameFormat.Default];

            return format.IndexOf("{0}") < format.IndexOf("{1}") ? DisplayUserNameFormat.FirstLast : DisplayUserNameFormat.LastFirst;
        }

        private string GetUserDisplayFormat(DisplayUserNameFormat format)
        {
            if (!s_forceFormatChecked)
            {
                s_forceFormat = _configuration["core:user-display-format"];
                if (string.IsNullOrEmpty(s_forceFormat))
                {
                    s_forceFormat = null;
                }

                s_forceFormatChecked = true;
            }

            if (s_forceFormat != null)
            {
                return s_forceFormat;
            }

            var culture = Thread.CurrentThread.CurrentCulture.Name;
            if (!s_displayFormats.TryGetValue(culture, out var formats))
            {
                var twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!s_displayFormats.TryGetValue(twoletter, out formats))
                {
                    formats = s_displayFormats["default"];
                }
            }

            return formats[format];
        }

        int IComparer<UserInfo>.Compare(UserInfo x, UserInfo y)
        {
            return Compare(x, y, _format);
        }
    }
}