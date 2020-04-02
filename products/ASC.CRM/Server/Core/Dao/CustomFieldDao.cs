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
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.ElasticSearch;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Core.Search;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.CRM.Core.Dao
{
    public class CustomFieldDao : AbstractDao
    {
        public CustomFieldDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            TenantUtil tenantUtil,
            IOptionsMonitor<ILog> logger
            ) :
              base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger)
        {
            TenantUtil = tenantUtil;
        }

        public TenantUtil TenantUtil { get; }

        public FactoryIndexer<FieldsWrapper> FactoryIndexer { get; }

        public void SaveList(List<CustomField> items)
        {
            if (items == null || items.Count == 0) return;

            var tx = CRMDbContext.Database.BeginTransaction();

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

        private void SetFieldValueInDb(EntityType entityType, int entityID, int fieldID, String fieldValue)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            fieldValue = fieldValue.Trim();

            var itemToDelete = Query(CRMDbContext.FieldValue)
                .Where(x => x.EntityId == entityID && x.EntityType == entityType && x.FieldId == fieldID);

            CRMDbContext.FieldValue.RemoveRange(itemToDelete);
            CRMDbContext.SaveChanges();

            if (!String.IsNullOrEmpty(fieldValue))
            {
                var lastModifiedOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());

                var dbFieldValue = new DbFieldValue
                {
                    EntityId = entityID,
                    Value = fieldValue,
                    FieldId = fieldID,
                    EntityType = entityType,
                    LastModifedOn = lastModifiedOn,
                    LastModifedBy = SecurityContext.CurrentAccount.ID,
                    TenantId = TenantID
                };

                CRMDbContext.Add(dbFieldValue);
                CRMDbContext.SaveChanges();

                var id = dbFieldValue.Id;

                FactoryIndexer.IndexAsync(new FieldsWrapper
                {
                    Id = id,
                    EntityId = entityID,
                    EntityType = (int)entityType,
                    Value = fieldValue,
                    FieldId = fieldID,
                    LastModifiedOn = lastModifiedOn,
                    TenantId = TenantID
                });
            }
        }

        private string GetValidMask(CustomFieldType customFieldType, String mask)
        {
            var resultMask = new JObject();

            if (customFieldType == CustomFieldType.CheckBox || customFieldType == CustomFieldType.Heading || customFieldType == CustomFieldType.Date)
                return String.Empty;

            if (String.IsNullOrEmpty(mask))
                throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid);

            try
            {
                var maskObj = JToken.Parse(mask);
                if (customFieldType == CustomFieldType.TextField)
                {
                    var size = maskObj.Value<int>("size");
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
                    var rows = maskObj.Value<int>("rows");
                    var cols = maskObj.Value<int>("cols");

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
                    if (maskObj is JArray)
                    {
                        return mask;
                    }
                    else
                    {
                        throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid);
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
                    Logger.Error(ex);

                    throw ex;
                }
            }
            return JsonConvert.SerializeObject(resultMask);
        }

        public int CreateField(EntityType entityType, String label, CustomFieldType customFieldType, String mask)
        {
            if (!_supportedEntityType.Contains(entityType) || String.IsNullOrEmpty(label))
                throw new ArgumentException();
            var resultMask = GetValidMask(customFieldType, mask);

            var sortOrder = Query(CRMDbContext.FieldDescription).Select(x => x.SortOrder).Max() + 1;

            var itemToInsert = new DbFieldDescription
            {
                Label = label,
                Type = customFieldType,
                Mask = resultMask,
                SortOrder = sortOrder,
                EntityType = entityType,
                TenantId = TenantID
            };

            CRMDbContext.FieldDescription.Add(itemToInsert);

            CRMDbContext.SaveChanges();

            return itemToInsert.Id;
        }

        public String GetValue(EntityType entityType, int entityID, int fieldID)
        {
            var sqlQuery = Query(CRMDbContext.FieldValue).Where(x => x.FieldId == fieldID && x.EntityId == entityID);

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
            var sqlQuery = Query(CRMDbContext.FieldValue)
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
            return Query(CRMDbContext.FieldDescription).Where(x => x.Id == id).Any();
        }

        public int GetFieldId(EntityType entityType, String label, CustomFieldType customFieldType)
        {
            var sqlQuery = Query(CRMDbContext.FieldDescription).Where(x => x.Type == customFieldType && x.Label == label);

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
            if (String.IsNullOrEmpty(customField.Label))
                throw new ArgumentException();

            if (HaveRelativeLink(customField.ID))
            {
                try
                {
                    var resultMask = "";

                    var row = Query(CRMDbContext.FieldDescription).Where(x => x.Id == customField.ID).Select(x => new { x.Type, x.Mask }).Single();

                    var fieldType = row.Type;
                    var oldMask = row.Mask;

                    if (fieldType == CustomFieldType.SelectBox)
                    {
                        if (oldMask == customField.Mask || customField.Mask == "")
                        {
                            resultMask = oldMask;
                        }
                        else
                        {
                            var maskObjOld = JToken.Parse(oldMask);
                            var maskObjNew = JToken.Parse(customField.Mask);

                            if (!(maskObjOld is JArray && maskObjNew is JArray))
                            {
                                throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid);
                            }
                            var inm = (((JArray)maskObjNew).ToList()).Intersect(((JArray)maskObjOld).ToList()).ToList();
                            if (inm.Count == ((JArray)maskObjOld).ToList().Count)
                            {
                                resultMask = customField.Mask;
                            }
                            else
                            {
                                throw new ArgumentException(CRMErrorsResource.CustomFieldMaskNotValid);
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
                    Logger.Error(ex);

                    throw ex;
                }

                var itemToUpdate = Query(CRMDbContext.FieldDescription).FirstOrDefault(x => x.Id == customField.ID);

                itemToUpdate.Label = customField.Label;
                itemToUpdate.Mask = customField.Mask;

                CRMDbContext.Update(itemToUpdate);
                CRMDbContext.SaveChanges();

            }
            else
            {
                var resultMask = GetValidMask(customField.FieldType, customField.Mask);

                var itemToUpdate = Query(CRMDbContext.FieldDescription).FirstOrDefault(x => x.Id == customField.ID);

                itemToUpdate.Label = customField.Label;
                itemToUpdate.Type = customField.FieldType;
                itemToUpdate.Mask = customField.Mask;

                CRMDbContext.Update(itemToUpdate);
                CRMDbContext.SaveChanges();
            }
        }

        public void ReorderFields(int[] fieldID)
        {
            for (int index = 0; index < fieldID.Length; index++)
            {
                var itemToUpdate = Query(CRMDbContext.FieldDescription).FirstOrDefault(x => x.Id == fieldID[index]);

                itemToUpdate.SortOrder = index;

                CRMDbContext.Update(itemToUpdate);

                CRMDbContext.SaveChanges();
            }
        }

        private bool HaveRelativeLink(int fieldID)
        {
            return Query(CRMDbContext.FieldValue)
                .Where(x => x.FieldId == fieldID).Any();
        }

        public String GetContactLinkCountJSON(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CRMDbContext.FieldDescription).GroupJoin(Query(CRMDbContext.FieldValue),
                                      x => x.Id,
                                      y => y.FieldId,
                                      (x, y) => new { x = x, count = y.Count() }
                                     );

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType || x.x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.x.EntityType == entityType);
            }

            sqlQuery = sqlQuery.OrderBy(x => x.x.SortOrder);

            return JsonConvert.SerializeObject(sqlQuery.Select(x => x.count).ToList());
        }

        public int GetContactLinkCount(EntityType entityType, int entityID)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CRMDbContext.FieldDescription).GroupJoin(Query(CRMDbContext.FieldValue),
                          x => x.Id,
                          y => y.FieldId,
                          (x, y) => new { x = x, count = y.Count() }
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
            sqlQuery = sqlQuery.OrderBy(x => x.x.SortOrder);

            return sqlQuery.Single().count;
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

            var sqlQuery = Query(CRMDbContext.FieldDescription).GroupJoin(Query(CRMDbContext.FieldValue),
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
                sqlQuery = sqlQuery.Where(x => x.y != null && x.x.Type == CustomFieldType.Heading);

            return sqlQuery.ToList().ConvertAll(x => ToCustomField(x.x, x.y));
        }

        public CustomField GetFieldDescription(int fieldID)
        {
            return ToCustomField(Query(CRMDbContext.FieldDescription).FirstOrDefault(x => x.Id == fieldID));
        }

        public List<CustomField> GetFieldsDescription(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query(CRMDbContext.FieldDescription);

            if (entityType == EntityType.Company || entityType == EntityType.Person)
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType || x.EntityType == EntityType.Contact);
            }
            else
            {
                sqlQuery = sqlQuery.Where(x => x.EntityType == entityType);
            }

            return sqlQuery.ToList().ConvertAll(x => ToCustomField(x));
        }

        public void DeleteField(int fieldID)
        {
            //if (HaveRelativeLink(fieldID))
            //    throw new ArgumentException();

            var tx = CRMDbContext.Database.BeginTransaction();

            var fieldDescription = new DbFieldDescription { Id = fieldID };

            CRMDbContext.FieldDescription.Attach(fieldDescription);
            CRMDbContext.FieldDescription.Remove(fieldDescription);

            var fieldValue = Query(CRMDbContext.FieldValue).FirstOrDefault(x => x.FieldId == fieldID);

            CRMDbContext.Remove(fieldValue);

            CRMDbContext.SaveChanges();

            tx.Commit();
        }

        public CustomField ToCustomField(DbFieldDescription dbFieldDescription,
                                                DbFieldValue dbFieldValue = null)
        {
            if (dbFieldDescription == null || dbFieldValue == null) return null;

            var customField = new CustomField
            {
                ID = dbFieldDescription.Id,
                EntityType = dbFieldDescription.EntityType,
                FieldType = dbFieldDescription.Type,
                Label = dbFieldDescription.Label,
                Mask = dbFieldDescription.Mask,
                Position = dbFieldDescription.SortOrder

            };

            if (dbFieldValue != null)
            {
                dbFieldValue.Value = dbFieldValue.Value;
                dbFieldValue.EntityId = dbFieldValue.EntityId;
            }

            return customField;
        }
    }

}