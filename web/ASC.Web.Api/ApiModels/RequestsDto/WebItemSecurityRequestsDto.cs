namespace ASC.Web.Api.ApiModel.RequestsDto;

public class WebItemSecurityRequestsDto
{
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public IEnumerable<Guid> Subjects { get; set; }
    public IEnumerable<ItemKeyValuePair<string, bool>> Items { get; set; }
}