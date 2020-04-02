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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ASC.Common.Logging;
using ASC.Core;
using ASC.ElasticSearch;
using ASC.Mail.Core.Dao.Expressions.Conversation;
using ASC.Mail.Core.Dao.Expressions.Message;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;
using ASC.Mail.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using ASC.Mail.Core.Dao.Expressions;

namespace ASC.Mail.Core.Engine
{
    public class ChainEngine
    {
        public SecurityContext SecurityContext { get; }
        public TenantManager TenantManager { get; }
        public DaoFactory DaoFactory { get; }
        public MessageEngine MessageEngine { get; }
        public FolderEngine FolderEngine { get; }
        public UserFolderEngine UserFolderEngine { get; }
        public IndexEngine IndexEngine { get; }
        public QuotaEngine QuotaEngine { get; }
        public OperationEngine OperationEngine { get; }
        public FactoryIndexer<MailWrapper> FactoryIndexer { get; }
        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public IServiceProvider ServiceProvider { get; }
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

        public EngineFactory Factory { get; private set; }

        private const int CHUNK_SIZE = 3;

        public ChainEngine(
            SecurityContext securityContext,
            TenantManager tenantManager,
            DaoFactory daoFactory,
            MessageEngine messageEngine,
            FolderEngine folderEngine,
            UserFolderEngine userFolderEngine,
            IndexEngine indexEngine,
            QuotaEngine quotaEngine,
            OperationEngine operationEngine,
            FactoryIndexer<MailWrapper> factoryIndexer,
            FactoryIndexerHelper factoryIndexerHelper,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> option)
        {
            SecurityContext = securityContext;
            TenantManager = tenantManager;
            DaoFactory = daoFactory;
            MessageEngine = messageEngine;
            FolderEngine = folderEngine;
            UserFolderEngine = userFolderEngine;
            IndexEngine = indexEngine;
            QuotaEngine = quotaEngine;
            OperationEngine = operationEngine;
            FactoryIndexer = factoryIndexer;
            FactoryIndexerHelper = factoryIndexerHelper;
            ServiceProvider = serviceProvider;
            Log = option.Get("ASC.Mail.ChainEngine");
        }

        public long GetNextConversationId(int id, MailSearchFilterData filter)
        {
            var mail = DaoFactory.MailDao.GetMail(new ConcreteUserMessageExp(id, Tenant, User));

            if (mail == null)
                return 0;

            filter.FromDate = mail.ChainDate;
            filter.FromMessage = id;
            filter.PageSize = 1;

            var messages = GetFilteredConversations(filter, out bool hasMore);
            return messages.Any() ? messages.First().Id : 0;
        }

        public List<MailMessageData> GetConversations(MailSearchFilterData filterData, out bool hasMore)
        {
            if (filterData == null)
                throw new ArgumentNullException("filterData");

            var filter = (MailSearchFilterData)filterData.Clone();

            if (filter.UserFolderId.HasValue && Factory.UserFolderEngine.Get((uint)filter.UserFolderId.Value) == null)
                throw new ArgumentException("Folder not found");

            var filteredConversations = GetFilteredConversations(filter, out hasMore);

            if (!filteredConversations.Any())
                return filteredConversations;

            var chainIds = new List<string>();
            filteredConversations.ForEach(x => chainIds.Add(x.ChainId));

            var exp = SimpleConversationsExp.CreateBuilder(Tenant, User)
                .SetChainIds(chainIds)
                .SetFoldersIds(
                    filter.PrimaryFolder == FolderType.Inbox ||
                    filter.PrimaryFolder == FolderType.Sent
                        ? new List<int> { (int)FolderType.Inbox, (int)FolderType.Sent }
                        : new List<int> { (int)filter.PrimaryFolder })
                .Build();

            var extendedInfo = DaoFactory.ChainDao.GetChains(exp);

            foreach (var chain in filteredConversations)
            {
                var chainMessages = extendedInfo.FindAll(x => x.MailboxId == chain.MailboxId && x.Id == chain.ChainId);
                if (!chainMessages.Any()) continue;
                chain.IsNew = chainMessages.Any(x => x.Unread);
                chain.HasAttachments = chainMessages.Any(x => x.HasAttachments);
                chain.Important = chainMessages.Any(x => x.Importance);
                chain.ChainLength = chainMessages.Sum(x => x.Length);
                var firstOrDefault = chainMessages.FirstOrDefault(x => !string.IsNullOrEmpty(x.Tags));
                chain.LabelsString = firstOrDefault != null ? firstOrDefault.Tags : "";
            }

            return filteredConversations;
        }

