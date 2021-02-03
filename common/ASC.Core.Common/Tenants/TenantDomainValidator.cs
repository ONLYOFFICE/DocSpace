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
using System.Text.RegularExpressions;

using ASC.Common;

using Microsoft.Extensions.Configuration;

namespace ASC.Core.Tenants
{
    [Singletone]
    public class TenantDomainValidator
    {
        private static readonly Regex ValidDomain = new Regex("^[a-z0-9]([a-z0-9-]){1,98}[a-z0-9]$",
                                                              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private readonly int MinLength;
        private const int MaxLength = 100;

        public TenantDomainValidator(IConfiguration configuration)
        {
            MinLength = 6;

            if (int.TryParse(configuration["web:alias:min"], out var defaultMinLength))
            {
                MinLength = Math.Max(1, Math.Min(MaxLength, defaultMinLength));
            }
        }

        public void ValidateDomainLength(string domain)
        {
            if (string.IsNullOrEmpty(domain)
                || domain.Length < MinLength || MaxLength < domain.Length)
            {
                throw new TenantTooShortException("The domain name must be between " + MinLength + " and " + MaxLength + " characters long.", MinLength, MaxLength);
            }
        }

        public static void ValidateDomainCharacters(string domain)
        {
            if (!ValidDomain.IsMatch(domain))
            {
                throw new TenantIncorrectCharsException("Domain contains invalid characters.");
            }
        }
    }
}