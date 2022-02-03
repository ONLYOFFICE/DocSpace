namespace ASC.People.Models
{
    public class StartReassignModel
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public bool DeleteProfile { get; set; }
    }
}