        public List<MailMessageData> GetConversationMessages(int tenant, string user, int messageId,
            bool loadAllContent, bool needProxyHttp, bool needMailSanitazer, bool markRead = false)
        {
            var messageInfo = DaoFactory.MailInfoDao.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(tenant, user)
                    .SetMessageId(messageId)
                    .Build())
                .SingleOrDefault();

            if (messageInfo == null)
                throw new ArgumentException("Message Id not found");

            var searchFolders = new List<int>();

            if (messageInfo.Folder == FolderType.Inbox || messageInfo.Folder == FolderType.Sent)
                searchFolders.AddRange(new[] { (int)FolderType.Inbox, (int)FolderType.Sent });
            else
                searchFolders.Add((int)messageInfo.Folder);

            var exp = SimpleMessagesExp.CreateBuilder(tenant, user)
                .SetMailboxId(messageInfo.MailboxId)
                .SetChainId(messageInfo.ChainId)
                .SetFoldersIds(searchFolders)
                .Build();

            var mailInfoList = DaoFactory.MailInfoDao.GetMailInfoList(exp);

            var ids = mailInfoList.Select(m => m.Id).ToList();

            var messages =
                ids.ConvertAll<MailMessageData>(id => {
                    return MessageEngine.GetMessage(id,
                        new MailMessageData.Options
                        {
                            LoadImages = false,
                            LoadBody = loadAllContent || (id == messageId),
                            NeedProxyHttp = needProxyHttp,
                            NeedSanitizer = needMailSanitazer
                        });
                    })
                    .Where(mailInfo => mailInfo != null)
                    .OrderBy(m => m.Date)
                    .ToList();

            if (!markRead)
                return messages;

            var unreadMessages = messages.Where(message => message.WasNew).ToList();
            if (!unreadMessages.Any())
                return messages;

            var unreadMessagesCountByFolder = new Dictionary<FolderType, int>();

            foreach (var message in unreadMessages)
            {
                if (unreadMessagesCountByFolder.ContainsKey(message.Folder))
                    unreadMessagesCountByFolder[message.Folder] += 1;
                else
                    unreadMessagesCountByFolder.Add(message.Folder, 1);
            }

            uint? userFolder = null;

            if (unreadMessagesCountByFolder.Keys.Any(k => k == FolderType.UserFolder))
            {
                var item = DaoFactory.UserFolderXMailDao.Get(ids.First());
                userFolder = item == null ? (uint?)null : item.FolderId;
            }

            List<int> ids2Update;

            using (var tx = DaoFactory.BeginTransaction())
            {
                ids2Update = unreadMessages.Select(x => x.Id).ToList();

                DaoFactory.MailInfoDao.SetFieldValue(
                    SimpleMessagesExp.CreateBuilder(tenant, user)
                        .SetMessageIds(ids2Update)
                        .Build(),
                    "Unread",
                    false);

                foreach (var keyPair in unreadMessagesCountByFolder)
                {
                    var folderType = keyPair.Key;

                    var unreadMessDiff = keyPair.Value != 0 ? keyPair.Value * (-1) : (int?)null;

                    FolderEngine.ChangeFolderCounters(folderType, userFolder,
                            unreadMessDiff, unreadConvDiff: -1);

                    DaoFactory.ChainDao.SetFieldValue(
                        SimpleConversationsExp.CreateBuilder(tenant, user)
                            .SetChainId(messageInfo.ChainId)
                            .SetMailboxId(messageInfo.MailboxId)
                            .SetFolder((int)keyPair.Key)
                            .Build(),
                        "Unread",
                        false);
                }

                if (userFolder.HasValue)
                {
                    var userFoldersIds = DaoFactory.UserFolderXMailDao.GetList(mailIds: ids)
                        .Select(ufxm => (int)ufxm.FolderId)
                        .Distinct()
                        .ToList();

                    UserFolderEngine.RecalculateCounters(DaoFactory, userFoldersIds);
                }

                tx.Commit();
            }

            var data = new MailWrapper
            {
                Unread = false
            };

            IndexEngine.Update(data, s => s.In(m => m.Id, ids2Update.ToArray()), wrapper => wrapper.Unread);

            return messages;
        }

