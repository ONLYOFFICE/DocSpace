namespace ASC.Files.Core.Model
{
    public class CopyAsModel<T>
    {
        public string DestTitle { get; set; }

        public T DestFolderId { get; set; }

        public bool EnableExternalExt { get; set; }

        public string Password { get; set; }
    }
}
