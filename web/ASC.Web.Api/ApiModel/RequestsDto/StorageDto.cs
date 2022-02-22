namespace ASC.Web.Api.ApiModel.RequestsDto;

public class StorageDto
{
    public string Module { get; set; }
    public IEnumerable<ItemKeyValuePair<string, string>> Props { get; set; }
}