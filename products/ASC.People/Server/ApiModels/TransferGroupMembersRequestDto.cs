namespace ASC.People.Models
{
    public class TransferGroupMembersRequestDto
    {
        public Guid GroupId { get; set; }
        public Guid NewGroupId { get; set; }
    }
}
