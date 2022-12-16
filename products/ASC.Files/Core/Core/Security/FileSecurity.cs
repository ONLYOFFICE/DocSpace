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

    public bool IsDocSpaceAdministrator(Guid userId)
    {
        return _userManager.IsUserInGroup(userId, Constants.GroupAdmin.ID) ||
               _webItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, userId);
    }
}

[Scope]
public class FileSecurity : IFileSecurity
{
    private readonly IDaoFactory _daoFactory;
    public readonly FileShare DefaultMyShare = FileShare.Restrict;
    public readonly FileShare DefaultCommonShare = FileShare.Read;
    public readonly FileShare DefaultPrivacyShare = FileShare.Restrict;
    public readonly FileShare DefaultVirtualRoomsShare = FileShare.Restrict;

    private static readonly IDictionary<FileEntryType, IEnumerable<FilesSecurityActions>> _securityEntries =
    new Dictionary<FileEntryType, IEnumerable<FilesSecurityActions>>()
    {
            {
                FileEntryType.File, new List<FilesSecurityActions>()
                {
                    FilesSecurityActions.Read,
                    FilesSecurityActions.Comment,
                    FilesSecurityActions.FillForms,
                    FilesSecurityActions.Review,
                    FilesSecurityActions.Edit,
                    FilesSecurityActions.Delete,
                    FilesSecurityActions.CustomFilter,
                    FilesSecurityActions.Rename,
                    FilesSecurityActions.ReadHistory,
                    FilesSecurityActions.Lock,
                    FilesSecurityActions.EditHistory,
                    FilesSecurityActions.Copy,
                    FilesSecurityActions.Move,
                    FilesSecurityActions.Duplicate,
                }
            },
            {
                FileEntryType.Folder, new List<FilesSecurityActions>()
                {
                    FilesSecurityActions.Read,
                    FilesSecurityActions.Create,
                    FilesSecurityActions.Delete,
                    FilesSecurityActions.EditRoom,
                    FilesSecurityActions.Rename,
                    FilesSecurityActions.CopyTo,
                    FilesSecurityActions.MoveTo,
                    FilesSecurityActions.Copy,
                    FilesSecurityActions.Move,
                    FilesSecurityActions.Pin,
                    FilesSecurityActions.EditAccess,
                    FilesSecurityActions.Duplicate,
                }
            }
    };

    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly AuthManager _authManager;
    private readonly GlobalFolder _globalFolder;
    private readonly FileSecurityCommon _fileSecurityCommon;

    public FileSecurity(
        IDaoFactory daoFactory,
        UserManager userManager,
        TenantManager tenantManager,
        AuthContext authContext,
        AuthManager authManager,
        GlobalFolder globalFolder,
        FileSecurityCommon fileSecurityCommon)
    {
        _daoFactory = daoFactory;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _authManager = authManager;
        _globalFolder = globalFolder;
        _fileSecurityCommon = fileSecurityCommon;
    }

