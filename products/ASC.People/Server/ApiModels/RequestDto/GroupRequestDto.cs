namespace ASC.People.Models
{
    public class GroupRequestDto
    {
        public Guid GroupManager { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<Guid> Members { get; set; }
    }
}