        public List<MailInfo> GetChainedMessagesInfo(List<int> ids)
        {
            var chainsInfo = DaoFactory.MailInfoDao.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(Tenant, User)
                    .SetMessageIds(ids)
                    .Build());

            var chainArray = chainsInfo.Select(r => r.ChainId).Distinct().ToArray();

            const int max_query_count = 25;
            var i = 0;
            var unsortedMessages = new List<MailInfo>();

            do
            {
                var partChains = chainArray.Skip(i).Take(max_query_count).ToList();

                if (!partChains.Any())
                    break;

                var exp = SimpleMessagesExp.CreateBuilder(Tenant, User)
                        .SetChainIds(partChains)
                        .Build();

                var selectedMessages = DaoFactory.MailInfoDao
                    .GetMailInfoList(exp);

                unsortedMessages.AddRange(selectedMessages);

                i += max_query_count;

            } while (true);

            var result = unsortedMessages
                .Where(r => chainsInfo.FirstOrDefault(c =>
                    c.ChainId == r.ChainId &&
                    c.MailboxId == r.MailboxId &&
                    ((r.Folder == FolderType.Inbox || r.Folder == FolderType.Sent)
                        ? c.Folder == FolderType.Inbox || c.Folder == FolderType.Sent
                        : c.Folder == r.Folder)) != null)
                .ToList();

