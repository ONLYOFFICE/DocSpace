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

using DotNetOpenAuth.Messaging;

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
    public FileShare DefaultVirtualRoomsShare => FileShare.Restrict;

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
        return CanAsync(entries, _authContext.CurrentAccount.ID, FilesSecurityActions.Read);
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
        var copyShares = shares.ToList();

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
            if (await CanAsync(entry, x, action, copyShares))
            {
                result.Add(x);
            }
        }

        return result;
    }

    public IAsyncEnumerable<FileEntry<T>> FilterReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return FilterAsync(entries, FilesSecurityActions.Read, _authContext.CurrentAccount.ID);
    }

    private async Task<bool> CanAsync<T>(FileEntry<T> entry, Guid userId, FilesSecurityActions action, IEnumerable<FileShareRecord> shares = null)
    {
        var user = _userManager.GetUsers(userId);
        var isOutsider = _userManager.IsOutsider(user);

        if (isOutsider && action != FilesSecurityActions.Read)
        {
            return false;
        }

        var isVisitor = _userManager.IsVisitor(user);
        var isAuthenticated = _authManager.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId).IsAuthenticated;
        var isAdmin = _fileSecurityCommon.IsAdministrator(userId);

        return await FilterEntry(entry, action, userId, shares, isOutsider, isVisitor, isAuthenticated, isAdmin);
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
        var user = _userManager.GetUsers(userId);
        var isOutsider = _userManager.IsOutsider(user);

        if (isOutsider && action != FilesSecurityActions.Read)
        {
            yield break;
        }

        var isVisitor = _userManager.IsVisitor(user);
        var isAuthenticated = _authManager.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId).IsAuthenticated;
        var isAdmin = _fileSecurityCommon.IsAdministrator(userId);

        await foreach (var r in entry)
        {
            yield return new Tuple<FileEntry<T>, bool>(r, await FilterEntry(r, action, userId, null, isOutsider, isVisitor, isAuthenticated, isAdmin));
        }
    }

    private IAsyncEnumerable<FileEntry<T>> FilterAsync<T>(IAsyncEnumerable<FileEntry<T>> entries, FilesSecurityActions action, Guid userId)
    {
        var user = _userManager.GetUsers(userId);
        var isOutsider = _userManager.IsOutsider(user);

        if (isOutsider && action != FilesSecurityActions.Read)
        {
            return AsyncEnumerable.Empty<FileEntry<T>>();
        }

        return InternalFilterAsync(entries, action, userId, user, isOutsider);
    }

    private async IAsyncEnumerable<FileEntry<T>> InternalFilterAsync<T>(IAsyncEnumerable<FileEntry<T>> entries, FilesSecurityActions action, Guid userId, UserInfo user, bool isOutsider)
    {
        var isVisitor = _userManager.IsVisitor(user);
        var isAuthenticated = _authManager.GetAccountByID(_tenantManager.GetCurrentTenant().Id, userId).IsAuthenticated;
        var isAdmin = _fileSecurityCommon.IsAdministrator(userId);

        await foreach (var e in entries.Where(f => f != null))
        {
            if (await FilterEntry(e, action, userId, null, isOutsider, isVisitor, isAuthenticated, isAdmin))
            {
                yield return e;
            }
        }
    }

    private async Task<bool> FilterEntry<T>(FileEntry<T> e, FilesSecurityActions action, Guid userId, IEnumerable<FileShareRecord> shares, bool isOutsider, bool isVisitor, bool isAuthenticated, bool isAdmin)
    {
        if (e.RootFolderType == FolderType.COMMON ||
            e.RootFolderType == FolderType.USER ||
            e.RootFolderType == FolderType.SHARE ||
            e.RootFolderType == FolderType.Recent ||
            e.RootFolderType == FolderType.Favorites ||
            e.RootFolderType == FolderType.Templates ||
            e.RootFolderType == FolderType.Privacy ||
            e.RootFolderType == FolderType.Projects ||
            e.RootFolderType == FolderType.VirtualRooms ||
            e.RootFolderType == FolderType.Archive ||
            e.RootFolderType == FolderType.ThirdpartyBackup)
        {
            if (!isAuthenticated && userId != FileConstant.ShareLinkId)
            {
                return false;
            }

            if (isOutsider && (e.RootFolderType == FolderType.USER
                               || e.RootFolderType == FolderType.SHARE
                               || e.RootFolderType == FolderType.Privacy))
            {
                return false;
            }

            if (isVisitor && e.RootFolderType == FolderType.Templates)
            {
                return false;
            }

            if (isVisitor && e.RootFolderType == FolderType.Privacy)
            {
                return false;
            }

            if (isVisitor && e.RootFolderType == FolderType.USER)
            {
                return false;
            }

            var folder = e as Folder<T>;
            var file = e as File<T>;

            if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder)
            {
                if (folder == null)
                {
                    return false;
                }

                if (folder.FolderType == FolderType.Projects)
                {
                    // Root Projects folder read-only
                    return false;
                }

                if (folder.FolderType == FolderType.SHARE)
                {
                    // Root Share folder read-only
                    return false;
                }

                if (folder.FolderType == FolderType.Recent)
                {
                    // Recent folder read-only
                    return false;
                }

                if (folder.FolderType == FolderType.Favorites)
                {
                    // Favorites folder read-only
                    return false;
                }

                if (folder.FolderType == FolderType.Templates)
                {
                    // Templates folder read-only
                    return false;
                }

                if (folder.FolderType == FolderType.Archive)
                {
                    return false;
                }
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
                return true;
            }

            if (e.RootFolderType == FolderType.Privacy && e.RootCreateBy == userId && !isVisitor)
            {
                // user has all right in his privacy folder
                return true;
            }

            if (e.FileEntryType == FileEntryType.Folder)
            {
                if (folder == null)
                {
                    return false;
                }

                if (folder.FolderType == FolderType.VirtualRooms)
                {
                    // DocSpace admins and room admins can create rooms
                    if (action == FilesSecurityActions.Create && !isVisitor)
                    {
                        return true;
                    }

                    // all can read VirtualRooms folder
                    if (action == FilesSecurityActions.Read)
                    {
                        return true;
                    }
                }

                if (DefaultCommonShare == FileShare.Read && action == FilesSecurityActions.Read && folder.FolderType == FolderType.COMMON)
                {
                    // all can read Common folder
                    return true;
                }

                if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.SHARE)
                {
                    // all can read Share folder
                    return true;
                }

                if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Recent)
                {
                    // all can read recent folder
                    return true;
                }

                if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Favorites)
                {
                    // all can read favorites folder
                    return true;
                }

                if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Templates)
                {
                    // all can read templates folder
                    return true;
                }

                if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.VirtualRooms)
                {
                    return true;
                }

                if (action == FilesSecurityActions.Read && folder.FolderType == FolderType.Archive)
                {
                    return true;
                }
            }

            if (e.RootFolderType == FolderType.COMMON && isAdmin)
            {
                // administrator in Common has all right
                return true;
            }

            if ((e.RootFolderType == FolderType.VirtualRooms || e.RootFolderType == FolderType.Archive) && !isVisitor)
            {
                if (isAdmin || e.CreateBy == userId)
                {
                    return true;
                }

                var parentRoom = await _daoFactory.GetFolderDao<T>().GetParentFoldersAsync(e.ParentId)
                    .Where(f => DocSpaceHelper.IsRoom(f.FolderType) && f.CreateBy == userId).FirstOrDefaultAsync();

                if (parentRoom != null)
                {
                    return true;
                }
            }

            if (e.RootFolderType == FolderType.ThirdpartyBackup && isAdmin)
            {
                return true;
            }

            if (action == FilesSecurityActions.Delete && e.RootFolderType == FolderType.Archive && isAdmin)
            {
                return true;
            }

            if (action == FilesSecurityActions.RoomEdit && e.RootFolderType == FolderType.Archive && isAdmin)
            {
                return true;
            }

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

            if (action == FilesSecurityActions.Read && e.Access != FileShare.Restrict)
            {
                return true;
            }
            else if (action == FilesSecurityActions.Comment && (e.Access == FileShare.Comment || e.Access == FileShare.Review || e.Access == FileShare.CustomFilter || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
            {
                return true;
            }
            else if (action == FilesSecurityActions.FillForms && (e.Access == FileShare.FillForms || e.Access == FileShare.Review || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
            {
                return true;
            }
            else if (action == FilesSecurityActions.Review && (e.Access == FileShare.Review || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
            {
                return true;
            }
            else if (action == FilesSecurityActions.CustomFilter && (e.Access == FileShare.CustomFilter || e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
            {
                return true;
            }
            else if (action == FilesSecurityActions.Edit && (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager || e.Access == FileShare.Editing))
            {
                return true;
            }
            else if (action == FilesSecurityActions.Rename && (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager))
            {
                return true;
            }
            else if (action == FilesSecurityActions.RoomEdit && e.Access == FileShare.RoomManager)
            {
                return true;
            }
            else if (action == FilesSecurityActions.Create && (e.Access == FileShare.ReadWrite || e.Access == FileShare.RoomManager))
            {
                return true;
            }
            else if (e.Access != FileShare.Restrict && e.CreateBy == userId && (e.FileEntryType == FileEntryType.File || folder.FolderType != FolderType.COMMON))
            {
                return true;
            }
            else if (action == FilesSecurityActions.Delete && (e.Access == FileShare.RoomManager || e.Access == FileShare.ReadWrite))
            {
                if (file != null && (file.RootFolderType == FolderType.VirtualRooms || file.RootFolderType == FolderType.Archive))
                {
                    return true;
                }
                else if (folder != null && (folder.RootFolderType == FolderType.VirtualRooms || folder.RootFolderType == FolderType.Archive) &&
                    folder.FolderType == FolderType.DEFAULT)
                {
                    return true;
                }
            }

            if (e.CreateBy == userId)
            {
                e.Access = FileShare.None; //HACK: for client
            }
        }

        if (e.RootFolderType == FolderType.BUNCH)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var root = e.RootId;

            var rootsFolders = folderDao.GetFoldersAsync(root);
            var bunches = await folderDao.GetBunchObjectIDsAsync(await rootsFolders.Select(r => r.Id).ToListAsync());
            var findedAdapters = FilesIntegration.GetFileSecurity(bunches);

            findedAdapters.TryGetValue(e.RootId.ToString(), out var adapter);

            if (adapter == null)
            {
                return false;
            }

            if (await adapter.CanReadAsync(e, userId) &&
                await adapter.CanCreateAsync(e, userId) &&
                await adapter.CanEditAsync(e, userId) &&
                await adapter.CanDeleteAsync(e, userId))
            {
                e.Access = FileShare.None;
                return true;
            }
            else if (action == FilesSecurityActions.Comment && await adapter.CanCommentAsync(e, userId))
            {
                e.Access = FileShare.Comment;
                return true;
            }
            else if (action == FilesSecurityActions.FillForms && await adapter.CanFillFormsAsync(e, userId))
            {
                e.Access = FileShare.FillForms;
                return true;
            }
            else if (action == FilesSecurityActions.Review && await adapter.CanReviewAsync(e, userId))
            {
                e.Access = FileShare.Review;
                return true;
            }
            else if (action == FilesSecurityActions.CustomFilter && await adapter.CanCustomFilterEditAsync(e, userId))
            {
                e.Access = FileShare.CustomFilter;
                return true;
            }
            else if (action == FilesSecurityActions.Create && await adapter.CanCreateAsync(e, userId))
            {
                e.Access = FileShare.ReadWrite;
                return true;
            }
            else if (action == FilesSecurityActions.Delete && await adapter.CanDeleteAsync(e, userId))
            {
                e.Access = FileShare.ReadWrite;
                return true;
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

                return true;
            }
            else if (action == FilesSecurityActions.Edit && await adapter.CanEditAsync(e, userId))
            {
                e.Access = FileShare.ReadWrite;

                return true;
            }

        }

        // files in trash
        if ((action == FilesSecurityActions.Read || action == FilesSecurityActions.Delete) && e.RootFolderType == FolderType.TRASH)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var mytrashId = await folderDao.GetFolderIDTrashAsync(false, userId);
            if (!Equals(mytrashId, 0) && Equals(e.RootId, mytrashId))
            {
                return true;
            }
        }

        if (isAdmin && e.RootFolderType == FolderType.DEFAULT)
        {
            // administrator can work with crashed entries (crash in files_folder_tree)
            return true;
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
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject)
    {
        if (_fileSecurityCommon.IsAdministrator(_authContext.CurrentAccount.ID))
        {
            return await GetVirtualRoomsForAdminAsync(filterType, subjectId, searchText, searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject);
        }

        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = GetUserSubjects(_authContext.CurrentAccount.ID);
        var records = await securityDao.GetSharesAsync(subjects).ToListAsync();
        var thirdpartyIds = await GetThirdpartyRoomsIdsAsync(searchArea);
        var entries = new List<FileEntry>();

        var rooms = await GetVirtualRoomsForUserAsync(records.Where(r => r.EntryId is int), Array.Empty<int>(), subjects, filterType, subjectId, searchText, searchInContent,
            withSubfolders, searchArea, withoutTags, tagNames, excludeSubject);
        var thirdPartyRooms = await GetVirtualRoomsForUserAsync(records.Where(r => r.EntryId is string), thirdpartyIds, subjects, filterType, subjectId, searchText,
            searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject);

        entries.AddRange(rooms);
        entries.AddRange(thirdPartyRooms);

        return entries;
    }

    private async Task<IEnumerable<string>> GetThirdpartyRoomsIdsAsync(SearchArea searchArea)
    {
        var result = new List<string>();
        
        if (_userManager.IsVisitor(_authContext.CurrentAccount.ID))
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

    private async Task<List<FileEntry>> GetVirtualRoomsForAdminAsync(FilterType filterType, Guid subjectId, string search, bool searchInContent, bool withSubfolders,
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject)
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

            var roomsEntries = await folderDao.GetRoomsAsync(roomsFolderId, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject).ToListAsync();
            var thirdPartyRoomsEntries = await folderThirdPartyDao.GetRoomsAsync(Array.Empty<string>(), thirdPartyRoomsIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject)
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

            var roomsEntries = await folderDao.GetRoomsAsync(archiveFolderId, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject).ToListAsync();
            var thirdPartyRoomsEntries = await folderThirdPartyDao.GetRoomsAsync(Array.Empty<string>(), thirdPartyRoomsIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject)
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
        bool searchInContent, bool withSubfolders, SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject)
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
                roomsIds.Add(id, FileShare.Read);
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

            if (searchArea == SearchArea.Archive && f.RootFolderType == FolderType.Archive && (f.CreateBy == _authContext.CurrentAccount.ID || roomsIds[id] == FileShare.RoomManager))
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

        var fileEntries = await folderDao.GetRoomsAsync(rootFoldersIds, roomsIds.Keys, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject)
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

        if (_userManager.IsVisitor(_authContext.CurrentAccount.ID))
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
