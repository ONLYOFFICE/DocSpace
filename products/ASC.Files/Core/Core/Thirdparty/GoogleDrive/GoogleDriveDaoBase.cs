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

using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace ASC.Files.Thirdparty.GoogleDrive;

[Scope]
internal class GoogleDriveDaoBase : ThirdPartyProviderDao<DriveFile, DriveFile, DriveFile>, IDaoBase<DriveFile, DriveFile, DriveFile>
{
    private GoogleDriveProviderInfo _providerInfo;
    public GoogleDriveDaoBase(IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil, 
        IDbContextFactory<FilesDbContext> dbContextFactory, 
        SetupInfo setupInfo,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext,
        RegexDaoSelectorBase<DriveFile, DriveFile, DriveFile> regexDaoSelectorBase) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextFactory, setupInfo, fileUtility, tempPath, authContext, regexDaoSelectorBase)
    {
    }

    public void Init(string pathPrefix, IProviderInfo<DriveFile, DriveFile, DriveFile> providerInfo)
    {
        PathPrefix = pathPrefix;
        ProviderInfo = providerInfo;
        _providerInfo = providerInfo as GoogleDriveProviderInfo;
    }

    public string GetName(DriveFile item)
    {
        return item.Name;
    }

    public string GetId(DriveFile item)
    {
        return item.Id;
    }

    public string MakeThirdId(object entryId)
    {
        var id = Convert.ToString(entryId, CultureInfo.InvariantCulture);

        return string.IsNullOrEmpty(id)
                   ? "root"
                   : id.TrimStart('/');
    }

    public string GetParentFolderId(DriveFile driveEntry)
    {
        return driveEntry == null || driveEntry.Parents == null || driveEntry.Parents.Count == 0
                   ? null
                   : driveEntry.Parents[0];
    }

    public string MakeId(DriveFile driveEntry)
    {
        var path = string.Empty;
        if (driveEntry != null)
        {
            path = IsRoot(driveEntry) ? "root" : driveEntry.Id;
        }

        return MakeId(path);
    }

    public override string MakeId(string path = null)
    {
        var p = string.IsNullOrEmpty(path) || path == "root" || path == ProviderInfo.RootFolderId ? "" : ("-|" + path.TrimStart('/'));

        return $"{PathPrefix}{p}";
    }

    public string MakeFolderTitle(DriveFile driveFolder)
    {
        if (driveFolder == null || IsRoot(driveFolder))
        {
            return ProviderInfo.CustomerTitle;
        }

        return Global.ReplaceInvalidCharsAndTruncate(driveFolder.Name);
    }

    public string MakeFileTitle(DriveFile driveFile)
    {
        if (driveFile == null || string.IsNullOrEmpty(driveFile.Name))
        {
            return ProviderInfo.ProviderKey;
        }

        var title = driveFile.Name;

        var gExt = MimeMapping.GetExtention(driveFile.MimeType);
        if (GoogleLoginProvider.GoogleDriveExt.Contains(gExt))
        {
            var downloadableExtension = _fileUtility.GetGoogleDownloadableExtension(gExt);
            if (!downloadableExtension.Equals(FileUtility.GetFileExtension(title)))
            {
                title += downloadableExtension;
            }
        }

        return Global.ReplaceInvalidCharsAndTruncate(title);
    }

    public Folder<string> ToFolder(DriveFile driveEntry)
    {
        if (driveEntry == null)
        {
            return null;
        }

        if (driveEntry is ErrorDriveEntry)
        {
            //Return error entry
            return ToErrorFolder(driveEntry as ErrorDriveEntry);
        }

        if (driveEntry.MimeType != GoogleLoginProvider.GoogleDriveMimeTypeFolder)
        {
            return null;
        }

        var isRoot = IsRoot(driveEntry);

        var folder = GetFolder();

        folder.Id = MakeId(driveEntry);
        folder.ParentId = isRoot ? null : MakeId(GetParentFolderId(driveEntry));
        folder.CreateOn = isRoot ? ProviderInfo.CreateOn : (driveEntry.CreatedTime ?? default);
        folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : (driveEntry.ModifiedTime ?? default);
        folder.Private = ProviderInfo.Private;
        folder.HasLogo = ProviderInfo.HasLogo;
        SetFolderType(folder, isRoot);

        folder.Title = MakeFolderTitle(driveEntry);

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

    public bool IsRoot(DriveFile driveFolder)
    {
        return IsDriveFolder(driveFolder) && GetParentFolderId(driveFolder) == null;
    }

    private bool IsDriveFolder(DriveFile driveFolder)
    {
        return driveFolder != null && driveFolder.MimeType == GoogleLoginProvider.GoogleDriveMimeTypeFolder;
    }

    private File<string> ToErrorFile(ErrorDriveEntry driveEntry)
    {
        if (driveEntry == null)
        {
            return null;
        }

        var file = GetErrorFile(new ErrorEntry(driveEntry.Error, driveEntry.ErrorId));

        file.Title = MakeFileTitle(driveEntry);

        return file;
    }

    private Folder<string> ToErrorFolder(ErrorDriveEntry driveEntry)
    {
        if (driveEntry == null)
        {
            return null;
        }

        var folder = GetErrorFolder(new ErrorEntry(driveEntry.Error, driveEntry.ErrorId));

        folder.Title = MakeFolderTitle(driveEntry);

        return folder;
    }

    public File<string> ToFile(DriveFile driveFile)
    {
        if (driveFile == null)
        {
            return null;
        }

        if (driveFile is ErrorDriveEntry)
        {
            //Return error entry
            return ToErrorFile(driveFile as ErrorDriveEntry);
        }

        var file = GetFile();

        file.Id = MakeId(driveFile.Id);
        file.ContentLength = driveFile.Size.HasValue ? (long)driveFile.Size : 0;
        file.CreateOn = driveFile.CreatedTime.HasValue ? _tenantUtil.DateTimeFromUtc(driveFile.CreatedTime.Value) : default;
        file.ParentId = MakeId(GetParentFolderId(driveFile));
        file.ModifiedOn = driveFile.ModifiedTime.HasValue ? _tenantUtil.DateTimeFromUtc(driveFile.ModifiedTime.Value) : default;
        file.NativeAccessor = driveFile;
        file.Title = MakeFileTitle(driveFile);
        file.ThumbnailStatus = Thumbnail.Created;
        file.Encrypted = ProviderInfo.Private;

        return file;
    }

    public async Task<Folder<string>> GetRootFolderAsync()
    {
        return ToFolder(await GetFolderAsync(""));
    }

    public async Task<DriveFile> GetFileAsync(string entryId)
    {
        var driveId = MakeThirdId(entryId);
        try
        {
            var entry = await _providerInfo.GetFileAsync(driveId);

            return entry;
        }
        catch (Exception ex)
        {
            return new ErrorDriveEntry(ex, driveId);
        }
    }

    public async Task<DriveFile> GetFolderAsync(string entryId)
    {
        var driveId = MakeThirdId(entryId);
        try
        {
            var entry = await _providerInfo.GetFolderAsync(driveId);

            return entry;
        }
        catch (Exception ex)
        {
            return new ErrorDriveEntry(ex, driveId);
        }
    }

    public override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var entries = await GetItemsAsync(folderId);

        return entries.Select(entry => MakeId(entry.Id));
    }

    public async Task<List<DriveFile>> GetItemsAsync(string parentId, bool? folder = null)
    {
        var parentDriveId = MakeThirdId(parentId);
        if (folder == null)
        {
            return await _providerInfo.GetItemsAsync(parentDriveId);
        }
        else
        {
            return await _providerInfo.GetItemsAsync(parentDriveId, folder);
        }
    }

    public Task<string> GetAvailableTitleAsync(string requestTitle, string parentFolderId, Func<string, string, Task<bool>> isExist)
    {
        return Task.FromResult(requestTitle);
    }

    protected sealed class ErrorDriveEntry : DriveFile, IErrorItem
    {
        public string Error { get; set; }
        public string ErrorId { get; private set; }

        public ErrorDriveEntry(Exception e, object id)
        {
            ErrorId = id.ToString();
            if (id.ToString() == "root")
            {
                MimeType = GoogleLoginProvider.GoogleDriveMimeTypeFolder;
            }
            if (e != null)
            {
                Error = e.Message;
            }
        }
    }
}
