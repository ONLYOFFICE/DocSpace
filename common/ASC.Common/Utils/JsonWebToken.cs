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

using IJsonSerializer = JWT.IJsonSerializer;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        return (baseSerializer ? new JsonNetSerializer() : new JwtSerializer(), new HMACSHA256Algorithm(), new JwtBase64UrlEncoder());
#pragma warning restore CS0618 // Type or member is obsolete
    }
}

public class DictionaryStringObjectJsonConverter : JsonConverter<Dictionary<string, object>>
{
    public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }

        var dictionary = new Dictionary<string, object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("JsonTokenType was not PropertyName");
            }

            var propertyName = reader.GetString();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new JsonException("Failed to get property name");
            }

            reader.Read();

            dictionary.Add(propertyName, ExtractValue(ref reader, options));
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (reader.TryGetDateTime(out var date))
                {
                    return date;
                }
                return reader.GetString();
            case JsonTokenType.False:
                return false;
            case JsonTokenType.True:
                return true;
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var result))
                {
                    return result;
                }
                return reader.GetDecimal();
            case JsonTokenType.StartObject:
                return Read(ref reader, null, options);
            case JsonTokenType.StartArray:
                var list = new List<object>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }
                return list;
            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }
}


public class JwtSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options;

    public JwtSerializer()
    {
        _options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _options.Converters.Add(new DictionaryStringObjectJsonConverter());
    }

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    public object Deserialize(Type type, string json)
    {
        return JsonSerializer.Deserialize(json, type, _options);
    }
}
