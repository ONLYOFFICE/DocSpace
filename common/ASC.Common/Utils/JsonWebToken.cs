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

using Formatting = Newtonsoft.Json.Formatting;
using IJsonSerializer = JWT.IJsonSerializer;

namespace ASC.Web.Core.Files;

public static class JsonWebToken
{
    public static string Encode(object payload, string key)
    {
        var (serializer, algorithm, urlEncoder) = GetSettings();
        var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

        return encoder.Encode(payload, key);
    }

    public static string Decode(string token, string key, bool verify = true, bool baseSerializer = false)
    {
        var (serializer, algorithm, urlEncoder) = GetSettings(baseSerializer);

        var provider = new UtcDateTimeProvider();
        IJwtValidator validator = new JwtValidator(serializer, provider);

        var decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

        return decoder.Decode(token, key, verify);
    }

    private static (IJsonSerializer, IJwtAlgorithm, IBase64UrlEncoder) GetSettings(bool baseSerializer = false)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return (baseSerializer ? (IJsonSerializer)new JsonNetSerializer() : new JwtSerializer(), new HMACSHA256Algorithm(), new JwtBase64UrlEncoder());
#pragma warning restore CS0618 // Type or member is obsolete
    }
}

public class JwtSerializer : IJsonSerializer
{
    private class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);

            contract.DictionaryKeyResolver = propertyName => propertyName;

            return contract;
        }
    }

    public string Serialize(object obj)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
    }

    public T Deserialize<T>(string json)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public object Deserialize(Type type, string json)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        return JsonConvert.DeserializeObject(json, type, settings);
    }
}
