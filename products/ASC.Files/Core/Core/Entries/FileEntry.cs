namespace ASC.Files.Core;

[Serializable]
public abstract class FileEntry : ICloneable
{
    [JsonIgnore]
    public FileHelper FileHelper { get; set; }

    [JsonIgnore]
    public Global Global { get; set; }

    protected FileEntry() { }

    protected FileEntry(FileHelper fileHelper, Global global)
    {
        FileHelper = fileHelper;
        Global = global;
    }

    public virtual string Title { get; set; }
    public Guid CreateBy { get; set; }

    [JsonIgnore]
    public string CreateByString
    {
        get => !CreateBy.Equals(Guid.Empty) ? Global.GetUserName(CreateBy) : _createByString;
        set => _createByString = value;
    }

    public Guid ModifiedBy { get; set; }

    [JsonIgnore]
    public string ModifiedByString
    {
        get => !ModifiedBy.Equals(Guid.Empty) ? Global.GetUserName(ModifiedBy) : _modifiedByString;
        set => _modifiedByString = value;
    }

    [JsonIgnore]
    public string CreateOnString => CreateOn.Equals(default) ? null : CreateOn.ToString("g");

    [JsonIgnore]
    public string ModifiedOnString => ModifiedOn.Equals(default) ? null : ModifiedOn.ToString("g");

    public string Error { get; set; }
    public FileShare Access { get; set; }
    public bool Shared { get; set; }
    public int ProviderId { get; set; }
    public string ProviderKey { get; set; }

    [JsonIgnore]
    public bool ProviderEntry => !string.IsNullOrEmpty(ProviderKey);

    public DateTime CreateOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public FolderType RootFolderType { get; set; }
    public Guid RootFolderCreator { get; set; }
    public abstract bool IsNew { get; set; }
    public FileEntryType FileEntryType { get; set; }

    private string _modifiedByString;
    private string _createByString;

    public override string ToString()
    {
        return Title;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

public interface IFileEntry<in T>
{
    string UniqID { get; }
}


[Serializable]
public abstract class FileEntry<T> : FileEntry, ICloneable, IFileEntry<T>
{
    public T ID { get; set; }
    public T FolderID { get; set; }
    private T _folderIdDisplay;

    protected FileEntry() { }

    protected FileEntry(FileHelper fileHelper, Global global) : base(fileHelper, global) { }

    public T FolderIdDisplay
    {
        get
        {
            if (_folderIdDisplay != null)
            {
                return _folderIdDisplay;
            }

            return FolderID;
        }
        set => _folderIdDisplay = value;
    }

    public T RootFolderId { get; set; }

    [JsonIgnore]
    public virtual string UniqID => $"{GetType().Name.ToLower()}_{ID}";

    public override bool Equals(object obj)
    {
        return obj is FileEntry<T> f && Equals(f.ID, ID);
    }

    public virtual bool Equals(FileEntry<T> obj)
    {
        return Equals(obj.ID, ID);
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public override string ToString()
    {
        return Title;
    }
}
