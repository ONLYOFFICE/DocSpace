/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Files.Api;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Configuration;

namespace ASC.Files.Core.Security
{
    [Scope]
    public class FileSecurityCommon
    {
        private UserManager UserManager { get; }
        private WebItemSecurity WebItemSecurity { get; }

        public FileSecurityCommon(UserManager userManager, WebItemSecurity webItemSecurity)
        {
            UserManager = userManager;
            WebItemSecurity = webItemSecurity;
        }

        public bool IsAdministrator(Guid userId)
        {
            return UserManager.IsUserInGroup(userId, Constants.GroupAdmin.ID) ||
                   WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, userId);
        }
    }

    [Scope]
    public class FileSecurity : IFileSecurity
    {
        private readonly IDaoFactory daoFactory;

        public FileShare DefaultMyShare
        {
            get { return FileShare.Restrict; }
        }

        public FileShare DefaultProjectsShare
        {
            get { return FileShare.ReadWrite; }
        }

        public FileShare DefaultCommonShare
        {
            get { return FileShare.Read; }
        }

        public FileShare DefaultPrivacyShare
        {
            get { return FileShare.Restrict; }
        }

        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private AuthContext AuthContext { get; }
        private AuthManager AuthManager { get; }
        private GlobalFolder GlobalFolder { get; }
        private FileSecurityCommon FileSecurityCommon { get; }

        public FileSecurity(
            IDaoFactory daoFactory,
            UserManager userManager,
            TenantManager tenantManager,
            AuthContext authContext,
            AuthManager authManager,
            GlobalFolder globalFolder,
            FileSecurityCommon fileSecurityCommon)
        {
            this.daoFactory = daoFactory;
            UserManager = userManager;
            TenantManager = tenantManager;
            AuthContext = authContext;
            AuthManager = authManager;
            GlobalFolder = globalFolder;
            FileSecurityCommon = fileSecurityCommon;
        }

        public Task<List<Tuple<FileEntry<T>, bool>>> CanReadAsync<T>(IEnumerable<FileEntry<T>> entry, Guid userId)
        {
            return CanAsync(entry, userId, FilesSecurityActions.Read);
        }

        public Task<List<Tuple<FileEntry<T>, bool>>> CanReadAsync<T>(IEnumerable<FileEntry<T>> entry)
        {
            return CanAsync(entry, AuthContext.CurrentAccount.ID, FilesSecurityActions.Read);
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

        public Task<bool> CanReadAsync<T>(FileEntry<T> entry)
        {
            return CanReadAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanCommentAsync<T>(FileEntry<T> entry)
        {
            return CanCommentAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry)
        {
            return CanCustomFilterEditAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry)
        {
            return CanFillFormsAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanReviewAsync<T>(FileEntry<T> entry)
        {
            return CanReviewAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanCreateAsync<T>(FileEntry<T> entry)
        {
            return CanCreateAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanEditAsync<T>(FileEntry<T> entry)
        {
            return CanEditAsync(entry, AuthContext.CurrentAccount.ID);
        }

        public Task<bool> CanDeleteAsync<T>(FileEntry<T> entry)
        {
            return CanDeleteAsync(entry, AuthContext.CurrentAccount.ID);
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
                        EntryId = entry.ID,
                        EntryType = entry.FileEntryType,
                        Share = DefaultCommonShare,
                        Subject = Constants.GroupEveryone.ID,
                        Tenant = TenantManager.GetCurrentTenant().TenantId,
                        Owner = AuthContext.CurrentAccount.ID
                    };

                    if (!shares.Any())
                    {
                        if ((defaultShareRecord.Share == FileShare.Read && action == FilesSecurityActions.Read) ||
                            (defaultShareRecord.Share == FileShare.ReadWrite))
                            return UserManager.GetUsersByGroup(defaultShareRecord.Subject)
                                              .Where(x => x.Status == EmployeeStatus.Active).Select(y => y.ID).Distinct();

                        return Enumerable.Empty<Guid>();
                    }

                    break;

                case FolderType.USER:
                    defaultShareRecord = new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.ID,
                        EntryType = entry.FileEntryType,
                        Share = DefaultMyShare,
                        Subject = entry.RootFolderCreator,
                        Tenant = TenantManager.GetCurrentTenant().TenantId,
                        Owner = entry.RootFolderCreator
                    };

                    if (!shares.Any())
                        return new List<Guid>
                            {
                                entry.RootFolderCreator
                            };

                    break;

                case FolderType.Privacy:
                    defaultShareRecord = new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.ID,
                        EntryType = entry.FileEntryType,
                        Share = DefaultPrivacyShare,
                        Subject = entry.RootFolderCreator,
                        Tenant = TenantManager.GetCurrentTenant().TenantId,
                        Owner = entry.RootFolderCreator
                    };

                    if (!shares.Any())
                        return new List<Guid>
                            {
                                entry.RootFolderCreator
                            };

                    break;

                case FolderType.BUNCH:
                    if (action == FilesSecurityActions.Read)
                    {
                        var folderDao = daoFactory.GetFolderDao<T>();
                        var root = await folderDao.GetFolderAsync(entry.RootFolderId);
                        if (root != null)
                        {
                            var path = await folderDao.GetBunchObjectIDAsync(root.ID);

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

                default:
                    defaultShareRecord = null;
                    break;
            }

            if (defaultShareRecord != null)
                shares = shares.Concat(new[] { defaultShareRecord });

            var manyShares = shares.SelectMany(x =>
            {
                var groupInfo = UserManager.GetGroupInfo(x.Subject);

                if (groupInfo.ID != Constants.LostGroupInfo.ID)
                    return
                        UserManager.GetUsersByGroup(groupInfo.ID)
                                   .Where(p => p.Status == EmployeeStatus.Active)
                                   .Select(y => y.ID);

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
            return (await FilterAsync(entries, FilesSecurityActions.Read, AuthContext.CurrentAccount.ID)).Cast<File<T>>();
        }

        public async Task<IEnumerable<Folder<T>>> FilterReadAsync<T>(IEnumerable<Folder<T>> entries)
        {
            return (await FilterAsync(entries, FilesSecurityActions.Read, AuthContext.CurrentAccount.ID)).Cast<Folder<T>>();
        }

        public async Task<IEnumerable<File<T>>> FilterEditAsync<T>(IEnumerable<File<T>> entries)
        {
            return (await FilterAsync(entries.Cast<FileEntry<T>>(), FilesSecurityActions.Edit, AuthContext.CurrentAccount.ID)).Cast<File<T>>();
        }

        public async Task<IEnumerable<Folder<T>>> FilterEditAsync<T>(IEnumerable<Folder<T>> entries)
        {
            return (await FilterAsync(entries.Cast<FileEntry<T>>(), FilesSecurityActions.Edit, AuthContext.CurrentAccount.ID)).Cast<Folder<T>>();
        }

        private async Task<bool> CanAsync<T>(FileEntry<T> entry, Guid userId, FilesSecurityActions action, IEnumerable<FileShareRecord> shares = null)
        {
            return (await FilterAsync(new[] { entry }, action, userId, shares)).Any();
        }

        private async Task<List<Tuple<FileEntry<T>, bool>>> CanAsync<T>(IEnumerable<FileEntry<T>> entry, Guid userId, FilesSecurityActions action)
        {
            var filtres = await FilterAsync(entry, action, userId);
            return entry.Select(r => new Tuple<FileEntry<T>, bool>(r, filtres.Any(a => a.ID.Equals(r.ID)))).ToList();
        }

        private Task<IEnumerable<FileEntry<T>>> FilterAsync<T>(IEnumerable<FileEntry<T>> entries, FilesSecurityActions action, Guid userId, IEnumerable<FileShareRecord> shares = null)
        {
            if (entries == null || !entries.Any()) return Task.FromResult(Enumerable.Empty<FileEntry<T>>());

            var user = UserManager.GetUsers(userId);
            var isOutsider = user.IsOutsider(UserManager);

            if (isOutsider && action != FilesSecurityActions.Read) return Task.FromResult(Enumerable.Empty<FileEntry<T>>());

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
                     f.RootFolderType == FolderType.Projects;

            var isVisitor = user.IsVisitor(UserManager);

            if (entries.Any(filter))
            {
                List<Guid> subjects = null;
                foreach (var e in entries.Where(filter))
                {
                    if (!AuthManager.GetAccountByID(TenantManager.GetCurrentTenant().TenantId, userId).IsAuthenticated && userId != FileConstant.ShareLinkId)
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
                        if (folder == null) continue;

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

                    if (e.RootFolderType == FolderType.USER && e.RootFolderCreator == userId && !isVisitor)
                    {
                        // user has all right in his folder
                        result.Add(e);
                        continue;
                    }

                    if (e.RootFolderType == FolderType.Privacy && e.RootFolderCreator == userId && !isVisitor)
                    {
                        // user has all right in his privacy folder
                        result.Add(e);
                        continue;
                    }

                    if (e.FileEntryType == FileEntryType.Folder)
                    {
                        if (folder == null) continue;

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

                    if (e.RootFolderType == FolderType.COMMON && FileSecurityCommon.IsAdministrator(userId))
                    {
                        // administrator in Common has all right
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
                            .FirstOrDefault(r => Equals(r.EntryId, e.ID) && r.EntryType == FileEntryType.File);
                        if (ace == null)
                        {
                            // share on parent folders
                            ace = shares.Where(r => Equals(r.EntryId, file.FolderID) && r.EntryType == FileEntryType.Folder)
                                        .OrderBy(r => r, new SubjectComparer(subjects))
                                        .ThenBy(r => r.Level)
                                        .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                                        .FirstOrDefault();
                        }
                    }
                    else
                    {
                        ace = shares.Where(r => Equals(r.EntryId, e.ID) && r.EntryType == FileEntryType.Folder)
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

                    if (action == FilesSecurityActions.Read && e.Access != FileShare.Restrict) result.Add(e);
                    else if (action == FilesSecurityActions.Comment && (e.Access == FileShare.Comment || e.Access == FileShare.Review || e.Access == FileShare.CustomFilter || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.FillForms && (e.Access == FileShare.FillForms || e.Access == FileShare.Review || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.Review && (e.Access == FileShare.Review || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.CustomFilter && (e.Access == FileShare.CustomFilter || e.Access == FileShare.ReadWrite)) result.Add(e);
                    else if (action == FilesSecurityActions.Edit && e.Access == FileShare.ReadWrite) result.Add(e);
                    else if (action == FilesSecurityActions.Create && e.Access == FileShare.ReadWrite) result.Add(e);
                    else if (e.Access != FileShare.Restrict && e.CreateBy == userId && (e.FileEntryType == FileEntryType.File || folder.FolderType != FolderType.COMMON)) result.Add(e);

                    if (e.CreateBy == userId) e.Access = FileShare.None; //HACK: for client
                }
            }

            // files in bunch
            filter = f => f.RootFolderType == FolderType.BUNCH;
            if (entries.Any(filter))
            {
                var folderDao = daoFactory.GetFolderDao<T>();
                var filteredEntries = entries.Where(filter).ToList();
                var roots = filteredEntries
                        .Select(r => r.RootFolderId)
                        .ToList();

                var rootsFolders = folderDao.GetFoldersAsync(roots);
                var bunches = await folderDao.GetBunchObjectIDsAsync(await rootsFolders.Select(r => r.ID).ToListAsync());
                var findedAdapters = FilesIntegration.GetFileSecurity(bunches);

                foreach (var e in filteredEntries)
                {
                    findedAdapters.TryGetValue(e.RootFolderId.ToString(), out var adapter);

                    if (adapter == null) continue;

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
                            e.Access = FileShare.ReadWrite;
                        else
                            e.Access = FileShare.Read;

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
                var folderDao = daoFactory.GetFolderDao<T>();
                var mytrashId = await folderDao.GetFolderIDTrashAsync(false, userId);
                if (!Equals(mytrashId, 0))
                {
                    result.AddRange(entries.Where(filter).Where(e => Equals(e.RootFolderId, mytrashId)));
                }
            }

            if (FileSecurityCommon.IsAdministrator(userId))
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
            var securityDao = daoFactory.GetSecurityDao<T>();
            var r = new FileShareRecord
            {
                Tenant = TenantManager.GetCurrentTenant().TenantId,
                EntryId = entryId,
                EntryType = entryType,
                Subject = @for,
                Owner = AuthContext.CurrentAccount.ID,
                Share = share,
            };
            return securityDao.SetShareAsync(r);
        }

        public Task<IEnumerable<FileShareRecord>> GetSharesAsync<T>(IEnumerable<FileEntry<T>> entries)
        {
            return daoFactory.GetSecurityDao<T>().GetSharesAsync(entries);
        }

        public Task<IEnumerable<FileShareRecord>> GetSharesAsync<T>(FileEntry<T> entry)
        {
            return daoFactory.GetSecurityDao<T>().GetSharesAsync(entry);
        }

        public async Task<List<FileEntry>> GetSharesForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            var securityDao = daoFactory.GetSecurityDao<int>();
            var subjects = GetUserSubjects(AuthContext.CurrentAccount.ID);
            IEnumerable<FileShareRecord> records = await securityDao.GetSharesAsync(subjects);

            var result = new List<FileEntry>();
            result.AddRange(await GetSharesForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            result.AddRange(await GetSharesForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            return result;
        }

        private async Task<List<FileEntry>> GetSharesForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            var folderDao = daoFactory.GetFolderDao<T>();
            var fileDao = daoFactory.GetFileDao<T>();
            var securityDao = daoFactory.GetSecurityDao<T>();

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
                        folderIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
                else
                {
                    if (!fileIds.ContainsKey((T)r.firstRecord.EntryId))
                        fileIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }

            var entries = new List<FileEntry<T>>();

            if (filterType != FilterType.FoldersOnly)
            {
                var files = await fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent).ToListAsync();
                var share = await GlobalFolder.GetFolderShareAsync<T>(daoFactory);

                files.ForEach(x =>
                {
                    if (fileIds.TryGetValue(x.ID, out var access))
                    {
                        x.Access = fileIds[x.ID];
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
                var share = await GlobalFolder.GetFolderShareAsync<T>(daoFactory);
                folders.ForEach(x =>
                {
                    if (folderIds.TryGetValue(x.ID, out var access))
                    {
                        x.Access = folderIds[x.ID];
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
                                    && f.RootFolderCreator != AuthContext.CurrentAccount.ID // don't show my files
                ).ToList();

            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))
            {
                entries = entries.Where(r => !r.ProviderEntry).ToList();
            }

            var failedEntries = entries.Where(x => !string.IsNullOrEmpty(x.Error));
            var failedRecords = new List<FileShareRecord>();

            foreach (var failedEntry in failedEntries)
            {
                var entryType = failedEntry.FileEntryType;

                var failedRecord = records.First(x => x.EntryId.Equals(failedEntry.ID) && x.EntryType == entryType);

                failedRecord.Share = FileShare.None;

                failedRecords.Add(failedRecord);
            }

            if (failedRecords.Count > 0)
            {
                await securityDao.DeleteShareRecordsAsync(failedRecords);
            }

            return entries.Where(x => string.IsNullOrEmpty(x.Error)).Cast<FileEntry>().ToList();
        }

        public async Task<List<FileEntry>> GetPrivacyForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            var securityDao = daoFactory.GetSecurityDao<int>();
            var subjects = GetUserSubjects(AuthContext.CurrentAccount.ID);
            IEnumerable<FileShareRecord> records = await securityDao.GetSharesAsync(subjects);

            var result = new List<FileEntry>();
            result.AddRange(await GetPrivacyForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            result.AddRange(await GetPrivacyForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            return result;
        }

        private async Task<List<FileEntry<T>>> GetPrivacyForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            var folderDao = daoFactory.GetFolderDao<T>();
            var fileDao = daoFactory.GetFileDao<T>();

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
                        folderIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
                else
                {
                    if (!fileIds.ContainsKey((T)r.firstRecord.EntryId))
                        fileIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }

            var entries = new List<FileEntry<T>>();

            if (filterType != FilterType.FoldersOnly)
            {
                var files = await fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent).ToListAsync();
                var privateFolder = await GlobalFolder.GetFolderPrivacyAsync<T>(daoFactory);

                files.ForEach(x =>
                {
                    if (fileIds.TryGetValue(x.ID, out var access))
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
                var privacyFolder = await GlobalFolder.GetFolderPrivacyAsync<T>(daoFactory);
                folders.ForEach(x =>
                {
                    if (folderIds.TryGetValue(x.ID, out var access))
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
                                    && f.RootFolderCreator != AuthContext.CurrentAccount.ID // don't show my files
                ).ToList();

            return entries;
        }

        public Task RemoveSubjectAsync<T>(Guid subject)
        {
            return daoFactory.GetSecurityDao<T>().RemoveSubjectAsync(subject);
        }

        public List<Guid> GetUserSubjects(Guid userId)
        {
            // priority order
            // User, Departments, admin, everyone

            var result = new List<Guid> { userId };
            if (userId == FileConstant.ShareLinkId)
                return result;

            result.AddRange(UserManager.GetUserGroups(userId).Select(g => g.ID));
            if (FileSecurityCommon.IsAdministrator(userId)) result.Add(Constants.GroupAdmin.ID);
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
            CustomFilter
        }
    }
}