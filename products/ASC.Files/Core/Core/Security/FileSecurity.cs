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

namespace ASC.Files.Core.Security;

[Scope]
public class FileSecurityCommon
{
    private readonly UserManager _userManager;
    private readonly WebItemSecurity _webItemSecurity;

    public FileSecurityCommon(UserManager userManager, WebItemSecurity webItemSecurity)
    {
        _userManager = userManager;
        _webItemSecurity = webItemSecurity;
    }

    public bool IsAdministrator(Guid userId)
    {
        return _userManager.IsUserInGroup(userId, Constants.GroupAdmin.ID) ||
               _webItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, userId);
    }
}

[Scope]
public class FileSecurity : IFileSecurity
{
    private readonly IDaoFactory _daoFactory;

    public FileShare DefaultMyShare => FileShare.Restrict;
    public FileShare DefaultProjectsShare => FileShare.ReadWrite;
    public FileShare DefaultCommonShare => FileShare.Read;
    public FileShare DefaultPrivacyShare => FileShare.Restrict;

    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly AuthManager _authManager;
    private readonly GlobalFolder _globalFolder;
    private readonly FileSecurityCommon _fileSecurityCommon;
    private readonly FilesSettingsHelper _filesSettingsHelper;

    public FileSecurity(
        IDaoFactory daoFactory,
        UserManager userManager,
        TenantManager tenantManager,
        AuthContext authContext,
        AuthManager authManager,
        GlobalFolder globalFolder,
        FileSecurityCommon fileSecurityCommon,
        FilesSettingsHelper filesSettingsHelper)
    {
        _daoFactory = daoFactory;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _authManager = authManager;
        _globalFolder = globalFolder;
        _fileSecurityCommon = fileSecurityCommon;
        _filesSettingsHelper = filesSettingsHelper;
    }

