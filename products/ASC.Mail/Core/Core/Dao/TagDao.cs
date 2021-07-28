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


using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.Enums;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;

using CrmTag = ASC.Mail.Core.Entities.CrmTag;

namespace ASC.Mail.Core.Dao
{
    [Scope]
    public class TagDao : BaseMailDao, ITagDao
    {
        public TagDao(
             TenantManager tenantManager,
             SecurityContext securityContext,
             DbContextManager<MailDbContext> dbContext)
            : base(tenantManager, securityContext, dbContext)
        {
        }

        public Tag GetTag(int id)
        {
            if (id < 0)
                return GetCrmTag(id);

            var tag = MailDbContext.MailTag
                .Where(r => r.TenantId == Tenant && r.IdUser == UserId && r.Id == id)
                .Select(r => new Tag
                {
                    Id = r.Id,
                    Tenant = r.TenantId,
                    User = r.IdUser,
                    TagName = r.Name,
                    Style = r.Style,
                    Addresses = r.Addresses,
                    Count = r.Count,
                    CrmId = r.CrmId

                }).SingleOrDefault();

            return tag;
        }

        public Tag GetCrmTag(int id)
        {
            var crmTagId = id < 0 ? -id : id;

            var crmTag = MailDbContext.CrmTag
                .Where(r => r.IdTenant == Tenant && r.EntityType == (int)EntityType.Contact && r.Id == crmTagId)
                .Select(r => new Tag
                {
                    Id = -r.Id,
                    TagName = r.Title,
                    Tenant = Tenant,
                    User = UserId,
                    Style = "",
                    Addresses = "",
                    Count = 0,
                    CrmId = 0
                }).SingleOrDefault();

            return crmTag;
        }

        public Tag GetTag(string name)
        {
            var tag = MailDbContext.MailTag
                .Where(r => r.TenantId == Tenant && r.IdUser == UserId && r.Name == name)
                .Select(r => new Tag
                {
                    Id = r.Id,
                    Tenant = r.TenantId,
                    User = r.IdUser,
                    TagName = r.Name,
                    Style = r.Style,
                    Addresses = r.Addresses,
                    Count = r.Count,
                    CrmId = r.CrmId

                }).SingleOrDefault();

            return tag;
        }

        public List<Tag> GetTags()
        {
            var tags = MailDbContext.MailTag
                .Where(r => r.TenantId == Tenant && r.IdUser == UserId)
                .Select(r => new Tag
                {
                    Id = r.Id,
                    TagName = r.Name,
                    Tenant = r.TenantId,
                    User = r.IdUser,
                    Style = r.Style,
                    Addresses = r.Addresses,
                    Count = r.Count,
                    CrmId = r.CrmId
                }).ToList();

            return tags;
        }

        public List<Tag> GetCrmTags()
        {
            var crmTags = MailDbContext.CrmTag
                .Where(r => r.IdTenant == Tenant && r.EntityType == (int)EntityType.Contact)
                .Select(r => new Tag
                {
                    Id = -r.Id,
                    TagName = r.Title,
                    Tenant = Tenant,
                    User = UserId,
                    Style = "",
                    Addresses = "",
                    Count = 0,
                    CrmId = 0
                }).ToList();

            return crmTags;
        }

        public List<CrmTag> GetCrmTags(List<int> contactIds)
        {
            var query = MailDbContext.CrmEntityTag
                .Join(MailDbContext.CrmTag,
                    cet => cet.TagId,
                    ct => ct.Id,
                    (cet, ct) => new CrmTag
                    {
                        TagId = -ct.Id,
                        TagTitle = ct.Title,
                        TenantId = ct.IdTenant,
                        EntityId = cet.EntityId,
                        EntityType = cet.EntityType
                    })
                .Where(r =>
                r.EntityType == (int)EntityType.Contact
                && contactIds.Contains(r.EntityId)
                && r.TenantId == Tenant)
                .ToList();

            return query;
        }
        public int SaveTag(Tag tag)
        {
            var dbTag = new MailTag()
            {
                Id = tag.Id,
                TenantId = Tenant,
                IdUser = UserId,
                Name = tag.TagName,
                Style = tag.Style,
                Addresses = tag.Addresses,
                Count = tag.Count,
                CrmId = tag.CrmId
            };

            var entry = MailDbContext.AddOrUpdate(t => t.MailTag, dbTag);

            MailDbContext.SaveChanges();

            return entry.Id;
        }

        public int DeleteTag(int id)
        {
            var range = MailDbContext.MailTag
                .Where(r => r.TenantId == Tenant && r.IdUser == UserId && r.Id == id);

            MailDbContext.MailTag.RemoveRange(range);

            return MailDbContext.SaveChanges();
        }

        public int DeleteTags(List<int> tagIds)
        {
            var range = MailDbContext.MailTag
                .Where(r => r.TenantId == Tenant && r.IdUser == UserId && tagIds.Contains(r.Id));

            MailDbContext.MailTag.RemoveRange(range);

            return MailDbContext.SaveChanges();
        }
    }
}