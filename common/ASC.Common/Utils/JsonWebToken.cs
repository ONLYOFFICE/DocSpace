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

using JWT;
using JWT.Algorithms;
using JWT.Serializers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Formatting = Newtonsoft.Json.Formatting;

namespace ASC.Web.Core.Files
{
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
            return (baseSerializer ? (IJsonSerializer)new JsonNetSerializer() : new JwtSerializer(), new HMACSHA256Algorithm(), new JwtBase64UrlEncoder());
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
    }
}
