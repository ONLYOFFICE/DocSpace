// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Files.Thirdparty.Dropbox;

internal abstract class DropboxDaoBase : ThirdPartyProviderDao<DropboxProviderInfo>
{
    protected override string Id => "dropbox";

    protected DropboxDaoBase(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger monitor,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext)
    {
    }

    protected static string GetParentFolderPath(Metadata dropboxItem)
    {
        if (dropboxItem == null || IsRoot(dropboxItem.AsFolder))
        {
            return null;
        }

        var pathLength = dropboxItem.PathDisplay.Length - dropboxItem.Name.Length;

        return dropboxItem.PathDisplay.Substring(0, pathLength > 1 ? pathLength - 1 : 0);
    }

    protected static string MakeDropboxPath(object entryId)
    {
        return Convert.ToString(entryId, CultureInfo.InvariantCulture);
    }

    protected string MakeDropboxPath(Metadata dropboxItem)
    {
        string path = null;
        if (dropboxItem != null)
        {
            path = dropboxItem.PathDisplay;
        }

        return path;
    }

    protected string MakeId(Metadata dropboxItem)
    {
        return MakeId(MakeDropboxPath(dropboxItem));
    }

    protected override string MakeId(string path = null)
    {
        var p = string.IsNullOrEmpty(path) || path == "/" ? "" : ("-" + path.Replace('/', '|'));

        return $"{PathPrefix}{p}";
    }

    protected string MakeFolderTitle(FolderMetadata dropboxFolder)
    {
        if (dropboxFolder == null || IsRoot(dropboxFolder))
        {
            return ProviderInfo.CustomerTitle;
        }

        return Global.ReplaceInvalidCharsAndTruncate(dropboxFolder.Name);
    }

    protected string MakeFileTitle(FileMetadata dropboxFile)
    {
        if (dropboxFile == null || string.IsNullOrEmpty(dropboxFile.Name))
        {
            return ProviderInfo.ProviderKey;
        }

        return Global.ReplaceInvalidCharsAndTruncate(dropboxFile.Name);
    }

    protected Folder<string> ToFolder(FolderMetadata dropboxFolder)
    {
        if (dropboxFolder == null)
        {
            return null;
        }

        if (dropboxFolder is ErrorFolder)
        {
            //Return error entry
            return ToErrorFolder(dropboxFolder as ErrorFolder);
        }

        var isRoot = IsRoot(dropboxFolder);

        var folder = GetFolder();

        folder.Id = MakeId(dropboxFolder);
        folder.ParentId = isRoot ? null : MakeId(GetParentFolderPath(dropboxFolder));
        folder.CreateOn = isRoot ? ProviderInfo.CreateOn : default;
        folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : default;
        folder.Title = MakeFolderTitle(dropboxFolder);
        folder.Private = ProviderInfo.Private;
        folder.HasLogo = ProviderInfo.HasLogo;
        SetFolderType(folder, isRoot);

        if (folder.CreateOn != DateTime.MinValue && folder.CreateOn.Kind == DateTimeKind.Utc)
        {
            folder.CreateOn = _tenantUtil.DateTimeFromUtc(folder.CreateOn);
        }

        if (folder.ModifiedOn != DateTime.MinValue && folder.ModifiedOn.Kind == DateTimeKind.Utc)
        {
            folder.ModifiedOn = _tenantUtil.DateTimeFromUtc(folder.ModifiedOn);
        }

        return folder;
    }

    protected static bool IsRoot(FolderMetadata dropboxFolder)
    {
        return dropboxFolder != null && dropboxFolder.Id == "/";
    }

    private File<string> ToErrorFile(ErrorFile dropboxFile)
    {
        if (dropboxFile == null)
        {
            return null;
        }

        var file = GetErrorFile(new ErrorEntry(dropboxFile.ErrorId, dropboxFile.Error));

        file.Title = MakeFileTitle(dropboxFile);

        return file;
    }

    private Folder<string> ToErrorFolder(ErrorFolder dropboxFolder)
    {
        if (dropboxFolder == null)
        {
            return null;
        }

        var folder = GetErrorFolder(new ErrorEntry(dropboxFolder.Error, dropboxFolder.ErrorId));

        folder.Title = MakeFolderTitle(dropboxFolder);

        return folder;
    }

