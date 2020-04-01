/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Data;
using System.Linq;
using ASC.Common.Logging;
using ASC.ElasticSearch;
using ASC.Mail.Core.Dao.Expressions.UserFolder;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Models;
using ASC.Mail.Enums;
using ASC.Mail.Exceptions;
using ASC.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Mail.Core.Engine
{
    public class UserFolderEngine
    {
        public int Tenant { get; private set; }
        public string User { get; private set; }

        public ILog Log { get; private set; }

        public EngineFactory Factory { get; private set; }
        public DaoFactory DaoFactory { get; }
        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public IServiceProvider ServiceProvider { get; }
        public SecurityContext SecurityContext { get; }
        public TenantManager TenantManager { get; }

        public UserFolderEngine(
            SecurityContext securityContext,
            TenantManager tenantManager,
            EngineFactory engineFactory,
            DaoFactory daoFactory,
            FactoryIndexerHelper factoryIndexerHelper,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> option)
        {
            SecurityContext = securityContext;
            TenantManager = tenantManager;
            Factory = engineFactory;
            DaoFactory = daoFactory;
            FactoryIndexerHelper = factoryIndexerHelper;
            ServiceProvider = serviceProvider;
            Log = option.Get("ASC.Mail.UserFolderEngine");
        }

        public MailUserFolderData Get(uint id)
        {
            var userFolder = DaoFactory.UserFolderDao.Get(id);

            return ToMailUserFolderData(userFolder);
        }

        public MailUserFolderData GetByMail(uint mailId)
        {
            var userFolder = DaoFactory.UserFolderDao.GetByMail(mailId);

            return ToMailUserFolderData(userFolder);
        }

        public List<MailUserFolderData> GetList(List<uint> ids = null, uint? parentId = null)
        {
            var builder = SimpleUserFoldersExp.CreateBuilder(Tenant, User);

            if (ids != null && ids.Any())
            {
                builder.SetIds(ids);
            }

            if (parentId.HasValue)
            {
                builder.SetParent(parentId.Value);
            }

            var exp = builder.Build();

            var userFolderDataList = DaoFactory.UserFolderDao.GetList(exp)
                .ConvertAll(ToMailUserFolderData);

            return userFolderDataList;
        }

        public MailUserFolderData Create(string name, uint parentId = 0)
        {
            if (string.IsNullOrEmpty(name))
                throw new EmptyFolderException(@"Name is empty");

            var utsNow = DateTime.UtcNow;

            var newUserFolder = new UserFolder
            {
                Id = 0,
                ParentId = parentId,
                Name = name,
                User = User,
                Tenant = Tenant,
                TimeModified = utsNow
            };

            if (parentId > 0)
            {
                var parentUserFolder = DaoFactory.UserFolderDao.Get(parentId);

                if (parentUserFolder == null)
                    throw new ArgumentException(@"Parent folder not found", "parentId");
            }

            if (IsFolderNameAlreadyExists(newUserFolder))
            {
                throw new AlreadyExistsFolderException(
                    string.Format("Folder with name \"{0}\" already exists", newUserFolder.Name));
            }

            using (var tx = DaoFactory.BeginTransaction())
            {
                newUserFolder.Id = DaoFactory.UserFolderDao.Save(newUserFolder);

                if (newUserFolder.Id <= 0)
                    throw new Exception("Save user folder failed");

                var userFolderTreeItem = new UserFolderTreeItem
                {
                    FolderId = newUserFolder.Id,
                    ParentId = newUserFolder.Id,
                    Level = 0
                };

                //itself link
                DaoFactory.UserFolderTreeDao.Save(userFolderTreeItem);

                //full path to root
                DaoFactory.UserFolderTreeDao.InsertFullPathToRoot(newUserFolder.Id, newUserFolder.ParentId);

                tx.Commit();
            }

            DaoFactory.UserFolderDao.RecalculateFoldersCount(newUserFolder.Id);

            return ToMailUserFolderData(newUserFolder);
        }

        public MailUserFolderData Update(uint id, string name, uint? parentId = null)
        {
            if (id < 0)
                throw new ArgumentException("id");

            if (string.IsNullOrEmpty(name))
                throw new EmptyFolderException(@"Name is empty");

            if (parentId.HasValue && id == parentId.Value)
                throw new ArgumentException(@"id equals to parentId", "parentId");

            var oldUserFolder = DaoFactory.UserFolderDao.Get(id);

            if (oldUserFolder == null)
                throw new ArgumentException("Folder not found");

            var newUserFolder = new UserFolder
            {
                Id = id,
                ParentId = parentId ?? oldUserFolder.ParentId,
                Name = name,
                User = User,
                Tenant = Tenant,
                FolderCount = oldUserFolder.FolderCount,
                UnreadCount = oldUserFolder.UnreadCount,
                TotalCount = oldUserFolder.TotalCount,
                UnreadChainCount = oldUserFolder.UnreadChainCount,
                TotalChainCount = oldUserFolder.TotalChainCount,
                TimeModified = oldUserFolder.TimeModified
            };

            if (newUserFolder.Equals(oldUserFolder))
                return ToMailUserFolderData(oldUserFolder);

            if (IsFolderNameAlreadyExists(newUserFolder))
            {
                throw new AlreadyExistsFolderException(
                    string.Format("Folder with name \"{0}\" already exists", newUserFolder.Name));
            }

            var utsNow = DateTime.UtcNow;

            if (newUserFolder.ParentId != oldUserFolder.ParentId)
            {
                if (!CanMoveFolderTo(newUserFolder))
                {
                    throw new MoveFolderException(
                        string.Format("Can't move folder with id=\"{0}\" into the folder with id=\"{1}\"",
                            newUserFolder.Id, newUserFolder.ParentId));
                }
            }

            using (var tx = DaoFactory.BeginTransaction())
            {
                newUserFolder.TimeModified = utsNow;
                DaoFactory.UserFolderDao.Save(newUserFolder);

                if (newUserFolder.ParentId != oldUserFolder.ParentId)
                {
                    DaoFactory.UserFolderTreeDao.Move(newUserFolder.Id, newUserFolder.ParentId);
                }

                tx.Commit();
            }

            var recalcFolders = new List<uint> { newUserFolder.ParentId };

            if (newUserFolder.ParentId > 0)
            {
                recalcFolders.Add(newUserFolder.Id);
            }

            if (oldUserFolder.ParentId != 0 && !recalcFolders.Contains(oldUserFolder.ParentId))
            {
                recalcFolders.Add(oldUserFolder.ParentId);
            }

            recalcFolders.ForEach(fid =>
            {
                DaoFactory.UserFolderDao.RecalculateFoldersCount(fid);
            });

            return ToMailUserFolderData(newUserFolder);
        }

        public void Delete(uint folderId)
        {
            var affectedIds = new List<int>();

            //TODO: Check or increase timeout for DB connection
            //using (var db = new DbManager(Defines.CONNECTION_STRING_NAME, Defines.RecalculateFoldersTimeout))

            var folder = DaoFactory.UserFolderDao.Get(folderId);
            if (folder == null)
                return;

            using (var tx = DaoFactory.BeginTransaction())
            {
                //Find folder sub-folders
                var expTree = SimpleUserFoldersTreeExp.CreateBuilder()
                    .SetParent(folder.Id)
                    .Build();

                var removeFolderIds = DaoFactory.UserFolderTreeDao.Get(expTree)
                    .ConvertAll(f => f.FolderId);

                if (!removeFolderIds.Contains(folderId))
                    removeFolderIds.Add(folderId);

                //Remove folder with subfolders
                var expFolders = SimpleUserFoldersExp.CreateBuilder(Tenant, User)
                    .SetIds(removeFolderIds)
                    .Build();

                DaoFactory.UserFolderDao.Remove(expFolders);

                //Remove folder tree info
                expTree = SimpleUserFoldersTreeExp.CreateBuilder()
                    .SetIds(removeFolderIds)
                    .Build();

                DaoFactory.UserFolderTreeDao.Remove(expTree);

                //Move mails to trash
                foreach (var id in removeFolderIds)
                {
                    var listMailIds = DaoFactory.UserFolderXMailDao.GetMailIds(id);

                    if (!listMailIds.Any()) continue;

                    affectedIds.AddRange(listMailIds);

                    //Move mails to trash
                    Factory.MessageEngine.SetFolder(DaoFactory, listMailIds, FolderType.Trash);

                    //Remove listMailIds from 'mail_user_folder_x_mail'
                    DaoFactory.UserFolderXMailDao.Remove(listMailIds);
                }

                tx.Commit();
            }

            DaoFactory.UserFolderDao.RecalculateFoldersCount(folder.ParentId);

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t) || !affectedIds.Any())
                return;

            var data = new MailWrapper
            {
                Folder = (byte)FolderType.Trash
            };

            Factory.IndexEngine.Update(data, s => s.In(m => m.Id, affectedIds.ToArray()), wrapper => wrapper.Unread);
        }

        public void SetFolderMessages(uint userFolderId, List<int> ids)
        {
            DaoFactory.UserFolderXMailDao.Remove(ids);

            DaoFactory.UserFolderXMailDao.SetMessagesFolder(ids, userFolderId);
        }

        public void DeleteFolderMessages(IDaoFactory daoFactory, List<int> ids)
        {
            DaoFactory.UserFolderXMailDao.Remove(ids);
        }

        public void RecalculateCounters(IDaoFactory daoFactory, List<int> userFolderIds)
        {
            var totalUfMessList = DaoFactory.MailInfoDao.GetMailUserFolderCount(userFolderIds);
            var unreadUfMessUfList = DaoFactory.MailInfoDao.GetMailUserFolderCount(userFolderIds, true);
            var totalUfConvList = DaoFactory.ChainDao.GetChainUserFolderCount(userFolderIds);
            var unreadUfConvUfList = DaoFactory.ChainDao.GetChainUserFolderCount(userFolderIds, true);

            foreach (var id in userFolderIds.Select(id => (uint)id))
            {
                int totalMess;
                totalUfMessList.TryGetValue(id, out totalMess);

                int unreadMess;
                unreadUfMessUfList.TryGetValue(id, out unreadMess);

                int totalConv;
                totalUfConvList.TryGetValue(id, out totalConv);

                int unreadConv;
                unreadUfConvUfList.TryGetValue(id, out unreadConv);

                SetFolderCounters(id,
                    unreadMess, totalMess, unreadConv, totalConv);
            }
        }

        public void SetFolderCounters(
            uint userFolderId,
            int? unreadMess = null,
            int? totalMess = null,
            int? unreadConv = null,
            int? totalConv = null)
        {
            try
            {
                var res = DaoFactory.UserFolderDao
                    .SetFolderCounters(userFolderId, unreadMess, totalMess, unreadConv,
                        totalConv);

                if (res == 0)
                    throw new Exception("Need recalculation");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("UserFolderEngine->SetFolderCounters() Exception: {0}", ex.ToString());
                //TODO: Think about recalculation
                //var engine = new EngineFactory(Tenant, User);
                //engine.OperationEngine.RecalculateFolders();
            }
        }

        public void ChangeFolderCounters(
            uint userFolderId,
            int? unreadMessDiff = null,
            int? totalMessDiff = null,
            int? unreadConvDiff = null,
            int? totalConvDiff = null)
        {
            try
            {
                var res = DaoFactory.UserFolderDao
                    .ChangeFolderCounters(userFolderId, unreadMessDiff, totalMessDiff, unreadConvDiff,
                        totalConvDiff);

                if (res == 0)
                    throw new Exception("Need recalculation");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("UserFolderEngine->ChangeFolderCounters() Exception: {0}", ex.ToString());
                //TODO: Think about recalculation
                //var engine = new EngineFactory(Tenant, User);
                //engine.OperationEngine.RecalculateFolders();
            }
        }

        private bool IsFolderNameAlreadyExists(UserFolder newUserFolder)
        {
            //Find folder sub-folders
            var exp = SimpleUserFoldersExp.CreateBuilder(Tenant, User)
                .SetParent(newUserFolder.ParentId)
                .Build();

            var listExistinFolders = DaoFactory.UserFolderDao.GetList(exp);

            return listExistinFolders.Any(existinFolder => existinFolder.Name.Equals(newUserFolder.Name,
                StringComparison.InvariantCultureIgnoreCase));
        }

        private bool CanMoveFolderTo(UserFolder newUserFolder)
        {
            //Find folder sub-folders
            var exp = SimpleUserFoldersExp.CreateBuilder(Tenant, User)
                .SetParent(newUserFolder.Id)
                .SetIds(new List<uint> { newUserFolder.ParentId})
                .Build();

            var listExistinFolders = DaoFactory.UserFolderDao.GetList(exp);

            return !listExistinFolders.Any();
        }

        // ReSharper disable once UnusedMember.Local
        private static UserFolder ToUserFolder(MailUserFolderData folder, int tenant, string user)
        {
            if (folder == null)
                return null;

            var utcNow = DateTime.UtcNow;

            var userFolder = new UserFolder
            {
                Tenant = tenant,
                User = user,
                Id = folder.Id,
                ParentId = folder.ParentId,
                Name = folder.Name,
                FolderCount = folder.FolderCount,
                UnreadCount = folder.UnreadCount,
                TotalCount = folder.TotalCount,
                UnreadChainCount = folder.UnreadChainCount,
                TotalChainCount = folder.TotalChainCount,
                TimeModified = utcNow
            };

            return userFolder;
        }

        private static MailUserFolderData ToMailUserFolderData(UserFolder folder)
        {
            if (folder == null)
                return null;

            var userFolderData = new MailUserFolderData
            {
                Id = folder.Id,
                ParentId = folder.ParentId,
                Name = folder.Name,
                FolderCount = folder.FolderCount,
                UnreadCount = folder.UnreadCount,
                TotalCount = folder.TotalCount,
                UnreadChainCount = folder.UnreadChainCount,
                TotalChainCount = folder.TotalChainCount
            };

            return userFolderData;
        }
    }
}
