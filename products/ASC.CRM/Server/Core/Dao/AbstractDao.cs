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
using System.Linq.Expressions;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace ASC.CRM.Core.Dao
{
    public class AbstractDao
    {
        protected readonly List<EntityType> _supportedEntityType = new List<EntityType>();
        protected readonly ILog _log = LogManager.GetLogger("ASC.CRM");

        public CRMDbContext CRMDbContext { get; }
        public SecurityContext SecurityContext { get; }

        protected readonly ICache _cache;

        public AbstractDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext
            )
        {
            _cache = AscCache.Memory;
            CRMDbContext = dbContextManager.Get(CRMConstants.DatabaseId);
            TenantID = tenantManager.GetCurrentTenant().TenantId;
            SecurityContext = securityContext;

            _supportedEntityType.Add(EntityType.Company);
            _supportedEntityType.Add(EntityType.Person);
            _supportedEntityType.Add(EntityType.Contact);
            _supportedEntityType.Add(EntityType.Opportunity);
            _supportedEntityType.Add(EntityType.Case);

            /*
            _invoiceItemCacheKey = String.Concat(TenantID, "/invoiceitem");
            _invoiceTaxCacheKey = String.Concat(TenantID, "/invoicetax");
            _invoiceLineCacheKey = String.Concat(TenantID, "/invoiceline");
            
            if (_cache.Get(_invoiceItemCacheKey) == null)
            {
                _cache.Insert(_invoiceItemCacheKey, String.Empty);
            }
            if (_cache.Get(_invoiceTaxCacheKey) == null)
            {
                _cache.Insert(_invoiceTaxCacheKey, String.Empty);
            }
            if (_cache.Get(_invoiceLineCacheKey) == null)
            {
                _cache.Insert(_invoiceLineCacheKey, String.Empty);
            }
             */


        }

        /*
        protected readonly String _invoiceItemCacheKey;
        protected readonly String _invoiceTaxCacheKey;
        protected readonly String _invoiceLineCacheKey;
        */

        protected int TenantID { get; private set; }

        protected List<int> SearchByTags(EntityType entityType, int[] exceptIDs, IEnumerable<string> tags)
        {
            if (tags == null || !tags.Any())
                throw new ArgumentException();

            var tagIDs = new List<int>();

            foreach (var tag in tags) {
                tagIDs.Add(CRMDbContext
                    .Tags
                    .Where(x => x.EntityType == entityType && String.Compare(x.Title, tag.Trim(), true) == 0)
                    .Select(x => x.Id).Single());
            }
            
            var sqlQuery = CRMDbContext.EntityTags.Where(x => x.EntityType == entityType && tagIDs.Contains(x.TagId));

            if (exceptIDs != null && exceptIDs.Length > 0)
                sqlQuery = sqlQuery.Where(x => exceptIDs.Contains(x.EntityId));
                        
            return sqlQuery.GroupBy(x => x.EntityId)
                           .Where(x => x.Count() == tags.Count())
                           .Select(x => x.Key)
                           .ToList();
        }

        protected Dictionary<int, int[]> GetRelativeToEntity(int[] contactID, EntityType entityType, int[] entityID)
        {
            Expression<Func<DbEntityContact, bool>> exp = null;

            if (contactID != null && contactID.Length > 0 && (entityID == null || entityID.Length == 0))
                exp = x => x.EntityType == entityType && contactID.Contains(x.ContactId);
            else if (entityID != null && entityID.Length > 0 && (contactID == null || contactID.Length == 0))
                exp = x => x.EntityType == entityType && entityID.Contains(x.EntityId);
            
            return CRMDbContext.EntityContact.Where(exp).GroupBy(x => x.EntityId).ToDictionary(
                    x => x.Key,
                    x => x.Select(x => Convert.ToInt32(x.ContactId)).ToArray());         
        }

        protected int[] GetRelativeToEntity(int? contactID, EntityType entityType, int? entityID)
        {
            return GetRelativeToEntityInDb(contactID, entityType, entityID);
        }

        protected int[] GetRelativeToEntityInDb(int? contactID, EntityType entityType, int? entityID)
        {
            if (contactID.HasValue && !entityID.HasValue)
                return CRMDbContext.EntityContact
                       .Where(x => x.EntityType == entityType && x.ContactId == contactID.Value)
                       .Select(x => x.EntityId)
                       .ToArray();

            if (!contactID.HasValue && entityID.HasValue)
                return CRMDbContext.EntityContact
                       .Where(x => x.EntityType == entityType && x.EntityId == entityID.Value)
                       .Select(x => x.ContactId)
                       .ToArray();

            throw new ArgumentException();
        }

        protected void SetRelative(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID == 0)
                throw new ArgumentException();

            using var tx = CRMDbContext.Database.BeginTransaction();

            var exists = CRMDbContext.EntityContact
                                    .Where(x => x.EntityType == entityType && x.EntityId == entityID)
                                    .Select(x => x.ContactId)
                                    .ToArray();

            foreach (var existContact in exists)
            {
                var items = CRMDbContext.EntityContact
                                        .Where(x => x.EntityType == entityType && x.EntityId == entityID && x.ContactId == existContact);

                CRMDbContext.EntityContact.RemoveRange(items);
            }

            if (!(contactID == null || contactID.Length == 0))
                foreach (var id in contactID)
                    SetRelative(id, entityType, entityID);

            tx.Commit();
        }

        protected void SetRelative(int contactID, EntityType entityType, int entityID)
        {
            CRMDbContext.EntityContact.Add(new DbEntityContact
            {
                ContactId = contactID,
                EntityType = entityType,
                EntityId = entityID
            });

            CRMDbContext.SaveChanges();
        }

        protected void RemoveRelativeInDb(int[] contactID, EntityType entityType, int[] entityID)
        {
            if ((contactID == null || contactID.Length == 0) && (entityID == null || entityID.Length == 0))
                throw new ArgumentException();

            Expression<Func<DbEntityContact, bool>> expr = null;

            if (contactID != null && contactID.Length > 0)
                expr = x => contactID.Contains(x.ContactId);

            if (entityID != null && entityID.Length > 0)
                expr = x => entityID.Contains(x.EntityId) && x.EntityType == entityType;

            var dbCrmEntity = CRMDbContext.EntityContact;

            dbCrmEntity.RemoveRange(dbCrmEntity.Where(expr));

            CRMDbContext.SaveChanges();
        }

        protected void RemoveRelative(int contactID, EntityType entityType, int entityID)
        {
            int[] contactIDs = null;
            int[] entityIDs = null;


            if (contactID > 0)
                contactIDs = new[] { contactID };

            if (entityID > 0)
                entityIDs = new[] { entityID };


            RemoveRelativeInDb(contactIDs, entityType, entityIDs);
        }


        public int SaveOrganisationLogo(byte[] bytes)
        {            
            var entity = new DbOrganisationLogo
            {
                Content = Convert.ToBase64String(bytes),
                CreateOn = DateTime.UtcNow,
                CreateBy = SecurityContext.CurrentAccount.ID.ToString()
            };

            CRMDbContext.OrganisationLogo.Add(entity);

            CRMDbContext.SaveChanges();

            return entity.Id;
        }

        public string GetOrganisationLogoBase64(int logo_id)
        {           
            if (logo_id <= 0) throw new ArgumentException();
            
            return Query(CRMDbContext.OrganisationLogo)
                                .Where(x => x.Id == logo_id)
                                .Select(x => x.Content)
                                .FirstOrDefault();
        }

        public bool HasActivity()
        {
            return Query(CRMDbContext.Cases).Any() &&
                   Query(CRMDbContext.Deals).Any() &&
                   Query(CRMDbContext.Tasks).Any() &&
                   Query(CRMDbContext.Contacts).Any();
        }

        protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbCrm
        {
            return set.Where(r => r.TenantId == TenantID);
        }
        
        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";
            
            if (!table.Contains(" ")) return tenant;
        
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;        
        }

        protected static Guid ToGuid(object guid)
        {
            var str = guid as string;
            
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;        
        }
    }
}