            return result;
        }

        public void SetConversationsFolder(List<int> ids, FolderType folder, uint? userFolderId = null)
        {
            if (!ids.Any())
                throw new ArgumentNullException("ids");

            List<MailInfo> listObjects;

            listObjects = GetChainedMessagesInfo(ids);

            if (!listObjects.Any())
                return;

            using (var tx = DaoFactory.BeginTransaction())
            {
                MessageEngine.SetFolder(DaoFactory, listObjects, folder, userFolderId);
                tx.Commit();
            }


            if (folder == FolderType.Inbox || folder == FolderType.Sent || folder == FolderType.Spam)
                OperationEngine.ApplyFilters(listObjects.Select(o => o.Id).ToList());

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
                s => s.In(m => m.Id, listObjects.Select(o => o.Id).ToArray());

            IndexEngine.Update(data, exp, w => w.Folder);

            IndexEngine.Update(data, exp, UpdateAction.Replace, w => w.UserFolders);
        }

        public void RestoreConversations(int tenant, string user, List<int> ids)
        {
            if (!ids.Any())
                throw new ArgumentNullException("ids");

            List<MailInfo> listObjects;

            listObjects = GetChainedMessagesInfo(ids);

            if (!listObjects.Any())
                return;

            using (var tx = DaoFactory.BeginTransaction())
            {
                MessageEngine.Restore(DaoFactory, listObjects);
                tx.Commit();
            }

            var filterApplyIds =
                listObjects.Where(
                    m =>
                        m.FolderRestore == FolderType.Inbox || m.FolderRestore == FolderType.Sent ||
                        m.FolderRestore == FolderType.Spam).Select(m => m.Id).ToList();

            if (filterApplyIds.Any())
                OperationEngine.ApplyFilters(filterApplyIds);

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t))
                return;

            var mails = listObjects.ConvertAll(m => new MailWrapper
            {
                Id = m.Id,
                Folder = (byte)m.FolderRestore
            });

            IndexEngine.Update(mails, wrapper => wrapper.Folder);
        }

        public void DeleteConversations(int tenant, string user, List<int> ids)
        {
            if (!ids.Any())
                throw new ArgumentNullException("ids");

            long usedQuota;

            List<MailInfo> listObjects;

            listObjects = GetChainedMessagesInfo(ids);

            if (!listObjects.Any())
                return;

            using (var tx = DaoFactory.BeginTransaction())
            {
                usedQuota = MessageEngine.SetRemoved(DaoFactory, listObjects);
                tx.Commit();
            }

            QuotaEngine.QuotaUsedDelete(usedQuota);

            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t))
                return;

            IndexEngine.Remove(listObjects.Select(info => info.Id).ToList(), Tenant, new Guid(User));
        }

        private const string MM_ALIAS = "mm";

        public void SetConversationsImportanceFlags(int tenant, string user, bool important, List<int> ids)
        {
            List<MailInfo> mailInfos;

            mailInfos = GetChainedMessagesInfo(ids);

            var chainsInfo = mailInfos
                .Select(m => new
                {
                    m.ChainId,
                    m.MailboxId,
                    m.Folder
                })
                .Distinct().ToList();

            if (!chainsInfo.Any())
                throw new Exception("no chain messages belong to current user");

            using (var tx = DaoFactory.BeginTransaction())
            {
                Expression<Func<Dao.Entities.MailMail, bool>> exp = t => true;
                foreach (var chain in chainsInfo)
                {
                    Expression<Func<Dao.Entities.MailMail, bool>> innerWhere = m => m.ChainId == chain.ChainId && m.IdMailbox == chain.MailboxId;

                    if (chain.Folder == FolderType.Inbox || chain.Folder == FolderType.Sent)
                    {
                        innerWhere = innerWhere.And(m => m.Folder == (int)FolderType.Inbox || m.Folder == (int)FolderType.Sent);
                    }
                    else
                    {
                        innerWhere = innerWhere.And(m => m.Folder == (int)chain.Folder);
                    }

                    exp = exp.Or(innerWhere);
                }

                DaoFactory.MailInfoDao.SetFieldValue(
                    SimpleMessagesExp.CreateBuilder(tenant, user)
                        .SetExp(exp)
                        .Build(),
                    "Importance",
                    important);

                foreach (var message in ids)
                {
                    UpdateMessageChainImportanceFlag(tenant, user, message);
                }

                tx.Commit();
            }

            var data = new MailWrapper
            {
                Importance = important
            };

            IndexEngine.Update(data, s => s.In(m => m.Id, mailInfos.Select(o => o.Id).ToArray()),
                    wrapper => wrapper.Importance);
        }

        public void UpdateMessageChainAttachmentsFlag(int tenant, string user, int messageId)
        {
            UpdateMessageChainFlag(tenant, user, messageId, "AttachCount", "HasAttachments");
        }

        public void UpdateMessageChainUnreadFlag(int tenant, string user, int messageId)
        {
            UpdateMessageChainFlag(tenant, user, messageId, "Unread", "Unread");
        }

        public void UpdateMessageChainImportanceFlag(int tenant, string user, int messageId)
        {
            UpdateMessageChainFlag(tenant, user, messageId, "Importance", "Importance");
        }

        private void UpdateMessageChainFlag(int tenant, string user, int messageId, string fieldFrom, string fieldTo)
        {
            var mail = DaoFactory.MailDao.GetMail(new ConcreteUserMessageExp(messageId, tenant, user));

            if (mail == null)
                return;

            var maxValue = DaoFactory.MailInfoDao.GetFieldMaxValue<bool>(
                SimpleMessagesExp.CreateBuilder(tenant, user)
                    .SetChainId(mail.ChainId)
                    .SetMailboxId(mail.MailboxId)
                    .SetFolder((int)mail.Folder)
                    .Build(),
                fieldFrom);

            DaoFactory.ChainDao.SetFieldValue(
                SimpleConversationsExp.CreateBuilder(tenant, user)
                    .SetChainId(mail.ChainId)
                    .SetMailboxId(mail.MailboxId)
                    .SetFolder((int)mail.Folder)
                    .Build(),
                fieldTo,
                maxValue);
        }

        public void UpdateChainFields(int tenant, string user, List<int> ids)
        {
            var mailInfoList = DaoFactory.MailInfoDao.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(tenant, user, null)
                    .SetMessageIds(ids)
                    .Build())
                .ConvertAll(x => new
                {
                    id_mailbox = x.MailboxId,
                    chain_id = x.ChainId,
                    folder = x.Folder
                });

            if (!mailInfoList.Any()) return;

            foreach (var info in mailInfoList.GroupBy(t => new { t.id_mailbox, t.chain_id, t.folder }))
            {
                uint? userFolder = null;

                if (info.Key.folder == FolderType.UserFolder)
                {
                    var item = DaoFactory.UserFolderXMailDao.Get(ids.First());
                    userFolder = item == null ? (uint?)null : item.FolderId;
                }

                UpdateChain(info.Key.chain_id, info.Key.folder, userFolder, info.Key.id_mailbox, tenant, user);
            }
        }

        // Method for updating chain flags, date and length.
        public void UpdateChain(string chainId, FolderType folder, uint? userFolderId, int mailboxId, 
            int tenant, string user)
        {
            if (string.IsNullOrEmpty(chainId)) return;

            var chainInfo = DaoFactory.MailDb.MailMail
                .Where(m => m.Tenant == Tenant
                    && m.IdUser == User
                    && m.IsRemoved == false
                    && m.ChainId == chainId
                    && m.IdMailbox == mailboxId
                    && m.Folder == (int)folder)
                .GroupBy(m => m.Id)
                .Select(g => new
                {
                    length = g.Count(),
                    date = g.Max(m => m.DateSent),
                    unread = g.Max(m => m.Unread),
                    attach_count = g.Max(m => m.AttachmentsCount),
                    importance = g.Max(m => m.Importance)
                })
                .FirstOrDefault();

            if (chainInfo == null)
                throw new InvalidDataException("Conversation is absent in MAIL_MAIL");

            var query = SimpleConversationsExp.CreateBuilder(tenant, user)
                .SetMailboxId(mailboxId)
                .SetChainId(chainId)
                .SetFolder((int)folder)
                .Build();

            var storedChainInfo = DaoFactory.ChainDao.GetChains(query);

            var chainUnreadFlag = storedChainInfo.Any(c => c.Unread);

            if (0 == chainInfo.length)
            {
                var deletQuery = SimpleConversationsExp.CreateBuilder(tenant, user)
                    .SetFolder((int)folder)
                    .SetMailboxId(mailboxId)
                    .SetChainId(chainId)
                    .Build();

                var result = DaoFactory.ChainDao.Delete(deletQuery);

                Log.DebugFormat(
                    "UpdateChain() row deleted from chain table tenant='{0}', user_id='{1}', id_mailbox='{2}', folder='{3}', chain_id='{4}' result={5}",
                    tenant, user, mailboxId, folder, chainId, result);

                var unreadConvDiff = chainUnreadFlag ? -1 : (int?) null;

                FolderEngine.ChangeFolderCounters(folder, userFolderId,
                    unreadConvDiff: unreadConvDiff, totalConvDiff: -1);
            }
            else
            {
                var updateQuery = SimpleMessagesExp.CreateBuilder(tenant, user)
                        .SetChainId(chainId)
                        .SetMailboxId(mailboxId)
                        .SetFolder((int)folder)
                        .Build();

                DaoFactory.MailInfoDao.SetFieldValue(updateQuery,
                    "ChainDate",
                    chainInfo.date);

                var tags = GetChainTags(chainId, folder, mailboxId, tenant, user);

                var chain = new Chain
                {
                    Id = chainId,
                    Tenant = tenant,
                    User = user,
                    MailboxId = mailboxId,
                    Folder = folder,
                    Length = chainInfo.length,
                    Unread = chainInfo.unread,
                    HasAttachments = chainInfo.attach_count > 0,
                    Importance = chainInfo.importance,
                    Tags = tags
                };

                var result = DaoFactory.ChainDao.SaveChain(chain);

                if (result <= 0)
                    throw new InvalidOperationException("Invalid insert into mail_chain");

                Log.DebugFormat(
                    "UpdateChain() row inserted to chain table tenant='{0}', user_id='{1}', id_mailbox='{2}', folder='{3}', chain_id='{4}'",
                    tenant, user, mailboxId, folder, chainId);

                var unreadConvDiff = (int?) null;
                var totalConvDiff = (int?) null;

                if (!storedChainInfo.Any())
                {
                    totalConvDiff = 1;
                    unreadConvDiff = chainInfo.unread ? 1 : (int?) null;
                }
                else
                {
                    if (chainUnreadFlag != chainInfo.unread)
                    {
                        unreadConvDiff = chainInfo.unread ? 1 : -1;
                    }
                }

                FolderEngine.ChangeFolderCounters(folder, userFolderId,
                    unreadConvDiff: unreadConvDiff, totalConvDiff: totalConvDiff);
            }
        }

        public void UpdateChainTags(IDaoFactory daoFactory, string chainId, FolderType folder, int mailboxId, int tenant, string user)
        {
            var tags = GetChainTags(chainId, folder, mailboxId, tenant, user);

            var updateQuery = SimpleConversationsExp.CreateBuilder(tenant, user)
                    .SetChainId(chainId)
                    .SetMailboxId(mailboxId)
                    .SetFolder((int)folder)
                    .Build();

            DaoFactory.ChainDao.SetFieldValue(
                updateQuery,
                "Tags",
                tags);
        }

        private string GetChainTags(string chainId, FolderType folder, int mailboxId, int tenant, string user)
        {
            var tags = DaoFactory.MailDb.MailTagMail.Join(DaoFactory.MailDb.MailMail, t => t.IdMail, m => m.Id,
                (t, m) => new
                {
                    Tag = t,
                    Mail = m
                })
                .Where(g => g.Mail.ChainId == chainId && g.Mail.IsRemoved == false && g.Mail.Folder == (int)folder && g.Mail.IdMailbox == mailboxId)
                .Where(g => g.Tag.Tenant == Tenant && g.Tag.IdUser == User)
                .OrderBy(g => g.Tag.TimeCreated)
                .GroupBy(g => g.Tag.IdTag)
                .Select(g => g.Key)
                .ToList();

            return string.Join(",", tags);
        }

        private List<MailMessageData> GetFilteredConversations(MailSearchFilterData filter, out bool hasMore)
        {

            var conversations = new List<MailMessageData>();
            var skipFlag = false;
            var chunkIndex = 0;

            if (filter.FromDate.HasValue && filter.FromMessage.HasValue && filter.FromMessage.Value > 0)
            {
                skipFlag = true;
            }

            // Invert sort order for back paging
            if (filter.PrevFlag.GetValueOrDefault(false))
            {
                filter.SortOrder = filter.SortOrder == Defines.ASCENDING
                    ? Defines.DESCENDING
                    : Defines.ASCENDING;
            }

            var tenantInfo = TenantManager.GetTenant(Tenant);
            var utcNow = DateTime.UtcNow;
            var pageSize = filter.PageSize.GetValueOrDefault(25);

            while (conversations.Count < pageSize + 1)
            {
                filter.PageSize = CHUNK_SIZE*pageSize;

                IMessagesExp exp = null;

                var t = ServiceProvider.GetService<MailWrapper>();
                if (!filter.IsDefault() && FactoryIndexerHelper.Support(t) && FactoryIndexer.FactoryIndexerCommon.CheckState(false))
                {
                    filter.Page = chunkIndex*CHUNK_SIZE*pageSize; // Elastic Limit from {index of last message} to {count of messages}

                    if (FilterChainMessagesExp.TryGetFullTextSearchChains(FactoryIndexer, FactoryIndexerHelper, ServiceProvider,
                        filter, User, out List<MailWrapper> mailWrappers))
                    {
                        if (!mailWrappers.Any())
                            break;

                        var ids = mailWrappers.Select(c => c.Id).ToList();

                        exp = SimpleMessagesExp.CreateBuilder(Tenant, User)
                            .SetMessageIds(ids)
                            .SetOrderBy(filter.Sort)
                            .SetOrderAsc(filter.SortOrder == Defines.ASCENDING)
                            .Build();
                    }
                }
                else
                {
                    filter.Page = chunkIndex; // MySQL Limit from {page by size} to {size}

                    exp = new FilterChainMessagesExp(filter, Tenant, User);
                }

                chunkIndex++;

                var listMessages = DaoFactory.MailInfoDao.GetMailInfoList(exp, true)
                    .ConvertAll(m => MessageEngine.ToMailMessage(m, tenantInfo, utcNow));

                if (0 == listMessages.Count)
                    break;

                if (skipFlag && filter.FromMessage.HasValue)
                {
                    var messageData = listMessages.FirstOrDefault(m => m.Id == filter.FromMessage.Value);

                    if (messageData != null)
                    {
                        // Skip chain messages by FromMessage.
                        listMessages =
                            listMessages.Where(
                                m => !(m.ChainId.Equals(messageData.ChainId) && m.MailboxId == messageData.MailboxId))
                                .ToList();
                    }

                    skipFlag = false;
                }

                foreach (var messageData in listMessages)
                {
                    var existingChainIndex =
                        conversations.FindIndex(
                            c => c.ChainId == messageData.ChainId && c.MailboxId == messageData.MailboxId);

                    if (existingChainIndex > -1)
                    {
                        if (conversations[existingChainIndex].Date < messageData.Date)
                            conversations[existingChainIndex] = messageData;
                    }
                    else
                    {
                        conversations.Add(messageData);
                    }
                }

                if (conversations.Count > pageSize)
                    break;
            }

            hasMore = conversations.Count > pageSize;

            if (hasMore)
            {
                conversations = conversations.Take(pageSize).ToList();
            }

            if (filter.PrevFlag.GetValueOrDefault(false))
            {
                conversations.Reverse();
            }

            return conversations;
        }
    }
}
