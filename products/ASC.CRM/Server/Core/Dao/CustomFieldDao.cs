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
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.ElasticSearch;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Core.Search;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class CustomFieldDao : AbstractDao
    {
        private readonly TenantUtil _tenantUtil;
        private readonly FactoryIndexerFieldValue _factoryIndexer;

        public CustomFieldDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            TenantUtil tenantUtil,
            FactoryIndexerFieldValue factoryIndexer,
            ILogger logger,
            ICache ascCache,
            IMapper mapper
            ) :
              base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _tenantUtil = tenantUtil;
            _factoryIndexer = factoryIndexer;
        }


        public void SaveList(List<CustomField> items)
        {
            if (items == null || items.Count == 0) return;

            var tx = CrmDbContext.Database.BeginTransaction();

            foreach (var customField in items)
            {
                SetFieldValueInDb(customField.EntityType, customField.EntityID, customField.ID, customField.Value);
            }

            tx.Commit();
        }

        public void SetFieldValue(EntityType entityType, int entityID, int fieldID, String fieldValue)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            fieldValue = fieldValue.Trim();

            SetFieldValueInDb(entityType, entityID, fieldID, fieldValue);
        }

        private void SetFieldValueInDb(EntityType entityType, int entityID, int fieldID, string fieldValue)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            fieldValue = fieldValue.Trim();

            var dbEntity = Query(CrmDbContext.FieldValue)
                                    .FirstOrDefault(x => x.EntityId == entityID &&
                                                x.EntityType == entityType &&
                                                x.FieldId == fieldID);

            if (string.IsNullOrEmpty(fieldValue) && dbEntity != null)
            {
                _factoryIndexer.Delete(dbEntity);

                CrmDbContext.Remove(dbEntity);
            }
            else
            {
                if (dbEntity == null)
                {
                    dbEntity = new DbFieldValue
                    {
                        EntityId = entityID,
                        FieldId = fieldID,
                        EntityType = entityType,
                        TenantId = TenantID
                    };

                    CrmDbContext.Add(dbEntity);
                }


                dbEntity.Value = fieldValue;
                dbEntity.LastModifedOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());
                dbEntity.LastModifedBy = _securityContext.CurrentAccount.ID;

                CrmDbContext.SaveChanges();

                _factoryIndexer.Index(dbEntity);
            }

        }

        private string GetValidMask(CustomFieldType customFieldType, String mask)
        {
            var resultMask = new Dictionary<String, Object>();

            if (customFieldType == CustomFieldType.CheckBox || customFieldType == CustomFieldType.Heading || customFieldType == CustomFieldType.Date)
                return String.Empty;

            if (String.IsNullOrEmpty(mask))
                throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid);

            try
            {
                var maskObj = JsonDocument.Parse(mask).RootElement;
                if (customFieldType == CustomFieldType.TextField)
                {
                    var size = maskObj.GetProperty("size").GetInt32();
                    if (size == 0)
                    {
                        resultMask.Add("size", 1);
                    }
                    else if (size > Global.MaxCustomFieldSize)
                    {
                        resultMask.Add("size", Global.MaxCustomFieldSize);
                    }
                    else
                    {
                        resultMask.Add("size", size);
                    }
                }
                if (customFieldType == CustomFieldType.TextArea)
                {
                    var rows = maskObj.GetProperty("rows").GetInt32();
                    var cols = maskObj.GetProperty("cols").GetInt32();

                    if (rows == 0)
                    {
                        resultMask.Add("rows", 1);
                    }
                    else if (rows > Global.MaxCustomFieldRows)
                    {
                        resultMask.Add("rows", Global.MaxCustomFieldRows);
                    }
                    else
                    {
                        resultMask.Add("rows", rows);
                    }

                    if (cols == 0)
                    {
                        resultMask.Add("cols", 1);
                    }
                    else if (cols > Global.MaxCustomFieldCols)
                    {
                        resultMask.Add("cols", Global.MaxCustomFieldCols);
                    }
                    else
                    {
                        resultMask.Add("cols", cols);
                    }
                }
                if (customFieldType == CustomFieldType.SelectBox)
                {
                    try
                    {
                        var arrayLength = maskObj.GetArrayLength();

                        return mask;

                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                if (customFieldType == CustomFieldType.TextField)
                {
                    resultMask.Add("size", Global.DefaultCustomFieldSize);
                }
                if (customFieldType == CustomFieldType.TextArea)
                {
                    resultMask.Add("rows", Global.DefaultCustomFieldRows);
                    resultMask.Add("cols", Global.DefaultCustomFieldCols);
                }
                if (customFieldType == CustomFieldType.SelectBox)
                {
                    _logger.LogError(ex.ToString());

                    throw;
                }
            }
            return JsonSerializer.Serialize(resultMask);
        }

        public int CreateField(EntityType entityType, String label, CustomFieldType customFieldType, String mask)
        {
            if (!_supportedEntityType.Contains(entityType) || String.IsNullOrEmpty(label))
                throw new ArgumentException();

            var resultMask = GetValidMask(customFieldType, mask);

            var sortOrder = Query(CrmDbContext.FieldDescription).Select(x => x.SortOrder).Max() + 1;

            var dbEntity = new DbFieldDescription
            {
                Label = label,
                Type = customFieldType,
                Mask = resultMask,
                SortOrder = sortOrder,
                EntityType = entityType,
                TenantId = TenantID
            };

            CrmDbContext.FieldDescription.Add(dbEntity);

            CrmDbContext.SaveChanges();

            return dbEntity.Id;
        }

        public String GetValue(EntityType entityType, int entityID, int fieldID)
        {
            var sqlQuery = Query(CrmDbContext.FieldValue).Where(x => x.FieldId == fieldID && x.EntityId == entityID);

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType || x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType);
            }

            return sqlQuery.Select(x => x.Value).FirstOrDefault();
        }

        public List<Int32> GetEntityIds(EntityType entityType, int fieldID, String fieldValue)
        {
            var sqlQuery = Query(CrmDbContext.FieldValue)
                               .Where(x => x.FieldId == fieldID && String.Compare(x.Value, fieldValue, true) == 0);

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType || x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType);
            }


            return sqlQuery.Select(x => x.EntityId).ToList();
        }

        public bool IsExist(int id)
        {
            return Query(CrmDbContext.FieldDescription).Where(x => x.Id == id).Any();
        }

        public int GetFieldId(EntityType entityType, String label, CustomFieldType customFieldType)
        {
            var sqlQuery = Query(CrmDbContext.FieldDescription).Where(x => x.Type == customFieldType && x.Label == label);

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType || x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType);
            }


            var result = sqlQuery.FirstOrDefault();

            if (result == null) return 0;

            return result.Id;
        }

        public void EditItem(CustomField customField)
        {
            if (string.IsNullOrEmpty(customField.Label))
                throw new ArgumentException();

            if (HaveRelativeLink(customField.ID))
            {
                try
                {
                    var resultMask = "";

                    var row = Query(CrmDbContext.FieldDescription).Where(x => x.Id == customField.ID).Select(x => new { x.Type, x.Mask }).Single();

                    var fieldType = row.Type;
                    var oldMask = row.Mask;

                    if (fieldType == CustomFieldType.SelectBox)
                    {
                        if (oldMask == customField.Mask || customField.Mask.Length == 0)
                        {
                            resultMask = oldMask;
                        }
                        else
                        {
                            try
                            {

                                var maskObjOld = JsonSerializer.Deserialize<List<String>>(oldMask);
                                var maskObjNew = JsonSerializer.Deserialize<List<String>>(customField.Mask);

                                var inm = maskObjNew.Intersect(maskObjOld).ToList();

                                if (inm.Count == maskObjOld.Count)
                                {
                                    resultMask = customField.Mask;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid, ex);
                            }
                        }
                    }
                    else
                    {
                        resultMask = GetValidMask(fieldType, customField.Mask);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    throw;
                }

                var itemToUpdate = Query(CrmDbContext.FieldDescription).FirstOrDefault(x => x.Id == customField.ID);

                itemToUpdate.Label = customField.Label;
                itemToUpdate.Mask = customField.Mask;

                CrmDbContext.SaveChanges();

            }
            else
            {
                var resultMask = GetValidMask(customField.Type, customField.Mask);

                var itemToUpdate = Query(CrmDbContext.FieldDescription).FirstOrDefault(x => x.Id == customField.ID);

                itemToUpdate.Label = customField.Label;
                itemToUpdate.Type = customField.Type;
                itemToUpdate.Mask = customField.Mask;

                CrmDbContext.SaveChanges();
            }
        }

        public void ReorderFields(int[] ids)
        {
            var tx = CrmDbContext.Database.BeginTransaction();

            for (int index = 0; index < ids.Length; index++)
            {
                var dbEntity = CrmDbContext.FieldDescription.Find(ids[index]);

                dbEntity.SortOrder = index;

                CrmDbContext.SaveChanges();
            }

            tx.Commit();
        }

        private bool HaveRelativeLink(int fieldID)
        {
            return Query(CrmDbContext.FieldValue)
                .Where(x => x.FieldId == fieldID).Any();
        }

        public String GetContactLinkCountJSON(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CrmDbContext.FieldDescription).Join(Query(CrmDbContext.FieldValue),
                          x => x.Id,
                          y => y.FieldId,
                          (x, y) => new { x, y }
                         );

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType || x.x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType);
            }

            return JsonSerializer.Serialize(sqlQuery.GroupBy(x => x.x.Id)
                                                    .Select(x => x.Count()).ToList());
        }

        public int GetContactLinkCount(EntityType entityType, int entityID)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CrmDbContext.FieldDescription).Join(Query(CrmDbContext.FieldValue),
                          x => x.Id,
                          y => y.FieldId,
                          (x, y) => new { x, y }
                         );

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType || x.x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType);
            }

            sqlQuery = sqlQuery.Where(x => x.x.Id == entityID);

            return sqlQuery.GroupBy(x => x.x.Id)
                           .Select(x => x.Count()).SingleOrDefault();
        }

        public List<CustomField> GetEnityFields(EntityType entityType, int entityID, bool includeEmptyFields)
        {
            return GetEnityFields(entityType, entityID == 0 ? null : new[] { entityID }, includeEmptyFields);
        }

        public List<CustomField> GetEnityFields(EntityType entityType, int[] entityID)
        {
            return GetEnityFields(entityType, entityID, false);
        }

        private List<CustomField> GetEnityFields(EntityType entityType, int[] entityID, bool includeEmptyFields)
        {
            // TODO: Refactoring Query!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CrmDbContext.FieldDescription).GroupJoin(Query(CrmDbContext.FieldValue),
                                         x => x.Id,
                                         y => y.FieldId,
                                         (x, y) => new { x, y }
                                        ).SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x, y });

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType || x.x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType);
            }

            if (entityID != null && entityID.Length > 0)
            {
                sqlQuery = sqlQuery.Where(x => entityID.Contains(x.y.EntityId));
                sqlQuery = sqlQuery.OrderBy(x => x.x.SortOrder);
            }
            else
            {
                sqlQuery = sqlQuery.OrderBy(x => x.y.EntityId);
                sqlQuery = sqlQuery.OrderBy(x => x.x.SortOrder);
            }

            if (!includeEmptyFields)
                sqlQuery = sqlQuery.Where(x => x.y != null || x.x.Type == CustomFieldType.Heading);

            return sqlQuery.ToList().ConvertAll(x => ToCustomField(x.x, x.y));

        }

        public CustomField GetFieldDescription(int id)
        {
            var dbEntity = CrmDbContext.FieldDescription.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            return _mapper.Map<CustomField>(dbEntity);
        }

        public List<CustomField> GetFieldsDescription(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CrmDbContext.FieldDescription)
                            .AsNoTracking();

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType || x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType);
            }

            var dbEntities = sqlQuery.ToList();

            return _mapper.Map<List<DbFieldDescription>, List<CustomField>>(dbEntities);
        }

        public void DeleteField(int id)
        {
            if (HaveRelativeLink(id))
                throw new ArgumentException();

            var tx = CrmDbContext.Database.BeginTransaction();

            var dbFieldDescription = CrmDbContext.FieldDescription.Find(id);

            CrmDbContext.Remove(dbFieldDescription);

            var dbFieldValue = Query(CrmDbContext.FieldValue).FirstOrDefault(x => x.FieldId == id);

            _factoryIndexer.Delete(dbFieldValue);

            CrmDbContext.Remove(dbFieldValue);

            CrmDbContext.SaveChanges();

            tx.Commit();
        }

        public CustomField ToCustomField(DbFieldDescription dbFieldDescription,
                                               DbFieldValue dbFieldValue = null)
        {
            var customField = _mapper.Map<CustomField>(dbFieldDescription);

            if (customField != null && dbFieldValue != null)
            {
                customField.Value = dbFieldValue.Value;
                customField.EntityID = dbFieldValue.EntityId;
            }

            return customField;
        }
    }

}