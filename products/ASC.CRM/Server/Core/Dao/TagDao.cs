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
using System.Text.Json;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class TagDao : AbstractDao
    {
        public TagDao(DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            IMapper mapper) :
                        base(dbContextManager,
                                tenantManager,
                                securityContext,
                                logger,
                                ascCache,
                                mapper)
        {

        }

        public bool IsExist(EntityType entityType, String tagName)
        {
            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentNullException(tagName);

            return IsExistInDb(entityType, tagName);
        }

        private bool IsExistInDb(EntityType entityType, String tagName)
        {
            return Query(CrmDbContext.Tags)
                    .Where(x => x.EntityType == entityType && String.Compare(x.Title, tagName, true) == 0)
                    .Any();
        }

        private int GetTagId(EntityType entityType, String tagName)
        {
            return Query(CrmDbContext.Tags)
                    .Where(x => x.EntityType == entityType && String.Compare(x.Title, tagName, true) == 0)
                    .Select(x => x.Id)
                    .SingleOrDefault();
        }

        public String[] GetAllTags(EntityType entityType)
        {
            return CrmDbContext.Tags
                .AsQueryable()
                .Where(x => x.EntityType == entityType)
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToArray();
        }

        public List<KeyValuePair<EntityType, string>> GetAllTags()
        {
            return Query(CrmDbContext.Tags)
                .OrderBy(x => x.Title)
                .Select(x => new KeyValuePair<EntityType, string>(x.EntityType, x.Title)).ToList();
        }

        public String GetTagsLinkCountJSON(EntityType entityType)
        {
            int[] tags = GetTagsLinkCount(entityType).ToArray();

            return JsonSerializer.Serialize(tags);
        }

        public IEnumerable<int> GetTagsLinkCount(EntityType entityType)
        {
            return Query(CrmDbContext.Tags)
                       .Join(CrmDbContext.EntityTags,
                                   x => x.Id,
                                   y => y.TagId,
                                   (x, y) => new { x, y })
                       .Where(x => x.x.EntityType == entityType)
                       .GroupBy(x => x.x.Id)
                       .Select(x => x.Count());
        }


        public Dictionary<int, List<String>> GetEntitiesTags(EntityType entityType)
        {
            return CrmDbContext.EntityTags.Join(Query(CrmDbContext.Tags),
                              x => x.TagId,
                              y => y.Id,
                              (x, y) => new { x, y })
                         .Where(x => x.x.EntityType == entityType)
                         .Select(x => new { EntityId = x.x.EntityId, Title = x.y.Title })
                         .ToList()
                         .GroupBy(x => x.EntityId)
                         .ToDictionary(x => x.Key, x => x.ToList().ConvertAll(t => t.Title));
        }

        public String[] GetEntityTags(EntityType entityType, int entityID)
        {
            return Query(CrmDbContext.Tags)
                    .Join(CrmDbContext.EntityTags,
                            x => x.Id,
                            y => y.TagId,
                            (x, y) => new
                            {
                                x,
                                y

                            })
                    .Where(x => x.y.EntityId == entityID && x.y.EntityType == entityType)
                    .Select(x => x.x.Title)
                    .ToArray();
        }

        public string[] GetUnusedTags(EntityType entityType)
        {
            return Query(CrmDbContext.Tags)
                    .Join(CrmDbContext.EntityTags.AsQueryable().DefaultIfEmpty(),
                               x => x.Id,
                               y => y.TagId,
                               (x, y) => new { x, y })
                    .Where(x => x.y == null && x.x.EntityType == entityType)
                    .Select(x => x.x.Title)
                    .ToArray();
        }

        public bool CanDeleteTag(EntityType entityType, String tagName)
        {
            return Query(CrmDbContext.Tags)
                 .Where(x => string.Compare(x.Title, tagName, true) == 0 && x.EntityType == entityType)
                 .Select(x => x.Id)
                 .SingleOrDefault() != 0;
        }

        public void DeleteTag(EntityType entityType, String tagName)
        {
            if (!CanDeleteTag(entityType, tagName)) throw new ArgumentException();

            DeleteTagFromEntity(entityType, 0, tagName);
        }

        public void DeleteTagFromEntity(EntityType entityType, int entityID, String tagName)
        {
            var tagID = Query(CrmDbContext.Tags)
                        .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == entityType)
                        .Select(x => x.Id)
                        .SingleOrDefault();

            if (tagID == 0) return;

            var itemsToRemove = CrmDbContext.EntityTags.AsQueryable().Where(x => x.EntityType == entityType && x.TagId == tagID);

            if (entityID > 0)
                itemsToRemove = itemsToRemove.Where(x => x.EntityId == entityID);

            CrmDbContext.RemoveRange(itemsToRemove);

            if (entityID == 0)
                CrmDbContext.Tags.RemoveRange(Query(CrmDbContext.Tags).Where(x => x.Id == tagID && x.EntityType == entityType));

            CrmDbContext.SaveChanges();
        }

        public void DeleteAllTagsFromEntity(EntityType entityType, int entityID)
        {
            if (entityID <= 0) return;

            var itemsToRemove = CrmDbContext.EntityTags.AsQueryable().Where(x => x.EntityType == entityType && x.EntityId == entityID);

            CrmDbContext.EntityTags.RemoveRange(itemsToRemove);

            CrmDbContext.SaveChanges();
        }

        public void DeleteUnusedTags(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var itemToDelete = Query(CrmDbContext.Tags)
                                    .Join(CrmDbContext.EntityTags.AsQueryable().DefaultIfEmpty(),
                                        x => x.Id,
                                        y => y.TagId,
                                        (x, y) => new { x, y })
                                    .Where(x => x.x.EntityType == entityType && x.y == null).Select(x => x.x).ToList();

            CrmDbContext.RemoveRange(itemToDelete);

            CrmDbContext.SaveChanges();
        }

        public int AddTag(EntityType entityType, String tagName, bool returnExisted = false)
        {
            tagName = CorrectTag(tagName);

            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentNullException(CRMErrorsResource.TagNameNotSet);

            var existedTagId = GetTagId(entityType, tagName);

            if (existedTagId > 0)
            {
                if (returnExisted)
                    return existedTagId;

                throw new ArgumentException(CRMErrorsResource.TagNameBusy);
            }
            return AddTagInDb(entityType, tagName);
        }

        private int AddTagInDb(EntityType entityType, String tagName)
        {
            var dbTag = new DbTag
            {
                Title = tagName,
                EntityType = entityType,
                TenantId = TenantID
            };

            CrmDbContext.Tags.Add(dbTag);

            CrmDbContext.SaveChanges();

            return dbTag.Id;
        }


        public Dictionary<string, int> GetAndAddTags(EntityType entityType, String[] tags)
        {
            tags = tags.Select(CorrectTag).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

            using var tx = CrmDbContext.Database.BeginTransaction();

            var tagNamesAndIds = Query(CrmDbContext.Tags)
                .Where(x => tags.Contains(x.Title) && x.EntityType == entityType)
                .Select(x => new { x.Id, x.Title })
                .ToDictionary(x => x.Title, x => x.Id);

            var tagsForCreate = tags.Where(t => !tagNamesAndIds.ContainsKey(t));

            foreach (var tagName in tagsForCreate)
            {
                tagNamesAndIds.Add(tagName, AddTagInDb(entityType, tagName));
            }

            tx.Commit();

            return tagNamesAndIds;
        }

        private int AddTagToEntityInDb(EntityType entityType, int entityID, String tagName)
        {
            tagName = CorrectTag(tagName);

            if (String.IsNullOrEmpty(tagName) || entityID == 0)
                throw new ArgumentException();

            var tagID = Query(CrmDbContext.Tags)
            .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == entityType)
            .Select(x => x.Id).SingleOrDefault();

            if (tagID == 0)
                tagID = AddTagInDb(entityType, tagName);

            CrmDbContext.EntityTags.Add(new DbEntityTag
            {
                EntityId = entityID,
                EntityType = entityType,
                TagId = tagID
            });

            CrmDbContext.SaveChanges();

            return tagID;
        }

        public int AddTagToEntity(EntityType entityType, int entityID, String tagName)
        {
            return AddTagToEntityInDb(entityType, entityID, tagName);
        }

        public int AddTagToContacts(int[] contactID, String tagName)
        {
            tagName = CorrectTag(tagName);

            if (String.IsNullOrEmpty(tagName) || contactID == null || contactID.Length == 0)
                throw new ArgumentException();

            using var tx = CrmDbContext.Database.BeginTransaction();

            var tagID = Query(CrmDbContext.Tags)
                        .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == EntityType.Contact)
                        .Select(x => x.Id).SingleOrDefault();

            if (tagID == 0)
                tagID = AddTagInDb(EntityType.Contact, tagName);

            foreach (var id in contactID)
            {
                CrmDbContext.EntityTags.Add(new DbEntityTag
                {
                    EntityId = id,
                    EntityType = EntityType.Contact,
                    TagId = tagID
                });
            }

            CrmDbContext.SaveChanges();

            tx.Commit();

            return tagID;
        }

        public int DeleteTagFromContacts(int[] contactID, String tagName)
        {
            if (String.IsNullOrEmpty(tagName) || contactID == null || contactID.Length == 0)
                throw new ArgumentException();

            using var tx = CrmDbContext.Database.BeginTransaction();

            var tagID = Query(CrmDbContext.Tags)
                        .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == EntityType.Contact)
                        .Select(x => x.Id)
                        .SingleOrDefault();

            if (tagID == 0)
                throw new ArgumentException();

            var itemsToRemove = CrmDbContext
                                        .EntityTags
                                        .AsQueryable()
                                        .Where(x => x.EntityType == EntityType.Contact &&
                                                    x.TagId == tagID &&
                                                    contactID.Contains(x.EntityId));

            CrmDbContext.EntityTags.RemoveRange(itemsToRemove);

            CrmDbContext.SaveChanges();

            tx.Commit();

            return tagID;
        }

        public void SetTagToEntity(EntityType entityType, int entityID, String[] tags)
        {
            using var tx = CrmDbContext.Database.BeginTransaction();

            var itemsToDelete = CrmDbContext.EntityTags.AsQueryable().Where(x => x.EntityId == entityID && x.EntityType == entityType);

            CrmDbContext.EntityTags.RemoveRange(itemsToDelete);

            foreach (var tagName in tags.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                AddTagToEntityInDb(entityType, entityID, tagName);
            }

            CrmDbContext.SaveChanges();

            tx.Commit();
        }

        private void AddTagToEntityInDb(EntityType entityType, int entityID, int tagID)
        {
            CrmDbContext.EntityTags.Add(new DbEntityTag
            {
                EntityId = entityID,
                EntityType = entityType,
                TagId = tagID
            });

            CrmDbContext.SaveChanges();
        }

        public void AddTagToEntity(EntityType entityType, int entityID, int[] tagIDs)
        {
            using var tx = CrmDbContext.Database.BeginTransaction();

            foreach (var tagID in tagIDs)
            {
                AddTagToEntityInDb(entityType, entityID, tagID);
            }

            CrmDbContext.SaveChanges();

            tx.Commit();
        }

        private string CorrectTag(string tag)
        {
            return tag == null
                       ? null
                       : tag.Trim()
                            .Replace("\r\n", string.Empty)
                            .Replace("\n", string.Empty)
                            .Replace("\r", string.Empty);
        }

    }

}