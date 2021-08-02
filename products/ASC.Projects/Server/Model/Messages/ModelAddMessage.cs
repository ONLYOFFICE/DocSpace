namespace ASC.Projects.Model.Messages
{
    public class ModelAddMessage
    {
        public string Title { get; set;}
        public string Content { get; set; }
        public string Participants { get; set; }
        public bool? Notify { get; set; }
    }
}