    public IAsyncEnumerable<Tuple<FileEntry<T>, bool>> CanReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries, Guid userId)
    {
        return CanAsync(entries, userId, FilesSecurityActions.Read);
    }

    public IAsyncEnumerable<Tuple<FileEntry<T>, bool>> CanReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return CanReadAsync(entries, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanReadAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Read);
    }

    public Task<bool> CanReadHistoryAsync<T>(FileEntry<T> entry)
    {
        return CanReadHistoryAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanReadHistoryAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.ReadHistory);
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
        return CanAsync(entry, userId, FilesSecurityActions.EditRoom);
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

    public Task<bool> CanShareAsync<T>(FileEntry<T> entry)
    {
        return CanShareAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanLockAsync<T>(FileEntry<T> entry)
    {
        return CanLockAsync(entry, _authContext.CurrentAccount.ID);
    }

    public Task<bool> CanLockAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Lock);
    }

    public Task<bool> CanCopyToAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.CopyTo);
    }

    public Task<bool> CanCopyAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Copy);
    }

    public Task<bool> CanMoveToAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.MoveTo);
    }

    public Task<bool> CanMoveAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Move);
    }

    public Task<bool> CanPinAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Pin);
    }

    public Task<bool> CanEditAccessAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.EditAccess);
    }

    public Task<bool> CanEditHistoryAsync<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.EditHistory);
    }

    public Task<IEnumerable<Guid>> WhoCanReadAsync<T>(FileEntry<T> entry)
    {
        return WhoCanAsync(entry, FilesSecurityActions.Read);
    }

    private async Task<IEnumerable<Guid>> WhoCanAsync<T>(FileEntry<T> entry, FilesSecurityActions action)
    {
        var shares = await GetSharesAsync(entry);
        var copyShares = shares.ToList();

        FileShareRecord[] defaultRecords;

        switch (entry.RootFolderType)
        {
            case FolderType.COMMON:
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = DefaultCommonShare,
                        Subject = Constants.GroupEveryone.ID,
                        TenantId = _tenantManager.GetCurrentTenant().Id,
                        Owner = _authContext.CurrentAccount.ID
                    }
                };

                if (!shares.Any())
                {
                    var defaultShareRecord = defaultRecords.FirstOrDefault();

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
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = DefaultMyShare,
                        Subject = entry.RootCreateBy,
                        TenantId = _tenantManager.GetCurrentTenant().Id,
                        Owner = entry.RootCreateBy
                    }
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
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = DefaultPrivacyShare,
                        Subject = entry.RootCreateBy,
                        TenantId = _tenantManager.GetCurrentTenant().Id,
                        Owner = entry.RootCreateBy
                    }
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
                defaultRecords = null;
                break;

            case FolderType.VirtualRooms:
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = FileShare.Read,
                        Subject = WebItemManager.DocumentsProductID,
                        TenantId = _tenantManager.GetCurrentTenant().Id,
                        Owner = entry.RootCreateBy
                    },
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = FileShare.Read,
                        Subject = Constants.GroupAdmin.ID,
                        TenantId = _tenantManager.GetCurrentTenant().Id,
                        Owner = entry.RootCreateBy
                    }
                };

                if (!shares.Any())
                {
                    var users = new List<Guid>();

                    foreach (var defaultRecord in defaultRecords)
                    {
                        users.AddRange(_userManager.GetUsersByGroup(defaultRecord.Subject).Where(x => x.Status == EmployeeStatus.Active).Select(y => y.Id));
                    }

                    return users.Distinct();
                }

                break;

            default:
                defaultRecords = null;
                break;
        }

        if (defaultRecords != null)
        {
            shares = shares.Concat(defaultRecords);
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
        }).Distinct();

        var result = new List<Guid>();

        foreach (var x in manyShares)
        {
            if (await CanAsync(entry, x, action, copyShares))
            {
                result.Add(x);
            }
        }

        return result;
    }

    public async IAsyncEnumerable<FileEntry<T>> FilterReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        await foreach (var e in CanReadAsync(entries.Where(f => f != null)))
        {
            if (e.Item2)
            {
                yield return e.Item1;
            }
        }
    }

    public IAsyncEnumerable<FileEntry<T>> SetSecurity<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return SetSecurity(entries, _authContext.CurrentAccount.ID);
    }

    public async IAsyncEnumerable<FileEntry<T>> SetSecurity<T>(IAsyncEnumerable<FileEntry<T>> entries, Guid userId)
    {
        var user = _userManager.GetUsers(userId);
        var isOutsider = _userManager.IsOutsider(user);
        var isUser = _userManager.IsUser(user);
        var isAuthenticated = _authManager.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId).IsAuthenticated;
        var isDocSpaceAdmin = _fileSecurityCommon.IsDocSpaceAdministrator(userId);

        await foreach (var entry in entries)
        {
            if (entry.Security != null)
            {
                yield return entry;
            }

            var tasks = Enum.GetValues<FilesSecurityActions>()
                .Where(r => _securityEntries[entry.FileEntryType].Contains(r))
                .Select(async e =>
                {
                    var t = await FilterEntry(entry, e, userId, null, isOutsider, isUser, isAuthenticated, isDocSpaceAdmin);
                    return new KeyValuePair<FilesSecurityActions, bool>(e, t);
                });

            entry.Security = (await Task.WhenAll(tasks)).ToDictionary(a => a.Key, b => b.Value);

            yield return entry;
        }
    }

    private async Task<bool> CanAsync<T>(FileEntry<T> entry, Guid userId, FilesSecurityActions action, IEnumerable<FileShareRecord> shares = null)
    {
        if (entry.Security != null && entry.Security.ContainsKey(action))
        {
            return entry.Security[action];
        }

        var user = _userManager.GetUsers(userId);
        var isOutsider = _userManager.IsOutsider(user);

        if (isOutsider && action != FilesSecurityActions.Read)
        {
            return false;
        }

        var isUser = _userManager.IsUser(user);
        var isAuthenticated = _authManager.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId).IsAuthenticated;
        var isDocSpaceAdmin = _fileSecurityCommon.IsDocSpaceAdministrator(userId);

        return await FilterEntry(entry, action, userId, shares, isOutsider, isUser, isAuthenticated, isDocSpaceAdmin);
    }

    public IAsyncEnumerable<FileEntry<T>> FilterDownloadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return FilterReadAsync(entries).Where(CheckDenyDownload);
    }

    private bool CheckDenyDownload<T>(FileEntry<T> entry)
    {
        return entry.DenyDownload
            ? entry.Access != FileShare.Read && entry.Access != FileShare.Comment
            : true;
    }

    private bool CheckDenySharing<T>(FileEntry<T> entry)
    {
        return entry.DenySharing
            ? entry.Access != FileShare.ReadWrite
            : true;
    }

    private async IAsyncEnumerable<Tuple<FileEntry<T>, bool>> CanAsync<T>(IAsyncEnumerable<FileEntry<T>> entry, Guid userId, FilesSecurityActions action)
    {
        await foreach (var r in SetSecurity(entry, userId))
        {
            if (r.Security != null && r.Security.ContainsKey(action))
            {
                yield return new Tuple<FileEntry<T>, bool>(r, r.Security[action]);
            }
            else
            {
                yield return new Tuple<FileEntry<T>, bool>(r, await CanAsync(r, userId, action));
            }
        }
    }

    private async Task<bool> FilterEntry<T>(FileEntry<T> e, FilesSecurityActions action, Guid userId, IEnumerable<FileShareRecord> shares, bool isOutsider, bool isUser, bool isAuthenticated, bool isDocSpaceAdmin)
    {
        if (!isAuthenticated && userId != FileConstant.ShareLinkId)
        {
            return false;
        }

        var folder = e as Folder<T>;
        var isRoom = folder != null && DocSpaceHelper.IsRoom(folder.FolderType);

        if ((action == FilesSecurityActions.ReadHistory ||
             action == FilesSecurityActions.EditHistory) && 
             e.ProviderEntry)
        {
            return false;
        }

        if (e.FileEntryType == FileEntryType.Folder)
        {
            if (folder == null)
            {
                return false;
            }

            if (action != FilesSecurityActions.Read)
            {
                if ((action == FilesSecurityActions.Pin ||
                     action == FilesSecurityActions.EditAccess) &&
                    !isRoom)
                {
                    return false;
                }

                if (action == FilesSecurityActions.Copy && isRoom)
                {
                    return false;
                }

                if (!isUser)
                {
                    if (folder.FolderType == FolderType.USER)
                    {
                        if (action == FilesSecurityActions.Create ||
                            action == FilesSecurityActions.CopyTo ||
                            action == FilesSecurityActions.MoveTo)
                        {
                            return true;
                        }

                        return false;
                    }

                    if (folder.FolderType == FolderType.Archive)
                    {
                        if (action == FilesSecurityActions.MoveTo)
                        {
                            return true;
                        }

                        return false;
                    }

                    if (folder.FolderType == FolderType.VirtualRooms)
                    {
                        if (action == FilesSecurityActions.Create ||
                            action == FilesSecurityActions.MoveTo)
                        {
                            return true;
                        }

                        return false;
                    }

                    if (folder.FolderType == FolderType.TRASH)
                    {
                        return action == FilesSecurityActions.MoveTo;
                    }
                }
            }
            else
            {
                if (folder.FolderType == FolderType.VirtualRooms)
                {
                    // all can read VirtualRooms folder
                    return true;
                }

                if (folder.FolderType == FolderType.Archive)
                {
                    return true;
                }
            }
        }

        switch (e.RootFolderType)
        {
            case FolderType.DEFAULT:
                if (isDocSpaceAdmin)
                {
                    // administrator can work with crashed entries (crash in files_folder_tree)
                    return true;
                }
                break;
            case FolderType.TRASH:
                if (action != FilesSecurityActions.Read && action != FilesSecurityActions.Delete && action != FilesSecurityActions.Move)
                {
                    return false;
                }

                var folderDao = _daoFactory.GetFolderDao<T>();
                var mytrashId = await folderDao.GetFolderIDTrashAsync(false, userId);
                if (!Equals(mytrashId, 0) && Equals(e.RootId, mytrashId))
                {
                    if (folder != null && action == FilesSecurityActions.Delete && Equals(e.Id, mytrashId))
                    {
                        return false;
                    }

                    return true;
                }
                break;
            case FolderType.USER:
                if (isOutsider || isUser || action == FilesSecurityActions.Lock)
                {
                    return false;
                }
                if (e.RootCreateBy == userId)
                {
                    // user has all right in his folder
                    return true;
                }
                break;
            case FolderType.VirtualRooms:
                if (action == FilesSecurityActions.Delete && isRoom)
                {
                    return false;
                }

                if (await HasAccessAsync(e, userId, isUser, isDocSpaceAdmin, isRoom))
                {
                    return true;
                }
                break;
            case FolderType.Archive:
                if (action != FilesSecurityActions.Read &&
                    action != FilesSecurityActions.Delete &&
                    action != FilesSecurityActions.ReadHistory &&
                    action != FilesSecurityActions.Copy &&
                    action != FilesSecurityActions.Move
                    )
                {
                    return false;
                }

                if ((action == FilesSecurityActions.Delete || 
                    action == FilesSecurityActions.Move) && 
                    !isRoom)
                {
                    return false;
                }

                if (await HasAccessAsync(e, userId, isUser, isDocSpaceAdmin, isRoom))
                {
                    return true;
                }
                break;
            case FolderType.ThirdpartyBackup:
                if (isDocSpaceAdmin)
                {
                    return true;
                }
                break;
            default:
                break;
        }

        var file = e as File<T>;

        var subjects = new List<Guid>();
        if (shares == null)
        {
            subjects = GetUserSubjects(userId);
            shares = (await GetSharesAsync(e))
                    .Join(subjects, r => r.Subject, s => s, (r, s) => r)
                    .ToList();
            // shares ordered by level
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
                : e.RootFolderType == FolderType.VirtualRooms
                ? DefaultVirtualRoomsShare
                : e.RootFolderType == FolderType.USER
                ? DefaultMyShare
            : e.RootFolderType == FolderType.Privacy
                ? DefaultPrivacyShare
                : DefaultCommonShare;

        e.Access = ace != null ? ace.Share : defaultShare;

        e.Access = e.RootFolderType == FolderType.ThirdpartyBackup ? FileShare.Restrict : e.Access;

        switch (action)
        {
            case FilesSecurityActions.Read:
            case FilesSecurityActions.Pin:
                return e.Access != FileShare.Restrict;
            case FilesSecurityActions.Comment:
                if (e.Access == FileShare.Comment ||
                    e.Access == FileShare.Review ||
                    e.Access == FileShare.CustomFilter ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.FillForms)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.FillForms:
                if (e.Access == FileShare.FillForms ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Review:
                if (e.Access == FileShare.Review ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.FillForms)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Create:
                if (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Edit:
                if (e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Delete:
                if (e.Access == FileShare.RoomAdmin)
                {
                    if (file != null && (file.RootFolderType == FolderType.VirtualRooms))
                    {
                        return true;
                    }
                    else if (folder != null && folder.RootFolderType == FolderType.VirtualRooms &&
                        folder.FolderType == FolderType.DEFAULT)
                    {
                        return true;
                    }
                }
                break;
            case FilesSecurityActions.CustomFilter:
                if ((e.Access == FileShare.CustomFilter ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing))
                {
                    return true;
                }
                break;
            case FilesSecurityActions.EditRoom:
                if (e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Rename:
                if (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.ReadHistory:
                if (e.Access == FileShare.RoomAdmin || e.Access == FileShare.Editing)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Lock:
                if (e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.EditHistory:
                if (e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin)
                {
                    return file != null && !file.Encrypted;
                }
                break;
            case FilesSecurityActions.CopyTo:
            case FilesSecurityActions.Copy:
            case FilesSecurityActions.MoveTo:
            case FilesSecurityActions.EditAccess:
            case FilesSecurityActions.Duplicate:
                if (e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Move:
                if (e.Access == FileShare.RoomAdmin && !isRoom)
                {
                    return true;
                }
                break;
        }

        if (e.Access != FileShare.Restrict &&
            e.CreateBy == userId &&
            (e.FileEntryType == FileEntryType.File || folder.FolderType != FolderType.COMMON) &&
            e.RootFolderType != FolderType.Archive && e.RootFolderType != FolderType.VirtualRooms)
        {
            return true;
        }

        if (e.CreateBy == userId)
        {
            e.Access = FileShare.None; //HACK: for client
        }


        return false;
    }

    public Task ShareAsync<T>(T entryId, FileEntryType entryType, Guid @for, FileShare share, SubjectType subjectType = default, FileShareOptions fileShareOptions = null)
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
            SubjectType = subjectType,
            FileShareOptions = fileShareOptions,
        };

        return securityDao.SetShareAsync(r);
    }

    public Task<IEnumerable<FileShareRecord>> GetSharesAsync<T>(FileEntry<T> entry)
    {
        return _daoFactory.GetSecurityDao<T>().GetSharesAsync(entry);
    }

    public async IAsyncEnumerable<FileEntry> GetSharesForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        var records = await securityDao.GetSharesAsync(subjects).ToListAsync();

        var firstTask = GetSharesForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).ToListAsync();
        var secondTask = GetSharesForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).ToListAsync();

        foreach (var items in await Task.WhenAll(firstTask.AsTask(), secondTask.AsTask()))
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }

    public async Task<List<FileEntry>> GetVirtualRoomsAsync(FilterType filterType, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders,
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();

        var subjectEntries = subjectFilter is SubjectFilter.Member
                ? await securityDao.GetSharesAsync(new[] { subjectId }).Where(r => r.EntryType == FileEntryType.Folder).Select(r => r.EntryId.ToString()).ToListAsync()
                : null;

        if (_fileSecurityCommon.IsDocSpaceAdministrator(_authContext.CurrentAccount.ID))
        {
            return await GetVirtualRoomsForDocSpaceAdminAsync(filterType, subjectId, searchText, searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject, provider, subjectFilter, subjectEntries);
        }

        var currentUserSubjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        var currentUsersRecords = await securityDao.GetSharesAsync(currentUserSubjects).ToListAsync();
        var thirdpartyIds = await GetThirdpartyRoomsIdsAsync(searchArea);
        var entries = new List<FileEntry>();

        var rooms = await GetVirtualRoomsForUserAsync(currentUsersRecords.Where(r => r.EntryId is int), Array.Empty<int>(), currentUserSubjects, filterType, subjectId, searchText, searchInContent,
            withSubfolders, searchArea, withoutTags, tagNames, excludeSubject, provider, subjectFilter, subjectEntries);
        var thirdPartyRooms = await GetVirtualRoomsForUserAsync(currentUsersRecords.Where(r => r.EntryId is string), thirdpartyIds, currentUserSubjects, filterType, subjectId, searchText,
            searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject, provider, subjectFilter, subjectEntries);

        entries.AddRange(rooms);
        entries.AddRange(thirdPartyRooms);

        return entries;
    }

    private async Task<IEnumerable<string>> GetThirdpartyRoomsIdsAsync(SearchArea searchArea)
    {
        var result = new List<string>();

        if (_userManager.IsUser(_authContext.CurrentAccount.ID))
        {
            return Array.Empty<string>();
        }

        if (searchArea == SearchArea.Active || searchArea == SearchArea.Any)
        {
            var ids = await _daoFactory.ProviderDao.GetProvidersInfoAsync(FolderType.VirtualRooms)
                .Where(p => p.Owner == _authContext.CurrentAccount.ID).Select(p => p.FolderId).ToListAsync();

            result.AddRange(ids);
        }
        if (searchArea == SearchArea.Archive || searchArea == SearchArea.Any)
        {
            var ids = await _daoFactory.ProviderDao.GetProvidersInfoAsync(FolderType.Archive)
                .Where(p => p.Owner == _authContext.CurrentAccount.ID).Select(p => p.FolderId).ToListAsync();

            result.AddRange(ids);
        }

        return result;
    }

    private async Task<List<FileEntry>> GetVirtualRoomsForDocSpaceAdminAsync(FilterType filterType, Guid subjectId, string search, bool searchInContent, bool withSubfolders,
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntries)
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

            var roomsEntries = await folderDao.GetRoomsAsync(roomsFolderId, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries).ToListAsync();
            var thirdPartyRoomsEntries = await folderThirdPartyDao.GetRoomsAsync(Array.Empty<string>(), thirdPartyRoomsIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries)
                .ToListAsync();

            foldersInt.AddRange(roomsEntries);
            foldersString.AddRange(thirdPartyRoomsEntries);

            if (withSubfolders && filterType != FilterType.FoldersOnly)
            {
                List<File<int>> files;
                List<File<string>> thirdPartyFiles;

                if (!string.IsNullOrEmpty(search))
                {
                    files = await fileDao.GetFilesAsync(roomsFolderId, null, FilterType.None, false, Guid.Empty, search, searchInContent, true).ToListAsync();
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsIds, FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                }
                else
                {
                    files = await fileDao.GetFilesAsync(roomsEntries.Where(r => DocSpaceHelper.IsRoom(r.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsEntries.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                }

                entries.AddRange(files);
                entries.AddRange(thirdPartyFiles);
            }
        }
        if (searchArea is SearchArea.Any or SearchArea.Archive)
        {
            var archiveFolderId = await _globalFolder.GetFolderArchiveAsync<int>(_daoFactory);
            var thirdPartyRoomsIds = await providerDao.GetProvidersInfoAsync(FolderType.Archive).Select(p => p.FolderId).ToListAsync();

            var roomsEntries = await folderDao.GetRoomsAsync(archiveFolderId, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries).ToListAsync();
            var thirdPartyRoomsEntries = await folderThirdPartyDao.GetRoomsAsync(Array.Empty<string>(), thirdPartyRoomsIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries)
                .ToListAsync();

            foldersInt.AddRange(roomsEntries);
            foldersString.AddRange(thirdPartyRoomsEntries);

            if (withSubfolders && filterType != FilterType.FoldersOnly)
            {
                List<File<int>> files;
                List<File<string>> thirdPartyFiles;

                if (!string.IsNullOrEmpty(search))
                {
                    files = await fileDao.GetFilesAsync(archiveFolderId, null, FilterType.None, false, Guid.Empty, search, searchInContent, true).ToListAsync();
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsIds, FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                }
                else
                {
                    files = await fileDao.GetFilesAsync(roomsEntries.Where(r => DocSpaceHelper.IsRoom(r.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                    thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsEntries.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
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

    private async Task<List<FileEntry>> GetVirtualRoomsForUserAsync<T>(IEnumerable<FileShareRecord> records, IEnumerable<T> proivdersIds, List<Guid> subjects, FilterType filterType, Guid subjectId, string search,
        bool searchInContent, bool withSubfolders, SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntries)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var fileDao = _daoFactory.GetFileDao<T>();
        var entries = new List<FileEntry>();

        var rootFoldersIds = searchArea == SearchArea.Active ? new[] { await _globalFolder.GetFolderVirtualRoomsAsync<T>(_daoFactory) } :
            searchArea == SearchArea.Archive ? new[] { await _globalFolder.GetFolderArchiveAsync<T>(_daoFactory) } :
            new[] { await _globalFolder.GetFolderVirtualRoomsAsync<T>(_daoFactory), await _globalFolder.GetFolderArchiveAsync<T>(_daoFactory) };

        var roomsIds = new Dictionary<T, FileShare>();
        var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
            .First()
        });

        if (proivdersIds.Any())
        {
            foreach (var id in proivdersIds)
            {
                roomsIds.Add(id, FileShare.None);
            }
        }

        foreach (var record in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (!roomsIds.ContainsKey((T)record.firstRecord.EntryId) && record.firstRecord.EntryType == FileEntryType.Folder)
            {
                roomsIds.Add((T)record.firstRecord.EntryId, record.firstRecord.Share);
            }
        }

        Func<FileEntry<T>, bool> filter = f =>
        {
            var id = f.FileEntryType == FileEntryType.Folder ? f.Id : f.ParentId;

            if (searchArea == SearchArea.Archive && f.RootFolderType == FolderType.Archive)
            {
                f.Access = roomsIds.TryGetValue(id, out var share) ? share : FileShare.None;
                return true;
            }
            if (searchArea == SearchArea.Active && f.RootFolderType == FolderType.VirtualRooms)
            {
                f.Access = roomsIds.TryGetValue(id, out var share) ? share : FileShare.None;
                return true;
            }
            if (searchArea == SearchArea.Any && (f.RootFolderType == FolderType.VirtualRooms || f.RootFolderType == FolderType.Archive))
            {
                f.Access = roomsIds.TryGetValue(id, out var share) ? share : FileShare.None;
                return true;
            }

            return false;
        };

        var fileEntries = await folderDao.GetRoomsAsync(rootFoldersIds, roomsIds.Keys, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries)
            .Where(filter).ToListAsync();

        await SetTagsAsync(fileEntries);
        await SetPinAsync(fileEntries);

        entries.AddRange(fileEntries);

        if (withSubfolders && filterType != FilterType.FoldersOnly)
        {
            List<File<T>> files;

            if (!string.IsNullOrEmpty(search))
            {
                files = await fileDao.GetFilesAsync(roomsIds.Keys, FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
            }
            else
            {
                files = await fileDao.GetFilesAsync(fileEntries.OfType<Folder<T>>().Where(f => DocSpaceHelper.IsRoom(f.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
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

    private async IAsyncEnumerable<FileEntry> GetSharesForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
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
            var files = fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);
            var share = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);

            await foreach (var x in files)
            {
                if (fileIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = fileIds[x.Id];
                    x.FolderIdDisplay = share;
                }

                entries.Add(x);
            }
        }

        if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
        {
            IAsyncEnumerable<FileEntry<T>> folders = folderDao.GetFoldersAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

            if (withSubfolders)
            {
                folders = FilterReadAsync(folders);
            }

            var share = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);

            await foreach (var folder in folders)
            {
                if (folderIds.TryGetValue(folder.Id, out var access))
                {
                    folder.Access = folderIds[folder.Id];
                    folder.FolderIdDisplay = share;
                }

                entries.Add(folder);
            }
        }

        if (filterType != FilterType.FoldersOnly && withSubfolders)
        {
            IAsyncEnumerable<FileEntry<T>> filesInSharedFolders = fileDao.GetFilesAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
            filesInSharedFolders = FilterReadAsync(filesInSharedFolders);
            entries.AddRange(await filesInSharedFolders.Distinct().ToListAsync());
        }

        var data = entries.Where(f =>
                                f.RootFolderType == FolderType.USER // show users files
                                && f.RootCreateBy != _authContext.CurrentAccount.ID // don't show my files
            );

        if (_userManager.IsUser(_authContext.CurrentAccount.ID))
        {
            data = data.Where(r => !r.ProviderEntry);
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

        data = data.Where(x => string.IsNullOrEmpty(x.Error));

        foreach (var e in data)
        {
            yield return e;
        };
    }

    public async IAsyncEnumerable<FileEntry> GetPrivacyForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        var records = await securityDao.GetSharesAsync(subjects).ToListAsync();

        await foreach (var e in GetPrivacyForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders))
        {
            yield return e;
        }

        await foreach (var e in GetPrivacyForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders))
        {
            yield return e;
        }
    }

    private async IAsyncEnumerable<FileEntry<T>> GetPrivacyForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
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
            var files = fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);
            var privateFolder = await _globalFolder.GetFolderPrivacyAsync<T>(_daoFactory);

            await foreach (var x in files)
            {
                if (fileIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = access;
                    x.FolderIdDisplay = privateFolder;
                }

                entries.Add(x);
            }
        }

        if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
        {
            IAsyncEnumerable<FileEntry<T>> folders = folderDao.GetFoldersAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

            if (withSubfolders)
            {
                folders = FilterReadAsync(folders);
            }

            var privacyFolder = await _globalFolder.GetFolderPrivacyAsync<T>(_daoFactory);

            await foreach (var folder in folders)
            {
                if (folderIds.TryGetValue(folder.Id, out var access))
                {
                    folder.Access = access;
                    folder.FolderIdDisplay = privacyFolder;
                }

                entries.Add(folder);
            }
        }

        if (filterType != FilterType.FoldersOnly && withSubfolders)
        {
            IAsyncEnumerable<FileEntry<T>> filesInSharedFolders = fileDao.GetFilesAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
            filesInSharedFolders = FilterReadAsync(filesInSharedFolders);
            entries.AddRange(await filesInSharedFolders.Distinct().ToListAsync());
        }

        var data = entries.Where(f =>
                                f.RootFolderType == FolderType.Privacy // show users files
                                && f.RootCreateBy != _authContext.CurrentAccount.ID // don't show my files
            );

        foreach (var e in data)
        {
            yield return e;
        }
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
        if (_fileSecurityCommon.IsDocSpaceAdministrator(userId))
        {
            result.Add(Constants.GroupAdmin.ID);
        }

        result.Add(Constants.GroupEveryone.ID);

        return result;
    }

    private async Task<bool> HasAccessAsync<T>(FileEntry<T> entry, Guid userId, bool isUser, bool isDocSpaceAdmin, bool isRoom)
    {
        if (!isUser)
        {
            if (isDocSpaceAdmin || (isRoom && entry.CreateBy == userId))
            {
                return true;
            }

            var myRoom = await _daoFactory.GetFolderDao<T>().GetParentFoldersAsync(entry.ParentId)
                .Where(f => DocSpaceHelper.IsRoom(f.FolderType) && f.CreateBy == userId).FirstOrDefaultAsync();

            if (myRoom != null)
            {
                return true;
            }
        }

        return false;
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

    public enum FilesSecurityActions
    {
        Read,
        Comment,
        FillForms,
        Review,
        Create,
        Edit,
        Delete,
        CustomFilter,
        EditRoom,
        Rename,
        ReadHistory,
        Lock,
        EditHistory,
        CopyTo,
        Copy,
        MoveTo,
        Move,
        Pin,
        EditAccess,
        Duplicate,
    }
}
