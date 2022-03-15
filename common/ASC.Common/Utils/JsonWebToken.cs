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
