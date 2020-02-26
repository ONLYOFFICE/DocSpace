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
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;
using ASC.Mail.Models;
using ASC.Mail.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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
        public MailDbContext MailDb { get; }
        public TagEngine(
            DbContextManager<MailDbContext> dbContext,
            ApiContext apiContext,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> option,
            DaoFactory daoFactory)
        {
            ApiContext = apiContext;
            SecurityContext = securityContext;
            Log = option.Get("ASC.Mail.TagEngine");

            MailDb = dbContext.Get("mail");
            DaoFactory = daoFactory;
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

            //TODO: Fix if CRM exist
            //if (!WebItemSecurity.IsAvailableForMe(WebItemManager.CRMProductID)) 
            //{
            //    return tagList
            //        .Where(p => p.TagName != "")
            //        .OrderByDescending(p => p.Id)
            //        .ToList();
            //}

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

        //public List<Tag> GetCrmTags(string email)
        //{
        //    var tags = new List<Tag>();

        //    using (var daoFactory = new DaoFactory())
        //    {
        //        var daoCrmContacts = daoFactory.CreateCrmContactDao(Tenant, User);

        //        var allowedContactIds = daoCrmContacts.GetCrmContactIds(email);

        //        if (!allowedContactIds.Any())
        //            return tags;

        //        var daoTag = daoFactory.CreateTagDao(Tenant, User);

        //        tags = daoTag.GetCrmTags(allowedContactIds);

        //        return tags
        //            .Where(p => !string.IsNullOrEmpty(p.TagName))
        //            .OrderByDescending(p => p.Id)
        //            .ToList();
        //    }
        //}

        public bool IsTagExists(string name)
        {
            var tag = DaoFactory.TagDao.GetTag(name);

            return tag != null;

        }

        /*
        public Tag CreateTag(string name, string style, IEnumerable<string> addresses)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var tagDao = DaoFactory.CreateTagDao(); //Need transaction

            var tagAddressDao = DaoFactory.CreateTagAddressDao();

            var tag = tagDao.GetTag(name);

            if (tag != null)
                throw new ArgumentException("Tag name already exists");

            var emails = addresses as IList<string> ?? addresses.ToList();

            tag = new Tag
            {
                Id = 0,
                TagName = name,
                Tenant = Tenant,
                User = User,
                Addresses = string.Join(";", emails),
                Style = style,
                Count = 0,
                CrmId = 0
            };

            var id = tagDao.SaveTag(tag);

            if (id < 0)
                throw new Exception("Save failed");

            foreach (var email in emails)
            {
                tagAddressDao.Save(id, email);
            }

            tag.Id = id;

            //Commit transaction

            return tag;
        }

        public Tag UpdateTag(int id, string name, string style, IEnumerable<string> addresses)
        {
            var tagDao = DaoFactory.CreateTagDao();

            var tagAddressDao = DaoFactory.CreateTagAddressDao();

            var tag = tagDao.GetTag(id);

            if (tag == null)
                throw new ArgumentException(@"Tag not found");

            if (!tag.TagName.Equals(name))
            {
                var tagByName = tagDao.GetTag(name);

                if (tagByName != null && tagByName.Id != id)
                    throw new ArgumentException(@"Tag name already exists");

                tag.TagName = name;
                tag.Style = style;
            }

            //Start transaction
            var oldAddresses = tagAddressDao.GetTagAddresses(tag.Id);

            var newAddresses = addresses as IList<string> ?? addresses.ToList();
            tag.Addresses = string.Join(";", newAddresses);

            tagDao.SaveTag(tag);

            if (!newAddresses.Any())
            {
                if (oldAddresses.Any())
                    tagAddressDao.Delete(tag.Id);
            }
            else
            {
                foreach (var oldAddress in oldAddresses)
                {
                    if (!newAddresses.Contains(oldAddress))
                    {
                        tagAddressDao.Delete(tag.Id, oldAddress);
                    }
                }

                foreach (var newAddress in newAddresses)
                {
                    if (!oldAddresses.Contains(newAddress))
                    {
                        tagAddressDao.Save(tag.Id, newAddress);
                    }
                }
            }

            //Commit transaction

            return tag;
        }

        public bool DeleteTag(int id)
        {
            //Begin transaction
            var tagDao = DaoFactory.CreateTagDao();

            var tagAddressDao = DaoFactory.CreateTagAddressDao();

            var tagMailDao = DaoFactory.CreateTagMailDao();

            tagDao.DeleteTag(id);

            tagAddressDao.Delete(id);

            tagMailDao.DeleteByTagId(id);

            //Commit transaction

            return true;
        }

        public List<int> GetOrCreateTags(int tenant, string user, string[] names)
        {
            var tagIds = new List<int>();

            if (!names.Any())
                return tagIds;

            using (var daoFactory = new DaoFactory())
            {
                var dao = daoFactory.CreateTagDao(Tenant, User);

                var tags = dao.GetTags();

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

                    var id = dao.SaveTag(tag);

                    if (id > 0)
                    {
                        Log.InfoFormat("TagEngine->GetOrCreateTags(): new tag '{0}' with id = {1} has bee created",
                            name, id);

                        tagIds.Add(id);
                    }
                }
            }

            return tagIds;
        }

        public void SetMessagesTag(List<int> messageIds, int tagId)
        {
            using (var daoFactory = new DaoFactory())
            {
                using (var tx = daoFactory.DbManager.BeginTransaction())
                {
                    if (!SetMessagesTag(daoFactory, messageIds, tagId))
                    {
                        tx.Rollback();
                        return;
                    }

                    tx.Commit();
                }
            }

            UpdateIndexerTags(messageIds, UpdateAction.Add, tagId);

            Log.InfoFormat("TagEngine->SetMessagesTag(): tag with id = {0} has bee added to messages [{1}]", tagId,
                string.Join(",", messageIds));
        }

        public bool SetMessagesTag(IDaoFactory daoFactory, List<int> messageIds, int tagId)
        {
            var daoTag = daoFactory.CreateTagDao(Tenant, User);

            var tag = daoTag.GetTag(tagId);

            if (tag == null)
            {
                return false;
            }

            List<int> validIds;
            List<ChainInfo> chains;

            GetValidForUserMessages(daoFactory, messageIds, out validIds, out chains);

            var daoTagMail = daoFactory.CreateTagMailDao(Tenant, User);

            daoTagMail.SetMessagesTag(validIds, tag.Id);

            UpdateTagsCount(daoTagMail, daoTag, tag);
            
            foreach (var chain in chains)
            {
                Factory.ChainEngine.UpdateChainTags(daoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant, User);
            }

            var daoMail = daoFactory.CreateMailDao(Tenant, User);

            // Change time_modified for index
            daoMail.SetMessagesChanged(validIds);

            return true;
        }

        public void UnsetMessagesTag(List<int> messageIds, int tagId)
        {
            List<int> validIds;

            using (var daoFactory = new DaoFactory())
            {
                var daoTagMail = daoFactory.CreateTagMailDao(Tenant, User);

                using (var tx = daoFactory.DbManager.BeginTransaction())
                {
                    var daoTag = daoFactory.CreateTagDao(Tenant, User);

                    List<ChainInfo> chains;

                    GetValidForUserMessages(daoFactory, messageIds, out validIds, out chains);

                    daoTagMail.Delete(tagId, validIds);

                    var tag = daoTag.GetTag(tagId);

                    if(tag != null)
                        UpdateTagsCount(daoTagMail, daoTag, tag);

                    foreach (var chain in chains)
                    {
                        Factory.ChainEngine.UpdateChainTags(daoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant,
                            User);
                    }

                    var daoMail = daoFactory.CreateMailDao(Tenant, User);

                    // Change time_modified for index
                    daoMail.SetMessagesChanged(validIds);

                    tx.Commit();
                }
            }

            UpdateIndexerTags(validIds, UpdateAction.Remove, tagId);
        }

        public void SetConversationsTag(IEnumerable<int> messagesIds, int tagId)
        {
            var ids = messagesIds as IList<int> ?? messagesIds.ToList();

            if (!ids.Any()) return;

            List<int> validIds;

            using (var daoFactory = new DaoFactory())
            {
                var daoTagMail = daoFactory.CreateTagMailDao(Tenant, User);

                using (var tx = daoFactory.DbManager.BeginTransaction())
                {
                    var daoTag = daoFactory.CreateTagDao(Tenant, User);

                    var tag = daoTag.GetTag(tagId);

                    if (tag == null)
                    {
                        tx.Rollback();
                        return;
                    }

                    var foundedChains = Factory.ChainEngine.GetChainedMessagesInfo(daoFactory, (List<int>)messagesIds);

                    if (!foundedChains.Any())
                    {
                        tx.Rollback();
                        return;
                    }

                    validIds = foundedChains.Select(r => r.Id).ToList();
                    var chains =
                        foundedChains.GroupBy(r => new {r.ChainId, r.Folder, r.MailboxId})
                            .Select(
                                r =>
                                    new ChainInfo
                                    {
                                        Id = r.Key.ChainId,
                                        Folder = r.Key.Folder,
                                        MailboxId = r.Key.MailboxId
                                    });

                    daoTagMail.SetMessagesTag(validIds, tag.Id);

                    UpdateTagsCount(daoTagMail, daoTag, tag);

                    foreach (var chain in chains)
                    {
                        Factory.ChainEngine.UpdateChainTags(daoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant,
                            User);
                    }

                    var daoMail = daoFactory.CreateMailDao(Tenant, User);

                    // Change time_modified for index
                    daoMail.SetMessagesChanged(validIds);

                    tx.Commit();
                }
            }

            UpdateIndexerTags(validIds, UpdateAction.Add, tagId);
        }

        public void UnsetConversationsTag(IEnumerable<int> messagesIds, int tagId)
        {
            var ids = messagesIds as IList<int> ?? messagesIds.ToList();

            if (!ids.Any()) return;

            List<int> validIds;

            using (var daoFactory = new DaoFactory())
            {
                var daoTagMail = daoFactory.CreateTagMailDao(Tenant, User);

                using (var tx = daoFactory.DbManager.BeginTransaction())
                {
                    var daoTag = daoFactory.CreateTagDao(Tenant, User);

                    var foundedChains = Factory.ChainEngine.GetChainedMessagesInfo(daoFactory, (List<int>)messagesIds);

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

                    daoTagMail.Delete(tagId, validIds);

                    var tag = daoTag.GetTag(tagId);

                    if (tag != null)
                        UpdateTagsCount(daoTagMail, daoTag, tag);

                    foreach (var chain in chains)
                    {
                        Factory.ChainEngine.UpdateChainTags(daoFactory, chain.Id, chain.Folder, chain.MailboxId, Tenant,
                            User);
                    }

                    var daoMail = daoFactory.CreateMailDao(Tenant, User);

                    // Change time_modified for index
                    daoMail.SetMessagesChanged(validIds);

                    tx.Commit();
                }
            }

            UpdateIndexerTags(validIds, UpdateAction.Remove, tagId);
        }

        private void UpdateIndexerTags(List<int> ids, UpdateAction action, int tagId)
        {
            if (!FactoryIndexer<MailWrapper>.Support || !FactoryIndexer.CheckState(false))
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

            Factory.IndexEngine.Update(data, exp, action, s => s.Tags);
        }

        private static void UpdateTagsCount(ITagMailDao daoTagMail, ITagDao daoTag, Tag tag)
        {
            var count = daoTagMail.CalculateTagCount(tag.Id);

            tag.Count = count;

            daoTag.SaveTag(tag);
        }

        private void GetValidForUserMessages(IDaoFactory daoFactory, List<int> messagesIds, out List<int> validIds,
            out List<ChainInfo> chains)
        {
            var daoMailInfo = daoFactory.CreateMailInfoDao(Tenant, User);

            var mailInfoList = daoMailInfo.GetMailInfoList(
                SimpleMessagesExp.CreateBuilder(Tenant, User)
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
        }*/
    }

    public static class TagEngineeExtension
    {
        public static IServiceCollection AddTagEngineService(this IServiceCollection services)
        {
            services.TryAddScoped<TagEngine>();

            return services;
        }
    }
}
