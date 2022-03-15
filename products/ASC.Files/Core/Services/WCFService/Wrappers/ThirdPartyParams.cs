namespace ASC.Web.Files.Services.WCFService;

public class ThirdPartyParams
{
    [JsonPropertyName("auth_data")]
    public AuthData AuthData { get; set; }

    public bool Corporate { get; set; }

    [JsonPropertyName("customer_title")]
    public string CustomerTitle { get; set; }

    [JsonPropertyName("provider_id")]
    public string ProviderId { get; set; }

    [JsonPropertyName("provider_key")]
    public string ProviderKey { get; set; }
}
