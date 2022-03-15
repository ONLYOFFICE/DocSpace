namespace ASC.Web.Api.ApiModel.ResponseDto;

public class SmtpSettingsDto : IMapFrom<SmtpSettings>
{
    public string Host { get; set; }
    public int? Port { get; set; }
    public string SenderAddress { get; set; }
    public string SenderDisplayName { get; set; }
    public string CredentialsUserName { get; set; }
    public string CredentialsUserPassword { get; set; }
    public bool EnableSSL { get; set; }
    public bool EnableAuth { get; set; }

    public static SmtpSettingsDto GetSample()
    {
        return new SmtpSettingsDto
        {
            Host = "mail.example.com",
            Port = 25,
            CredentialsUserName = "notify@example.com",
            CredentialsUserPassword = "{password}",
            EnableAuth = true,
            EnableSSL = false,
            SenderAddress = "notify@example.com",
            SenderDisplayName = "Postman"
        };
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SmtpSettings, SmtpSettingsDto>();
    }
}