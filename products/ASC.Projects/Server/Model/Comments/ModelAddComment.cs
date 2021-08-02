namespace ASC.Projects.Model.Comments
{
    public class ModelAddComment
    {
        public string ParentCommentId { get; set; }
        public int EntityId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
    }
}