    public Task<List<Tuple<FileEntry<T>, bool>>> CanReadAsync<T>(IEnumerable<FileEntry<T>> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Read);
    }

    public Task<List<Tuple<FileEntry<T>, bool>>> CanReadAsync<T>(IEnumerable<FileEntry<T>> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Read);
    }

    public Task<bool> CanReadAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Read);
    }

    public Task<bool> CanCommentAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Comment);
    }

    public Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.FillForms);
    }

    public Task<bool> CanReviewAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Review);
    }

    public Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.CustomFilter);
    }

    public Task<bool> CanCreateAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Create);
    }

    public Task<bool> CanEditAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Edit);
    }

    public Task<bool> CanDeleteAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Delete);
    }

    public Task<bool> CanEditRoomAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.RoomEdit);
    }

    public Task<bool> CanRenameAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Rename);
    }

    public Task<bool> CanDownloadAsync<T>(FileEntry<T> entry)
    {
        return CanDownloadAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanDownloadAsync<T>(FileEntry<T> entry, Guid userId)
    {
        if (!await CanReadAsync(entry, userId))
        {
            return false;
        }

        return CheckDenyDownload(entry);
    }

    public async Task<bool> CanShareAsync<T>(FileEntry<T> entry, Guid userId)
    {
        if (!await CanEditAsync(entry, userId))
        {
            return false;
        }

        return CheckDenySharing(entry);
    }

    public Task<bool> CanReadAsync<T>(FileEntry<T> entry)
    {
        return CanReadAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanCommentAsync<T>(FileEntry<T> entry)
    {
        return CanCommentAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry)
    {
        return CanCustomFilterEditAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry)
    {
        return CanFillFormsAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanReviewAsync<T>(FileEntry<T> entry)
    {
        return CanReviewAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanCreateAsync<T>(FileEntry<T> entry)
    {
        return CanCreateAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanEditAsync<T>(FileEntry<T> entry)
    {
        return CanEditAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanRenameAsync<T>(FileEntry<T> entry)
    {
        return CanRenameAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanDeleteAsync<T>(FileEntry<T> entry)
    {
        return CanDeleteAsync(entry, _authContext.CurrentAccount.ID);
    }
    public Task<bool> CanDownload<T>(FileEntry<T> entry)
    {
        return CanDownloadAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanEditRoomAsync<T>(FileEntry<T> entry)
    {
        return CanEditRoomAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanShare<T>(FileEntry<T> entry)
    {
        return CanShareAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<IEnumerable<Guid>> WhoCanReadAsync<T>(FileEntry<T> entry)
    {
        return WhoCanAsync(entry, FilesSecurityActions.Read);
    }

    private async Task<IEnumerable<Guid>> WhoCanAsync<T>(FileEntry<T> entry, FilesSecurityActions action)
    {
        var shares = await GetSharesAsync(entry);

        FileShareRecord defaultShareRecord;

        switch (entry.RootFolderType)
        {
            case FolderType.COMMON:
                defaultShareRecord = new FileShareRecord
                {
                    Level = int.MaxValue,
                    EntryId = entry.Id,
                    EntryType = entry.FileEntryType,
                    Share = DefaultCommonShare,
                    Subject = Constants.GroupEveryone.ID,
                    TenantId = _tenantManager.GetCurrentTenant().Id,
                    Owner = _authContext.CurrentAccount.ID
                };

                if (!shares.Any())
                {
                    if ((defaultShareRecord.Share == FileShare.Read && action == FilesSecurityActions.Read) ||
                        (defaultShareRecord.Share == FileShare.ReadWrite))
                    {
                        return _userManager.GetUsersByGroup(defaultShareRecord.Subject)
                                          .Where(x => x.Status == EmployeeStatus.Active).Select(y => y.Id).Distinct();
                    }

                    return Enumerable.Empty<Guid>();
                }

                break;

            case FolderType.USER:
                defaultShareRecord = new FileShareRecord
                {
                    Level = int.MaxValue,
                    EntryId = entry.Id,
                    EntryType = entry.FileEntryType,
                    Share = DefaultMyShare,
                    Subject = entry.RootCreateBy,
                    TenantId = _tenantManager.GetCurrentTenant().Id,
                    Owner = entry.RootCreateBy
                };

                if (!shares.Any())
                {
                    return new List<Guid>
                        {
                            entry.RootCreateBy
                        };
                }

                break;

            case FolderType.Privacy:
                defaultShareRecord = new FileShareRecord
                {
                    Level = int.MaxValue,
                    EntryId = entry.Id,
                    EntryType = entry.FileEntryType,
                    Share = DefaultPrivacyShare,
                    Subject = entry.RootCreateBy,
                    TenantId = _tenantManager.GetCurrentTenant().Id,
                    Owner = entry.RootCreateBy
                };

                if (!shares.Any())
                {
                    return new List<Guid>
                        {
                            entry.RootCreateBy
                        };
                }

                break;

            case FolderType.BUNCH:
                if (action == FilesSecurityActions.Read)
                {
                    var folderDao = _daoFactory.GetFolderDao<T>();
                    var root = await folderDao.GetFolderAsync(entry.RootId);
                    if (root != null)
                    {
                        var path = await folderDao.GetBunchObjectIDAsync(root.Id);

                        var adapter = FilesIntegration.GetFileSecurity(path);

                        if (adapter != null)
                        {
                            return await adapter.WhoCanReadAsync(entry);
                        }
                    }
                }

                // TODO: For Projects and other
                defaultShareRecord = null;
                break;

            case FolderType.VirtualRooms:
                defaultShareRecord = new FileShareRecord
                {
                    Level = int.MaxValue,
                    EntryId = entry.Id,
                    EntryType = entry.FileEntryType,
                    Share = FileShare.Read,
                    Subject = WebItemManager.DocumentsProductID,
                    TenantId = _tenantManager.GetCurrentTenant().Id,
                    Owner = entry.RootCreateBy
                };

                if (!shares.Any())
                {
                    if ((defaultShareRecord.Share == FileShare.Read && action == FilesSecurityActions.Read) ||
                        (defaultShareRecord.Share == FileShare.ReadWrite))
                    {
                        return _userManager.GetUsersByGroup(defaultShareRecord.Subject)
                                          .Where(x => x.Status == EmployeeStatus.Active).Select(y => y.Id).Distinct();
                    }

                    return Enumerable.Empty<Guid>();
                }

                break;

            default:
                defaultShareRecord = null;
                break;
        }

        if (defaultShareRecord != null)
        {
            shares = shares.Concat(new[] { defaultShareRecord });
        }

        var manyShares = shares.SelectMany(x =>
        {
            var groupInfo = _userManager.GetGroupInfo(x.Subject);

            if (groupInfo.ID != Constants.LostGroupInfo.ID)
            {
                return _userManager.GetUsersByGroup(groupInfo.ID)
                .Where(p => p.Status == EmployeeStatus.Active)
                .Select(y => y.Id);
            }

            return new[] { x.Subject };
        })
            .Distinct();

        var result = new List<Guid>();

        foreach (var x in manyShares)
        {
            if (await CanAsync(entry, x, action))
            {
                result.Add(x);
            }
        }

        return result;
    }

    public async Task<IEnumerable<File<T>>> FilterReadAsync<T>(IEnumerable<File<T>> entries)
    {
        return (await FilterAsync(entries, FilesSecurityActions.Read, _authContext.CurrentAccount.ID)).Cast<File<T>>();
    }

    public async Task<IEnumerable<Folder<T>>> FilterReadAsync<T>(IEnumerable<Folder<T>> entries)
    {
        return (await FilterAsync(entries, FilesSecurityActions.Read, _authContext.CurrentAccount.ID)).Cast<Folder<T>>();
    }

    public async Task<IEnumerable<File<T>>> FilterEditAsync<T>(IEnumerable<File<T>> entries)
    {
        return (await FilterAsync(entries.Cast<FileEntry<T>>(), FilesSecurityActions.Edit, _authContext.CurrentAccount.ID)).Cast<File<T>>();
    }

    public async Task<IEnumerable<Folder<T>>> FilterEditAsync<T>(IEnumerable<Folder<T>> entries)
    {
        return (await FilterAsync(entries.Cast<FileEntry<T>>(), FilesSecurityActions.Edit, _authContext.CurrentAccount.ID)).Cast<Folder<T>>();
    }

    private async Task<bool> CanAsync<T>(FileEntry<T> entry, Guid userId, FilesSecurityActions action, IEnumerable<FileShareRecord> shares = null)
    {
        return (await FilterAsync(new[] { entry }, action, userId, shares)).Any();
    }

    public async Task<IEnumerable<File<T>>> FilterDownloadAsync<T>(IEnumerable<File<T>> entries)
    {
        return (await FilterReadAsync(entries)).Where(CheckDenyDownload).ToList();
    }

    public async Task<IEnumerable<Folder<T>>> FilterDownloadAsync<T>(IEnumerable<Folder<T>> entries)
    {
        return (await FilterReadAsync(entries)).Where(CheckDenyDownload).ToList();
    }

    private bool CheckDenyDownload<T>(FileEntry<T> entry)
    {
        return entry.DenyDownload
            ? entry.Access != FileShare.Read && entry.Access != FileShare.Comment
            : true;
    }

    public async Task<IEnumerable<File<T>>> FilterSharingAsync<T>(IEnumerable<File<T>> entries)
    {
        return (await FilterEditAsync(entries)).Where(CheckDenySharing).ToList();
    }

    private bool CheckDenySharing<T>(FileEntry<T> entry)
    {
        return entry.DenySharing
            ? entry.Access != FileShare.ReadWrite
            : true;
    }

    private async Task<List<Tuple<FileEntry<T>, bool>>> CanAsync<T>(IEnumerable<FileEntry<T>> entry, Guid userId, FilesSecurityActions action)
    {
        var filtres = await FilterAsync(entry, action, userId);

        return entry.Select(r => new Tuple<FileEntry<T>, bool>(r, filtres.Any(a => a.Id.Equals(r.Id)))).ToList();
    }

    private Task<IEnumerable<FileEntry<T>>> FilterAsync<T>(IEnumerable<FileEntry<T>> entries, FilesSecurityActions action, Guid userId, IEnumerable<FileShareRecord> shares = null)
    {
        if (entries == null || !entries.Any())
        {
            return Task.FromResult(Enumerable.Empty<FileEntry<T>>());
        }

        var user = _userManager.GetUsers(userId);
        var isOutsider = user.IsOutsider(_userManager);

        if (isOutsider && action != FilesSecurityActions.Read)
        {
            return Task.FromResult(Enumerable.Empty<FileEntry<T>>());
        }

        return InternalFilterAsync(entries, action, userId, shares, user, isOutsider);
    }

    private async Task<IEnumerable<FileEntry<T>>> InternalFilterAsync<T>(IEnumerable<FileEntry<T>> entries, FilesSecurityActions action, Guid userId, IEnumerable<FileShareRecord> shares, UserInfo user, bool isOutsider)
    {
        entries = entries.Where(f => f != null).ToList();
        var result = new List<FileEntry<T>>(entries.Count());

        // save entries order
        var order = entries.Select((f, i) => new { Id = f.UniqID, Pos = i }).ToDictionary(e => e.Id, e => e.Pos);

        // common or my files
        Func<FileEntry<T>, bool> filter =
            f => f.RootFolderType == FolderType.COMMON ||
                 f.RootFolderType == FolderType.USER ||
                 f.RootFolderType == FolderType.SHARE ||
                 f.RootFolderType == FolderType.Recent ||
                 f.RootFolderType == FolderType.Favorites ||
                 f.RootFolderType == FolderType.Templates ||
                 f.RootFolderType == FolderType.Privacy ||
                 f.RootFolderType == FolderType.Projects ||
                 f.RootFolderType == FolderType.VirtualRooms ||
                 f.RootFolderType == FolderType.Archive;

        var isVisitor = user.IsVisitor(_userManager);

        if (entries.Any(filter))
        {
            List<Guid> subjects = null;
            foreach (var e in entries.Where(filter))
            {
                if (!_authManager.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId).IsAuthenticated && userId != FileConstant.ShareLinkId)
                {
                    continue;
                }

                if (isOutsider && (e.RootFolderType == FolderType.USER
                                   || e.RootFolderType == FolderType.SHARE
                                   || e.RootFolderType == FolderType.Privacy))
                {
                    continue;
                }

                if (isVisitor && e.RootFolderType == FolderType.Recent)
                {
                    continue;
                }

                if (isVisitor && e.RootFolderType == FolderType.Favorites)
                {
                    continue;
                }

                if (isVisitor && e.RootFolderType == FolderType.Templates)
                {
                    continue;
                }

                if (isVisitor && e.RootFolderType == FolderType.Privacy)
                {
                    continue;
                }

                var folder = e as Folder<T>;
                var file = e as File<T>;

                if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder)
                {
                    if (folder == null)
                    {
                        continue;
                    }

                    if (folder.FolderType == FolderType.Projects)
                    {
                        // Root Projects folder read-only
                        continue;
                    }

                    if (folder.FolderType == FolderType.SHARE)
                    {
                        // Root Share folder read-only
                        continue;
                    }

                    if (folder.FolderType == FolderType.Recent)
                    {
                        // Recent folder read-only
                        continue;
                    }

                    if (folder.FolderType == FolderType.Favorites)
                    {
                        // Favorites folder read-only
                        continue;
                    }

                    if (folder.FolderType == FolderType.Templates)
                    {
                        // Templates folder read-only
                        continue;
                    }

                    if (folder.FolderType == FolderType.Archive)
                    {
                        continue;
                    }
                }

                if (isVisitor && e.ProviderEntry)
                {
                    continue;
                }

                //if (e.FileEntryType == FileEntryType.File
                //    && file.IsFillFormDraft)
                //{
                //    e.Access = FileShare.FillForms;

                //    if (action != FilesSecurityActions.Read
                //        && action != FilesSecurityActions.FillForms
                //        && action != FilesSecurityActions.Delete)
                //    {
                //        continue;
                //    }
                //}

                if (e.RootFolderType == FolderType.USER && e.RootCreateBy == userId && !isVisitor)
                {
                    // user has all right in his folder
                    result.Add(e);
                    continue;
                }

                if (e.RootFolderType == FolderType.Privacy && e.RootCreateBy == userId && !isVisitor)
                {
                    // user has all right in his privacy folder
                    result.Add(e);
                    continue;
                }

                if (e.FileEntryType == FileEntryType.Folder)
                {
                    if (folder == null)
                    {
                        continue;
                    }

                    if (DefaultCommonShare == FileShare.Read && action == FilesSecurityActions.Read && folder.FolderType == FolderType.COMMON)
                    {
                        // all can read Common folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.SHARE)
                    {
                        // all can read Share folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Recent)
                    {
                        // all can read recent folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Favorites)
                    {
                        // all can read favorites folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Templates)
                    {
                        // all can read templates folder
                        result.Add(e);
                        continue;
                    }
                }

                if (e.RootFolderType == FolderType.COMMON && _fileSecurityCommon.IsAdministrator(userId))
                {
                    // administrator in Common has all right
                    result.Add(e);
                    continue;
                }

                if (e.RootFolderType == FolderType.VirtualRooms && _fileSecurityCommon.IsAdministrator(userId))
                {
                    // administrator in VirtualRooms has all right
                    result.Add(e);
                    continue;
                }

                if (action == FilesSecurityActions.Delete && e.RootFolderType == FolderType.Archive && _fileSecurityCommon.IsAdministrator(userId))
                {
                    result.Add(e);
                    continue;
                }

                if (subjects == null)
                {
                    subjects = GetUserSubjects(userId);
                    if (shares == null)
                    {
                        shares = await GetSharesAsync(entries);
                        // shares ordered by level
                    }
                    shares = shares
                        .Join(subjects, r => r.Subject, s => s, (r, s) => r)
                        .ToList();
                }

                FileShareRecord ace;
                if (e.FileEntryType == FileEntryType.File)
                {
                    ace = shares
                        .OrderBy(r => r, new SubjectComparer(subjects))
                        .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                        .FirstOrDefault(r => Equals(r.EntryId, e.Id) && r.EntryType == FileEntryType.File);
                    if (ace == null)
                    {
                        // share on parent folders
                        ace = shares.Where(r => Equals(r.EntryId, file.ParentId) && r.EntryType == FileEntryType.Folder)
                                    .OrderBy(r => r, new SubjectComparer(subjects))
                                    .ThenBy(r => r.Level)
                                    .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                                    .FirstOrDefault();
                    }
                }
                else
                {
                    ace = shares.Where(r => Equals(r.EntryId, e.Id) && r.EntryType == FileEntryType.Folder)
                                .OrderBy(r => r, new SubjectComparer(subjects))
                                .ThenBy(r => r.Level)
                                .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                                .FirstOrDefault();
                }

                var defaultShare = userId == FileConstant.ShareLinkId
                        ? FileShare.Restrict
                        : e.RootFolderType == FolderType.USER
                        ? DefaultMyShare
                    : e.RootFolderType == FolderType.Privacy
                        ? DefaultPrivacyShare
                        : DefaultCommonShare;

                e.Access = ace != null ? ace.Share : defaultShare;

                if (action == FilesSecurityActions.Read && e.Access != FileShare.Restrict)
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Comment && (e.Access == FileShare.Comment || e.Access == FileShare.Review || e.Access == FileShare.CustomFilter || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.FillForms && (e.Access == FileShare.FillForms || e.Access == FileShare.Review || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Review && (e.Access == FileShare.Review || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.CustomFilter && (e.Access == FileShare.CustomFilter || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Edit && (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Rename && (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.RoomEdit && e.Access == FileShare.RoomManager)
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Create && (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager))
                {
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Delete && (e.Access == FileShare.RoomManager || e.Access == FileShare.ReadWrite))
                {
                    if (file != null && (file.RootFolderType == FolderType.VirtualRooms || file.RootFolderType == FolderType.Archive))
                    {
                        result.Add(e);
                    }
                    else if (folder != null && (folder.RootFolderType == FolderType.VirtualRooms || folder.RootFolderType == FolderType.Archive) &&
                        folder.FolderType == FolderType.DEFAULT)
                    {
                        result.Add(e);
                    }
                }
                else if (e.Access != FileShare.Restrict && e.CreateBy == userId && (e.FileEntryType == FileEntryType.File || folder.FolderType != FolderType.COMMON))
                {
                    result.Add(e);
                }

                if (e.CreateBy == userId)
                {
                    e.Access = FileShare.None; //HACK: for client
                }
            }
        }

        // files in bunch
        filter = f => f.RootFolderType == FolderType.BUNCH;
        if (entries.Any(filter))
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var filteredEntries = entries.Where(filter).ToList();
            var roots = filteredEntries
                    .Select(r => r.RootId)
                    .ToList();

            var rootsFolders = folderDao.GetFoldersAsync(roots);
            var bunches = await folderDao.GetBunchObjectIDsAsync(await rootsFolders.Select(r => r.Id).ToListAsync());
            var findedAdapters = FilesIntegration.GetFileSecurity(bunches);

            foreach (var e in filteredEntries)
            {
                findedAdapters.TryGetValue(e.RootId.ToString(), out var adapter);

                if (adapter == null)
                {
                    continue;
                }

                if (await adapter.CanReadAsync(e, userId) &&
                    await adapter.CanCreateAsync(e, userId) &&
                    await adapter.CanEditAsync(e, userId) &&
                    await adapter.CanDeleteAsync(e, userId))
                {
                    e.Access = FileShare.None;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Comment && await adapter.CanCommentAsync(e, userId))
                {
                    e.Access = FileShare.Comment;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.FillForms && await adapter.CanFillFormsAsync(e, userId))
                {
                    e.Access = FileShare.FillForms;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Review && await adapter.CanReviewAsync(e, userId))
                {
                    e.Access = FileShare.Review;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.CustomFilter && await adapter.CanCustomFilterEditAsync(e, userId))
                {
                    e.Access = FileShare.CustomFilter;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Create && await adapter.CanCreateAsync(e, userId))
                {
                    e.Access = FileShare.ReadWrite;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Delete && await adapter.CanDeleteAsync(e, userId))
                {
                    e.Access = FileShare.ReadWrite;
                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Read && await adapter.CanReadAsync(e, userId))
                {
                    if (await adapter.CanCreateAsync(e, userId) ||
                        await adapter.CanDeleteAsync(e, userId) ||
                        await adapter.CanEditAsync(e, userId))
                    {
                        e.Access = FileShare.ReadWrite;
                    }
                    else
                    {
                        e.Access = FileShare.Read;
                    }

                    result.Add(e);
                }
                else if (action == FilesSecurityActions.Edit && await adapter.CanEditAsync(e, userId))
                {
                    e.Access = FileShare.ReadWrite;

                    result.Add(e);
                }
            }
        }

        // files in trash
        filter = f => f.RootFolderType == FolderType.TRASH;
        if ((action == FilesSecurityActions.Read || action == FilesSecurityActions.Delete) && entries.Any(filter))
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var mytrashId = await folderDao.GetFolderIDTrashAsync(false, userId);
            if (!Equals(mytrashId, 0))
            {
                result.AddRange(entries.Where(filter).Where(e => Equals(e.RootId, mytrashId)));
            }
        }

        if (_fileSecurityCommon.IsAdministrator(userId))
        {
            // administrator can work with crashed entries (crash in files_folder_tree)
            filter = f => f.RootFolderType == FolderType.DEFAULT;
            result.AddRange(entries.Where(filter));
        }

        // restore entries order
        result.Sort((x, y) => order[x.UniqID].CompareTo(order[y.UniqID]));

        return result;
    }

    public Task ShareAsync<T>(T entryId, FileEntryType entryType, Guid @for, FileShare share)
    {
        var securityDao = _daoFactory.GetSecurityDao<T>();
        var r = new FileShareRecord
        {
            TenantId = _tenantManager.GetCurrentTenant().Id,
            EntryId = entryId,
            EntryType = entryType,
            Subject = @for,
            Owner = _authContext.CurrentAccount.ID,
            Share = share,
        };

        return securityDao.SetShareAsync(r);
    }

    public Task<IEnumerable<FileShareRecord>> GetSharesAsync<T>(IEnumerable<FileEntry<T>> entries)
    {
        return _daoFactory.GetSecurityDao<T>().GetSharesAsync(entries);
    }

    public Task<IEnumerable<FileShareRecord>> GetSharesAsync<T>(FileEntry<T> entry)
    {
        return _daoFactory.GetSecurityDao<T>().GetSharesAsync(entry);
    }

    public async Task<List<FileEntry>> GetSharesForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        IEnumerable<FileShareRecord> records = await securityDao.GetSharesAsync(subjects);

        var result = new List<FileEntry>();
        result.AddRange(await GetSharesForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
        result.AddRange(await GetSharesForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));

        return result;
    }

    private async Task<List<FileEntry>> GetSharesForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var fileDao = _daoFactory.GetFileDao<T>();
        var securityDao = _daoFactory.GetSecurityDao<T>();

        var fileIds = new Dictionary<T, FileShare>();
        var folderIds = new Dictionary<T, FileShare>();

        var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
                .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                .First()
        });

        foreach (var r in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (r.firstRecord.EntryType == FileEntryType.Folder)
            {
                if (!folderIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    folderIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
            else
            {
                if (!fileIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    fileIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
        }

        var entries = new List<FileEntry<T>>();

        if (filterType != FilterType.FoldersOnly)
        {
            var files = await fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent).ToListAsync();
            var share = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);

            files.ForEach(x =>
            {
                if (fileIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = fileIds[x.Id];
                    x.FolderIdDisplay = share;
                }
            });

            entries.AddRange(files);
        }

        if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
        {
            var folders = await folderDao.GetFoldersAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false).ToListAsync();

            if (withSubfolders)
            {
                var filteredFolders = await FilterReadAsync(folders);
                folders = filteredFolders.ToList();
            }

            var share = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);
            folders.ForEach(x =>
            {
                if (folderIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = folderIds[x.Id];
                    x.FolderIdDisplay = share;
                }
            });

            entries.AddRange(folders.Cast<FileEntry<T>>());
        }

        if (filterType != FilterType.FoldersOnly && withSubfolders)
        {
            var filesInSharedFolders = await fileDao.GetFilesAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
            filesInSharedFolders = (await FilterReadAsync(filesInSharedFolders)).ToList();
            entries.AddRange(filesInSharedFolders);
            entries = entries.Distinct().ToList();
        }

        entries = entries.Where(f =>
                                f.RootFolderType == FolderType.USER // show users files
                                && f.RootCreateBy != _authContext.CurrentAccount.ID // don't show my files
                                && (!f.ProviderEntry || _filesSettingsHelper.EnableThirdParty) // show thirdparty provider only if enabled
            ).ToList();

        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            entries = entries.Where(r => !r.ProviderEntry).ToList();
        }

        var failedEntries = entries.Where(x => !string.IsNullOrEmpty(x.Error));
        var failedRecords = new List<FileShareRecord>();

        foreach (var failedEntry in failedEntries)
        {
            var entryType = failedEntry.FileEntryType;

            var failedRecord = records.First(x => x.EntryId.Equals(failedEntry.Id) && x.EntryType == entryType);

            failedRecord.Share = FileShare.None;

            failedRecords.Add(failedRecord);
        }

        if (failedRecords.Count > 0)
        {
            await securityDao.DeleteShareRecordsAsync(failedRecords);
        }

        return entries.Where(x => string.IsNullOrEmpty(x.Error)).Cast<FileEntry>().ToList();
    }

    public async Task<List<FileEntry>> GetVirtualRoomsAsync(FilterType filterType, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders, 
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool withoutMe)
    {
        if (_fileSecurityCommon.IsAdministrator(_authContext.CurrentAccount.ID))
        {
            return await GetVirtualRoomsForAdminAsync(filterType, subjectId, searchText, searchInContent, withSubfolders, searchArea, withoutTags, tagNames, withoutMe);
        }

        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        var records = await securityDao.GetSharesAsync(subjects);
        var entries = new List<FileEntry>();

        var rooms = await GetVirtualRoomsForUserAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectId, searchText, searchInContent, 
            withSubfolders, searchArea, withoutTags, tagNames, withoutMe);
        var thirdPartyRooms = await GetVirtualRoomsForUserAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectId, searchText, 
            searchInContent, withSubfolders, searchArea, withoutTags, tagNames, withoutMe);

        entries.AddRange(rooms);
        entries.AddRange(thirdPartyRooms);

        return entries;
    }

    private async Task<List<FileEntry>> GetVirtualRoomsForAdminAsync(FilterType filterType, Guid subjectId, string search, bool searchInContent, bool withSubfolders, 
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool withoutMe)
    {
        var folderDao = _daoFactory.GetFolderDao<int>();
        var folderThirdPartyDao = _daoFactory.GetFolderDao<string>();
        var fileDao = _daoFactory.GetFileDao<int>();
        var fileThirdPartyDao = _daoFactory.GetFileDao<string>();
        var providerDao = _daoFactory.ProviderDao;
        var entries = new List<FileEntry>();

        var foldersInt = new List<FileEntry<int>>();
        var foldersString = new List<FileEntry<string>>();

        if (searchArea is SearchArea.Any or SearchArea.Active)
        {
            var roomsFolderId = await _globalFolder.GetFolderVirtualRoomsAsync<int>(_daoFactory);
            var thirdPartyRoomsIds = await providerDao.GetProvidersInfoAsync(FolderType.VirtualRooms).Select(p => p.FolderId).ToListAsync();

            var roomsEntries = await folderDao.GetRoomsAsync(roomsFolderId, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, withoutMe).ToListAsync();
            var thirdPartyRoomsEntries = await folderThirdPartyDao.GetRoomsAsync(thirdPartyRoomsIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, withoutMe)
                .ToListAsync();

            foldersInt.AddRange(roomsEntries);
            foldersString.AddRange(thirdPartyRoomsEntries);

            if (withSubfolders)
            {
                List<File<int>> files;
                List<File<string>> thirdPartyFiles;

                if (!string.IsNullOrEmpty(search))
                {
                    files = await fileDao.GetFilesAsync(roomsFolderId, null, FilterType.None, false, Guid.Empty, search, searchInContent, true).ToListAsync();
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsIds, FilterType.None, false, Guid.Empty, search, searchInContent);
                }
                else
                {
                    files = await fileDao.GetFilesAsync(roomsEntries.Where(r => DocSpaceHelper.IsRoom(r.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search, 
                        searchInContent);
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsEntries.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent);
                }

                entries.AddRange(files);
                entries.AddRange(thirdPartyFiles);
            }
        }
        if (searchArea is SearchArea.Any or SearchArea.Archive)
        {
            var archiveFolderId = await _globalFolder.GetFolderArchive<int>(_daoFactory);
            var thirdPartyRoomsIds = await providerDao.GetProvidersInfoAsync(FolderType.Archive).Select(p => p.FolderId).ToListAsync();

            var roomsEntries = await folderDao.GetRoomsAsync(archiveFolderId, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, withoutMe).ToListAsync();
            var thirdPartyRoomsEntries = await folderThirdPartyDao.GetRoomsAsync(thirdPartyRoomsIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, withoutMe)
                .ToListAsync();

            foldersInt.AddRange(roomsEntries);
            foldersString.AddRange(thirdPartyRoomsEntries);

            if (withSubfolders)
            {
                List<File<int>> files;
                List<File<string>> thirdPartyFiles;

                if (!string.IsNullOrEmpty(search))
                {
                    files = await fileDao.GetFilesAsync(archiveFolderId, null, FilterType.None, false, Guid.Empty, search, searchInContent, true).ToListAsync();
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsIds, FilterType.None, false, Guid.Empty, search, searchInContent);
                }
                else
                {
                    files = await fileDao.GetFilesAsync(roomsEntries.Where(r => DocSpaceHelper.IsRoom(r.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search,
                        searchInContent);
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsEntries.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent);
                }

                entries.AddRange(files);
                entries.AddRange(thirdPartyFiles);
            }
        }

        await SetTagsAsync(foldersInt);
        await SetTagsAsync(foldersString);
        await SetPinAsync(foldersInt);
        await SetPinAsync(foldersString);

        entries.AddRange(foldersInt);
        entries.AddRange(foldersString);

        return entries;
    }

    private async Task<List<FileEntry>> GetVirtualRoomsForUserAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, Guid subjectId, string search, 
        bool searchInContent, bool withSubfolders, SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool withoutMe)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var fileDao = _daoFactory.GetFileDao<T>();
        var entries = new List<FileEntry>();

        var roomsIds = new Dictionary<T, FileShare>();
        var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
            .First()
        });

        foreach (var record in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (!roomsIds.ContainsKey((T)record.firstRecord.EntryId))
            {
                roomsIds.Add((T)record.firstRecord.EntryId, record.firstRecord.Share);
            }
        }

        Func<FileEntry<T>, bool> filter = f =>
        {
            var id = f.FileEntryType == FileEntryType.Folder ? f.Id : f.ParentId;

            if (searchArea == SearchArea.Archive && f.RootFolderType == FolderType.Archive && roomsIds[id] == FileShare.RoomManager)
            {
                return true;
            }
            if (searchArea == SearchArea.Active && f.RootFolderType == FolderType.VirtualRooms)
            {
                return true;
            }
            if (searchArea == SearchArea.Any && (f.RootFolderType == FolderType.VirtualRooms || (f.RootFolderType == FolderType.Archive && roomsIds[id] == FileShare.RoomManager)))
            {
                return true;
            }

            return false;
        };

        var fileEntries = await folderDao.GetRoomsAsync(roomsIds.Keys, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, withoutMe)
            .Where(filter).ToListAsync();

        await SetTagsAsync(fileEntries);
        await SetPinAsync(fileEntries);

        entries.AddRange(fileEntries);

        if (withSubfolders)
        {
            List<File<T>> files;

            if (!string.IsNullOrEmpty(search))
            {
                files = await fileDao.GetFilesAsync(roomsIds.Keys, FilterType.None, false, Guid.Empty, search, searchInContent);
            }
            else
            {
                files = await fileDao.GetFilesAsync(fileEntries.OfType<Folder<T>>().Where(f => DocSpaceHelper.IsRoom(f.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent);
            }

            entries.AddRange(files.Where(filter));
        }

        return entries;
    }

    private async Task SetTagsAsync<T>(IEnumerable<FileEntry<T>> entries)
    {
        if (!entries.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tags = await tagDao.GetTagsAsync(TagType.Custom, entries).ToLookupAsync(f => (T)f.EntryId);

        foreach (var room in entries)
        {
            room.Tags = tags[room.Id];
        }
    }

    private async Task SetPinAsync<T>(IEnumerable<FileEntry<T>> entries)
    {
        if (!entries.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tags = await tagDao.GetTagsAsync(_authContext.CurrentAccount.ID, TagType.Pin, entries).ToDictionaryAsync(t => (T)t.EntryId);

        foreach (var fileEntry in entries.Where(e => e.FileEntryType == FileEntryType.Folder))
        {
            var room = (Folder<T>)fileEntry;
            if (tags.ContainsKey(room.Id))
            {
                room.Pinned = true;
            }
        }
    }

    public async Task<List<FileEntry>> GetPrivacyForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        IEnumerable<FileShareRecord> records = await securityDao.GetSharesAsync(subjects);

        var result = new List<FileEntry>();
        result.AddRange(await GetPrivacyForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
        result.AddRange(await GetPrivacyForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));

        return result;
    }

    private async Task<List<FileEntry<T>>> GetPrivacyForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var fileDao = _daoFactory.GetFileDao<T>();

        var fileIds = new Dictionary<T, FileShare>();
        var folderIds = new Dictionary<T, FileShare>();

        var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
                .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                .First()
        });

        foreach (var r in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (r.firstRecord.EntryType == FileEntryType.Folder)
            {
                if (!folderIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    folderIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
            else
            {
                if (!fileIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    fileIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
        }

        var entries = new List<FileEntry<T>>();

        if (filterType != FilterType.FoldersOnly)
        {
            var files = await fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent).ToListAsync();
            var privateFolder = await _globalFolder.GetFolderPrivacyAsync<T>(_daoFactory);

            files.ForEach(x =>
            {
                if (fileIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = access;
                    x.FolderIdDisplay = privateFolder;
                }
            });

            entries.AddRange(files);
        }

        if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
        {
            var folders = await folderDao.GetFoldersAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false).ToListAsync();

            if (withSubfolders)
            {
                var filteredFolders = await FilterReadAsync(folders);
                folders = filteredFolders.ToList();
            }

            var privacyFolder = await _globalFolder.GetFolderPrivacyAsync<T>(_daoFactory);
            folders.ForEach(x =>
            {
                if (folderIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = access;
                    x.FolderIdDisplay = privacyFolder;
                }
            });

            entries.AddRange(folders.Cast<FileEntry<T>>());
        }

        if (filterType != FilterType.FoldersOnly && withSubfolders)
        {
            var filesInSharedFolders = await fileDao.GetFilesAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
            filesInSharedFolders = (await FilterReadAsync(filesInSharedFolders)).ToList();
            entries.AddRange(filesInSharedFolders);
            entries = entries.Distinct().ToList();
        }

        entries = entries.Where(f =>
                                f.RootFolderType == FolderType.Privacy // show users files
                                && f.RootCreateBy != _authContext.CurrentAccount.ID // don't show my files
            ).ToList();

        return entries;
    }

    public Task RemoveSubjectAsync<T>(Guid subject)
    {
        return _daoFactory.GetSecurityDao<T>().RemoveSubjectAsync(subject);
    }

    public List<Guid> GetUserSubjects(Guid userId)
    {
        // priority order
        // User, Departments, admin, everyone

        var result = new List<Guid> { userId };
        if (userId == FileConstant.ShareLinkId)
        {
            return result;
        }

        result.AddRange(_userManager.GetUserGroups(userId).Select(g => g.ID));
        if (_fileSecurityCommon.IsAdministrator(userId))
        {
            result.Add(Constants.GroupAdmin.ID);
        }

        result.Add(Constants.GroupEveryone.ID);

        return result;
    }

    private sealed class SubjectComparer : IComparer<FileShareRecord>
    {
        private readonly List<Guid> _subjects;

        public SubjectComparer(List<Guid> subjects)
        {
            _subjects = subjects;
        }

        public int Compare(FileShareRecord x, FileShareRecord y)
        {
            if (x.Subject == y.Subject)
            {
                return 0;
            }

            var index1 = _subjects.IndexOf(x.Subject);
            var index2 = _subjects.IndexOf(y.Subject);
            if (index1 == 0 || index2 == 0 // UserId
                || Constants.BuildinGroups.Any(g => g.ID == x.Subject) || Constants.BuildinGroups.Any(g => g.ID == y.Subject)) // System Groups
            {
                return index1.CompareTo(index2);
            }

            // Departments are equal.
            return 0;
        }
    }

    private enum FilesSecurityActions
    {
        Read,
        Comment,
        FillForms,
        Review,
        Create,
        Edit,
        Delete,
        CustomFilter,
        RoomEdit,
        Rename
    }
}
