namespace ASC.People.ApiModels.RequestDto;

public class UpdateMembersRequestDto
{
    public IEnumerable<Guid> UserIds { get; set; }
}
