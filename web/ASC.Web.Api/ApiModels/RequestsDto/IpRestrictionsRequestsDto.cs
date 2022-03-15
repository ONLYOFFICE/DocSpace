namespace ASC.Web.Api.ApiModel.RequestsDto;

public class IpRestrictionsRequestsDto
{
    public IEnumerable<string> Ips { get; set; }
    public bool Enable { get; set; }
}
