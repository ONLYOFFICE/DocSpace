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

namespace ASC.Common.Utils;

[Singletone]
public class Signature
{
    private readonly MachinePseudoKeys _machinePseudoKeys;

    public Signature(MachinePseudoKeys machinePseudoKeys)
    {
        _machinePseudoKeys = machinePseudoKeys;
    }

    public string Create<T>(T obj)
    {
        return Create(obj, Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()));
    }

    public static string Create<T>(T obj, string secret)
    {
        var str = JsonConvert.SerializeObject(obj);
        var payload = GetHashBase64(str + secret) + "?" + str;

        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(payload));
    }

    public T Read<T>(string signature)
    {
        return Read<T>(signature, Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()));
    }

    public static T Read<T>(string signature, string secret)
    {
        try
        {
            var lastSignChar = Int32.Parse(signature.Substring(signature.Length - 1));
            signature = signature.Remove(signature.Length - 1);

            while(lastSignChar > 0)
            {
                signature = signature + "=";
                lastSignChar--;
            }
            var payloadParts = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(signature)).Split('?');

            if (GetHashBase64(payloadParts[1].Trim() + secret) == payloadParts[0])
            {
                return JsonConvert.DeserializeObject<T>(payloadParts[1].Trim()); //Sig correct
            }
        }
        catch (Exception) { }

        return default;
    }

    private static string GetHashBase64(string str)
    {
        using var sha256 = SHA256.Create();

        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)));
    }

    private static string GetHashBase64MD5(string str)
    {
        using var md5 = MD5.Create();

        return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
    }
}
