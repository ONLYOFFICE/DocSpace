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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.ElasticSearch;
using ASC.Mail.Core.Dao.Expressions.Attachment;
using ASC.Mail.Core.Dao.Expressions.Conversation;
using ASC.Mail.Core.Dao.Expressions.Message;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Data.Storage;
using ASC.Mail.Enums;
using ASC.Mail.Extensions;
using ASC.Mail.Models;
using ASC.Mail.Utils;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.Extensions.DependencyInjection;
using ASC.Common;

namespace ASC.Mail.Core.Engine
{
    public class MessageEngine
    {
        public DaoFactory DaoFactory { get; }
        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public TenantUtil TenantUtil { get; }
        public CoreSettings CoreSettings { get; }
        public FactoryIndexer<MailWrapper> FactoryIndexer { get; }
        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public IServiceProvider ServiceProvider { get; }
        public StorageFactory StorageFactory { get; }
        public ILog Log { get; private set; }
        public int Tenant
        {
            get
            {
                return TenantManager.GetCurrentTenant().TenantId;
            }
        }

        public string User
        {
            get
            {
                return SecurityContext.CurrentAccount.ID.ToString();
            }
        }

        public IDataStore Storage { get; set; }

        public EngineFactory Factory { get; private set; }

        public MessageEngine(
            DaoFactory daoFactory,
            TenantManager tenantManager,
            SecurityContext securityContext,
            TenantUtil tenantUtil,
            CoreSettings coreSettings,
            FactoryIndexer<MailWrapper> factoryIndexer,
            FactoryIndexerHelper factoryIndexerHelper,
            IServiceProvider serviceProvider,
            StorageFactory storageFactory,
            IOptionsMonitor<ILog> option)
        {
            DaoFactory = daoFactory;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            TenantUtil = tenantUtil;
            CoreSettings = coreSettings;
            FactoryIndexer = factoryIndexer;
            FactoryIndexerHelper = factoryIndexerHelper;
            ServiceProvider = serviceProvider;
            StorageFactory = storageFactory;

            Storage = StorageFactory.GetMailStorage(Tenant);

            Log = option.Get("ASC.Mail.MessageEngine");
        }

        public MailMessageData GetMessage(int messageId, MailMessageData.Options options)
        {
            var mail = DaoFactory.MailDao.GetMail(new ConcreteUserMessageExp(messageId, Tenant, User, !options.OnlyUnremoved));

            return GetMessage(mail, options);
        }

        public MailMessageData GetNextMessage(int messageId, MailMessageData.Options options)
        {
            var mail = DaoFactory.MailDao.GetNextMail(new ConcreteNextUserMessageExp(messageId, Tenant, User, !options.OnlyUnremoved));

            return GetMessage(mail, options);
        }

        public Stream GetMessageStream(int id)
        {
            var mail = DaoFactory.MailDao.GetMail(new ConcreteUserMessageExp(id, Tenant, User, false));

            if (mail == null)
                throw new ArgumentException("Message not found with id=" + id);

            var dataStore = StorageFactory.GetMailStorage(Tenant);

            var key = MailStoragePathCombiner.GetBodyKey(User, mail.Stream);

            return dataStore.GetReadStream(string.Empty, key);
        }

        public Tuple<int, int> GetRangeMessages(IMessagesExp exp)
        {
            return DaoFactory.MailInfoDao.GetRangeMails(exp);
        }

        private MailMessageData GetMessage(Entities.Mail mail, MailMessageData.Options options)
        {
            if (mail == null)
                return null;

            var tagIds = DaoFactory.TagMailDao.GetTagIds(new List<int> { mail.Id });

            var attachments = DaoFactory.AttachmentDao.GetAttachments(
                new ConcreteMessageAttachmentsExp(mail.Id, Tenant, User,
                    onlyEmbedded: options.LoadEmebbedAttachements));

            return ToMailMessage(mail, tagIds, attachments, options);
        }

        public List<MailMessageData> GetFilteredMessages(MailSearchFilterData filter, out long totalMessagesCount)
        {
            var res = new List<MailMessageData>();

            var ids = new List<int>();

            long total = 0;

            if (filter.UserFolderId.HasValue && Factory.UserFolderEngine.Get((uint)filter.UserFolderId.Value) == null)
                throw new ArgumentException("Folder not found");

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!filter.IsDefault() && FactoryIndexerHelper.Support(t) && FactoryIndexer.FactoryIndexerCommon.CheckState(false))
            {
                if (FilterMessagesExp.TryGetFullTextSearchIds(FactoryIndexer, FactoryIndexerHelper, ServiceProvider,
                    filter, User, out ids, out total))
                {
                    if (!ids.Any())
                    {
                        totalMessagesCount = 0;
                        return res;
                    }
                }
            }

            IMessagesExp exp;

            var tenantInfo = TenantManager.GetTenant(Tenant);
            var utcNow = DateTime.UtcNow;

            if (ids.Any())
            {
                var pageSize = filter.PageSize.GetValueOrDefault(25);
                var page = filter.Page.GetValueOrDefault(1);

                exp = SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetMessageIds(ids)
                        .SetOrderBy(filter.Sort)
                        .SetOrderAsc(filter.SortOrder == Defines.ASCENDING)
                        .SetLimit(pageSize)
                        .Build();

                var list = DaoFactory.MailInfoDao.GetMailInfoList(exp)
                    .ConvertAll(m => ToMailMessage(m, tenantInfo, utcNow));

                var pagedCount = (list.Count + page * pageSize);

                totalMessagesCount = page == 0 ? total : total - pagedCount;

                return list;
            }
            else
            {
                exp = new FilterMessagesExp(ids, Tenant, User, filter);

                if (filter.IsDefault())
                {
                    var folders = DaoFactory.FolderDao.GetFolders();

                    var currentFolder =
                        folders.FirstOrDefault(f => f.FolderType == filter.PrimaryFolder);

                    if (currentFolder != null && currentFolder.FolderType == FolderType.UserFolder)
                    {
                        totalMessagesCount = DaoFactory.MailInfoDao.GetMailInfoTotal(exp);
                    }
                    else
                    {
                        totalMessagesCount = currentFolder == null
                            ? 0
                            : filter.Unread.HasValue
                                ? filter.Unread.Value
                                    ? currentFolder.UnreadCount
                                    : currentFolder.TotalCount - currentFolder.UnreadCount
                                : currentFolder.TotalCount;
                    }
                }
                else
                {
                    totalMessagesCount = DaoFactory.MailInfoDao.GetMailInfoTotal(exp);
                }

                if (totalMessagesCount == 0)
                    return res;

                var list = DaoFactory.MailInfoDao.GetMailInfoList(exp)
                    .ConvertAll(m => ToMailMessage(m, tenantInfo, utcNow));

                return list;
            }
        }

        public List<MailMessageData> GetFilteredMessages(MailSieveFilterData filter, int page, int pageSize, out long totalMessagesCount)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            var res = new List<MailMessageData>();

            if (FilterSieveMessagesExp.TryGetFullTextSearchIds(FactoryIndexer, FactoryIndexerHelper, ServiceProvider,
                filter, User, out List<int> ids, out long total))
            {
                if (!ids.Any())
                {
                    totalMessagesCount = 0;
                    return res;
                }
            }

            var exp = new FilterSieveMessagesExp(ids, Tenant, User, filter, page, pageSize, FactoryIndexer, FactoryIndexerHelper, ServiceProvider);

