namespace ASC.People.Models
{
    public class UpdateMembersRequestDto
    {
        public IEnumerable<Guid> UserIds { get; set; }
    }
}
