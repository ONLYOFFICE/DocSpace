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

using ASC.Core.Common.Settings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Core.Utility
{
    [Serializable]
    public sealed class PasswordSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("aa93a4d1-012d-4ccd-895a-e094e809c840"); }
        }

        public const int MaxLength = 30;

        /// <summary>
        /// Minimal length password has
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Password must contains upper case
        /// </summary>
        public bool UpperCase { get; set; }

        /// <summary>
        /// Password must contains digits
        /// </summary>
        public bool Digits { get; set; }

        /// <summary>
        /// Password must contains special symbols
        /// </summary>
        public bool SpecSymbols { get; set; }

        public ISettings GetDefault(IConfiguration configuration)
        {
            var def = new PasswordSettings { MinLength = 8, UpperCase = false, Digits = false, SpecSymbols = false };

            if (int.TryParse(configuration["web.password.min"], out var defaultMinLength))
            {
                def.MinLength = Math.Max(1, Math.Min(MaxLength, defaultMinLength));
            }

            return def;
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return GetDefault(serviceProvider.GetService<IConfiguration>());
        }
    }
}