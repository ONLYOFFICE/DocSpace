namespace ASC.Web.Api.ApiModel.RequestsDto;

public class AuthRequestsDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PasswordHash { get; set; }
    public string Provider { get; set; }
    public string AccessToken { get; set; }
    public string SerializedProfile { get; set; }
    public string Code { get; set; }
    public bool Session { get; set; }
}

public class MobileRequestsDto
{
    public string MobilePhone { get; set; }
}
