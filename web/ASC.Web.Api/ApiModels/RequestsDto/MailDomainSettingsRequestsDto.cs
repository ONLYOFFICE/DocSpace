namespace ASC.Web.Api.ApiModel.RequestsDto;

public class MailDomainSettingsRequestsDto
{
    public TenantTrustedDomainsType Type { get; set; }
    public List<string> Domains { get; set; }
    public bool InviteUsersAsVisitors { get; set; }
}

public class AdminMessageSettingsRequestsDto
{
    public string Email { get; set; }
    public string Message { get; set; }
    public bool TurnOn { get; set; }
}