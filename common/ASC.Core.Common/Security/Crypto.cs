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
using System.IO;
using System.Security.Cryptography;
using System.Text;

using ASC.Common.Security;

namespace ASC.Core
{
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
                using var plainTextStream = new MemoryStream(Convert.FromBase64String(data));
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
}