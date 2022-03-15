namespace ASC.Files.Core.Security;

public class FileShareRecord
{
    public int Tenant { get; set; }
    public object EntryId { get; set; }
    public FileEntryType EntryType { get; set; }
    public Guid Subject { get; set; }
    public Guid Owner { get; set; }
    public FileShare Share { get; set; }
    public int Level { get; set; }

    public class ShareComparer : IComparer<FileShare>
    {
        private static readonly int[] ShareOrder = new[]
        {
                (int)FileShare.None,
                (int)FileShare.ReadWrite,
                (int)FileShare.CustomFilter,
                (int)FileShare.Review,
                (int)FileShare.FillForms,
                (int)FileShare.Comment,
                (int)FileShare.Read,
                (int)FileShare.Restrict,
                (int)FileShare.Varies
        };

        public int Compare(FileShare x, FileShare y)
        {
            return Array.IndexOf(ShareOrder, (int)x).CompareTo(Array.IndexOf(ShareOrder, (int)y));
        }
    }
}

public class SmallShareRecord
{
    public Guid ShareTo { get; set; }
    public Guid ShareParentTo { get; set; }
    public Guid ShareBy { get; set; }
    public DateTime ShareOn { get; set; }
    public FileShare Share { get; set; }
}
