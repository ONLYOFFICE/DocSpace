namespace ASC.People.Models
{
    public class StartReassignRequestDto
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public bool DeleteProfile { get; set; }
    }
}
