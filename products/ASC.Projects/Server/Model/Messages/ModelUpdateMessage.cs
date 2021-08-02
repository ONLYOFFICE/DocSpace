namespace ASC.Projects.Model.Messages
{
    public class ModelUpdateMessage
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Participants { get; set; }
        public bool? Notify { get; set; }
    }
}
