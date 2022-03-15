namespace ASC.Web.Api.ApiModel.RequestsDto;

public class EncryptionSettingsRequestsDto
{
    public string Password { get; set; }
    public EncryprtionStatus Status { get; set; }
    public bool NotifyUsers { get; set; }
    public string ServerRootPath { get; set; }
}