namespace ASC.Web.Api.ApiModel.ResponseDto;

public class TenantVersionDto
{
    public int Current { get; set; }
    public IEnumerable<TenantVersion> Versions { get; set; }

    public TenantVersionDto(int version, IEnumerable<TenantVersion> tenantVersions)
    {
        Current = version;
        Versions = tenantVersions;
    }
}