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

namespace ASC.Core;

public static class Crypto
{
    private static byte[] GetSK1(bool rewrite)
    {
        return GetSK(rewrite.GetType().Name.Length);
    }

    private static byte[] GetSK2(bool rewrite)
    {
        return GetSK(rewrite.GetType().Name.Length * 2);
    }

    private static byte[] GetSK(int seed)
    {
        var random = new AscRandom(seed);
        var randomKey = new byte[32];
        for (var i = 0; i < randomKey.Length; i++)
        {
            randomKey[i] = (byte)random.Next(byte.MaxValue);
        }

        return randomKey;
    }

    public static string GetV(string data, int keyno, bool reverse)
    {
        var hasher = Aes.Create();
        hasher.Key = keyno == 1 ? GetSK1(false) : GetSK2(false);
        hasher.IV = new byte[hasher.BlockSize >> 3];

        if (reverse)
        {
            using var ms = new MemoryStream();
            using var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write);
            using var plainTextStream = new MemoryStream(Encoding.Unicode.GetBytes(data));
            plainTextStream.CopyTo(ss);
            ss.FlushFinalBlock();
            hasher.Clear();

            return Convert.ToBase64String(ms.ToArray());
        }
        else
        {
            using var ms = new MemoryStream(Convert.FromBase64String(data));
            using var ss = new CryptoStream(ms, hasher.CreateDecryptor(), CryptoStreamMode.Read);
            using var plainTextStream = new MemoryStream();
            ss.CopyTo(plainTextStream);
            hasher.Clear();

            return Encoding.Unicode.GetString(plainTextStream.ToArray());
        }
    }

    internal static byte[] GetV(byte[] data, int keyno, bool reverse)
    {
        var hasher = Aes.Create();
        hasher.Key = keyno == 1 ? GetSK1(false) : GetSK2(false);
        hasher.IV = new byte[hasher.BlockSize >> 3];

        if (reverse)
        {
            using var ms = new MemoryStream();
            using var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write);
            using var plainTextStream = new MemoryStream(data);
            plainTextStream.CopyTo(ss);
            ss.FlushFinalBlock();
            hasher.Clear();

            return ms.ToArray();
        }
        else
        {
            using var ms = new MemoryStream(data);
            using var ss = new CryptoStream(ms, hasher.CreateDecryptor(), CryptoStreamMode.Read);
            using var plainTextStream = new MemoryStream();
            ss.CopyTo(plainTextStream);
            hasher.Clear();

            return plainTextStream.ToArray();
        }
    }
}
