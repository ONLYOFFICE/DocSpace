namespace ASC.Web.Api.ApiModel;

public class StorageDto
{
    public string Module { get; set; }
    public IEnumerable<ItemKeyValuePair<string, string>> Props { get; set; }
}