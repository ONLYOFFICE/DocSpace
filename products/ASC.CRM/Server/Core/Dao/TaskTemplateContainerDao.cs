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
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger
            )
            : base(dbContextManager,
                  tenantManager,
                  securityContext,
                  logger)
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
             
                CRMDbContext.SaveChanges();

                item.ID = dbTaskTemplateContainer.Id;
            }
            else
            {
                CRMDbContext.TaskTemplateContainer.Attach(dbTaskTemplateContainer);
                CRMDbContext.TaskTemplateContainer.Update(dbTaskTemplateContainer);

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
            SecurityContext securityContext,
            IOptionsMonitor<ILog> logger)
            : base(dbContextManager,
                 tenantManager,
                 securityContext, 
                 logger)
        {

        }

        public int SaveOrUpdate(TaskTemplate item)
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
                CreateOn = item.CreateOn,
                CreateBy = item.CreateBy,
                TenantId = TenantID
            };

            if (item.ID == 0)
            {
                itemToInsert.CreateOn = DateTime.UtcNow;
                itemToInsert.CreateBy = SecurityContext.CurrentAccount.ID;
                            
                CRMDbContext.TaskTemplates.Add(itemToInsert);
                CRMDbContext.SaveChanges();
            }
            else
            {

                itemToInsert.LastModifedOn = DateTime.UtcNow;
                itemToInsert.LastModifedBy = SecurityContext.CurrentAccount.ID;

                CRMDbContext.TaskTemplates.Attach(itemToInsert);
                CRMDbContext.TaskTemplates.Update(itemToInsert);

                CRMDbContext.SaveChanges();
            }

            return item.ID;
        }

        public TaskTemplate GetNext(int taskID)
        {
            using var tx = CRMDbContext.Database.BeginTransaction();

            var sqlResult = Query(CRMDbContext.TaskTemplateTask)
                 .Join(Query(CRMDbContext.TaskTemplates),
                       x => x.TaskTemplateId,
                       y => y.Id,
                       (x, y) => new { x, y }
                       ).Where(x => x.x.TaskId == taskID)
                        .Select(x => new { x.y.ContainerId, x.y.SortOrder })
                        .SingleOrDefault();

            if (sqlResult == null) return null;

            var result = ToObject(Query(CRMDbContext.TaskTemplates)
                                       .FirstOrDefault(x => x.ContainerId == sqlResult.ContainerId && 
                                                            x.SortOrder > sqlResult.SortOrder && !x.DeadLineIsFixed));

            CRMDbContext.Remove(new DbTaskTemplateTask
            {
                TaskId = taskID,
                TenantId = TenantID
            });

            tx.Commit();

            return result;        
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