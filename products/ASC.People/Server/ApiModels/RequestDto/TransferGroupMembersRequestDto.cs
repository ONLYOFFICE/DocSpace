namespace ASC.People.ApiModels.RequestDto;

public class TransferGroupMembersRequestDto
{
    public Guid GroupId { get; set; }
    public Guid NewGroupId { get; set; }
}
