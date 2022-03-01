namespace ASC.Web.Api.ApiModel.RequestsDto;

public class SettingsDto
{
    public Guid DefaultProductID { get; set; }
    public string Lng { get; set; }
    public string TimeZoneID { get; set; }
    public string Theme { get; set; }
    public bool Show { get; set; } //tips
    public int VersionId { get; set; }
    public Guid OwnerId { get; set; }
}
