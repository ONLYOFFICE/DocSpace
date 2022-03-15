namespace ASC.Web.Api.ApiModel.RequestsDto;

public class StorageRequestsDto
{
    public string Module { get; set; }
    public IEnumerable<ItemKeyValuePair<string, string>> Props { get; set; }
}