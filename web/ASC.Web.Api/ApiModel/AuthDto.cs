namespace ASC.Web.Api.ApiModel;

public class AuthDto
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

public class MobileDto
{
    public string MobilePhone { get; set; }
}
