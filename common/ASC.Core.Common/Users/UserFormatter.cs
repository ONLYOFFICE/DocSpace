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


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using ASC.Common;

using Microsoft.Extensions.Configuration;

namespace ASC.Core.Users
{
    [Singletone]
    public class UserFormatter : IComparer<UserInfo>
    {
        private readonly DisplayUserNameFormat _format;
        private static bool forceFormatChecked;
        private static string forceFormat;

        public UserFormatter(IConfiguration configuration)
        {
            _format = DisplayUserNameFormat.Default;
            Configuration = configuration;
            UserNameRegex = new Regex(Configuration["core:username:regex"] ?? "");
        }

        public string GetUserName(UserInfo userInfo, DisplayUserNameFormat format)
        {
            if (userInfo == null) throw new ArgumentNullException(nameof(userInfo));
            return string.Format(GetUserDisplayFormat(format), userInfo.FirstName, userInfo.LastName);
        }

        public string GetUserName(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) throw new ArgumentException();

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
            if (x == null && y == null) return 0;
            if (x == null && y != null) return -1;
            if (x != null && y == null) return +1;
            if (format == DisplayUserNameFormat.Default) format = GetUserDisplayDefaultOrder();

            int result;
            if (format == DisplayUserNameFormat.FirstLast)
            {
                result = string.Compare(x.FirstName, y.FirstName, true);
                if (result == 0) result = string.Compare(x.LastName, y.LastName, true);
            }
            else
            {
                result = string.Compare(x.LastName, y.LastName, true);
                if (result == 0) result = string.Compare(x.FirstName, y.FirstName, true);
            }
            return result;
        }

        private static readonly Dictionary<string, Dictionary<DisplayUserNameFormat, string>> DisplayFormats = new Dictionary<string, Dictionary<DisplayUserNameFormat, string>>
        {
            { "ru", new Dictionary<DisplayUserNameFormat, string>{ { DisplayUserNameFormat.Default, "{1} {0}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1} {0}" } } },
            { "default", new Dictionary<DisplayUserNameFormat, string>{ {DisplayUserNameFormat.Default, "{0} {1}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1}, {0}" } } },
        };


        private string GetUserDisplayFormat(DisplayUserNameFormat format)
        {
            if (!forceFormatChecked)
            {
                forceFormat = Configuration["core:user-display-format"];
                if (string.IsNullOrEmpty(forceFormat)) forceFormat = null;
                forceFormatChecked = true;
            }
            if (forceFormat != null) return forceFormat;
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            if (!DisplayFormats.TryGetValue(culture, out var formats))
            {
                var twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!DisplayFormats.TryGetValue(twoletter, out formats)) formats = DisplayFormats["default"];
            }
            return formats[format];
        }

        public static DisplayUserNameFormat GetUserDisplayDefaultOrder()
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            if (!DisplayFormats.TryGetValue(culture, out var formats))
            {
                var twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!DisplayFormats.TryGetValue(twoletter, out formats)) formats = DisplayFormats["default"];
            }
            var format = formats[DisplayUserNameFormat.Default];
            return format.IndexOf("{0}") < format.IndexOf("{1}") ? DisplayUserNameFormat.FirstLast : DisplayUserNameFormat.LastFirst;
        }

        public Regex UserNameRegex { get; set; }

        private IConfiguration Configuration { get; }

        public bool IsValidUserName(string firstName, string lastName)
        {
            return UserNameRegex.IsMatch(firstName + lastName);
        }
    }
}
