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
using ASC.Core.Common.EF.Context;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Enums;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.CRM.Core.Dao
{
    public class AbstractDao
    {
        protected readonly List<EntityType> _supportedEntityType = new List<EntityType>();

        private Lazy<CrmDbContext> LazyCrmDbContext { get; }
        public CrmDbContext CrmDbContext { get => LazyCrmDbContext.Value; }
        protected readonly SecurityContext _securityContext;
        protected readonly ICache _cache;
        protected ILog _logger;
        protected IMapper _mapper;

        public AbstractDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger,
            ICache ascCache,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _logger = logger.Get("ASC.CRM");

            _cache = ascCache;

            LazyCrmDbContext = new Lazy<CrmDbContext>(() => dbContextManager.Get(CrmConstants.DatabaseId));
            TenantID = tenantManager.GetCurrentTenant().TenantId;
            _securityContext = securityContext;

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

            foreach (var tag in tags)
            {
                tagIDs.Add(CrmDbContext
                    .Tags
                    .AsQueryable()
                    .Where(x => x.EntityType == entityType && String.Compare(x.Title, tag.Trim(), true) == 0)
                    .Select(x => x.Id).Single());
            }

            var sqlQuery = CrmDbContext.EntityTags.AsQueryable().Where(x => x.EntityType == entityType && tagIDs.Contains(x.TagId));

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

            return CrmDbContext.EntityContact
                .Where(exp)
                .Select(x => new { EntityId = x.EntityId, ContactId = x.ContactId })
                .ToList()
                .GroupBy(x => x.EntityId)
                .ToDictionary(x => x.Key, y => y.Select(c => c.ContactId).ToArray());
        }

        protected int[] GetRelativeToEntity(int? contactID, EntityType entityType, int? entityID)
        {
            return GetRelativeToEntityInDb(contactID, entityType, entityID);
        }

        protected int[] GetRelativeToEntityInDb(int? contactID, EntityType entityType, int? entityID)
        {
            if (contactID.HasValue && !entityID.HasValue)
                return CrmDbContext.EntityContact
                        .AsQueryable()
                       .Where(x => x.EntityType == entityType && x.ContactId == contactID.Value)
                       .Select(x => x.EntityId)
                       .ToArray();

            if (!contactID.HasValue && entityID.HasValue)
                return CrmDbContext.EntityContact
                       .AsQueryable()
                       .Where(x => x.EntityType == entityType && x.EntityId == entityID.Value)
                       .Select(x => x.ContactId)
                       .ToArray();

            throw new ArgumentException();
        }

        protected void SetRelative(int[] contactID, EntityType entityType, int entityID)
        {
            if (entityID == 0)
                throw new ArgumentException();

            using var tx = CrmDbContext.Database.BeginTransaction();

            var exists = CrmDbContext.EntityContact
                                    .AsQueryable()
                                    .Where(x => x.EntityType == entityType && x.EntityId == entityID)
                                    .Select(x => x.ContactId)
                                    .ToArray();

            foreach (var existContact in exists)
            {
                var items = CrmDbContext.EntityContact
                                        .AsQueryable()
                                        .Where(x => x.EntityType == entityType && x.EntityId == entityID && x.ContactId == existContact);

                CrmDbContext.EntityContact.RemoveRange(items);
            }

            if (!(contactID == null || contactID.Length == 0))
                foreach (var id in contactID)
                    SetRelative(id, entityType, entityID);

            tx.Commit();
        }

        protected void SetRelative(int contactID, EntityType entityType, int entityID)
        {
            var dbEntity = new DbEntityContact
            {
                ContactId = contactID,
                EntityType = entityType,
                EntityId = entityID
            };

            CrmDbContext.EntityContact.Add(dbEntity);

            CrmDbContext.SaveChanges();
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

            var dbCrmEntity = CrmDbContext.EntityContact;

            dbCrmEntity.RemoveRange(dbCrmEntity.Where(expr));

            CrmDbContext.SaveChanges();
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
            var dbEntity = new DbOrganisationLogo
            {
                Content = Convert.ToBase64String(bytes),
                CreateOn = DateTime.UtcNow,                
                CreateBy = _securityContext.CurrentAccount.ID.ToString(),
                TenantId = TenantID
            };

            CrmDbContext.OrganisationLogo.Add(dbEntity);

            CrmDbContext.SaveChanges();

            return dbEntity.Id;
        }

        public string GetOrganisationLogoBase64(int logo_id)
        {
            if (logo_id <= 0) throw new ArgumentException();

            return Query(CrmDbContext.OrganisationLogo)
                                .Where(x => x.Id == logo_id)
                                .Select(x => x.Content)
                                .FirstOrDefault();
        }

        public bool HasActivity()
        {
            return Query(CrmDbContext.Cases).Any() &&
                   Query(CrmDbContext.Deals).Any() &&
                   Query(CrmDbContext.Tasks).Any() &&
                   Query(CrmDbContext.Contacts).Any();
        }

        protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbCrm
        {
            return set.AsQueryable().Where(r => r.TenantId == TenantID);
        }

        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";

            if (!table.Contains(' ')) return tenant;

            return table.Substring(table.IndexOf(' ')).Trim() + "." + tenant;
        }

        protected static Guid ToGuid(object guid)
        {
            var str = guid as string;

            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }
    }
}
