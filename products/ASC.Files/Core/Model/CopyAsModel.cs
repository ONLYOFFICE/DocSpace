namespace ASC.Files.Core.Model
{
    public class CopyAsModel<T>
    {
        public string Title { get; set; }

        public T FolderId { get; set; }

        public bool EnableExternalExt { get; set; }
    }
}
