using File = Microsoft.SharePoint.Client.File;
using Folder = Microsoft.SharePoint.Client.Folder;

namespace ASC.Files.Thirdparty.SharePoint;

public class SharePointFileErrorEntry : File
{
    public SharePointFileErrorEntry(ClientRuntimeContext cc, ObjectPath op)
        : base(cc, op)
    {
    }

    public string Error { get; set; }
    public object ID { get; set; }
}

public class SharePointFolderErrorEntry : Folder
{
    public SharePointFolderErrorEntry(ClientRuntimeContext cc, ObjectPath op)
        : base(cc, op)
    {
    }

    public string Error { get; set; }
    public object ID { get; set; }
}
