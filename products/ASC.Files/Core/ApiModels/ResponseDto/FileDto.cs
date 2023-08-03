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


namespace ASC.Files.Core.ApiModels.ResponseDto;

/// <summary>
/// </summary>
public class FileDto<T> : FileEntryDto<T>
{
    /// <summary>Folder ID</summary>
    /// <type>System.Int32, System</type>
    public T FolderId { get; set; }

    /// <summary>Version</summary>
    /// <type>System.Int32, System</type>
    public int Version { get; set; }

    /// <summary>Version group</summary>
    /// <type>System.Int32, System</type>
    public int VersionGroup { get; set; }

    /// <summary>Content length</summary>
    /// <type>System.String, System</type>
    public string ContentLength { get; set; }

    /// <summary>Pure content length</summary>
    /// <type>System.Nullable{System.Int64}, System</type>
    public long? PureContentLength { get; set; }

    /// <summary>File status</summary>
    /// <type>ASC.Files.Core.FileStatus, ASC.Files.Core</type>
    public FileStatus FileStatus { get; set; }

    /// <summary>Muted or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Mute { get; set; }

    /// <summary>URL to view a file</summary>
    /// <type>System.String, System</type>
    public string ViewUrl { get; set; }

    /// <summary>Web URL</summary>
    /// <type>System.String, System</type>
    public string WebUrl { get; set; }

    /// <summary>File type</summary>
    /// <type>ASC.Web.Core.Files.FileType, ASC.Web.Core</type>
    public FileType FileType { get; set; }

    /// <summary>File extension</summary>
    /// <type>System.String, System</type>
    public string FileExst { get; set; }

    /// <summary>Comment</summary>
    /// <type>System.String, System</type>
    public string Comment { get; set; }

    /// <summary>Encrypted or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? Encrypted { get; set; }

    /// <summary>Thumbnail URL</summary>
    /// <type>System.String, System</type>
    public string ThumbnailUrl { get; set; }

    /// <summary>Thumbnail status</summary>
    /// <type>ASC.Files.Core.Thumbnail, ASC.Files.Core</type>
    public Thumbnail ThumbnailStatus { get; set; }

    /// <summary>Locked or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? Locked { get; set; }

    /// <summary>User ID who locked a file</summary>
    /// <type>System.String, System</type>
    public string LockedBy { get; set; }

    /// <summary>Denies file downloading or not</summary>
    /// <type>System.Boolean, System</type>
    public bool DenyDownload { get; set; }

    /// <summary>Denies file sharing or not</summary>
    /// <type>System.Boolean, System</type>
    public bool DenySharing { get; set; }

    /// <summary>File accessibility</summary>
    /// <type>System.Collections.IDictionary{ASC.Files.Core.Helpers.Accessability, System.Boolean}, System.Collections</type>
    public IDictionary<Accessability, bool> ViewAccessability { get; set; }

    protected internal override FileEntryType EntryType { get => FileEntryType.File; }

    public FileDto() { }

    public static FileDto<int> GetSample()
    {
        return new FileDto<int>
        {
            Access = FileShare.ReadWrite,
            //Updated = ApiDateTime.GetSample(),
            //Created = ApiDateTime.GetSample(),
            //CreatedBy = EmployeeWraper.GetSample(),
            Id = 10,
            RootFolderType = FolderType.BUNCH,
            Shared = false,
            Title = "Some titile.txt",
            FileExst = ".txt",
            FileType = FileType.Document,
            //UpdatedBy = EmployeeWraper.GetSample(),
            ContentLength = 12345.ToString(CultureInfo.InvariantCulture),
            FileStatus = FileStatus.IsNew,
            FolderId = 12334,
            Version = 3,
            VersionGroup = 1,
            ViewUrl = "http://www.onlyoffice.com/viewfile?fileid=2221"
        };
    }
}

