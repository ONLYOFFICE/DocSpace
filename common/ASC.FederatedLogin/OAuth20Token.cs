using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.FederatedLogin;

[DebuggerDisplay("{AccessToken} (expired: {IsExpired})")]
public class OAuth20Token
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("expires_in")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long ExpiresIn { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientID { get; set; }

    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; }

    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonIgnore]
    public string OriginJson { get; set; }

    public bool IsExpired
    {
        get
        {
            if (!ExpiresIn.Equals(default))
            {
                return DateTime.UtcNow > Timestamp + TimeSpan.FromSeconds(ExpiresIn);
            }

            return true;
        }
    }

    public OAuth20Token() { }

    public OAuth20Token(OAuth20Token oAuth20Token)
    {
        Copy(oAuth20Token);
    }

    public OAuth20Token(string json)
    {
        Copy(FromJson(json));
    }

    public static OAuth20Token FromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<OAuth20Token>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public override string ToString()
    {
        return AccessToken;
    }

    private void Copy(OAuth20Token oAuth20Token)
    {
        if (oAuth20Token == null)
        {
            return;
        }

        AccessToken = oAuth20Token.AccessToken;
        RefreshToken = oAuth20Token.RefreshToken;
        ExpiresIn = oAuth20Token.ExpiresIn;
        ClientID = oAuth20Token.ClientID;
        ClientSecret = oAuth20Token.ClientSecret;
        RedirectUri = oAuth20Token.RedirectUri;
        Timestamp = oAuth20Token.Timestamp;
    }
}
