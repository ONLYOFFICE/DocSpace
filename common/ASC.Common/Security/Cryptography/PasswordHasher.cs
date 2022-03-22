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

namespace ASC.Security.Cryptography;

[Singletone]
public class PasswordHasher
{
    public int Size { get; private set; }
    public int Iterations { get; private set; }
    public string Salt { get; private set; }

    public PasswordHasher(IConfiguration configuration, MachinePseudoKeys machinePseudoKeys)
    {
        if (!int.TryParse(configuration["core:password:size"], out var size))
        {
            size = 256;
        }

        Size = size;

        if (!int.TryParse(configuration["core.password.iterations"], out var iterations))
        {
            iterations = 100000;
        }

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

    public string GetClientPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            password = Guid.NewGuid().ToString();
        }

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