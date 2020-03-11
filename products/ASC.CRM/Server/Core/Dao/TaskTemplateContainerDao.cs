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
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.CRM.Core.Dao
{
    public class TaskTemplateContainerDao : AbstractDao
    {
        public TaskTemplateContainerDao(
            DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext)
            : base(dbContextManager,
                  tenantManager,
                  securityContext)
        {

        }

        public virtual int SaveOrUpdate(TaskTemplateContainer item)
        {
            var dbTaskTemplateContainer = new DbTaskTemplateContainer
            {
                Id = item.ID,
                Title = item.Title,
                EntityType = item.EntityType,
                CreateOn = DateTime.UtcNow,
                CreateBy = SecurityContext.CurrentAccount.ID,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = SecurityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            if (item.ID == 0 && Query(CRMDbContext.TaskTemplateContainer).Where(x => x.Id == item.ID).Any())
            {
                CRMDbContext.TaskTemplateContainer.Add(dbTaskTemplateContainer);

                item.ID = dbTaskTemplateContainer.Id;

                CRMDbContext.SaveChanges();

            }
            else
            {
                CRMDbContext.TaskTemplateContainer.Attach(dbTaskTemplateContainer);

                CRMDbContext.SaveChanges();
            }


            return item.ID;
        }

        public virtual void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var itemToDelete = new DbTaskTemplateContainer { Id = id };

            CRMDbContext.TaskTemplateContainer.Attach(itemToDelete);
            CRMDbContext.TaskTemplateContainer.Remove(itemToDelete);

            CRMDbContext.SaveChanges();
        }

        public virtual TaskTemplateContainer GetByID(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            return ToObject(Query(CRMDbContext.TaskTemplateContainer).FirstOrDefault(x => x.Id == id));
        }

        public virtual List<TaskTemplateContainer> GetItems(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException("", entityType.ToString());

            return Query(CRMDbContext.TaskTemplateContainer)
                .Where(x => x.EntityType == entityType)
                .ToList()
                .ConvertAll(ToObject);
        }
            
        protected TaskTemplateContainer ToObject(DbTaskTemplateContainer dbTaskTemplateContainer)
        {
            if (dbTaskTemplateContainer == null) return null;

            return new TaskTemplateContainer
            {
                ID = dbTaskTemplateContainer.Id,
                Title = dbTaskTemplateContainer.Title,
                EntityType = dbTaskTemplateContainer.EntityType
            };
        }
    }

    public class TaskTemplateDao : AbstractDao
    {
        public TaskTemplateDao(DbContextManager<CRMDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext)
            : base(dbContextManager,
                 tenantManager,
                 securityContext)
        {

        }

        public int SaveOrUpdate(TaskTemplate item)
        {
            if (item.ID == 0)
            {
                var itemToInsert = new DbTaskTemplate
                {
                    Id = item.ID,
                    Title = item.Title,
                    CategoryId = item.CategoryID,
                    Description = item.Description,
                    ResponsibleId = item.ResponsibleID,
                    IsNotify = item.isNotify,
                    Offset = item.Offset.Ticks,
                    DeadLineIsFixed = item.DeadLineIsFixed,
                    ContainerId = item.ContainerID,
                    CreateOn = DateTime.UtcNow,
                    CreateBy = SecurityContext.CurrentAccount.ID,
                    LastModifedOn = DateTime.UtcNow,
                    LastModifedBy = SecurityContext.CurrentAccount.ID,
                    TenantId = TenantID
                };

                CRMDbContext.TaskTemplates.Add(itemToInsert);
                CRMDbContext.SaveChanges();
            }
            else
            {

                //Db.ExecuteNonQuery(
                //    Update("crm_task_template")
                //        .Set("title", item.Title)
                //        .Set("category_id", item.CategoryID)
                //        .Set("description", item.Description)
                //        .Set("responsible_id", item.ResponsibleID)
                //        .Set("is_notify", item.isNotify)
                //        .Set("offset", item.Offset.Ticks)
                //        .Set("deadLine_is_fixed", item.DeadLineIsFixed)
                //        .Set("container_id", item.ContainerID)
                //        .Set("last_modifed_on", DateTime.UtcNow)
                //        .Set("last_modifed_by", SecurityContext.CurrentAccount.ID)
                //        .Where("id", item.ID));

                //CRMDbContext.TaskTemplates.Add(itemToInsert);
                //CRMDbContext.SaveChanges();
            }

            return item.ID;
        }

        public TaskTemplate GetNext(int taskID)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();

            var temp = Query(CRMDbContext.TaskTemplateTask)
                 .Join(CRMDbContext.TaskTemplates,
                     x => new { x.TenantId, x.TaskTemplateId },
                     y => new { y.TenantId, y.Id },
                     (x, y) => new
                     {
                         x, y
                     });

            //var sqlResult = Db.ExecuteList(
            //     Query("crm_task_template_task tblTTT")
            //     .Select("tblTT.container_id")
            //     .Select("tblTT.sort_order")
            //     .LeftOuterJoin("crm_task_template tblTT", Exp.EqColumns("tblTT.tenant_id", "tblTTT.tenant_id") & Exp.EqColumns("tblTT.id", "tblTTT.task_template_id"))
            //     .Where(Exp.Eq("tblTTT.task_id", taskID) & Exp.Eq("tblTT.tenant_id", TenantID)));

            //if (sqlResult.Count == 0) return null;

            //var result = Db.ExecuteList(GetQuery(Exp.Eq("container_id", sqlResult[0][0]) &
            //                                Exp.Gt("sort_order", sqlResult[0][1]) &
            //                                Exp.Eq("deadLine_is_fixed", false)).SetMaxResults(1)).ConvertAll(
            //                                    row => ToObject(row));

            //Db.ExecuteNonQuery(Delete("crm_task_template_task").Where(Exp.Eq("task_id", taskID)));

            tx.Commit();

//            if (result.Count == 0) return null;

  //          return result[0];

        }

        public List<TaskTemplate> GetAll()
        {
            return Query(CRMDbContext.TaskTemplates)
                    .OrderBy(x => x.SortOrder)
                    .ToList()
                    .ConvertAll(ToObject);
        }

        public List<TaskTemplate> GetList(int containerID)
        {
            if (containerID <= 0)
                throw new NotImplementedException();

            return Query(CRMDbContext.TaskTemplates)
                .Where(x => x.ContainerId == containerID)
                .OrderBy(x=> x.SortOrder)
                .ToList()
                .ConvertAll(ToObject);
        }

        public virtual TaskTemplate GetByID(int id)
        {
            if (id <= 0)
                throw new NotImplementedException();

            return ToObject(CRMDbContext.TaskTemplates.FirstOrDefault(x => x.Id == id));
        }

        public virtual void Delete(int id)
        {
            if (id <= 0)
                throw new NotImplementedException();

            var itemToRemove = new DbTaskTemplate
            {
                Id = id
            };

            CRMDbContext.Entry(itemToRemove).State = EntityState.Deleted;
            CRMDbContext.SaveChanges();
        }

        protected TaskTemplate ToObject(DbTaskTemplate dbTaskTemplate)
        {
            if (dbTaskTemplate == null) return null;

            return new TaskTemplate
            {
                ID = dbTaskTemplate.Id,
                Title = dbTaskTemplate.Title,
                CategoryID = dbTaskTemplate.CategoryId,
                Description = dbTaskTemplate.Description,
                ResponsibleID = dbTaskTemplate.ResponsibleId,
                isNotify = dbTaskTemplate.IsNotify,
                Offset = TimeSpan.FromTicks(dbTaskTemplate.Offset),
                DeadLineIsFixed = dbTaskTemplate.DeadLineIsFixed,
                ContainerID = dbTaskTemplate.ContainerId,
                CreateOn = dbTaskTemplate.CreateOn,
                CreateBy = dbTaskTemplate.CreateBy
            };
        }     
    }

}