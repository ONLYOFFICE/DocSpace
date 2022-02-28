namespace ASC.Files.Core.Model;

public class ThirdPartyRequestDto
{
    public string Url { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
    public bool IsCorporate { get; set; }
    public string CustomerTitle { get; set; }
    public string ProviderKey { get; set; }
    public string ProviderId { get; set; }
}
