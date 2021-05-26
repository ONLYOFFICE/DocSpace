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

        public List<Tuple<FileEntry<T>, bool>> CanRead<T>(IEnumerable<FileEntry<T>> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Read);
        }

        public List<Tuple<FileEntry<T>, bool>> CanRead<T>(IEnumerable<FileEntry<T>> entry)
        {
            return Can(entry, AuthContext.CurrentAccount.ID, FilesSecurityActions.Read);
        }

        public bool CanRead<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Read);
        }

        public bool CanComment<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Comment);
        }

        public bool CanFillForms<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.FillForms);
        }

        public bool CanReview<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Review);
        }

        public bool CanCustomFilterEdit<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.CustomFilter);
        }


        public bool CanCreate<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Create);
        }

        public bool CanEdit<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Edit);
        }

        public bool CanDelete<T>(FileEntry<T> entry, Guid userId)
        {
            return Can(entry, userId, FilesSecurityActions.Delete);
        }

        public bool CanRead<T>(FileEntry<T> entry)
        {
            return CanRead(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanComment<T>(FileEntry<T> entry)
        {
            return CanComment(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanCustomFilterEdit<T>(FileEntry<T> entry)
        {
            return CanCustomFilterEdit(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanFillForms<T>(FileEntry<T> entry)
        {
            return CanFillForms(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanReview<T>(FileEntry<T> entry)
        {
            return CanReview(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanCreate<T>(FileEntry<T> entry)
        {
            return CanCreate(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanEdit<T>(FileEntry<T> entry)
        {
            return CanEdit(entry, AuthContext.CurrentAccount.ID);
        }

        public bool CanDelete<T>(FileEntry<T> entry)
        {
            return CanDelete(entry, AuthContext.CurrentAccount.ID);
        }

        public IEnumerable<Guid> WhoCanRead<T>(FileEntry<T> entry)
        {
            return WhoCan(entry, FilesSecurityActions.Read);
        }

        private IEnumerable<Guid> WhoCan<T>(FileEntry<T> entry, FilesSecurityActions action)
        {
            var shares = GetShares(entry);

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
                        var root = folderDao.GetFolder(entry.RootFolderId);
                        if (root != null)
                        {
                            var path = folderDao.GetBunchObjectID(root.ID);

                            var adapter = FilesIntegration.GetFileSecurity(path);

                            if (adapter != null)
                            {
                                return adapter.WhoCanRead(entry);
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

            return shares.SelectMany(x =>
                                         {
                                             var groupInfo = UserManager.GetGroupInfo(x.Subject);

                                             if (groupInfo.ID != Constants.LostGroupInfo.ID)
                                                 return
                                                     UserManager.GetUsersByGroup(groupInfo.ID)
                                                                .Where(p => p.Status == EmployeeStatus.Active)
                                                                .Select(y => y.ID);

                                             return new[] { x.Subject };
                                         })
                         .Distinct()
                         .Where(x => Can(entry, x, action))
                         .ToList();
        }

        public IEnumerable<File<T>> FilterRead<T>(IEnumerable<File<T>> entries)
        {
            return Filter(entries, FilesSecurityActions.Read, AuthContext.CurrentAccount.ID).Cast<File<T>>();
        }

        public IEnumerable<Folder<T>> FilterRead<T>(IEnumerable<Folder<T>> entries)
        {
            return Filter(entries, FilesSecurityActions.Read, AuthContext.CurrentAccount.ID).Cast<Folder<T>>();
        }

        public IEnumerable<File<T>> FilterEdit<T>(IEnumerable<File<T>> entries)
        {
            return Filter(entries.Cast<FileEntry<T>>(), FilesSecurityActions.Edit, AuthContext.CurrentAccount.ID).Cast<File<T>>();
        }

        public IEnumerable<Folder<T>> FilterEdit<T>(IEnumerable<Folder<T>> entries)
        {
            return Filter(entries.Cast<FileEntry<T>>(), FilesSecurityActions.Edit, AuthContext.CurrentAccount.ID).Cast<Folder<T>>();
        }

        private bool Can<T>(FileEntry<T> entry, Guid userId, FilesSecurityActions action)
        {
            return Filter(new[] { entry }, action, userId).Any();
        }

        private List<Tuple<FileEntry<T>, bool>> Can<T>(IEnumerable<FileEntry<T>> entry, Guid userId, FilesSecurityActions action)
        {
            var filtres = Filter(entry, action, userId);
            return entry.Select(r => new Tuple<FileEntry<T>, bool>(r, filtres.Any(a => a.ID.Equals(r.ID)))).ToList();
        }

        private IEnumerable<FileEntry<T>> Filter<T>(IEnumerable<FileEntry<T>> entries, FilesSecurityActions action, Guid userId)
        {
            if (entries == null || !entries.Any()) return Enumerable.Empty<FileEntry<T>>();

            var user = UserManager.GetUsers(userId);
            var isOutsider = user.IsOutsider(UserManager);

            if (isOutsider && action != FilesSecurityActions.Read) return Enumerable.Empty<FileEntry<T>>();

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
                var subjects = GetUserSubjects(userId);
                List<FileShareRecord> shares = null;
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

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((IFolder)e).FolderType == FolderType.Projects)
                    {
                        // Root Projects folder read-only
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((IFolder)e).FolderType == FolderType.SHARE)
                    {
                        // Root Share folder read-only
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((IFolder)e).FolderType == FolderType.Recent)
                    {
                        // Recent folder read-only
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((IFolder)e).FolderType == FolderType.Favorites)
                    {
                        // Favorites folder read-only
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder && ((IFolder)e).FolderType == FolderType.Templates)
                    {
                        // Templates folder read-only
                        continue;
                    }

                    if (isVisitor && e.ProviderEntry)
                    {
                        continue;
                    }

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

                    if (DefaultCommonShare == FileShare.Read && action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((IFolder)e).FolderType == FolderType.COMMON)
                    {
                        // all can read Common folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((IFolder)e).FolderType == FolderType.SHARE)
                    {
                        // all can read Share folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((IFolder)e).FolderType == FolderType.Recent)
                    {
                        // all can read recent folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((IFolder)e).FolderType == FolderType.Favorites)
                    {
                        // all can read favorites folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && e.FileEntryType == FileEntryType.Folder &&
                        ((IFolder)e).FolderType == FolderType.Templates)
                    {
                        // all can read templates folder
                        result.Add(e);
                        continue;
                    }


                    if (e.RootFolderType == FolderType.COMMON && FileSecurityCommon.IsAdministrator(userId))
                    {
                        // administrator in Common has all right
                        result.Add(e);
                        continue;
                    }

                    if (shares == null)
                    {
                        shares = GetShares(entries).Join(subjects, r => r.Subject, s => s, (r, s) => r).ToList();
                        // shares ordered by level
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
                            ace = shares.Where(r => Equals(r.EntryId, ((File<T>)e).FolderID) && r.EntryType == FileEntryType.Folder)
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
                    else if (e.Access != FileShare.Restrict && e.CreateBy == userId && (e.FileEntryType == FileEntryType.File || ((IFolder)e).FolderType != FolderType.COMMON)) result.Add(e);

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

                var rootsFolders = folderDao.GetFolders(roots);
                var bunches = folderDao.GetBunchObjectIDs(rootsFolders.Select(r => r.ID).ToList());
                var findedAdapters = FilesIntegration.GetFileSecurity(bunches);

                foreach (var e in filteredEntries)
                {
                    findedAdapters.TryGetValue(e.RootFolderId.ToString(), out var adapter);

                    if (adapter == null) continue;

                    if (adapter.CanRead(e, userId) &&
                        adapter.CanCreate(e, userId) &&
                        adapter.CanEdit(e, userId) &&
                        adapter.CanDelete(e, userId))
                    {
                        e.Access = FileShare.None;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.Comment && adapter.CanComment(e, userId))
                    {
                        e.Access = FileShare.Comment;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.FillForms && adapter.CanFillForms(e, userId))
                    {
                        e.Access = FileShare.FillForms;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.Review && adapter.CanReview(e, userId))
                    {
                        e.Access = FileShare.Review;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.CustomFilter && adapter.CanCustomFilterEdit(e, userId))
                    {
                        e.Access = FileShare.CustomFilter;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.Create && adapter.CanCreate(e, userId))
                    {
                        e.Access = FileShare.ReadWrite;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.Delete && adapter.CanDelete(e, userId))
                    {
                        e.Access = FileShare.ReadWrite;
                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.Read && adapter.CanRead(e, userId))
                    {
                        if (adapter.CanCreate(e, userId) ||
                            adapter.CanDelete(e, userId) ||
                            adapter.CanEdit(e, userId))
                            e.Access = FileShare.ReadWrite;
                        else
                            e.Access = FileShare.Read;

                        result.Add(e);
                    }
                    else if (action == FilesSecurityActions.Edit && adapter.CanEdit(e, userId))
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
                var mytrashId = folderDao.GetFolderIDTrash(false, userId);
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

        public void Share<T>(T entryId, FileEntryType entryType, Guid @for, FileShare share)
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
            securityDao.SetShare(r);
        }

        public IEnumerable<FileShareRecord> GetShares<T>(IEnumerable<FileEntry<T>> entries)
        {
            return daoFactory.GetSecurityDao<T>().GetShares(entries);
        }

        public IEnumerable<FileShareRecord> GetShares<T>(FileEntry<T> entry)
        {
            return daoFactory.GetSecurityDao<T>().GetShares(entry);
        }

        public List<FileEntry> GetSharesForMe(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            var securityDao = daoFactory.GetSecurityDao<int>();
            var subjects = GetUserSubjects(AuthContext.CurrentAccount.ID);
            var records = securityDao.GetShares(subjects);

            var result = new List<FileEntry>();
            result.AddRange(GetSharesForMe<int>(records.Where(r=> r.EntryId.GetType() == typeof(int)), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            result.AddRange(GetSharesForMe<string>(records.Where(r => r.EntryId.GetType() == typeof(string)), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            return result;
        }

        private List<FileEntry> GetSharesForMe<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
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
                var files = fileDao.GetFilesFiltered(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);

                files.ForEach(x =>
                    {
                        if (fileIds.ContainsKey(x.ID))
                        {
                            x.Access = fileIds[x.ID];
                            x.FolderIdDisplay = GlobalFolder.GetFolderShare<T>(daoFactory);
                        }
                    });

                entries.AddRange(files);
            }

            if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
            {
                var folders = folderDao.GetFolders(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

                if (withSubfolders)
                {
                    folders = FilterRead(folders).ToList();
                }
                folders.ForEach(x =>
                    {
                        if (folderIds.ContainsKey(x.ID))
                        {
                            x.Access = folderIds[x.ID];
                            x.FolderIdDisplay = GlobalFolder.GetFolderShare<T>(daoFactory);
                        }
                    });

                entries.AddRange(folders.Cast<FileEntry<T>>());
            }

            if (filterType != FilterType.FoldersOnly && withSubfolders)
            {
                var filesInSharedFolders = fileDao.GetFiles(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
                filesInSharedFolders = FilterRead(filesInSharedFolders).ToList();
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

            if (failedRecords.Any())
            {
                securityDao.DeleteShareRecords(failedRecords);
            }

            return entries.Where(x => string.IsNullOrEmpty(x.Error)).Cast<FileEntry>().ToList();
        }

        public List<FileEntry> GetPrivacyForMe(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
        {
            var securityDao = daoFactory.GetSecurityDao<int>();
            var subjects = GetUserSubjects(AuthContext.CurrentAccount.ID);
            var records = securityDao.GetShares(subjects);

            var result = new List<FileEntry>();
            result.AddRange(GetPrivacyForMe<int>(records.Where(r => r.EntryId.GetType() == typeof(int)), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            result.AddRange(GetPrivacyForMe<string>(records.Where(r => r.EntryId.GetType() == typeof(string)), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders));
            return result;
        }

        private List<FileEntry<T>> GetPrivacyForMe<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
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
                var files = fileDao.GetFilesFiltered(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);

                files.ForEach(x =>
                {
                    if (fileIds.ContainsKey(x.ID))
                    {
                        x.Access = fileIds[x.ID];
                        x.FolderIdDisplay = GlobalFolder.GetFolderPrivacy<T>(daoFactory);
                    }
                });

                entries.AddRange(files);
            }

            if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
            {
                var folders = folderDao.GetFolders(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

                if (withSubfolders)
                {
                    folders = FilterRead(folders).ToList();
                }
                folders.ForEach(x =>
                {
                    if (folderIds.ContainsKey(x.ID))
                    {
                        x.Access = folderIds[x.ID];
                        x.FolderIdDisplay = GlobalFolder.GetFolderPrivacy<T>(daoFactory);
                    }
                });

                entries.AddRange(folders.Cast<FileEntry<T>>());
            }

            if (filterType != FilterType.FoldersOnly && withSubfolders)
            {
                var filesInSharedFolders = fileDao.GetFiles(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
                filesInSharedFolders = FilterRead(filesInSharedFolders).ToList();
                entries.AddRange(filesInSharedFolders);
                entries = entries.Distinct().ToList();
            }

            entries = entries.Where(f =>
                                    f.RootFolderType == FolderType.Privacy // show users files
                                    && f.RootFolderCreator != AuthContext.CurrentAccount.ID // don't show my files
                ).ToList();

            return entries;
        }


        public void RemoveSubject<T>(Guid subject)
        {
            daoFactory.GetSecurityDao<T>().RemoveSubject(subject);
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

        private class SubjectComparer : IComparer<FileShareRecord>
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