    public File<string> ToFile(FileMetadata dropboxFile)
    {
        if (dropboxFile == null)
        {
            return null;
        }

        if (dropboxFile is ErrorFile)
        {
            //Return error entry
            return ToErrorFile(dropboxFile as ErrorFile);
        }

        var file = GetFile();

        file.Id = MakeId(dropboxFile);
        file.ContentLength = (long)dropboxFile.Size;
        file.CreateOn = _tenantUtil.DateTimeFromUtc(dropboxFile.ServerModified);
        file.ParentId = MakeId(GetParentFolderPath(dropboxFile));
        file.ModifiedOn = _tenantUtil.DateTimeFromUtc(dropboxFile.ServerModified);
        file.NativeAccessor = dropboxFile;
        file.Title = MakeFileTitle(dropboxFile);
        file.ThumbnailStatus = Thumbnail.Created;
        file.Encrypted = ProviderInfo.Private;

        return file;
    }

    public async Task<Folder<string>> GetRootFolderAsync(string folderId)
    {
        return ToFolder(await GetDropboxFolderAsync(string.Empty));
    }

    protected async Task<FolderMetadata> GetDropboxFolderAsync(string folderId)
    {
        var dropboxFolderPath = MakeDropboxPath(folderId);
        try
        {
            var folder = await ProviderInfo.GetDropboxFolderAsync(dropboxFolderPath);
            return folder;
        }
        catch (Exception ex)
        {
            return new ErrorFolder(ex, dropboxFolderPath);
        }
    }

    protected Task<FileMetadata> GetDropboxFileAsync(object fileId)
    {
        var dropboxFilePath = MakeDropboxPath(fileId);
        try
        {
            return ProviderInfo.GetDropboxFileAsync(dropboxFilePath);
        }
        catch (Exception ex)
        {
            return Task.FromResult<FileMetadata>(new ErrorFile(ex, dropboxFilePath));
        }
    }

    protected override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var items = await GetDropboxItemsAsync(folderId);

        return items.Select(MakeId);
    }

    protected async Task<List<Metadata>> GetDropboxItemsAsync(object parentId, bool? folder = null)
    {
        var dropboxFolderPath = MakeDropboxPath(parentId);
        var items = await ProviderInfo.GetDropboxItemsAsync(dropboxFolderPath);

        if (folder.HasValue)
        {
            if (folder.Value)
            {
                return items.Where(i => i.AsFolder != null).ToList();
            }

            return items.Where(i => i.AsFile != null).ToList();
        }

        return items;
    }

    protected sealed class ErrorFolder : FolderMetadata
    {
        public string Error { get; set; }
        public string ErrorId { get; private set; }

        public ErrorFolder(Exception e, object id)
        {
            ErrorId = id.ToString();
            if (e != null)
            {
                Error = e.Message;
            }
        }
    }

    protected sealed class ErrorFile : FileMetadata
    {
        public string Error { get; set; }
        public string ErrorId { get; private set; }

        public ErrorFile(Exception e, object id)
        {
            ErrorId = id.ToString();
            if (e != null)
            {
                Error = e.Message;
            }
        }
    }

    protected string GetAvailableTitle(string requestTitle, string parentFolderPath, Func<string, string, bool> isExist)
    {
        if (!isExist(requestTitle, parentFolderPath))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf('.') != -1)
            {
                insertIndex = requestTitle.LastIndexOf('.');
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (isExist(requestTitle, parentFolderPath))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    protected async Task<string> GetAvailableTitleAsync(string requestTitle, string parentFolderPath, Func<string, string, Task<bool>> isExist)
    {
        if (!await isExist(requestTitle, parentFolderPath))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                insertIndex = requestTitle.LastIndexOf(".", StringComparison.InvariantCulture);
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (await isExist(requestTitle, parentFolderPath))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    private string MatchEvaluator(Match match)
    {
        var index = Convert.ToInt32(match.Groups[2].Value);
        var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);

        return string.Format(" ({0}){1}", index + 1, staticText);
    }
}