[Scope]
public class FileDtoHelper : FileEntryDtoHelper
{
    private readonly AuthContext _authContext;
    private readonly IDaoFactory _daoFactory;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly BadgesSettingsHelper _badgesSettingsHelper;

    public FileDtoHelper(
        ApiDateTimeHelper apiDateTimeHelper,
        EmployeeDtoHelper employeeWrapperHelper,
        AuthContext authContext,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        GlobalFolderHelper globalFolderHelper,
        CommonLinkUtility commonLinkUtility,
        FilesLinkUtility filesLinkUtility,
        FileUtility fileUtility,
        FileSharingHelper fileSharingHelper,
        BadgesSettingsHelper badgesSettingsHelper)
        : base(apiDateTimeHelper, employeeWrapperHelper, fileSharingHelper, fileSecurity)
    {
        _authContext = authContext;
        _daoFactory = daoFactory;
        _globalFolderHelper = globalFolderHelper;
        _commonLinkUtility = commonLinkUtility;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _badgesSettingsHelper = badgesSettingsHelper;
    }

    public async Task<FileDto<T>> GetAsync<T>(File<T> file, List<Tuple<FileEntry<T>, bool>> folders = null)
    {
        var result = await GetFileWrapperAsync(file);

        result.FolderId = file.ParentId;
        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootCreateBy, _authContext.CurrentAccount.ID))
        {
            result.RootFolderType = FolderType.SHARE;
            var folderDao = _daoFactory.GetFolderDao<T>();
            FileEntry<T> parentFolder;

            if (folders != null)
            {
                var folderWithRight = folders.FirstOrDefault(f => f.Item1.Id.Equals(file.ParentId));
                if (folderWithRight == null || !folderWithRight.Item2)
                {
                    result.FolderId = await _globalFolderHelper.GetFolderShareAsync<T>();
                }
            }
            else
            {
                parentFolder = await folderDao.GetFolderAsync(file.ParentId);
                if (!await _fileSecurity.CanReadAsync(parentFolder))
                {
                    result.FolderId = await _globalFolderHelper.GetFolderShareAsync<T>();
                }
            }
        }

        result.ViewAccessability = _fileUtility.GetAccessability(file.Title);

        return result;
    }

    private async Task<FileDto<T>> GetFileWrapperAsync<T>(File<T> file)
    {
        var result = await GetAsync<FileDto<T>, T>(file);
        var isEnabledBadges = await _badgesSettingsHelper.GetEnabledForCurrentUserAsync();

        result.FileExst = FileUtility.GetFileExtension(file.Title);
        result.FileType = FileUtility.GetFileTypeByExtention(result.FileExst);
        result.Version = file.Version;
        result.VersionGroup = file.VersionGroup;
        result.ContentLength = file.ContentLengthString;
        result.FileStatus = file.FileStatus;
        result.Mute = !isEnabledBadges;
        result.PureContentLength = file.ContentLength.NullIfDefault();
        result.Comment = file.Comment;
        result.Encrypted = file.Encrypted.NullIfDefault();
        result.Locked = file.Locked.NullIfDefault();
        result.LockedBy = file.LockedBy;
        result.DenyDownload = file.DenyDownload;
        result.DenySharing = file.DenySharing;
        result.Access = file.Access;

        try
        {
            result.ViewUrl = _commonLinkUtility.GetFullAbsolutePath(file.DownloadUrl);

            result.WebUrl = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebPreviewUrl(_fileUtility, file.Title, file.Id, file.Version));

            result.ThumbnailStatus = file.ThumbnailStatus;

            var cacheKey = Math.Abs(result.Updated.GetHashCode());

            if (file.ThumbnailStatus == Thumbnail.Created)
            {
                result.ThumbnailUrl = _commonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileThumbnailUrl(file.Id, file.Version)) + $"&hash={cacheKey}"; 
            }
        }
        catch (Exception)
        {
            //Don't catch anything here because of httpcontext
        }

        return result;
    }
}
