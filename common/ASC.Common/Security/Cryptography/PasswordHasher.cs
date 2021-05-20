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
using System.Text;

using ASC.Common;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace ASC.Security.Cryptography
{
    [Singletone]
    public class PasswordHasher
    {
        public PasswordHasher(IConfiguration configuration, MachinePseudoKeys machinePseudoKeys)
        {
            if (!int.TryParse(configuration["core:password:size"], out var size)) size = 256;
            Size = size;

            if (!int.TryParse(configuration["core.password.iterations"], out var iterations)) iterations = 100000;
            Iterations = iterations;

            Salt = (configuration["core:password:salt"] ?? "").Trim();
            if (string.IsNullOrEmpty(Salt))
            {
                var salt = Hasher.Hash("{9450BEF7-7D9F-4E4F-A18A-971D8681722D}", HashAlg.SHA256);

                var PasswordHashSaltBytes = KeyDerivation.Pbkdf2(
                                                   Encoding.UTF8.GetString(machinePseudoKeys.GetMachineConstant()),
                                                   salt,
                                                   KeyDerivationPrf.HMACSHA256,
                                                   Iterations,
                                                   Size / 8);
                Salt = BitConverter.ToString(PasswordHashSaltBytes).Replace("-", string.Empty).ToLower();
            }
        }

        public int Size
        {
            get;
            private set;
        }

        public int Iterations
        {
            get;
            private set;
        }

        public string Salt
        {
            get;
            private set;
        }

        public string GetClientPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) password = Guid.NewGuid().ToString();

            var salt = new UTF8Encoding(false).GetBytes(Salt);

            var hashBytes = KeyDerivation.Pbkdf2(
                               password,
                               salt,
                               KeyDerivationPrf.HMACSHA256,
                               Iterations,
                               Size / 8);

            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();

            return hash;
        }
    }

}