namespace ASC.Files.Core;

[DebuggerDisplay("{Id} into {FolderId}")]
[Serializable]
public class ChunkedUploadSession<T> : CommonChunkedUploadSession
{
    public T FolderId { get; set; }
    public File<T> File { get; set; }
    public bool Encrypted { get; set; }

    //hack for Backup bug 48873
    [NonSerialized]
    public bool CheckQuota = true;

    public ChunkedUploadSession(File<T> file, long bytesTotal) : base(bytesTotal)
    {
        File = file;
    }

    public override object Clone()
    {
        var clone = (ChunkedUploadSession<T>)MemberwiseClone();
        clone.File = (File<T>)File.Clone();

        return clone;
    }

    public override Stream Serialize()
    {
        var str = JsonSerializer.Serialize(this);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));

        return stream;
    }

    public static ChunkedUploadSession<T> Deserialize(Stream stream, FileHelper fileHelper)
    {
        var chunkedUploadSession = JsonSerializer.Deserialize<ChunkedUploadSession<T>>(stream);
        chunkedUploadSession.File.FileHelper = fileHelper;
        chunkedUploadSession.TransformItems();

        return chunkedUploadSession;

    }
}

[Scope]
public class ChunkedUploadSessionHelper
{
    public ILog Logger { get; }
    private readonly EntryManager _entryManager;

    public ChunkedUploadSessionHelper(IOptionsMonitor<ILog> options, EntryManager entryManager)
    {
        _entryManager = entryManager;
        Logger = options.CurrentValue;
    }

    public async Task<object> ToResponseObjectAsync<T>(ChunkedUploadSession<T> session, bool appendBreadCrumbs = false)
    {
        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(session.FolderId); //todo: check how?
        var pathFolder = appendBreadCrumbs
            ? breadCrumbs.Select(f =>
            {
                if (f == null)
                {
                    Logger.ErrorFormat("GetBreadCrumbs {0} with null", session.FolderId);

                    return default;
                }

                if (f is Folder<string> f1)
                {
                    return (T)Convert.ChangeType(f1.ID, typeof(T));
                }

                if (f is Folder<int> f2)
                {
                    return (T)Convert.ChangeType(f2.ID, typeof(T));
                }

                return (T)Convert.ChangeType(0, typeof(T));
            })
            : new List<T> { session.FolderId };

        return new
        {
            id = session.Id,
            path = pathFolder,
            created = session.Created,
            expired = session.Expired,
            location = session.Location,
            bytes_uploaded = session.BytesUploaded,
            bytes_total = session.BytesTotal
        };
    }
}
