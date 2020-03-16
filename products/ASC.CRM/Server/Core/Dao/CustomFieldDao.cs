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
            TenantUtil tenantUtil       
            ) :
              base(dbContextManager,
                 tenantManager,
                 securityContext)
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
                                
                var id = Db.ExecuteScalar<int>(
                        Insert("crm_field_value")
                        .InColumnValue("id", 0)
                        .InColumnValue("entity_id", entityID)
                        .InColumnValue("value", fieldValue)
                        .InColumnValue("field_id", fieldID)
                        .InColumnValue("entity_type", (int)entityType)
                        .InColumnValue("last_modifed_on", lastModifiedOn)
                        .InColumnValue("last_modifed_by", SecurityContext.CurrentAccount.ID)
                        .Identity(1, 0, true));

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
                    _log.Error(ex);
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

            Query(CRMDbContext.FieldValue).Where(x => x.FieldId == fieldID && x.EntityId == entityID);


            var sqlQuery = Query("crm_field_value")
                          .Select("value")
                          .Where(Exp.Eq("field_id", fieldID)
                                 & BuildEntityTypeConditions(entityType, "entity_type")
                                 & Exp.Eq("entity_id", entityID));

            return Db.ExecuteScalar<String>(sqlQuery);
        }

        public List<Int32> GetEntityIds(EntityType entityType, int fieldID, String fieldValue)
        {

            Query(CRMDbContext.FieldValue).Where(x => x.FieldId == fieldID && String.Compare(x.Value, fieldValue, true) == 0);


            var sqlQuery = Query("crm_field_value")
                          .Select("entity_id")
                          .Where(Exp.Eq("field_id", fieldID)
                                 & BuildEntityTypeConditions(entityType, "entity_type")
                                 & Exp.Eq("value", fieldValue));

            return Db.ExecuteList(sqlQuery).ConvertAll(row => Convert.ToInt32(row[0]));
        }

        public bool IsExist(int id)
        {
            return Query(CRMDbContext.FieldDescription).Where(x => x.Id == id).Any();
        }

        public int GetFieldId(EntityType entityType, String label, CustomFieldType customFieldType)
        {
            var result = Db.ExecuteList(GetFieldDescriptionSqlQuery(
                Exp.Eq("type", (int)customFieldType)
                & BuildEntityTypeConditions(entityType, "entity_type")
                & Exp.Eq("label", label))).ConvertAll(row => ToCustomField(row));

            if (result.Count == 0) return 0;

            else return result[0].ID;
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

                    var row = Db.ExecuteList(Query("crm_field_description")
                    .Where(Exp.Eq("id", customField.ID))
                    .Select("type", "mask")).FirstOrDefault();

                    var fieldType = (CustomFieldType)Convert.ToInt32(row[0]);
                    var oldMask = Convert.ToString(row[1]);

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
                    _log.Error(ex);
                    throw ex;
                }

                Db.ExecuteNonQuery(
                    Update("crm_field_description")
                    .Set("label", customField.Label)
                    .Set("mask", customField.Mask)
                    .Where(Exp.Eq("id", customField.ID)));
            }
            else
            {
                var resultMask = GetValidMask(customField.FieldType, customField.Mask);
                
                Db.ExecuteNonQuery(
                    Update("crm_field_description")
                    .Set("label", customField.Label)
                    .Set("type", (int)customField.FieldType)
                    .Set("mask", resultMask)
                    .Where(Exp.Eq("id", customField.ID)));
            
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

            var sqlQuery = Query("crm_field_description tblFD")
                .Select("count(tblFV.field_id)")
                .LeftOuterJoin("crm_field_value tblFV", Exp.EqColumns("tblFD.id", "tblFV.field_id"))
                .OrderBy("tblFD.sort_order", true)
                .GroupBy("tblFD.id");

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "tblFD.entity_type"));

            var queryResult = Db.ExecuteList(sqlQuery);

            return JsonConvert.SerializeObject(queryResult.ConvertAll(row => row[0]));
        }

        public int GetContactLinkCount(EntityType entityType, int entityID)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            Query();
                
            var sqlQuery = Query("crm_field_description tblFD")
                .Select("count(tblFV.field_id)")
                .LeftOuterJoin("crm_field_value tblFV", Exp.EqColumns("tblFD.id", "tblFV.field_id"))
                .Where(Exp.Eq("tblFD.id", entityID))
                .OrderBy("tblFD.sort_order", true);

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "tblFD.entity_type"));

            return Db.ExecuteScalar<int>(sqlQuery);
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

            SqlQuery sqlQuery = Query("crm_field_description tbl_field")
                .Select("tbl_field.id",
                        "tbl_field_value.entity_id",
                        "tbl_field.label",
                        "tbl_field_value.value",
                        "tbl_field.type",
                        "tbl_field.sort_order",
                        "tbl_field.mask",
                        "tbl_field.entity_type");

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "tbl_field.entity_type"));

            if (entityID != null && entityID.Length > 0)
                sqlQuery.LeftOuterJoin("crm_field_value tbl_field_value",
                                   Exp.EqColumns("tbl_field_value.field_id", "tbl_field.id") &
                                   Exp.In("tbl_field_value.entity_id", entityID))
                .OrderBy("tbl_field.sort_order", true);
            else
                sqlQuery.LeftOuterJoin("crm_field_value tbl_field_value",
                                      Exp.EqColumns("tbl_field_value.field_id", "tbl_field.id"))
               .Where(Exp.Eq("tbl_field_value.tenant_id", TenantID))
               .OrderBy("tbl_field_value.entity_id", true)
               .OrderBy("tbl_field.sort_order", true);

            if (!includeEmptyFields)
                return Db.ExecuteList(sqlQuery)
                        .ConvertAll(row => ToCustomField(row)).FindAll(item =>
                        {
                            if (item.FieldType == CustomFieldType.Heading)
                                return true;

                            return !String.IsNullOrEmpty(item.Value.Trim());

                        }).ToList();

            return Db.ExecuteList(sqlQuery)
                    .ConvertAll(row => ToCustomField(row));
        }

        public CustomField GetFieldDescription(int fieldID)
        {

            var sqlQuery = GetFieldDescriptionSqlQuery(null);

            sqlQuery.Where(Exp.Eq("id", fieldID));

            var fields = Db.ExecuteList(sqlQuery).ConvertAll(row => ToCustomField(row));

            return fields.Count == 0 ? null : fields[0];
        }

        public List<CustomField> GetFieldsDescription(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            SqlQuery sqlQuery = GetFieldDescriptionSqlQuery(null);

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "entity_type"));

            return Db.ExecuteList(sqlQuery)
                .ConvertAll(row => ToCustomField(row));
        }

        private SqlQuery GetFieldDescriptionSqlQuery(Exp where)
        {
            var sqlQuery = Query("crm_field_description")
                .Select("id",
                        "-1",
                        "label",
                        "\" \"",
                        "type",
                        "sort_order",
                        "mask",
                        "entity_type")
                .OrderBy("sort_order", true);

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        private Exp BuildEntityTypeConditions(EntityType entityType, String dbFieldName)
        {
            switch (entityType)
            {
                case EntityType.Company:
                case EntityType.Person:
                    return Exp.In(dbFieldName, new[] { (int)entityType, (int)EntityType.Contact });

                default:
                    return Exp.Eq(dbFieldName, (int)entityType);

            }

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

        public static CustomField ToCustomField(DbFieldDescription dbFieldDescription,
                                                DbFieldValue dbFieldValue)
        {


            throw new NotImplementedException();
            //return new CustomField
            //{
            //    ID = Convert.ToInt32(row[0]),
            //    EntityID = Convert.ToInt32(row[1]),
            //    EntityType = (EntityType)Convert.ToInt32(row[7]),
            //    Label = Convert.ToString(row[2]),
            //    Value = Convert.ToString(row[3]),
            //    FieldType = (CustomFieldType)Convert.ToInt32(row[4]),
            //    Position = Convert.ToInt32(row[5]),
            //    Mask = Convert.ToString(row[6])
            //};
        }
    }

}