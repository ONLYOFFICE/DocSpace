namespace ASC.Files.Core.Services.NotifyService;

public static class NotifyConstants
{
    #region Events

    public static readonly INotifyAction EventDocuSignComplete = new NotifyAction("DocuSignComplete", "docusign complete");
    public static readonly INotifyAction EventDocuSignStatus = new NotifyAction("DocuSignStatus", "docusign status");
    public static readonly INotifyAction EventMailMergeEnd = new NotifyAction("MailMergeEnd", "mail merge end");
    public static readonly INotifyAction EventShareDocument = new NotifyAction("ShareDocument", "share document");
    public static readonly INotifyAction EventShareEncryptedDocument = new NotifyAction("ShareEncryptedDocument", "share encrypted document");
    public static readonly INotifyAction EventShareFolder = new NotifyAction("ShareFolder", "share folder");
    public static readonly INotifyAction EventEditorMentions = new NotifyAction("EditorMentions", "editor mentions");

    #endregion

    #region  Tags

    public static readonly string TagFolderID = "FolderID";
    public static readonly string TagDocumentTitle = "DocumentTitle";
    public static readonly string TagDocumentUrl = "DocumentURL";
    public static readonly string TagAccessRights = "AccessRights";
    public static readonly string TagMessage = "Message";
    public static readonly string TagMailsCount = "MailsCount";

    #endregion
}
