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
using System.Security;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Web.Files.Classes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Web.Files.Utils
{

    [Singletone]
    public class FileMarkerHelper<T>
    {
        private IServiceProvider ServiceProvider { get; }
        public ILog Log { get; }
        public DistributedTaskQueue Tasks { get; set; }

        public FileMarkerHelper(
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> optionsMonitor,
            DistributedTaskQueueOptionsManager distributedTaskQueueOptionsManager)
        {
            ServiceProvider = serviceProvider;
            Log = optionsMonitor.CurrentValue;
            Tasks = distributedTaskQueueOptionsManager.Get<AsyncTaskData<T>>();
        }

        internal void Add(AsyncTaskData<T> taskData)
        {
            Tasks.QueueTask((d, c) => ExecMarkFileAsNew(taskData), taskData);
        }

        private void ExecMarkFileAsNew(AsyncTaskData<T> obj)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
                fileMarker.ExecMarkFileAsNew(obj);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    [Scope(Additional = typeof(FileMarkerExtention))]
    public class FileMarker
    {
        private readonly ICache cache;

        private const string CacheKeyFormat = "MarkedAsNew/{0}/folder_{1}";

        private TenantManager TenantManager { get; }
        private UserManager UserManager { get; }
        private IDaoFactory DaoFactory { get; }
        private GlobalFolder GlobalFolder { get; }
        private FileSecurity FileSecurity { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private AuthContext AuthContext { get; }
        private IServiceProvider ServiceProvider { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }

        public FileMarker(
            TenantManager tenantManager,
            UserManager userManager,
            IDaoFactory daoFactory,
            GlobalFolder globalFolder,
            FileSecurity fileSecurity,
            CoreBaseSettings coreBaseSettings,
            AuthContext authContext,
            IServiceProvider serviceProvider,
            FilesSettingsHelper filesSettingsHelper, 
            ICache cache)
        {
            TenantManager = tenantManager;
            UserManager = userManager;
            DaoFactory = daoFactory;
            GlobalFolder = globalFolder;
            FileSecurity = fileSecurity;
            CoreBaseSettings = coreBaseSettings;
            AuthContext = authContext;
            ServiceProvider = serviceProvider;
            FilesSettingsHelper = filesSettingsHelper;
            this.cache = cache;
        }

        internal void ExecMarkFileAsNew<T>(AsyncTaskData<T> obj)
        {
            TenantManager.SetCurrentTenant(Convert.ToInt32(obj.TenantID));

            var folderDao = DaoFactory.GetFolderDao<T>();
            T parentFolderId;

            if (obj.FileEntry.FileEntryType == FileEntryType.File)
                parentFolderId = ((File<T>)obj.FileEntry).FolderID;
            else
                parentFolderId = ((Folder<T>)obj.FileEntry).ID;
            var parentFolders = folderDao.GetParentFolders(parentFolderId);
            parentFolders.Reverse();

            var userIDs = obj.UserIDs;

            var userEntriesData = new Dictionary<Guid, List<FileEntry>>();

            if (obj.FileEntry.RootFolderType == FolderType.BUNCH)
            {
                if (!userIDs.Any()) return;

                parentFolders.Add(folderDao.GetFolder(GlobalFolder.GetFolderProjects<T>(DaoFactory)));

                var entries = new List<FileEntry> { obj.FileEntry };
                entries = entries.Concat(parentFolders).ToList();

                userIDs.ForEach(userID =>
                                        {
                                            if (userEntriesData.ContainsKey(userID))
                                                userEntriesData[userID].AddRange(entries);
                                            else
                                                userEntriesData.Add(userID, entries);

                                            RemoveFromCahce(GlobalFolder.GetFolderProjects(DaoFactory), userID);
                                        });
            }
            else
            {
                var filesSecurity = FileSecurity;

                if (!userIDs.Any())
                {
                    userIDs = filesSecurity.WhoCanRead(obj.FileEntry).Where(x => x != obj.CurrentAccountId).ToList();
                }
                if (obj.FileEntry.ProviderEntry)
                {
                    userIDs = userIDs.Where(u => !UserManager.GetUsers(u).IsVisitor(UserManager)).ToList();
                }

                parentFolders.ForEach(parentFolder =>
                                      filesSecurity
                                          .WhoCanRead(parentFolder)
                                          .Where(userID => userIDs.Contains(userID) && userID != obj.CurrentAccountId)
                                          .ToList()
                                          .ForEach(userID =>
                                                       {
                                                           if (userEntriesData.ContainsKey(userID))
                                                               userEntriesData[userID].Add(parentFolder);
                                                           else
                                                               userEntriesData.Add(userID, new List<FileEntry> { parentFolder });
                                                       })
                    );



                if (obj.FileEntry.RootFolderType == FolderType.USER)
                {
                    var folderDaoInt = DaoFactory.GetFolderDao<int>();
                    var folderShare = folderDaoInt.GetFolder(GlobalFolder.GetFolderShare(DaoFactory));

                    foreach (var userID in userIDs)
                    {
                        var userFolderId = GlobalFolder.GetFolderMy(this, DaoFactory);
                        if (Equals(userFolderId, 0)) continue;

                        Folder<int> rootFolder = null;
                        if (obj.FileEntry.ProviderEntry)
                        {
                            rootFolder = obj.FileEntry.RootFolderCreator == userID
                                             ? folderDaoInt.GetFolder(userFolderId)
                                             : folderShare;
                        }
                        else if (!Equals(obj.FileEntry.RootFolderId, userFolderId))
                        {
                            rootFolder = folderShare;
                        }
                        else
                        {
                            RemoveFromCahce(userFolderId, userID);
                        }

                        if (rootFolder == null) continue;

                        if (userEntriesData.ContainsKey(userID))
                            userEntriesData[userID].Add(rootFolder);
                        else
                            userEntriesData.Add(userID, new List<FileEntry> { rootFolder });

                        RemoveFromCahce(rootFolder.ID, userID);
                    }
                }
                else if (obj.FileEntry.RootFolderType == FolderType.COMMON)
                {
                    userIDs.ForEach(userID => RemoveFromCahce(GlobalFolder.GetFolderCommon(this, DaoFactory), userID));

                    if (obj.FileEntry.ProviderEntry)
                    {
                        var commonFolder = folderDao.GetFolder(GlobalFolder.GetFolderCommon<T>(this, DaoFactory));
                        userIDs.ForEach(userID =>
                                            {
                                                if (userEntriesData.ContainsKey(userID))
                                                    userEntriesData[userID].Add(commonFolder);
                                                else
                                                    userEntriesData.Add(userID, new List<FileEntry> { commonFolder });

                                                RemoveFromCahce(GlobalFolder.GetFolderCommon(this, DaoFactory), userID);
                                            });
                    }
                }
                else if (obj.FileEntry.RootFolderType == FolderType.Privacy)
                {
                    foreach (var userID in userIDs)
                    {
                        var privacyFolderId = folderDao.GetFolderIDPrivacy(false, userID);
                        if (Equals(privacyFolderId, 0)) continue;

                        var rootFolder = folderDao.GetFolder(privacyFolderId);
                        if (rootFolder == null) continue;

                        if (userEntriesData.ContainsKey(userID))
                            userEntriesData[userID].Add(rootFolder);
                        else
                            userEntriesData.Add(userID, new List<FileEntry> { rootFolder });

                        RemoveFromCahce(rootFolder.ID, userID);
                    }
                }

                userIDs.ForEach(userID =>
                                    {
                                        if (userEntriesData.ContainsKey(userID))
                                            userEntriesData[userID].Add(obj.FileEntry);
                                        else
                                            userEntriesData.Add(userID, new List<FileEntry> { obj.FileEntry });
                                    });
            }

            var tagDao = DaoFactory.GetTagDao<T>();
            var newTags = new List<Tag>();
            var updateTags = new List<Tag>();

            foreach (var userID in userEntriesData.Keys)
            {
                if (tagDao.GetNewTags(userID, obj.FileEntry).Any())
                    continue;

                var entries = userEntriesData[userID].Distinct().ToList();

                GetNewTags(userID, entries.OfType<FileEntry<int>>().ToList());
                GetNewTags(userID, entries.OfType<FileEntry<string>>().ToList());
            }

            if (updateTags.Any())
                tagDao.UpdateNewTags(updateTags);
            if (newTags.Any())
                tagDao.SaveTags(newTags);

            void GetNewTags<T1>(Guid userID, List<FileEntry<T1>> entries)
            {
                var tagDao1 = DaoFactory.GetTagDao<T1>();
                var exist = tagDao1.GetNewTags(userID, entries).ToList();
                var update = exist.Where(t => t.EntryType == FileEntryType.Folder).ToList();
                update.ForEach(t => t.Count++);
                updateTags.AddRange(update);

                entries.ForEach(entry =>
                                    {
                                        if (entry != null && exist.All(tag => tag != null && !tag.EntryId.Equals(entry.ID)))
                                        {
                                            newTags.Add(Tag.New(userID, entry));
                                        }
                                    });
            }
        }

        public void MarkAsNew<T>(FileEntry<T> fileEntry, List<Guid> userIDs = null)
        {
            if (CoreBaseSettings.Personal) return;

            if (fileEntry == null) return;
            userIDs ??= new List<Guid>();

            var taskData = ServiceProvider.GetService<AsyncTaskData<T>>();
            taskData.FileEntry = (FileEntry<T>)fileEntry.Clone();
            taskData.UserIDs = userIDs;

            if (fileEntry.RootFolderType == FolderType.BUNCH && !userIDs.Any())
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var path = folderDao.GetBunchObjectID(fileEntry.RootFolderId);

                var projectID = path.Split('/').Last();
                if (string.IsNullOrEmpty(projectID)) return;

                var projectTeam = FileSecurity.WhoCanRead(fileEntry)
                                        .Where(x => x != AuthContext.CurrentAccount.ID).ToList();

                if (!projectTeam.Any()) return;

                taskData.UserIDs = projectTeam;
            }

            ServiceProvider.GetService<FileMarkerHelper<T>>().Add(taskData);
        }

        public void RemoveMarkAsNew<T>(FileEntry<T> fileEntry, Guid userID = default)
        {
            if (CoreBaseSettings.Personal) return;

            userID = userID.Equals(default) ? AuthContext.CurrentAccount.ID : userID;

            if (fileEntry == null) return;

            var tagDao = DaoFactory.GetTagDao<T>();
            var internalFolderDao = DaoFactory.GetFolderDao<int>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            if (!tagDao.GetNewTags(userID, fileEntry).Any()) return;

            T folderID;
            int valueNew;
            var userFolderId = internalFolderDao.GetFolderIDUser(false, userID);
            var privacyFolderId = internalFolderDao.GetFolderIDPrivacy(false, userID);

            var removeTags = new List<Tag>();

            if (fileEntry.FileEntryType == FileEntryType.File)
            {
                folderID = ((File<T>)fileEntry).FolderID;

                removeTags.Add(Tag.New(userID, fileEntry));
                valueNew = 1;
            }
            else
            {
                folderID = fileEntry.ID;

                var listTags = tagDao.GetNewTags(userID, (Folder<T>)fileEntry, true).ToList();
                valueNew = listTags.FirstOrDefault(tag => tag.EntryId.Equals(fileEntry.ID)).Count;

                if (Equals(fileEntry.ID, userFolderId) || Equals(fileEntry.ID, GlobalFolder.GetFolderCommon(this, DaoFactory)) || Equals(fileEntry.ID, GlobalFolder.GetFolderShare(DaoFactory)))
                {
                    var folderTags = listTags.Where(tag => tag.EntryType == FileEntryType.Folder);

                    var providerFolderTags = folderTags.Select(tag => new KeyValuePair<Tag, Folder<T>>(tag, folderDao.GetFolder((T)tag.EntryId)))
                                                       .Where(pair => pair.Value != null && pair.Value.ProviderEntry).ToList();

                    foreach (var providerFolderTag in providerFolderTags)
                    {
                        listTags.Remove(providerFolderTag.Key);
                        listTags.AddRange(tagDao.GetNewTags(userID, providerFolderTag.Value, true));
                    }
                }

                removeTags.AddRange(listTags);
            }

            var parentFolders = folderDao.GetParentFolders(folderID);
            parentFolders.Reverse();

            var rootFolder = parentFolders.LastOrDefault();
            int rootFolderId = default;
            int cacheFolderId = default;
            if (rootFolder == null)
            {
            }
            else if (rootFolder.RootFolderType == FolderType.BUNCH)
            {
                cacheFolderId = rootFolderId = GlobalFolder.GetFolderProjects(DaoFactory);
            }
            else if (rootFolder.RootFolderType == FolderType.COMMON)
            {
                if (rootFolder.ProviderEntry)
                    cacheFolderId = rootFolderId = GlobalFolder.GetFolderCommon(this, DaoFactory);
                else
                    cacheFolderId = GlobalFolder.GetFolderCommon(this, DaoFactory);
            }
            else if (rootFolder.RootFolderType == FolderType.USER)
            {
                if (rootFolder.ProviderEntry && rootFolder.RootFolderCreator == userID)
                    cacheFolderId = rootFolderId = userFolderId;
                else if (!rootFolder.ProviderEntry && !Equals(rootFolder.RootFolderId, userFolderId)
                         || rootFolder.ProviderEntry && rootFolder.RootFolderCreator != userID)
                    cacheFolderId = rootFolderId = GlobalFolder.GetFolderShare(DaoFactory);
                else
                    cacheFolderId = userFolderId;
            }
            else if (rootFolder.RootFolderType == FolderType.Privacy)
            {
                if (!Equals(privacyFolderId, 0))
                {
                    cacheFolderId = rootFolderId = privacyFolderId;
                }
            }
            else if (rootFolder.RootFolderType == FolderType.SHARE)
            {
                cacheFolderId = GlobalFolder.GetFolderShare(DaoFactory);
            }

            var updateTags = new List<Tag>();

            if (!rootFolderId.Equals(default))
            {
                UpdateRemoveTags(internalFolderDao.GetFolder(rootFolderId));
            }

            if (!cacheFolderId.Equals(default))
            {
                RemoveFromCahce(cacheFolderId, userID);
            }

            foreach (var parentFolder in parentFolders)
            {
                UpdateRemoveTags(parentFolder);
            }

            if (updateTags.Any())
                tagDao.UpdateNewTags(updateTags);
            if (removeTags.Any())
                tagDao.RemoveTags(removeTags);

            void UpdateRemoveTags<TFolder>(Folder<TFolder> folder)
            {
                var tagDao = DaoFactory.GetTagDao<TFolder>();
                var parentTag = tagDao.GetNewTags(userID, folder).FirstOrDefault();

                if (parentTag != null)
                {
                    parentTag.Count -= valueNew;

                    if (parentTag.Count > 0)
                    {
                        updateTags.Add(parentTag);
                    }
                    else
                    {
                        removeTags.Add(parentTag);
                    }
                }
            }
        }

        public void RemoveMarkAsNewForAll<T>(FileEntry<T> fileEntry)
        {
            List<Guid> userIDs;

            var tagDao = DaoFactory.GetTagDao<T>();
            var tags = tagDao.GetTags(fileEntry.ID, fileEntry.FileEntryType == FileEntryType.File ? FileEntryType.File : FileEntryType.Folder, TagType.New);
            userIDs = tags.Select(tag => tag.Owner).Distinct().ToList();

            foreach (var userID in userIDs)
            {
                RemoveMarkAsNew(fileEntry, userID);
            }
        }

        public int GetRootFoldersIdMarkedAsNew<T>(T rootId)
        {
            var fromCache = GetCountFromCahce(rootId);
            if (fromCache == -1)
            {
                var tagDao = DaoFactory.GetTagDao<T>();
                var folderDao = DaoFactory.GetFolderDao<T>();
                var requestTags = tagDao.GetNewTags(AuthContext.CurrentAccount.ID, folderDao.GetFolder(rootId));
                var requestTag = requestTags.FirstOrDefault(tag => tag.EntryId.Equals(rootId));
                var count = requestTag == null ? 0 : requestTag.Count;
                InsertToCahce(rootId, count);

                return count;
            }
            else if (fromCache > 0)
            {
                return fromCache;
            }

            return 0;
        }

        public List<FileEntry> MarkedItems<T>(Folder<T> folder)
        {
            if (folder == null) throw new ArgumentNullException("folder", FilesCommonResource.ErrorMassage_FolderNotFound);
            if (!FileSecurity.CanRead(folder)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
            if (folder.RootFolderType == FolderType.TRASH && !Equals(folder.ID, GlobalFolder.GetFolderTrash<T>(DaoFactory))) throw new SecurityException(FilesCommonResource.ErrorMassage_ViewTrashItem);

            var tagDao = DaoFactory.GetTagDao<T>();
            var fileDao = DaoFactory.GetFileDao<T>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            var providerFolderDao = DaoFactory.GetFolderDao<string>();
            var providerTagDao = DaoFactory.GetTagDao<string>();
            var tags = (tagDao.GetNewTags(AuthContext.CurrentAccount.ID, folder, true) ?? new List<Tag>()).ToList();

            if (!tags.Any()) return new List<FileEntry>();

            if (Equals(folder.ID, GlobalFolder.GetFolderMy(this, DaoFactory)) || 
                Equals(folder.ID, GlobalFolder.GetFolderCommon(this, DaoFactory)) || 
                Equals(folder.ID, GlobalFolder.GetFolderShare(DaoFactory)))
            {
                var folderTags = tags.Where(tag => tag.EntryType == FileEntryType.Folder && tag.EntryId.GetType() == typeof(string));

                var providerFolderTags = folderTags
                    .Select(tag => new KeyValuePair<Tag, Folder<string>>(tag, providerFolderDao.GetFolder(tag.EntryId.ToString())))
                    .Where(pair => pair.Value != null && pair.Value.ProviderEntry)
                    .ToList();

                providerFolderTags.Reverse();

                foreach (var providerFolderTag in providerFolderTags)
                {
                    tags.AddRange(providerTagDao.GetNewTags(AuthContext.CurrentAccount.ID, providerFolderTag.Value, true));
                }
            }

            tags = tags
                .Where(r => !Equals(r.EntryId, folder.ID))
                .Distinct()
                .ToList();

            //TODO: refactoring
            var entryTagsProvider = GetEntryTags<string>(tags.Where(r=> r.EntryId.GetType() == typeof(string)));
            var entryTagsInternal = GetEntryTags<int>(tags.Where(r=> r.EntryId.GetType() == typeof(int)));

            foreach (var entryTag in entryTagsInternal)
            {
                var parentEntry = entryTagsInternal.Keys
                    .FirstOrDefault(entryCountTag => Equals(entryCountTag.ID, entryTag.Key.FolderID));

                if (parentEntry != null)
                {
                    entryTagsInternal[parentEntry].Count -= entryTag.Value.Count;
                }
            }

            foreach (var entryTag in entryTagsProvider)
                {
                if (int.TryParse(entryTag.Key.FolderID, out var fId))
                {
                    var parentEntryInt = entryTagsInternal.Keys
                            .FirstOrDefault(entryCountTag => Equals(entryCountTag.ID, fId));

                    if (parentEntryInt != null)
                    {
                        entryTagsInternal[parentEntryInt].Count -= entryTag.Value.Count;
                }

                    continue;
            }

                var parentEntry = entryTagsProvider.Keys
                    .FirstOrDefault(entryCountTag => Equals(entryCountTag.ID, entryTag.Key.FolderID));

                if (parentEntry != null)
                {
                    entryTagsProvider[parentEntry].Count -= entryTag.Value.Count;
            }
            }

            var result = new List<FileEntry>();

            GetResult(entryTagsInternal);
            GetResult(entryTagsProvider);

            return result;

            void GetResult<TEntry>(Dictionary<FileEntry<TEntry>, Tag> entryTags)
            {
            foreach (var entryTag in entryTags)
            {
                if (!string.IsNullOrEmpty(entryTag.Key.Error))
                {
                    RemoveMarkAsNew(entryTag.Key);
                    continue;
                }

                if (entryTag.Value.Count > 0)
                {
                    result.Add(entryTag.Key);
                }
            }
        }
        }

        private Dictionary<FileEntry<T>, Tag> GetEntryTags<T>(IEnumerable<Tag> tags)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            var entryTags = new Dictionary<FileEntry<T>, Tag>();

            foreach (var tag in tags)
            {
                var entry = tag.EntryType == FileEntryType.File
                                ? fileDao.GetFile((T)tag.EntryId)
                                : (FileEntry<T>)folderDao.GetFolder((T)tag.EntryId);
                if (entry != null && (!entry.ProviderEntry || FilesSettingsHelper.EnableThirdParty))
                {
                    entryTags.Add(entry, tag);
                }
                else
                {
                    //todo: RemoveMarkAsNew(tag);
                }
            }

            return entryTags;
        }


        public IEnumerable<FileEntry> SetTagsNew<T>(Folder<T> parent, IEnumerable<FileEntry> entries)
        {
            var tagDao = DaoFactory.GetTagDao<T>();
            var folderDao = DaoFactory.GetFolderDao<T>();
            var totalTags = tagDao.GetNewTags(AuthContext.CurrentAccount.ID, parent, false).ToList();

            if (totalTags.Any())
            {
                var parentFolderTag = Equals(GlobalFolder.GetFolderShare<T>(DaoFactory), parent.ID)
                                            ? tagDao.GetNewTags(AuthContext.CurrentAccount.ID, folderDao.GetFolder(GlobalFolder.GetFolderShare<T>(DaoFactory))).FirstOrDefault()
                                            : totalTags.FirstOrDefault(tag => tag.EntryType == FileEntryType.Folder && Equals(tag.EntryId, parent.ID));

                totalTags.Remove(parentFolderTag);
                var countSubNew = 0;
                totalTags.ForEach(tag => countSubNew += tag.Count);

                if (parentFolderTag == null)
                {
                    parentFolderTag = Tag.New(AuthContext.CurrentAccount.ID, parent, 0);
                    parentFolderTag.Id = -1;
                }

                if (parentFolderTag.Count != countSubNew)
                {
                    if (countSubNew > 0)
                    {
                        var diff = parentFolderTag.Count - countSubNew;

                        parentFolderTag.Count -= diff;
                        if (parentFolderTag.Id == -1)
                        {
                            tagDao.SaveTags(parentFolderTag);
                        }
                        else
                        {
                            tagDao.UpdateNewTags(parentFolderTag);
                        }

                        var cacheFolderId = parent.ID;
                        var parentsList = DaoFactory.GetFolderDao<T>().GetParentFolders(parent.ID);
                        parentsList.Reverse();
                        parentsList.Remove(parent);

                        if (parentsList.Any())
                        {
                            var rootFolder = parentsList.Last();
                            T rootFolderId = default;
                            cacheFolderId = rootFolder.ID;
                            if (rootFolder.RootFolderType == FolderType.BUNCH)
                                cacheFolderId = rootFolderId = GlobalFolder.GetFolderProjects<T>(DaoFactory);
                            else if (rootFolder.RootFolderType == FolderType.USER && !Equals(rootFolder.RootFolderId, GlobalFolder.GetFolderMy(this, DaoFactory)))
                                cacheFolderId = rootFolderId = GlobalFolder.GetFolderShare<T>(DaoFactory);

                            if (rootFolderId != null)
                            {
                                parentsList.Add(DaoFactory.GetFolderDao<T>().GetFolder(rootFolderId));
                            }

                            var fileSecurity = FileSecurity;

                            foreach (var folderFromList in parentsList)
                            {
                                var parentTreeTag = tagDao.GetNewTags(AuthContext.CurrentAccount.ID, folderFromList).FirstOrDefault();

                                if (parentTreeTag == null)
                                {
                                    if (fileSecurity.CanRead(folderFromList))
                                    {
                                        tagDao.SaveTags(Tag.New(AuthContext.CurrentAccount.ID, folderFromList, -diff));
                                    }
                                }
                                else
                                {
                                    parentTreeTag.Count -= diff;
                                    tagDao.UpdateNewTags(parentTreeTag);
                                }
                            }
                        }

                        if (cacheFolderId != null)
                        {
                            RemoveFromCahce(cacheFolderId);
                        }
                    }
                    else
                    {
                        RemoveMarkAsNew(parent);
                    }
                }

                SetTagsNew(entries.OfType<FileEntry<int>>().ToList());
                SetTagsNew(entries.OfType<FileEntry<string>>().ToList());
            }

            void SetTagsNew<T1>(List<FileEntry<T1>> fileEntries)
            {
                fileEntries
                    .ForEach(
                    entry =>
                    {
                        var curTag = totalTags.FirstOrDefault(tag => tag.EntryType == entry.FileEntryType && tag.EntryId.Equals(entry.ID));

                        if (entry.FileEntryType == FileEntryType.Folder)
                        {
                            ((IFolder)entry).NewForMe = curTag != null ? curTag.Count : 0;
                        }
                        else if (curTag != null)
                        {
                            entry.IsNew = true;
                        }
                    });
            }

            return entries;
        }

        private void InsertToCahce(object folderId, int count)
        {
            var key = string.Format(CacheKeyFormat, AuthContext.CurrentAccount.ID, folderId);
            cache.Insert(key, count.ToString(), TimeSpan.FromMinutes(10));
        }

        private int GetCountFromCahce(object folderId)
        {
            var key = string.Format(CacheKeyFormat, AuthContext.CurrentAccount.ID, folderId);
            var count = cache.Get<string>(key);
            return count == null ? -1 : int.Parse(count);
        }

        private void RemoveFromCahce(object folderId)
        {
            RemoveFromCahce(folderId, AuthContext.CurrentAccount.ID);
        }

        private void RemoveFromCahce(object folderId, Guid userId)
        {
            var key = string.Format(CacheKeyFormat, userId, folderId);
            cache.Remove(key);
        }
    }

    [Transient]
    public class AsyncTaskData<T> : DistributedTask
    {
        public AsyncTaskData(TenantManager tenantManager, AuthContext authContext) : base()
        {
            TenantID = tenantManager.GetCurrentTenant().TenantId;
            CurrentAccountId = authContext.CurrentAccount.ID;
        }

        public int TenantID { get; private set; }

        public FileEntry<T> FileEntry { get; set; }

        public List<Guid> UserIDs { get; set; }

        public Guid CurrentAccountId { get; set; }
    }

    public class FileMarkerExtention
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<AsyncTaskData<int>>();
            services.TryAdd<FileMarkerHelper<int>>();
            services.AddDistributedTaskQueueService<AsyncTaskData<int>>(1);

            services.TryAdd<AsyncTaskData<string>>();
            services.TryAdd<FileMarkerHelper<string>>();
            services.AddDistributedTaskQueueService<AsyncTaskData<string>>(1);
        }
    }
}