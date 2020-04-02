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


using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.ElasticSearch;
using ASC.Mail.Core.Dao;
using ASC.Mail.Core.Dao.Expressions.Message;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;
using ASC.Mail.Models;
using ASC.Web.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace ASC.Mail.Core.Engine
{
    public class TagEngine
    {
        public DbContextManager<MailDbContext> DbContext { get; }
        public int Tenant
        {
            get
            {
                return ApiContext.Tenant.TenantId;
            }
        }
        public string UserId
        {
            get
            {
                return SecurityContext.CurrentAccount.ID.ToString();
            }
        }
        public SecurityContext SecurityContext { get; }
        public ApiContext ApiContext { get; }
        public ILog Log { get; }
        public DaoFactory DaoFactory { get; }
        public ChainEngine ChainEngine { get; }
        public IndexEngine IndexEngine { get; }
        public FactoryIndexer<MailWrapper> FactoryIndexer { get; }
        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public IServiceProvider ServiceProvider { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public MailDbContext MailDb { get; }
        public TagEngine(
            ApiContext apiContext,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> option,
            DaoFactory daoFactory,
            ChainEngine chainEngine,
            IndexEngine indexEngine,
            FactoryIndexer<MailWrapper> factoryIndexer,
            FactoryIndexerHelper factoryIndexerHelper,
            IServiceProvider serviceProvider,
            WebItemSecurity webItemSecurity)
        {
            ApiContext = apiContext;
            SecurityContext = securityContext;

            DaoFactory = daoFactory;
            ChainEngine = chainEngine;
            IndexEngine = indexEngine;
            MailDb = DaoFactory.MailDb;

            FactoryIndexer = factoryIndexer;
            FactoryIndexerHelper = factoryIndexerHelper;
            ServiceProvider = serviceProvider;
            WebItemSecurity = webItemSecurity;

            Log = option.Get("ASC.Mail.TagEngine");
        }

        public Tag GetTag(int id)
        {
            return DaoFactory.TagDao.GetTag(id);
        }

        public Tag GetTag(string name)
        {
            return DaoFactory.TagDao.GetTag(name);
        }

        public List<Tag> GetTags()
        {
            var tagList = DaoFactory.TagDao.GetTags();

            if (!WebItemSecurity.IsAvailableForMe(WebItemManager.CRMProductID))
            {
                return tagList
                    .Where(p => p.TagName != "")
                    .OrderByDescending(p => p.Id)
                    .ToList();
            }

            var actualCrmTags = DaoFactory.TagDao.GetCrmTags();

            var removedCrmTags =
                tagList.Where(t => t.Id < 0 && !actualCrmTags.Exists(ct => ct.Id == t.Id))
                    .ToList();

            if (removedCrmTags.Any())
            {
                DaoFactory.TagDao.DeleteTags(removedCrmTags.Select(t => t.Id).ToList());
                removedCrmTags.ForEach(t => tagList.Remove(t));
            }

            foreach (var crmTag in actualCrmTags)
            {
                var tag = tagList.FirstOrDefault(t => t.Id == crmTag.Id);
                if (tag != null)
                    tag.TagName = crmTag.TagName;
                else
                    tagList.Add(crmTag);
            }

            return tagList
                .Where(p => !string.IsNullOrEmpty(p.TagName))
                .OrderByDescending(p => p.Id)
                .ToList();
        }

        public List<Tag> GetCrmTags(string email)
        {
            var tags = new List<Tag>();

            var allowedContactIds = DaoFactory.CrmContactDao.GetCrmContactIds(email);

            if (!allowedContactIds.Any())
                return tags;

            tags = DaoFactory.TagDao.GetCrmTags(allowedContactIds);

            return tags
                .Where(p => !string.IsNullOrEmpty(p.TagName))
                .OrderByDescending(p => p.Id)
                .ToList();
        }

        public bool IsTagExists(string name)
        {
            var tag = DaoFactory.TagDao.GetTag(name);

            return tag != null;

        }

        public Tag CreateTag(string name, string style, IEnumerable<string> addresses)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            //TODO: Need transaction?

            var tag = DaoFactory.TagDao.GetTag(name);

            if (tag != null)
                throw new ArgumentException("Tag name already exists");

            var emails = addresses as IList<string> ?? addresses.ToList();

            tag = new Tag
            {
                Id = 0,
                TagName = name,
                Tenant = Tenant,
                User = UserId,
                Addresses = string.Join(";", emails),
                Style = style,
                Count = 0,
                CrmId = 0
            };

            var id = DaoFactory.TagDao.SaveTag(tag);

            if (id < 0)
                throw new Exception("Save failed");

            foreach (var email in emails)
            {
                DaoFactory.TagAddressDao.Save(id, email);
            }

            tag.Id = id;

            //Commit transaction

            return tag;
        }

        public Tag UpdateTag(int id, string name, string style, IEnumerable<string> addresses)
        {
            var tag = DaoFactory.TagDao.GetTag(id);

            if (tag == null)
                throw new ArgumentException(@"Tag not found");

            if (!tag.TagName.Equals(name))
            {
                var tagByName = DaoFactory.TagDao.GetTag(name);

                if (tagByName != null && tagByName.Id != id)
                    throw new ArgumentException(@"Tag name already exists");

                tag.TagName = name;
                tag.Style = style;
            }

            //Start transaction
            var oldAddresses = DaoFactory.TagAddressDao.GetTagAddresses(tag.Id);

            var newAddresses = addresses as IList<string> ?? addresses.ToList();
            tag.Addresses = string.Join(";", newAddresses);

            DaoFactory.TagDao.SaveTag(tag);

            if (!newAddresses.Any())
            {
                if (oldAddresses.Any())
                    DaoFactory.TagAddressDao.Delete(tag.Id);
            }
            else
            {
                foreach (var oldAddress in oldAddresses)
                {
                    if (!newAddresses.Contains(oldAddress))
                    {
                        DaoFactory.TagAddressDao.Delete(tag.Id, oldAddress);
                    }
                }

                foreach (var newAddress in newAddresses)
                {
                    if (!oldAddresses.Contains(newAddress))
                    {
                        DaoFactory.TagAddressDao.Save(tag.Id, newAddress);
                    }
                }
            }

            //Commit transaction

            return tag;
        }

        public bool DeleteTag(int id)
        {
            //Begin transaction

            DaoFactory.TagDao.DeleteTag(id);

            DaoFactory.TagAddressDao.Delete(id);

            DaoFactory.TagMailDao.DeleteByTagId(id);

            //Commit transaction

            return true;
        }

        public List<int> GetOrCreateTags(int tenant, string user, string[] names)
        {
            var tagIds = new List<int>();

            if (!names.Any())
                return tagIds;


            var tags = DaoFactory.TagDao.GetTags();

            foreach (var name in names)
            {
                var tag =
                    tags.FirstOrDefault(t => t.TagName.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (tag != null)
                {
                    tagIds.Add(tag.Id);
                    continue;
                }

                tag = new Tag
                {
                    Id = 0,
                    TagName = name,
                    Addresses = "",
                    Count = 0,
                    CrmId = 0,
                    Style = (Math.Abs(name.GetHashCode() % 16) + 1).ToString(CultureInfo.InvariantCulture),
                    Tenant = tenant,
                    User = user
                };

                var id = DaoFactory.TagDao.SaveTag(tag);

                if (id > 0)
                {
                    Log.InfoFormat("TagEngine->GetOrCreateTags(): new tag '{0}' with id = {1} has bee created",
                        name, id);

                    tagIds.Add(id);
                }
            }

            return tagIds;
        }

        public void SetMessagesTag(List<int> messageIds, int tagId)
        {
            using (var tx = DaoFactory.BeginTransaction())
            {
                if (!SetMessagesTag(DaoFactory, messageIds, tagId))
                {
                    tx.Rollback();
                    return;
                }

                tx.Commit();
            }

            UpdateIndexerTags(messageIds, UpdateAction.Add, tagId);

            Log.InfoFormat("TagEngine->SetMessagesTag(): tag with id = {0} has bee added to messages [{1}]", tagId,
                string.Join(",", messageIds));
        }

        public bool SetMessagesTag(IDaoFactory daoFactory, List<int> messageIds, int tagId)
        {
            var tag = DaoFactory.TagDao.GetTag(tagId);

            if (tag == null)
            {
                return false;
            }

            GetValidForUserMessages(messageIds, out List<int> validIds, out List<ChainInfo> chains);

            DaoFactory.TagMailDao.SetMessagesTag(validIds, tag.Id);

            UpdateTagsCount(tag);
            
            foreach (var chain in chains)
            {
                ChainEngine.UpdateChainTags(daoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant, UserId);
            }

            // Change time_modified for index
            DaoFactory.MailDao.SetMessagesChanged(validIds);

            return true;
        }

        public void UnsetMessagesTag(List<int> messageIds, int tagId)
        {
            List<int> validIds;


            using (var tx = DaoFactory.BeginTransaction())
            {
                GetValidForUserMessages(messageIds, out validIds, out List<ChainInfo> chains);

                DaoFactory.TagMailDao.Delete(tagId, validIds);

                var tag = DaoFactory.TagDao.GetTag(tagId);

                if (tag != null)
                    UpdateTagsCount(tag);

                foreach (var chain in chains)
                {
                    ChainEngine.UpdateChainTags(DaoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant,
                        UserId);
                }

                // Change time_modified for index
                DaoFactory.MailDao.SetMessagesChanged(validIds);

                tx.Commit();
            }

            UpdateIndexerTags(validIds, UpdateAction.Remove, tagId);
        }

        public void SetConversationsTag(IEnumerable<int> messagesIds, int tagId)
        {
            var ids = messagesIds as IList<int> ?? messagesIds.ToList();

            if (!ids.Any()) return;

            List<int> validIds;

            using (var tx = DaoFactory.BeginTransaction())
            {
                var tag = DaoFactory.TagDao.GetTag(tagId);

                if (tag == null)
                {
                    tx.Rollback();
                    return;
                }

                var foundedChains = ChainEngine.GetChainedMessagesInfo(messagesIds.ToList());

                if (!foundedChains.Any())
                {
                    tx.Rollback();
                    return;
                }

                validIds = foundedChains.Select(r => r.Id).ToList();
                var chains =
                    foundedChains.GroupBy(r => new { r.ChainId, r.Folder, r.MailboxId })
                        .Select(
                            r =>
                                new ChainInfo
                                {
                                    Id = r.Key.ChainId,
                                    Folder = r.Key.Folder,
                                    MailboxId = r.Key.MailboxId
                                });

                DaoFactory.TagMailDao.SetMessagesTag(validIds, tag.Id);

                UpdateTagsCount(tag);

                foreach (var chain in chains)
                {
                    ChainEngine.UpdateChainTags(DaoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant,
                        UserId);
                }

                // Change time_modified for index
                DaoFactory.MailDao.SetMessagesChanged(validIds);

                tx.Commit();
            }

            UpdateIndexerTags(validIds, UpdateAction.Add, tagId);
        }

        public void UnsetConversationsTag(IEnumerable<int> messagesIds, int tagId)
        {
            var ids = messagesIds as IList<int> ?? messagesIds.ToList();

            if (!ids.Any()) return;

            List<int> validIds;

            using (var tx = DaoFactory.BeginTransaction())
            {
                var foundedChains = ChainEngine.GetChainedMessagesInfo(messagesIds.ToList());

                if (!foundedChains.Any())
                {
                    tx.Rollback();
                    return;
                }

                validIds = foundedChains.Select(r => r.Id).ToList();

                var chains =
                    foundedChains.GroupBy(r => new { r.ChainId, r.Folder, r.MailboxId })
                        .Select(
                            r =>
                                new ChainInfo
                                {
                                    Id = r.Key.ChainId,
                                    Folder = r.Key.Folder,
                                    MailboxId = r.Key.MailboxId
                                });

                DaoFactory.TagMailDao.Delete(tagId, validIds);

                var tag = DaoFactory.TagDao.GetTag(tagId);

                if (tag != null)
                    UpdateTagsCount(tag);

                foreach (var chain in chains)
                {
                    ChainEngine.UpdateChainTags(DaoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant,
                        UserId);
                }

                // Change time_modified for index
                DaoFactory.MailDao.SetMessagesChanged(validIds);

                tx.Commit();
            }

            UpdateIndexerTags(validIds, UpdateAction.Remove, tagId);
        }

        private void UpdateIndexerTags(List<int> ids, UpdateAction action, int tagId)
        {
            var t = ServiceProvider.GetService<MailWrapper>();
            if (!FactoryIndexerHelper.Support(t) || !FactoryIndexer.FactoryIndexerCommon.CheckState(false))
                return;

            if(ids == null || !ids.Any())
                return;

            var data = new MailWrapper
            {
                Tags = new List<TagWrapper>
                    {
                        new TagWrapper
                        {
                            Id = tagId
                        }
                    }
            };

            Expression<Func<Selector<MailWrapper>, Selector<MailWrapper>>> exp =
                s => s.In(m => m.Id, ids.ToArray());

            IndexEngine.Update(data, exp, action, s => s.Tags);
        }

        private void UpdateTagsCount(Tag tag)
        {
            var count = DaoFactory.TagMailDao.CalculateTagCount(tag.Id);

            tag.Count = count;

            DaoFactory.TagDao.SaveTag(tag);
        }

        private void GetValidForUserMessages(List<int> messagesIds, out List<int> validIds,
            out List<ChainInfo> chains)
        {
            var mailInfoList = DaoFactory.MailInfoDao.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(Tenant, UserId)
                    .SetMessageIds(messagesIds)
                    .Build());

            validIds = new List<int>();
            chains = new List<ChainInfo>();

            foreach (var mailInfo in mailInfoList)
            {
                validIds.Add(mailInfo.Id);
                chains.Add(new ChainInfo
                {
                    Id = mailInfo.ChainId,
                    Folder = mailInfo.Folder,
                    MailboxId = mailInfo.MailboxId
                });
            }
        }
    }

    public static class TagEngineeExtension
    {
        public static DIHelper AddTagEngineService(this DIHelper services)
        {
            services.TryAddScoped<TagEngine>();

            return services;
        }
    }
}
