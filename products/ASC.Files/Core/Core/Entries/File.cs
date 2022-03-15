namespace ASC.Files.Core;

[Flags]
public enum FileStatus
{
    None = 0x0,
    IsEditing = 0x1,
    IsNew = 0x2,
    IsConverting = 0x4,
    IsOriginal = 0x8,
    IsEditingAlone = 0x10,
    IsFavorite = 0x20,
    IsTemplate = 0x40,
    IsFillFormDraft = 0x80
}

[Transient]
[Serializable]
[DebuggerDisplay("{Title} ({ID} v{Version})")]
public class File<T> : FileEntry<T>, IFileEntry<T>
{
    private FileStatus _status;

    public File()
    {
        Version = 1;
        VersionGroup = 1;
        FileEntryType = FileEntryType.File;
    }

    public File(FileHelper fileHelper, Global global) : this()
    {
        FileHelper = fileHelper;
        Global = global;
    }

    public int Version { get; set; }
    public int VersionGroup { get; set; }
    public string Comment { get; set; }
    public string PureTitle
    {
        get => base.Title;
        set => base.Title = value;
    }
    public long ContentLength { get; set; }

    [JsonIgnore]
    public string ContentLengthString => FileSizeComment.FilesSizeToString(ContentLength);

    [JsonIgnore]
    public FilterType FilterType
    {
        get
        {
            switch (FileUtility.GetFileTypeByFileName(Title))
            {
                case FileType.Image:
                    return FilterType.ImagesOnly;
                case FileType.Document:
                    return FilterType.DocumentsOnly;
                case FileType.Presentation:
                    return FilterType.PresentationsOnly;
                case FileType.Spreadsheet:
                    return FilterType.SpreadsheetsOnly;
                case FileType.Archive:
                    return FilterType.ArchiveOnly;
                case FileType.Audio:
                case FileType.Video:
                    return FilterType.MediaOnly;
            }

            return FilterType.None;
        }
    }

    public FileStatus FileStatus
    {
        get => FileHelper.GetFileStatus(this, ref _status);
        set => _status = value;
    }

    public override string UniqID => $"file_{ID}";

    [JsonIgnore]
    public override string Title => FileHelper.GetTitle(this);


    [JsonIgnore]
    public string DownloadUrl => FileHelper.GetDownloadUrl(this);

    public bool Locked { get; set; }
    public string LockedBy { get; set; }

    [JsonIgnore]
    public override bool IsNew
    {
        get => (_status & FileStatus.IsNew) == FileStatus.IsNew;
        set
        {
            if (value)
            {
                _status |= FileStatus.IsNew;
            }
            else
            {
                _status &= ~FileStatus.IsNew;
            }
        }
    }

    [JsonIgnore]
    public bool IsFavorite
    {
        get => (_status & FileStatus.IsFavorite) == FileStatus.IsFavorite;
        set
        {
            if (value)
            {
                _status |= FileStatus.IsFavorite;
            }
            else
            {
                _status &= ~FileStatus.IsFavorite;
            }
        }
    }

    [JsonIgnore]
    public bool IsTemplate
    {
        get => (_status & FileStatus.IsTemplate) == FileStatus.IsTemplate;
        set
        {
            if (value)
            {
                _status |= FileStatus.IsTemplate;
            }
            else
            {
                _status &= ~FileStatus.IsTemplate;
            }
        }
    }

    [JsonIgnore]
    public bool IsFillFormDraft
    {
        get => (_status & FileStatus.IsFillFormDraft) == FileStatus.IsFillFormDraft;
        set
        {
            if (value)
            {
                _status |= FileStatus.IsFillFormDraft;
            }
            else
            {
                _status &= ~FileStatus.IsFillFormDraft;
            }
        }
    }

    public bool Encrypted { get; set; }
    public Thumbnail ThumbnailStatus { get; set; }
    public ForcesaveType Forcesave { get; set; }
    public string ConvertedType { get; set; }

    [JsonIgnore]
    public string ConvertedExtension
    {
        get
        {
            if (string.IsNullOrEmpty(ConvertedType))
            {
                return FileUtility.GetFileExtension(Title);
            }

            var curFileType = FileUtility.GetFileTypeByFileName(Title);

            return curFileType switch
            {
                FileType.Image => ConvertedType.Trim('.') == "zip" ? ".pptt" : ConvertedType,
                FileType.Spreadsheet => ConvertedType.Trim('.') != "xlsx" ? ".xlst" : ConvertedType,
                FileType.Document => ConvertedType.Trim('.') == "zip" ? ".doct" : ConvertedType,
                _ => ConvertedType,
            };
        }
    }

    public object NativeAccessor { get; set; }
}