            totalMessagesCount = ids.Any() ? total : DaoFactory.MailInfoDao.GetMailInfoTotal(exp);

            if (totalMessagesCount == 0)
            {
                return res;
            }

            var tenantInfo = TenantManager.GetTenant(Tenant);
            var utcNow = DateTime.UtcNow;

            var list = DaoFactory.MailInfoDao.GetMailInfoList(exp)
                .ConvertAll(m => ToMailMessage(m, tenantInfo, utcNow));

            return list;
        }

        public int GetNextFilteredMessageId(int messageId, MailSearchFilterData filter)
        {
            var mail = DaoFactory.MailDao.GetMail(new ConcreteUserMessageExp(messageId, Tenant, User, false));

            if (mail == null)
                return -1;

            var t = ServiceProvider.GetService<MailWrapper>();
            if (FactoryIndexerHelper.Support(t) && FactoryIndexer.FactoryIndexerCommon.CheckState(false))
            {
                if (FilterMessagesExp.TryGetFullTextSearchIds(FactoryIndexer, FactoryIndexerHelper, ServiceProvider,
                    filter, User, out List<int> ids, out long total, mail.DateSent))
                {
                    if (!ids.Any())
                        return -1;

                    return ids.Where(id => id != messageId)
                        .DefaultIfEmpty(-1)
                        .First();
                }
            }

            var exp = new FilterNextMessageExp(mail.DateSent, Tenant, User, filter);

            var list = DaoFactory.MailInfoDao.GetMailInfoList(exp);

            return list.Where(m => m.Id != messageId)
                .Select(m => m.Id)
                .DefaultIfEmpty(-1)
                .First();
        }

        //TODO: Simplify
        public bool SetUnread(List<int> ids, bool unread, bool allChain = false)
        {
            var ids2Update = new List<int>();
            List<MailInfo> chainedMessages;

            using (var tx = DaoFactory.BeginTransaction())
            {
                chainedMessages = Factory.ChainEngine.GetChainedMessagesInfo(ids);

                if (!chainedMessages.Any())
                    return true;

                var listIds = allChain
                    ? chainedMessages.Where(x => x.IsNew == !unread).Select(x => x.Id).ToList()
                    : ids;

                if (!listIds.Any())
                    return true;

                var exp = SimpleMessagesExp.CreateBuilder(Tenant, User)
                            .SetMessageIds(listIds)
                            .Build();

                DaoFactory.MailInfoDao.SetFieldValue(exp, "Unread", unread);

                var sign = unread ? 1 : -1;

                var folderConvMessCounters = new List<Tuple<FolderType, int, int>>();

                var fGroupedChains = chainedMessages.GroupBy(m => new { m.ChainId, m.Folder, m.MailboxId });

                uint? userFolder = null;

                if (chainedMessages.Any(m => m.Folder == FolderType.UserFolder))
                {
                    var item = DaoFactory.UserFolderXMailDao.Get(ids.First());
                    userFolder = item == null ? (uint?)null : item.FolderId;
                }

                foreach (var fChainMessages in fGroupedChains)
                {
                    var chainUnreadBefore = fChainMessages.Any(m => m.IsNew);

                    var firstFlag = true;

                    var unreadMessDiff = 0;

                    foreach (var m in fChainMessages.Where(m => listIds.Contains(m.Id) && m.IsNew != unread))
                    {
                        m.IsNew = unread;

                        unreadMessDiff++;

                        if (!firstFlag)
                            continue;

                        ids2Update.Add(m.Id);

                        firstFlag = false;
                    }

                    var chainUnreadAfter = fChainMessages.Any(m => m.IsNew);

                    var unreadConvDiff = chainUnreadBefore == chainUnreadAfter ? 0 : 1;

                    var tplFolderIndex =
                        folderConvMessCounters.FindIndex(tpl => tpl.Item1 == fChainMessages.Key.Folder);

                    if (tplFolderIndex == -1)
                    {
                        folderConvMessCounters.Add(
                            Tuple.Create(fChainMessages.Key.Folder,
                                unreadMessDiff,
                                unreadConvDiff));
                    }
                    else
                    {
                        var tplFolder = folderConvMessCounters[tplFolderIndex];

                        folderConvMessCounters[tplFolderIndex] = Tuple.Create(fChainMessages.Key.Folder,
                            tplFolder.Item2 + unreadMessDiff,
                            tplFolder.Item3 + unreadConvDiff);
                    }
                }

                foreach (var f in folderConvMessCounters)
                {
                    var folder = f.Item1;

                    var unreadMessDiff = f.Item2 != 0 ? sign * f.Item2 : (int?)null;
                    var unreadConvDiff = f.Item3 != 0 ? sign * f.Item3 : (int?)null;

                    Factory.FolderEngine.ChangeFolderCounters(folder, userFolder,
                        unreadMessDiff, unreadConvDiff: unreadConvDiff);
                }

                foreach (var id in ids2Update)
                    Factory.ChainEngine.UpdateMessageChainUnreadFlag(Tenant, User, id);

                if (userFolder.HasValue)
                {
                    var userFoldersIds = DaoFactory.UserFolderXMailDao.GetList(mailIds: chainedMessages.Select(m => m.Id).ToList())
                        .Select(ufxm => (int)ufxm.FolderId)
                        .Distinct()
                        .ToList();

                    Factory.UserFolderEngine.RecalculateCounters(DaoFactory, userFoldersIds);
                }

                tx.Commit();
            }

            var data = new MailWrapper
            {
                Unread = unread
            };

            ids2Update = allChain ? chainedMessages.Select(m => m.Id).ToList() : ids;

            Factory.IndexEngine.Update(data, s => s.In(m => m.Id, ids2Update.ToArray()), wrapper => wrapper.Unread);

            return true;
        }

        public bool SetImportant(List<int> ids, bool importance)
        {
            using (var tx = DaoFactory.BeginTransaction())
            {
                var exp = SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetMessageIds(ids)
                        .Build();

                DaoFactory.MailInfoDao.SetFieldValue(exp, "Importance", importance);

                foreach (var messageId in ids)
                    Factory.ChainEngine.UpdateMessageChainImportanceFlag(Tenant, User, messageId);

                tx.Commit();
            }

            var data = new MailWrapper
            {
                Importance = importance
            };

            Factory.IndexEngine.Update(data, s => s.In(m => m.Id, ids.ToArray()), wrapper => wrapper.Importance);

            return true;
        }

        public void Restore(List<int> ids)
        {
            List<MailInfo> mailInfoList;

            mailInfoList = DaoFactory.MailInfoDao.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetMessageIds(ids)
                        .Build());

            if (!mailInfoList.Any())
                return;

            using (var tx = DaoFactory.BeginTransaction())
            {
                Restore(DaoFactory, mailInfoList);
                tx.Commit();
            }

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t))
                return;

            var mails = mailInfoList.ConvertAll(m => new MailWrapper
            {
                Id = m.Id,
                Folder = (byte)m.FolderRestore
            });

            Factory.IndexEngine.Update(mails, wrapper => wrapper.Folder);
        }

        //TODO: Simplify
        public void Restore(IDaoFactory daoFactory, List<MailInfo> mailsInfo)
        {
            if (!mailsInfo.Any())
                return;

            var uniqueChainInfo = mailsInfo
                .ConvertAll(x => new
                {
                    folder = x.Folder,
                    chain_id = x.ChainId,
                    id_mailbox = x.MailboxId
                })
                .Distinct();

            var prevInfo = mailsInfo.ConvertAll(x => new
            {
                id = x.Id,
                unread = x.IsNew,
                folder = x.Folder,
                folder_restore = x.FolderRestore,
                chain_id = x.ChainId,
                id_mailbox = x.MailboxId
            });

            var ids = mailsInfo.ConvertAll(x => x.Id);

            var exp = SimpleMessagesExp.CreateBuilder(Tenant, User)
                    .SetMessageIds(ids)
                    .Build();

            DaoFactory.MailInfoDao.SetFieldsEqual(exp, "FolderRestore", "Folder");

            // Update chains in old folder
            foreach (var info in uniqueChainInfo)
                Factory.ChainEngine.UpdateChain(info.chain_id, info.folder, null, info.id_mailbox, Tenant, User);

            var unreadMessagesCountCollection = new Dictionary<FolderType, int>();
            var totalMessagesCountCollection = new Dictionary<FolderType, int>();

            foreach (var info in prevInfo)
            {
                if (totalMessagesCountCollection.ContainsKey(info.folder_restore))
                    totalMessagesCountCollection[info.folder_restore] += 1;
                else
                    totalMessagesCountCollection.Add(info.folder_restore, 1);

                if (!info.unread) continue;
                if (unreadMessagesCountCollection.ContainsKey(info.folder_restore))
                    unreadMessagesCountCollection[info.folder_restore] += 1;
                else
                    unreadMessagesCountCollection.Add(info.folder_restore, 1);
            }

            // Update chains in new restored folder
            Factory.ChainEngine.UpdateChainFields(Tenant, User, ids);

            var prevTotalUnreadCount = 0;
            var prevTotalCount = 0;

            int? totalMessDiff;
            int? unreadMessDiff;
            foreach (var keyPair in totalMessagesCountCollection)
            {
                var folderRestore = keyPair.Key;
                var totalRestore = keyPair.Value;

                totalMessDiff = totalRestore != 0 ? totalRestore : (int?) null;

                int unreadRestore;
                unreadMessagesCountCollection.TryGetValue(folderRestore, out unreadRestore);

                unreadMessDiff = unreadRestore != 0 ? unreadRestore : (int?) null;

                Factory.FolderEngine.ChangeFolderCounters(folderRestore, null,
                    unreadMessDiff, totalMessDiff);

                prevTotalUnreadCount -= unreadRestore;
                prevTotalCount -= totalRestore;
            }

            // Subtract the restored number of messages in the previous folder

            unreadMessDiff = prevTotalUnreadCount != 0 ? prevTotalUnreadCount : (int?) null;
            totalMessDiff = prevTotalCount != 0 ? prevTotalCount : (int?) null;

            Factory.FolderEngine.ChangeFolderCounters(prevInfo[0].folder, null,
                unreadMessDiff, totalMessDiff);
        }

        public void SetFolder(List<int> ids, FolderType folder, uint? userFolderId = null)
        {
            if (!ids.Any())
                throw new ArgumentNullException("ids");

            if (userFolderId.HasValue && folder != FolderType.UserFolder)
            {
                folder = FolderType.UserFolder;
            }

            using (var tx = DaoFactory.BeginTransaction())
            {
                SetFolder(DaoFactory, ids, folder, userFolderId);
                tx.Commit();
            }

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t))
                return;

            var data = new MailWrapper
            {
                Folder = (byte)folder,
                UserFolders = userFolderId.HasValue
                    ? new List<UserFolderWrapper>
                    {
                        new UserFolderWrapper
                        {
                            Id = (int) userFolderId.Value
                        }
                    }
                    : new List<UserFolderWrapper>()
            };

            Expression<Func<Selector<MailWrapper>, Selector<MailWrapper>>> exp =
                s => s.In(m => m.Id, ids.ToArray());

            Factory.IndexEngine.Update(data, exp, w => w.Folder);

            Factory.IndexEngine.Update(data, exp, UpdateAction.Replace, w => w.UserFolders);
        }

        public void SetFolder(IDaoFactory daoFactory, List<int> ids, FolderType toFolder,
            uint? toUserFolderId = null)
        {
            var query = SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetMessageIds(ids)
                        .Build();

            var mailInfoList = DaoFactory.MailInfoDao.GetMailInfoList(query);

            if (!mailInfoList.Any()) return;

            SetFolder(daoFactory, mailInfoList, toFolder, toUserFolderId);
        }

        public void SetFolder(IDaoFactory daoFactory, List<MailInfo> mailsInfo, FolderType toFolder,
            uint? toUserFolderId = null)
        {
            if (!mailsInfo.Any())
                return;

            if(toUserFolderId.HasValue && Factory.UserFolderEngine.Get(toUserFolderId.Value) == null)
                throw new ArgumentException("Folder not found");

            var messages = mailsInfo.ConvertAll(x =>
            {
                var srcUserFolderId = (uint?)null;

                if (x.Folder == FolderType.UserFolder)
                {
                    var item = DaoFactory.UserFolderXMailDao.Get(x.Id);
                    srcUserFolderId = item == null ? (uint?)null : item.FolderId;
                }

                return new
                {
                    id = x.Id,
                    unread = x.IsNew,
                    folder = x.Folder,
                    userFolderId = srcUserFolderId,
                    chain_id = x.ChainId,
                    id_mailbox = x.MailboxId
                };
            })
            .Where(m => m.folder != toFolder || m.userFolderId != toUserFolderId)
            .ToList();

            if(!messages.Any())
                return;

            var uniqueChainInfo = messages
                .ConvertAll(x => new
                {
                    x.folder,
                    x.userFolderId,
                    x.chain_id,
                    x.id_mailbox
                })
                .Distinct();

            var prevInfo = messages.ConvertAll(x => new
            {
                x.id,
                x.unread,
                x.folder,
                x.userFolderId,
                x.chain_id,
                x.id_mailbox
            });

            var ids = messages.Select(x => x.id).ToList();

            var updateQuery = SimpleMessagesExp.CreateBuilder(Tenant, User)
                    .SetMessageIds(ids)
                    .Build();

            DaoFactory.MailInfoDao.SetFieldValue(updateQuery,
                "Folder",
                toFolder);

            if (toUserFolderId.HasValue)
            {
                Factory.UserFolderEngine.SetFolderMessages(toUserFolderId.Value, ids);
            }
            else if (prevInfo.Any(x => x.userFolderId.HasValue))
            {
                var prevIds = prevInfo.Where(x => x.userFolderId.HasValue).Select(x => x.id).ToList();

                Factory.UserFolderEngine.DeleteFolderMessages(daoFactory, prevIds);
            }

            foreach (var info in uniqueChainInfo)
            {
                Factory.ChainEngine.UpdateChain(
                    info.chain_id,
                    info.folder,
                    info.userFolderId,
                    info.id_mailbox,
                    Tenant, User);
            }

            var totalMessages = prevInfo.GroupBy(x => new {x.folder, x.userFolderId})
                .Select(group => new {group.Key, Count = group.Count()});

            var unreadMessages = prevInfo.Where(x => x.unread)
                .GroupBy(x => new {x.folder, x.userFolderId})
                .Select(group => new {group.Key, Count = group.Count()})
                .ToList();

            Factory.ChainEngine.UpdateChainFields(Tenant, User, ids);

            var movedTotalUnreadCount = 0;
            var movedTotalCount = 0;
            int? totalMessDiff;
            int? unreadMessDiff;

            foreach (var keyPair in totalMessages)
            {
                var srcFolder = keyPair.Key.folder;
                var srcUserFolder = keyPair.Key.userFolderId;
                var totalMove = keyPair.Count;

                var unreadItem = unreadMessages.FirstOrDefault(
                        x => x.Key.folder == srcFolder && x.Key.userFolderId == srcUserFolder);

                var unreadMove = unreadItem != null ? unreadItem.Count : 0;  

                unreadMessDiff = unreadMove != 0 ? unreadMove*(-1) : (int?) null;
                totalMessDiff = totalMove != 0 ? totalMove*(-1) : (int?) null;

                Factory.FolderEngine.ChangeFolderCounters(srcFolder, srcUserFolder,
                    unreadMessDiff, totalMessDiff);

                movedTotalUnreadCount += unreadMove;
                movedTotalCount += totalMove;
            }

            unreadMessDiff = movedTotalUnreadCount != 0 ? movedTotalUnreadCount : (int?) null;
            totalMessDiff = movedTotalCount != 0 ? movedTotalCount : (int?) null;

            Factory.FolderEngine.ChangeFolderCounters(toFolder, toUserFolderId,
                unreadMessDiff, totalMessDiff);

            // Correction of UserFolders counters

            var userFolderIds = prevInfo.Where(x => x.folder == FolderType.UserFolder)
                .Select(x => (int)x.userFolderId.Value)
                .Distinct()
                .ToList();

            if (userFolderIds.Count() == 0 && !toUserFolderId.HasValue) // Only for movement from/to UserFolders
                return;

            if(toUserFolderId.HasValue)
                userFolderIds.Add((int)toUserFolderId.Value);

            Factory.UserFolderEngine.RecalculateCounters(daoFactory, userFolderIds);
        }

        public void SetRemoved(List<int> ids)
        {
            if (!ids.Any())
                throw new ArgumentNullException("ids");

            long usedQuota;

            var mailInfoList =
                DaoFactory.MailInfoDao.GetMailInfoList(
                    SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetMessageIds(ids)
                        .Build());

            if (!mailInfoList.Any()) return;

            using (var tx = DaoFactory.BeginTransaction())
            {
                usedQuota = SetRemoved(DaoFactory, mailInfoList);
                tx.Commit();
            }

            Factory.QuotaEngine.QuotaUsedDelete(usedQuota);

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t))
                return;

            Factory.IndexEngine.Remove(ids, Tenant, new Guid(User));
        }

        public long SetRemoved(IDaoFactory daoFactory, List<MailInfo> deleteInfo)
        {
            if (!deleteInfo.Any())
                return 0;

            var messageFieldsInfo = deleteInfo
                .ConvertAll(r =>
                    new
                    {
                        id = r.Id,
                        folder = r.Folder,
                        unread = r.IsNew
                    });

            var ids = messageFieldsInfo.Select(m => m.id).ToList();

            DaoFactory.MailInfoDao.SetFieldValue(
                SimpleMessagesExp.CreateBuilder(Tenant, User)
                    .SetMessageIds(ids)
                    .Build(),
                "IsRemoved",
                true);

            var exp = new ConcreteMessagesAttachmentsExp(ids, Tenant, User, onlyEmbedded: null);

            var usedQuota = DaoFactory.AttachmentDao.GetAttachmentsSize(exp);

            DaoFactory.AttachmentDao.SetAttachmnetsRemoved(exp);

            var tagIds = DaoFactory.TagMailDao.GetTagIds(ids.ToList());

            DaoFactory.TagMailDao.DeleteByMailIds(tagIds);

            foreach (var tagId in tagIds)
            {
                var tag = DaoFactory.TagDao.GetTag(tagId);

                if (tag == null)
                    continue;

                var count = DaoFactory.TagMailDao.CalculateTagCount(tag.Id);

                tag.Count = count;

                DaoFactory.TagDao.SaveTag(tag);
            }

            var totalCollection = (from row in messageFieldsInfo
                                   group row by row.folder
                into g
                                   select new { id = g.Key, diff = -g.Count() })
                .ToList();

            var unreadCollection = (from row in messageFieldsInfo.Where(m => m.unread)
                                    group row by row.folder
                into g
                                    select new { id = g.Key, diff = -g.Count() })
                .ToList();

            foreach (var folder in totalCollection)
            {
                var unreadInFolder = unreadCollection
                    .FirstOrDefault(f => f.id == folder.id);

                var unreadMessDiff = unreadInFolder != null ? unreadInFolder.diff : (int?)null;
                var totalMessDiff = folder.diff != 0 ? folder.diff : (int?)null;

                Factory.FolderEngine.ChangeFolderCounters(folder.id, null,
                    unreadMessDiff, totalMessDiff);
            }

            Factory.ChainEngine.UpdateChainFields(Tenant, User,
                messageFieldsInfo.Select(m => Convert.ToInt32(m.id)).ToList());

            return usedQuota;
        }

        public void SetRemoved(FolderType folder)
        {
            long usedQuota;

            var mailInfoList = DaoFactory.MailInfoDao.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(Tenant, User)
                    .SetFolder((int)folder)
                    .Build());

            if (!mailInfoList.Any()) return;

            var ids = mailInfoList.Select(m => m.Id).ToList();

            using (var tx = DaoFactory.BeginTransaction())
            {
                DaoFactory.MailInfoDao.SetFieldValue(
                    SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetFolder((int)folder)
                        .Build(),
                    "IsRemoved",
                    true);

                var exp = new ConcreteMessagesAttachmentsExp(ids, Tenant, User, onlyEmbedded: null);

                usedQuota = DaoFactory.AttachmentDao.GetAttachmentsSize(exp);

                DaoFactory.AttachmentDao.SetAttachmnetsRemoved(exp);


                var tagIds = DaoFactory.TagMailDao.GetTagIds(ids.ToList());

                DaoFactory.TagMailDao.DeleteByMailIds(tagIds);

                foreach (var tagId in tagIds)
                {
                    var tag = DaoFactory.TagDao.GetTag(tagId);

                    if (tag == null)
                        continue;

                    var count = DaoFactory.TagMailDao.CalculateTagCount(tag.Id);

                    tag.Count = count;

                    DaoFactory.TagDao.SaveTag(tag);
                }

                DaoFactory.ChainDao.Delete(SimpleConversationsExp.CreateBuilder(Tenant, User)
                        .SetFolder((int)folder)
                        .Build());

                DaoFactory.FolderDao.ChangeFolderCounters(folder, 0, 0, 0, 0);

                tx.Commit();
            }

            if (usedQuota <= 0)
                return;

            Factory.QuotaEngine.QuotaUsedDelete(usedQuota);
        }

        public int MailSave(MailBoxData mailbox, MailMessageData message,
            int messageId, FolderType folder, FolderType folderRestore, uint? userFolderId,
            string uidl, string md5, bool saveAttachments)
        {
            int id;

            using (var tx = DaoFactory.BeginTransaction())
            {
                long usedQuota;

                id = MailSave(mailbox, message, messageId,
                    folder, folderRestore, userFolderId,
                    uidl, md5, saveAttachments, out usedQuota);

                tx.Commit();
            }

            return id;
        }

        public int MailSave(MailBoxData mailbox, MailMessageData message,
            int messageId, FolderType folder, FolderType folderRestore, uint? userFolderId,
            string uidl, string md5, bool saveAttachments, out long usedQuota)
        {
            var countAttachments = 0;
            usedQuota = 0;

            if (messageId != 0)
            {
                countAttachments = DaoFactory.AttachmentDao.GetAttachmentsCount(
                    new ConcreteMessageAttachmentsExp(messageId, mailbox.TenantId, mailbox.UserId));
            }

            var address = mailbox.EMail.Address.ToLowerInvariant();

            var mail = new Entities.Mail
            {
                Id = messageId,
                Tenant = Tenant,
                User = User,
                MailboxId = mailbox.MailBoxId,
                Address = address,
                From = message.From,
                To = message.To,
                Reply = message.ReplyTo,
                Subject = message.Subject,
                Cc = message.Cc,
                Bcc = message.Bcc,
                Importance = message.Important,
                DateReceived = DateTime.UtcNow,
                DateSent = message.Date.ToUniversalTime(),
                Size = message.Size,
                AttachCount = !saveAttachments
                    ? countAttachments
                    : (message.Attachments != null ? message.Attachments.Count : 0),
                Unread = message.IsNew,
                IsAnswered = message.IsAnswered,
                IsForwarded = message.IsForwarded,
                Stream = message.StreamId,
                Folder = folder,
                FolderRestore = folderRestore,
                Spam = false,
                MimeMessageId = message.MimeMessageId,
                MimeInReplyTo = message.MimeReplyToId,
                ChainId = message.ChainId,
                Introduction = message.Introduction,
                HasParseError = message.HasParseError,
                CalendarUid = message.CalendarUid,
                Uidl = uidl,
                Md5 = md5
            };

            var mailId = DaoFactory.MailDao.Save(mail);

            if (messageId == 0)
            {
                var unreadMessDiff = message.IsNew ? 1 : (int?)null;
                Factory.FolderEngine.ChangeFolderCounters(folder, userFolderId, unreadMessDiff, 1);

                if (userFolderId.HasValue)
                {
                    Factory.UserFolderEngine.SetFolderMessages(userFolderId.Value, new List<int> { mailId });
                }
            }

            if (saveAttachments &&
                message.Attachments != null &&
                message.Attachments.Count > 0)
            {
                var exp = new ConcreteMessageAttachmentsExp(mailId, mailbox.TenantId, mailbox.UserId, onlyEmbedded: null);

                usedQuota = DaoFactory.AttachmentDao.GetAttachmentsSize(exp);

                DaoFactory.AttachmentDao.SetAttachmnetsRemoved(exp);

                foreach (var attachment in message.Attachments)
                {
                    var newId = DaoFactory.AttachmentDao.SaveAttachment(attachment.ToAttachmnet(mailId));
                    attachment.fileId = newId;
                }

                var count = DaoFactory.AttachmentDao.GetAttachmentsCount(
                                new ConcreteMessageAttachmentsExp(mailId, mailbox.TenantId, mailbox.UserId));

                DaoFactory.MailInfoDao.SetFieldValue(
                    SimpleMessagesExp.CreateBuilder(mailbox.TenantId, mailbox.UserId)
                        .SetMessageId(mailId)
                        .Build(),
                    "AttachCount",
                    count);
            }

            if (!string.IsNullOrEmpty(message.FromEmail) && message.FromEmail.Length > 0)
            {
                if (messageId > 0)
                    DaoFactory.TagMailDao.DeleteByMailIds(new List<int> { mailId });

                if (message.TagIds == null)
                    message.TagIds = new List<int>();

                var tagAddressesTagIds = DaoFactory.TagAddressDao.GetTagIds(message.FromEmail);

                tagAddressesTagIds.ForEach(tagId =>
                {
                    if (!message.TagIds.Contains(tagId))
                        message.TagIds.Add(tagId);
                });

                if (message.TagIds.Any())
                {
                    foreach (var tagId in message.TagIds)
                    {
                        var tag = DaoFactory.TagDao.GetTag(tagId);

                        if (tag == null)
                            continue;

                        DaoFactory.TagMailDao.SetMessagesTag(new[] { mailId }, tag.Id);

                        var count = DaoFactory.TagMailDao.CalculateTagCount(tag.Id);

                        tag.Count = count;

                        DaoFactory.TagDao.SaveTag(tag);
                    }
                }
            }

            UpdateMessagesChains(DaoFactory, mailbox, message.MimeMessageId, message.ChainId, folder, userFolderId);

            Log.DebugFormat("MailSave() tenant='{0}', user_id='{1}', email='{2}', from='{3}', id_mail='{4}'",
                mailbox.TenantId, mailbox.UserId, mailbox.EMail, message.From, mailId);

            return mailId;
        }

        public ChainInfo DetectChain(MailBoxData mailbox, string mimeMessageId, string mimeReplyToId, string subject)
        {
            return DetectChain(DaoFactory, mailbox, mimeMessageId, mimeReplyToId, subject);
        }

        public ChainInfo DetectChain(IDaoFactory daoFactory, MailBoxData mailbox, string mimeMessageId,
            string mimeReplyToId, string subject)
        {
            var chainId = mimeMessageId; //Chain id is equal to root conversataions message - MimeMessageId
            var chainDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(mimeMessageId) && !string.IsNullOrEmpty(mimeReplyToId))
            {
                chainId = mimeReplyToId;

                try
                {
                    var chainAndSubject =
                        DaoFactory.MailInfoDao.GetMailInfoList(
                            SimpleMessagesExp.CreateBuilder(Tenant, User)
                                .SetMailboxId(mailbox.MailBoxId)
                                .SetMimeMessageId(mimeReplyToId)
                                .Build())
                            .ConvertAll(x => new
                            {
                                chain_id = x.ChainId,
                                subject = x.Subject,
                                chainDate = x.ChainDate
                            })
                            .Distinct()
                            .FirstOrDefault()
                        ?? DaoFactory.MailInfoDao.GetMailInfoList(
                            SimpleMessagesExp.CreateBuilder(Tenant, User)
                                .SetMailboxId(mailbox.MailBoxId)
                                .SetChainId(mimeReplyToId)
                                .Build())
                            .ConvertAll(x => new
                            {
                                chain_id = x.ChainId,
                                subject = x.Subject,
                                chainDate = x.ChainDate
                            })
                            .Distinct()
                            .FirstOrDefault();

                    if (chainAndSubject != null)
                    {
                        var chainSubject = MailUtil.NormalizeSubject(chainAndSubject.subject);
                        var messageSubject = MailUtil.NormalizeSubject(subject);

                        if (chainSubject.Equals(messageSubject))
                        {
                            chainId =  chainAndSubject.chain_id;
                            chainDate = chainAndSubject.chainDate;
                        }
                        else
                        {
                            chainId = mimeMessageId;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Log.WarnFormat(
                        "DetectChainId() params tenant={0}, user_id='{1}', mailbox_id={2}, mime_message_id='{3}' Exception:\r\n{4}",
                        mailbox.TenantId, mailbox.UserId, mailbox.MailBoxId, mimeMessageId, ex.ToString());
                }
            }

            Log.DebugFormat(
                "DetectChainId() tenant='{0}', user_id='{1}', mailbox_id='{2}', mime_message_id='{3}' Result: {4}",
                mailbox.TenantId, mailbox.UserId, mailbox.MailBoxId, mimeMessageId, chainId);

            return new ChainInfo
            {
                Id = chainId,
                MailboxId = mailbox.MailBoxId,
                ChainDate = chainDate
            };
        }

        //TODO: Need refactoring
        public MailMessageData Save(
            MailBoxData mailbox, MimeMessage mimeMessage, string uidl, MailFolder folder, 
            uint? userFolderId, bool unread = true, ILog log = null)
        {
            if (mailbox == null)
                throw new ArgumentException(@"mailbox is null", "mailbox");

            if (mimeMessage == null)
                throw new ArgumentException(@"message is null", "mimeMessage");

            if (uidl == null)
                throw new ArgumentException(@"uidl is null", "uidl");

            if (log == null)
                log = new NullLog();

            var fromEmail = mimeMessage.From.Mailboxes.FirstOrDefault();

            var md5 =
                    string.Format("{0}|{1}|{2}|{3}",
                        mimeMessage.From.Mailboxes.Any() ? mimeMessage.From.Mailboxes.First().Address : "",
                        mimeMessage.Subject, mimeMessage.Date.UtcDateTime, mimeMessage.MessageId).GetMd5();

            var fromThisMailBox = fromEmail != null &&
                                  fromEmail.Address.ToLowerInvariant()
                                      .Equals(mailbox.EMail.Address.ToLowerInvariant());

            var toThisMailBox =
                mimeMessage.To.Mailboxes.Select(addr => addr.Address.ToLowerInvariant())
                    .Contains(mailbox.EMail.Address.ToLowerInvariant());

            List<int> tagsIds = null;

            if (folder.Tags.Any())
            {
                log.Debug("GetOrCreateTags()");
                tagsIds = Factory.TagEngine.GetOrCreateTags(mailbox.TenantId, mailbox.UserId, folder.Tags);
            }

            log.Debug("UpdateExistingMessages()");

            var found = UpdateExistingMessages(mailbox, folder.Folder, uidl, md5,
                mimeMessage.MessageId, fromThisMailBox, toThisMailBox, tagsIds, log);

            var needSave = !found;
            if (!needSave)
                return null;

            log.Debug("DetectChainId()");

            var chainInfo = Factory.MessageEngine.DetectChain(mailbox, mimeMessage.MessageId, mimeMessage.InReplyTo,
                mimeMessage.Subject);

            var streamId = MailUtil.CreateStreamId();

            log.Debug("Convert MimeMessage->MailMessage");

            var message = mimeMessage.ConvertToMailMessage(
                TenantManager, CoreSettings,
                folder, unread, chainInfo.Id,
                chainInfo.ChainDate, streamId,
                mailbox.MailBoxId, true, log);

            log.Debug("TryStoreMailData()");

            if (!TryStoreMailData(message, mailbox, log))
            {
                return null;
            }

            log.Debug("MailSave()");

            if (TrySaveMail(mailbox, message, folder, userFolderId, uidl, md5, log))
            {
                return message;
            }

            if (TryRemoveMailDirectory(mailbox, message.StreamId, log))
            {
                log.InfoFormat("Problem with mail proccessing(Account:{0}). Body and attachment have been deleted", mailbox.EMail);
            }
            else
            {
                throw new Exception("Can't delete mail folder with data");
            }

            return null;
        }
        //TODO: Need refactoring
        public string StoreMailBody(MailBoxData mailBoxData, MailMessageData messageItem, ILog log)
        {
            if (string.IsNullOrEmpty(messageItem.HtmlBody) && (messageItem.HtmlBodyStream == null || messageItem.HtmlBodyStream.Length == 0))
                return string.Empty;

            // Using id_user as domain in S3 Storage - allows not to add quota to tenant.
            var savePath = MailStoragePathCombiner.GetBodyKey(mailBoxData.UserId, messageItem.StreamId);

            Storage.QuotaController = null;

            try
            {
                string response;

                if (messageItem.HtmlBodyStream != null && messageItem.HtmlBodyStream.Length > 0)
                {
                    messageItem.HtmlBodyStream.Seek(0, SeekOrigin.Begin);

                    response = Storage
                            .Save(savePath, messageItem.HtmlBodyStream, MailStoragePathCombiner.BODY_FILE_NAME)
                            .ToString();
                }
                else
                {
                    using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(messageItem.HtmlBody)))
                    {
                        response = Storage
                            .Save(savePath, reader, MailStoragePathCombiner.BODY_FILE_NAME)
                            .ToString();
                    }
                }

                log.DebugFormat("StoreMailBody() tenant='{0}', user_id='{1}', save_body_path='{2}' Result: {3}",
                            mailBoxData.TenantId, mailBoxData.UserId, savePath, response);

                return response;
            }
            catch (Exception ex)
            {
                log.DebugFormat(
                    "StoreMailBody() Problems with message saving in messageId={0}. \r\n Exception: \r\n {0}\r\n",
                    messageItem.MimeMessageId, ex.ToString());

                Storage.Delete(string.Empty, savePath);
                throw;
            }
        }
        //TODO: Need refactoring
        public static Dictionary<int, string> GetPop3NewMessagesIDs(DaoFactory daoFactory, MailBoxData mailBox, Dictionary<int, string> uidls,
            int chunk)
        {
            var newMessages = new Dictionary<int, string>();

            if (!uidls.Any() || uidls.Count == mailBox.MessagesCount)
                return newMessages;

            var i = 0;

            var chunkUidls = uidls.Skip(i).Take(chunk).ToList();

            do
            {
                var checkList = chunkUidls.Select(u => u.Value).Distinct().ToList();

                var existingUidls = daoFactory.MailDao.GetExistingUidls(mailBox.MailBoxId, checkList);

                if (!existingUidls.Any())
                {
                    var messages = newMessages;
                    foreach (var item in
                        chunkUidls.Select(uidl => new KeyValuePair<int, string>(uidl.Key, uidl.Value))
                            .Where(item => !messages.Contains(item)))
                    {
                        newMessages.Add(item.Key, item.Value);
                    }
                }
                else if (existingUidls.Count != chunkUidls.Count)
                {
                    var messages = newMessages;
                    foreach (var item in (from uidl in chunkUidls
                                          where !existingUidls.Contains(uidl.Value)
                                          select new KeyValuePair<int, string>(uidl.Key, uidl.Value)).Where(
                            item => !messages.Contains(item)))
                    {
                        newMessages.Add(item.Key, item.Value);
                    }
                }

                i += chunk;

                chunkUidls = uidls.Skip(i).Take(chunk).ToList();

            } while (chunkUidls.Any());

            return newMessages;
        }

        private void UpdateMessagesChains(IDaoFactory daoFactory, MailBoxData mailbox, string mimeMessageId, 
            string chainId, FolderType folder, uint? userFolderId)
        {
            var chainsForUpdate = new[] {new {id = chainId, folder}};

            // if mime_message_id == chain_id - message is first in chain, because it isn't reply
            if (!string.IsNullOrEmpty(mimeMessageId) && mimeMessageId != chainId)
            {
                var chains = DaoFactory.ChainDao.GetChains(SimpleConversationsExp.CreateBuilder(mailbox.TenantId, mailbox.UserId)
                    .SetMailboxId(mailbox.MailBoxId)
                    .SetChainId(mimeMessageId)
                    .Build())
                    .Select(x => new {id = x.Id, folder = x.Folder})
                    .ToArray();

                if (chains.Any())
                {
                    DaoFactory.MailInfoDao.SetFieldValue(
                        SimpleMessagesExp.CreateBuilder(mailbox.TenantId, mailbox.UserId)
                            .SetChainId(mimeMessageId)
                            .Build(),
                        "ChainId",
                        chainId);

                    chainsForUpdate = chains.Concat(chainsForUpdate).ToArray();

                    var newChainsForUpdate =
                        DaoFactory.MailInfoDao.GetMailInfoList(
                            SimpleMessagesExp.CreateBuilder(Tenant, User)
                                .SetMailboxId(mailbox.MailBoxId)
                                .SetChainId(chainId)
                                .Build())
                            .ConvertAll(x => new
                            {
                                id = chainId,
                                folder = x.Folder
                            })
                            .Distinct();

                    chainsForUpdate = chainsForUpdate.Concat(newChainsForUpdate).ToArray();
                }
            }

            foreach (var c in chainsForUpdate.Distinct())
            {
                Factory.ChainEngine.UpdateChain(c.id, c.folder, userFolderId, mailbox.MailBoxId,
                    mailbox.TenantId, mailbox.UserId);
            }
        }

        //TODO: Need refactoring
        private bool TrySaveMail(MailBoxData mailbox, MailMessageData message, 
            MailFolder folder, uint? userFolderId, string uidl, string md5, ILog log)
        {
            try
            {
                var folderRestoreId = folder.Folder == FolderType.Spam ? FolderType.Inbox : folder.Folder;

                var attempt = 1;

                while (attempt < 3)
                {
                    try
                    {
                        message.Id = MailSave(mailbox, message, 0,
                            folder.Folder, folderRestoreId, userFolderId, uidl, md5, true);

                        break;
                    }
                    catch (Exception exSql)
                    {
                        if (!exSql.Message.StartsWith("Deadlock found"))
                            throw;

                        if (attempt > 2)
                            throw;

                        log.WarnFormat("[DEADLOCK] MailSave() try again (attempt {0}/2)", attempt);

                        attempt++;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("TrySaveMail Exception:\r\n{0}\r\n", ex.ToString());
            }

            return false;
        }

        //TODO: Need refactoring
        public bool TryStoreMailData(MailMessageData message, MailBoxData mailbox, ILog log)
        {
            try
            {
                if (message.Attachments.Any())
                {
                    log.Debug("StoreAttachments()");
                    var index = 0;
                    message.Attachments.ForEach(att =>
                    {
                        att.fileNumber = ++index;
                        att.mailboxId = mailbox.MailBoxId;
                    });

                    Factory.AttachmentEngine.StoreAttachments(mailbox, message.Attachments, message.StreamId);

                    log.Debug("MailMessage.ReplaceEmbeddedImages()");
                    message.ReplaceEmbeddedImages(log);
                }

                log.Debug("StoreMailBody()");
                StoreMailBody(mailbox, message, log);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("TryStoreMailData(Account:{0}): Exception:\r\n{1}\r\n", mailbox.EMail, ex.ToString());

                //Trying to delete all attachments and mailbody
                if (TryRemoveMailDirectory(mailbox, message.StreamId, log))
                {
                    log.InfoFormat("Problem with mail proccessing(Account:{0}). Body and attachment have been deleted", mailbox.EMail);
                }

                return false;
            }

            return true;
        }
        //TODO: Need refactoring
        private bool TryRemoveMailDirectory(MailBoxData mailbox, string streamId, ILog log)
        {
            //Trying to delete all attachments and mailbody
            try
            {
                Storage.DeleteDirectory(string.Empty,
                    MailStoragePathCombiner.GetMessageDirectory(mailbox.UserId, streamId));
                return true;
            }
            catch (Exception ex)
            {
                log.DebugFormat(
                    "Problems with mail_directory deleting. Account: {0}. Folder: {1}/{2}/{3}. Exception: {4}",
                    mailbox.EMail, mailbox.TenantId, mailbox.UserId, streamId, ex.ToString());

                return false;
            }
        }
        //TODO: Need refactoring
        private bool UpdateExistingMessages(MailBoxData mailbox, FolderType folder, string uidl, string md5,
            string mimeMessageId, bool fromThisMailBox, bool toThisMailBox, List<int> tagsIds, ILog log)
        {
            if ((string.IsNullOrEmpty(md5) || md5.Equals(Defines.MD5_EMPTY)) && string.IsNullOrEmpty(mimeMessageId))
            {
                return false;
            }

                var builder = SimpleMessagesExp.CreateBuilder(mailbox.TenantId, mailbox.UserId, null)
                    .SetMailboxId(mailbox.MailBoxId);

                var exp = (string.IsNullOrEmpty(mimeMessageId)
                    ? builder.SetMd5(md5)
                    : builder.SetMimeMessageId(mimeMessageId))
                    .Build();

                var messagesInfo = DaoFactory.MailInfoDao.GetMailInfoList(exp);

                if (!messagesInfo.Any())
                    return false;

                var idList = messagesInfo.Where(m => !m.IsRemoved).Select(m => m.Id).ToList();
                if (!idList.Any())
                {
                    log.Info("Message already exists and it was removed from portal.");
                    return true;
                }

                if (mailbox.Imap)
                {
                    if (tagsIds != null) // Add new tags to existing messages
                    {
                        using (var tx = DaoFactory.BeginTransaction())
                        {
                            if (tagsIds.Any(tagId => !Factory.TagEngine.SetMessagesTag(DaoFactory, idList, tagId)))
                            {
                                tx.Rollback();
                                return false;
                            }

                            tx.Commit();
                        }
                    }

                    if ((!fromThisMailBox || !toThisMailBox) && messagesInfo.Exists(m => m.FolderRestore == folder))
                    {
                        var clone = messagesInfo.FirstOrDefault(m => m.FolderRestore == folder && m.Uidl == uidl);
                        if (clone != null)
                            log.InfoFormat("Message already exists: mailId={0}. Clone", clone.Id);
                        else
                            log.Info("Message already exists. by MD5/MimeMessageId");

                        return true;
                    }
                }
                else
                {
                    if (!fromThisMailBox && toThisMailBox && messagesInfo.Count == 1)
                    {
                        log.InfoFormat("Message already exists: mailId={0}. Outbox clone", messagesInfo.First().Id);
                        return true;
                    }
                }

                if (folder == FolderType.Sent)
                {
                    var sentCloneForUpdate =
                        messagesInfo.FirstOrDefault(
                            m => m.FolderRestore == FolderType.Sent && string.IsNullOrEmpty(m.Uidl));

                    if (sentCloneForUpdate != null)
                    {
                        if (!sentCloneForUpdate.IsRemoved)
                        {
                            DaoFactory.MailInfoDao.SetFieldValue(
                                SimpleMessagesExp.CreateBuilder(mailbox.TenantId, mailbox.UserId)
                                    .SetMessageId(sentCloneForUpdate.Id)
                                    .Build(),
                                "Uidl",
                                uidl);
                        }

                        log.InfoFormat("Message already exists: mailId={0}. Outbox clone", sentCloneForUpdate.Id);

                        return true;
                    }
                }

                if (folder == FolderType.Spam)
                {
                    var first = messagesInfo.First();

                    log.InfoFormat("Message already exists: mailId={0}. It was moved to spam on server", first.Id);

                    return true;
                }

                var fullClone = messagesInfo.FirstOrDefault(m => m.FolderRestore == folder && m.Uidl == uidl);
                if (fullClone == null)
                    return false;

                log.InfoFormat("Message already exists: mailId={0}. Full clone", fullClone.Id);
                return true;

        }

        public MailMessageData ToMailMessage(MailInfo mailInfo, Tenant tenantInfo, DateTime utcNow)
        {
            var now = TenantUtil.DateTimeFromUtc(tenantInfo.TimeZone, utcNow);
            var date = TenantUtil.DateTimeFromUtc(tenantInfo.TimeZone, mailInfo.DateSent);
            var chainDate = TenantUtil.DateTimeFromUtc(tenantInfo.TimeZone, mailInfo.ChainDate);

            var isToday = (now.Year == date.Year && now.Date == date.Date);
            var isYesterday = (now.Year == date.Year && now.Date == date.Date.AddDays(1));

            return new MailMessageData
            {
                Id = mailInfo.Id,
                From = mailInfo.From,
                To = mailInfo.To,
                Cc = mailInfo.Cc,
                ReplyTo = mailInfo.ReplyTo,
                Subject = mailInfo.Subject,
                Important = mailInfo.Importance,
                Date = date,
                Size = mailInfo.Size,
                HasAttachments = mailInfo.HasAttachments,
                IsNew = mailInfo.IsNew,
                IsAnswered = mailInfo.IsAnswered,
                IsForwarded = mailInfo.IsForwarded,
                LabelsString = mailInfo.LabelsString,
                RestoreFolderId = mailInfo.FolderRestore,
                Folder = mailInfo.Folder,
                ChainId = mailInfo.ChainId ?? "",
                ChainLength = 1,
                ChainDate = chainDate,
                IsToday = isToday,
                IsYesterday = isYesterday,
                MailboxId = mailInfo.MailboxId,
                CalendarUid = mailInfo.CalendarUid,
                Introduction = mailInfo.Intoduction
            };
        }

        protected MailMessageData ToMailMessage(Entities.Mail mail, List<int> tags, List<Attachment> attachments,
            MailMessageData.Options options)
        {
            var now = TenantUtil.DateTimeFromUtc(TenantManager.GetTenant(Tenant).TimeZone, DateTime.UtcNow);
            var date = TenantUtil.DateTimeFromUtc(TenantManager.GetTenant(Tenant).TimeZone, mail.DateSent);
            var isToday = (now.Year == date.Year && now.Date == date.Date);
            var isYesterday = (now.Year == date.Year && now.Date == date.Date.AddDays(1));

            var item = new MailMessageData
            {
                Id = mail.Id,
                ChainId = mail.ChainId,
                ChainDate = mail.ChainDate,
                Attachments = null,
                Address = mail.Address,
                Bcc = mail.Bcc,
                Cc = mail.Cc,
                Date = date,
                From = mail.From,
                HasAttachments = mail.AttachCount > 0,
                Important = mail.Importance,
                IsAnswered = mail.IsAnswered,
                IsForwarded = mail.IsForwarded,
                IsNew = false,
                TagIds = tags,
                ReplyTo = mail.Reply,
                Size = mail.Size,
                Subject = mail.Subject,
                To = mail.To,
                StreamId = mail.Stream,
                Folder = mail.Folder,
                WasNew = mail.Unread,
                IsToday = isToday,
                IsYesterday = isYesterday,
                Introduction = !string.IsNullOrEmpty(mail.Introduction) ? mail.Introduction.Trim() : "",
                TextBodyOnly = mail.IsTextBodyOnly,
                MailboxId = mail.MailboxId,
                RestoreFolderId = mail.FolderRestore,
                HasParseError = mail.HasParseError,
                MimeMessageId = mail.MimeMessageId,
                MimeReplyToId = mail.MimeInReplyTo,
                CalendarUid = mail.CalendarUid,
                Uidl = mail.Uidl
            };

            //Reassemble paths
            if (options.LoadBody)
            {
                var htmlBody = "";

                if (!item.HasParseError)
                {
#if DEBUG
                    var watch = new Stopwatch();
                    double swtGetBodyMilliseconds;
                    double swtSanitazeilliseconds = 0;
#endif

                    var dataStore = StorageFactory.GetMailStorage(Tenant);
                    var key = MailStoragePathCombiner.GetBodyKey(User, item.StreamId);

                    try
                    {
#if DEBUG
                        Log.DebugFormat(
                            "Mail->GetMailInfo(id={0})->Start Body Load tenant: {1}, user: '{2}', key='{3}'",
                            mail.Id, Tenant, User, key);

                        watch.Start();
#endif
                        using (var s = dataStore.GetReadStream(string.Empty, key))
                        {
                            htmlBody = Encoding.UTF8.GetString(s.ReadToEnd());
                        }
#if DEBUG
                        watch.Stop();
                        swtGetBodyMilliseconds = watch.Elapsed.TotalMilliseconds;
                        watch.Reset();
#endif
                        if (options.NeedSanitizer && item.Folder != FolderType.Draft &&
                            !item.From.Equals(Defines.MailDaemonEmail))
                        {
#if DEBUG
                            watch.Start();
#endif
                            bool imagesAreBlocked;

                            Log.DebugFormat(
                                "Mail->GetMailInfo(id={0})->Start Sanitize Body tenant: {1}, user: '{2}', BodyLength: {3} bytes",
                                mail.Id, Tenant, User, htmlBody.Length);

                            htmlBody = HtmlSanitizer.Sanitize(htmlBody, out imagesAreBlocked,
                                new HtmlSanitizer.Options(options.LoadImages, options.NeedProxyHttp));

#if DEBUG
                            watch.Stop();
                            swtSanitazeilliseconds = watch.Elapsed.TotalMilliseconds;
#endif
                            item.ContentIsBlocked = imagesAreBlocked;
                        }
#if DEBUG
                        Log.DebugFormat(
                            "Mail->GetMailInfo(id={0})->Elapsed: BodyLoad={1}ms, Sanitaze={2}ms (NeedSanitizer={3}, NeedProxyHttp={4})",
                            mail.Id, swtGetBodyMilliseconds, swtSanitazeilliseconds, options.NeedSanitizer,
                            options.NeedProxyHttp);
#endif
                    }
                    catch (Exception ex)
                    {
                        item.IsBodyCorrupted = true;
                        htmlBody = "";
                        Log.Error(
                            string.Format("Mail->GetMailInfo(tenant={0} user=\"{1}\" messageId={2} key=\"{3}\")",
                                Tenant, User, mail.Id, key), ex);
#if DEBUG
                        watch.Stop();
                        swtGetBodyMilliseconds = watch.Elapsed.TotalMilliseconds;
                        Log.DebugFormat(
                            "Mail->GetMailInfo(id={0})->Elapsed [BodyLoadFailed]: BodyLoad={1}ms, Sanitaze={2}ms (NeedSanitizer={3}, NeedProxyHttp={4})",
                            mail.Id, swtGetBodyMilliseconds, swtSanitazeilliseconds, options.NeedSanitizer,
                            options.NeedProxyHttp);
#endif
                    }
                }

                item.HtmlBody = htmlBody;
            }

            item.Attachments = attachments.ConvertAll(AttachmentEngine.ToAttachmentData);

            return item;
        }
    }

    public static class MessageEngineExtension
    {
        public static DIHelper AddMessageEngineService(this DIHelper services)
        {
            services.TryAddScoped<MessageEngine>();

            return services;
        }
    }
}
