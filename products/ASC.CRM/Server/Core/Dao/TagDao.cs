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


using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.CRM.Core.Dao
{
    public class TagDao : AbstractDao
    {
        public TagDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger) :
                                            base(dbContextManager,
                                                 tenantManager,
                                                 securityContext,
                                                 logger)
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
            return Query(CRMDbContext.Tags)
                    .Where(x => x.EntityType == entityType && String.Compare(x.Title, tagName, true) == 0).Any();
        }

        private int GetTagId(EntityType entityType, String tagName)
        {
            return Query(CRMDbContext.Tags)
                    .Where(x => x.EntityType == entityType && String.Compare(x.Title, tagName, true) == 0)
                    .Select(x => x.Id)
                    .SingleOrDefault();
        }

        public String[] GetAllTags(EntityType entityType)
        {
            return CRMDbContext.Tags
                .Where(x => x.EntityType == entityType)
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToArray();
        }

        public List<KeyValuePair<EntityType, string>> GetAllTags()
        {
            return Query(CRMDbContext.Tags)
                .OrderBy(x => x.Title)
                .Select(x => new KeyValuePair<EntityType, string> (x.EntityType, x.Title)).ToList();            
        }

        public String GetTagsLinkCountJSON(EntityType entityType)
        {
            int[] tags = GetTagsLinkCount(entityType).ToArray();
            return JsonConvert.SerializeObject(tags);
        }

        public IEnumerable<int> GetTagsLinkCount(EntityType entityType)
        {
            return Query(CRMDbContext.Tags)
                       .GroupJoin(CRMDbContext.EntityTags,
                                   x => x.Id,
                                   y => y.TagId,
                                   (x, y) => new { x = x, count = y.Count() })
                       .Where(x => x.x.EntityType == entityType)
                       .OrderBy(x => x.x.Title)
                       .Select(x => x.count).ToList();
        }


        public Dictionary<int, List<String>> GetEntitiesTags(EntityType entityType)
        {
           return CRMDbContext.EntityTags.Join(Query(CRMDbContext.Tags),
                             x => x.TagId,
                             y => y.Id,
                             (x, y) => new { x, y })
                        .Where(x => x.x.EntityType == entityType)
                        .GroupBy(x => x.x.EntityId)
                        .ToDictionary(x => x.Key, x => x.ToList().ConvertAll(x => x.y.Title));         
        }

        public String[] GetEntityTags(EntityType entityType, int entityID)
        {
            return Query(CRMDbContext.Tags)
                    .Join(CRMDbContext.EntityTags,
                            x => x.Id,
                            y => y.TagId,
                            (x, y) => new
                            {
                                x, y

                            })
                    .Where(x => x.y.EntityId == entityID && x.y.EntityType == entityType)
                    .Select(x => x.x.Title).ToArray();
        }
                
        public string[] GetUnusedTags(EntityType entityType)
        {
            return Query(CRMDbContext.Tags)
                    .GroupJoin(CRMDbContext.EntityTags.DefaultIfEmpty(),
                               x => x.Id,
                               y => y.TagId,
                               (x, y) => new { x, y })
                    .Where(x => x.y == null && x.x.EntityType == entityType)
                    .Select(x => x.x.Title).ToArray();
        }

        public bool CanDeleteTag(EntityType entityType, String tagName)
        {
            return Query(CRMDbContext.Tags)
                 .Where(x => string.Compare(x.Title, tagName, true) == 0 && x.EntityType == entityType)
                 .Select(x => x.Id).SingleOrDefault() != 0;
        }

        public void DeleteTag(EntityType entityType, String tagName)
        {
            if (!CanDeleteTag(entityType, tagName)) throw new ArgumentException();

            DeleteTagFromEntity(entityType, 0, tagName);
        }

        public void DeleteTagFromEntity(EntityType entityType, int entityID, String tagName)
        {
            var tagID = Query(CRMDbContext.Tags)
                        .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == entityType)
                        .Select(x => x.Id).SingleOrDefault();
            
            if (tagID == 0) return;

            var itemsToRemove = CRMDbContext.EntityTags.Where(x => x.EntityType == entityType && x.TagId == tagID);

            if (entityID > 0)
                itemsToRemove = itemsToRemove.Where(x => x.EntityId == entityID);

            CRMDbContext.RemoveRange(itemsToRemove);

            if (entityID == 0)
                CRMDbContext.Tags.RemoveRange(Query(CRMDbContext.Tags).Where(x => x.Id == tagID && x.EntityType == entityType));

            CRMDbContext.SaveChanges();
        }

        public void DeleteAllTagsFromEntity(EntityType entityType, int entityID)
        {
            if (entityID <= 0) return;

            var itemsToRemove = CRMDbContext.EntityTags.Where(x => x.EntityType == entityType && x.EntityId == entityID);
            
            CRMDbContext.EntityTags.RemoveRange(itemsToRemove);

            CRMDbContext.SaveChanges();
        }

        public void DeleteUnusedTags(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();
              
            var itemToDelete = Query(CRMDbContext.Tags).GroupJoin(CRMDbContext.EntityTags,
                                                                      x => x.Id,
                                                                      y => y.TagId,
                                                                      (x, y) => new { x, y }
                                                                ).Where(x => x.x.EntityType == entityType && x.y == null).Select(x => x.x).ToList();
                        
            CRMDbContext.RemoveRange(itemToDelete);
                        
            CRMDbContext.SaveChanges();
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
                EntityType = entityType
            };

            CRMDbContext.Tags.Add(dbTag);

            CRMDbContext.SaveChanges();

            return dbTag.Id;
        }


        public Dictionary<string, int> GetAndAddTags(EntityType entityType, String[] tags)
        {
            tags = tags.Select(CorrectTag).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
                      
            using var tx = CRMDbContext.Database.BeginTransaction();

            var tagNamesAndIds =  Query(CRMDbContext.Tags)
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

            var tagID = Query(CRMDbContext.Tags)
            .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == entityType)
            .Select(x => x.Id).SingleOrDefault();

            if (tagID == 0)
                tagID = AddTagInDb(entityType, tagName);

            CRMDbContext.EntityTags.Add(new DbEntityTag
            {
                EntityId = entityID,
                EntityType = entityType,
                TagId = tagID
            });

            CRMDbContext.SaveChanges();

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

            using var tx = CRMDbContext.Database.BeginTransaction();

            var tagID = Query(CRMDbContext.Tags)
                        .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == EntityType.Contact)
                        .Select(x => x.Id).SingleOrDefault();

            if (tagID == 0)
                tagID = AddTagInDb(EntityType.Contact, tagName);

            foreach (var id in contactID)
            {
                CRMDbContext.EntityTags.Add(new DbEntityTag
                {
                    EntityId = id,
                    EntityType = EntityType.Contact,
                    TagId = tagID
                });
            }

            CRMDbContext.SaveChanges();

            tx.Commit();

            return tagID;
        }

        public int DeleteTagFromContacts(int[] contactID, String tagName)
        {
            if (String.IsNullOrEmpty(tagName) || contactID == null || contactID.Length == 0)
                throw new ArgumentException();

            using var tx = CRMDbContext.Database.BeginTransaction();

            var tagID = Query(CRMDbContext.Tags)
                        .Where(x => String.Compare(x.Title, tagName, true) == 0 && x.EntityType == EntityType.Contact)
                        .Select(x => x.Id)
                        .SingleOrDefault();

            if (tagID == 0)
                throw new ArgumentException();

            var itemsToRemove = CRMDbContext
                                        .EntityTags
                                        .Where(x => x.EntityType == EntityType.Contact && 
                                                    x.TagId == tagID && 
                                                    contactID.Contains(x.EntityId));
            
            CRMDbContext.EntityTags.RemoveRange(itemsToRemove);
            
            CRMDbContext.SaveChanges();

            tx.Commit();

            return tagID;
        }

        public void SetTagToEntity(EntityType entityType, int entityID, String[] tags)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();
            
            var itemsToDelete = CRMDbContext.EntityTags.Where(x => x.EntityId == entityID && x.EntityType == entityType);

            CRMDbContext.EntityTags.RemoveRange(itemsToDelete);

            foreach (var tagName in tags.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                AddTagToEntityInDb(entityType, entityID, tagName);
            }

            CRMDbContext.SaveChanges();

            tx.Commit();
        }

        private void AddTagToEntityInDb(EntityType entityType, int entityID, int tagID)
        {
            CRMDbContext.EntityTags.Add(new DbEntityTag
            {
                EntityId = entityID,
                EntityType = entityType,
                TagId = tagID
            });

            CRMDbContext.SaveChanges();
        }

        public void AddTagToEntity(EntityType entityType, int entityID, int[] tagIDs)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();

            foreach (var tagID in tagIDs)
            {
                AddTagToEntityInDb(entityType, entityID, tagID);
            }

            CRMDbContext.SaveChanges();

            tx.Commit();
        }

        private static string CorrectTag(string tag)
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