namespace ASC.Files.Core;

public enum FolderType
{
    DEFAULT = 0,
    COMMON = 1,
    BUNCH = 2,
    TRASH = 3,
    USER = 5,
    SHARE = 6,
    Projects = 8,
    Favorites = 10,
    Recent = 11,
    Templates = 12,
    Privacy = 13,
}

public interface IFolder
{
    public FolderType FolderType { get; set; }
    public int TotalFiles { get; set; }
    public int TotalSubFolders { get; set; }
    public bool Shareable { get; set; }
    public int NewForMe { get; set; }
    public string FolderUrl { get; set; }
}

[DebuggerDisplay("{Title} ({ID})")]
[Transient]
public class Folder<T> : FileEntry<T>, IFolder
{
    public FolderType FolderType { get; set; }
    public int TotalFiles { get; set; }
    public int TotalSubFolders { get; set; }
    public bool Shareable { get; set; }
    public int NewForMe { get; set; }
    public string FolderUrl { get; set; }
    public override bool IsNew
    {
        get => Convert.ToBoolean(NewForMe);
        set => NewForMe = Convert.ToInt32(value);
    }

    public Folder()
    {
        Title = string.Empty;
        FileEntryType = FileEntryType.Folder;
    }

    public Folder(FileHelper fileHelper, Global global) : this()
    {
        FileHelper = fileHelper;
        Global = global;
    }

    public override string UniqID => $"folder_{ID}";
}
