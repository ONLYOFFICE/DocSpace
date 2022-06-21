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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class TaskTemplateContainerDao : AbstractDao
    {
        public TaskTemplateContainerDao(
            DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            ILogger logger,
            ICache cache,
            IMapper mapper
            )
            : base(dbContextManager,
                  tenantManager,
                  securityContext,
                  logger,
                  cache,
                  mapper)
        {

        }

        public int SaveOrUpdate(TaskTemplateContainer item)
        {
            var dbEntity = new DbTaskTemplateContainer
            {
                Id = item.ID,
                Title = item.Title,
                EntityType = item.EntityType,
                CreateOn = item.CreateOn == DateTime.MinValue ? DateTime.UtcNow : item.CreateOn,
                CreateBy = item.CreateBy == Guid.Empty ? _securityContext.CurrentAccount.ID : item.CreateBy,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CrmDbContext.Update(dbEntity);
            CrmDbContext.SaveChanges();

            return dbEntity.Id;
        }

        public void Delete(int id)
        {
            var dbEntity = CrmDbContext.TaskTemplateContainer.Find(id);

            CrmDbContext.TaskTemplateContainer.Remove(dbEntity);

            CrmDbContext.SaveChanges();
        }

        public TaskTemplateContainer GetByID(int id)
        {
            var dbEntity = CrmDbContext.TaskTemplateContainer.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            return _mapper.Map<TaskTemplateContainer>(dbEntity);
        }

        public List<TaskTemplateContainer> GetItems(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException("", entityType.ToString());

            var dbEntities = Query(CrmDbContext.TaskTemplateContainer)
                            .AsNoTracking()
                            .Where(x => x.EntityType == entityType)
                            .ToList();

            return _mapper.Map<List<DbTaskTemplateContainer>, List<TaskTemplateContainer>>(dbEntities);
        }
    }

    [Scope]
    public class TaskTemplateDao : AbstractDao
    {
        public TaskTemplateDao(DbContextManager<CrmDbContext> dbContextManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            ILogger logger,
            ICache cache,
            IMapper mapper)
            : base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 cache,
                 mapper)
        {

        }

        public int SaveOrUpdate(TaskTemplate item)
        {
            var dbEntity = new DbTaskTemplate
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
                CreateOn = item.CreateOn == DateTime.MinValue ? DateTime.UtcNow : item.CreateOn,
                CreateBy = item.CreateBy == Guid.Empty ? _securityContext.CurrentAccount.ID : item.CreateBy,
                LastModifedOn = DateTime.UtcNow,
                LastModifedBy = _securityContext.CurrentAccount.ID,
                SortOrder = item.SortOrder,
                TenantId = TenantID
            };

            CrmDbContext.Update(dbEntity);
            CrmDbContext.SaveChanges();

            return dbEntity.Id;

        }

        public TaskTemplate GetNext(int taskID)
        {
            var sqlResult = Query(CrmDbContext.TaskTemplateTask)
                 .Join(Query(CrmDbContext.TaskTemplates),
                       x => x.TaskTemplateId,
                       y => y.Id,
                       (x, y) => new { x, y }
                       ).Where(x => x.x.TaskId == taskID)
                        .Select(x => new { x.y.ContainerId, x.y.SortOrder })
                        .SingleOrDefault();

            if (sqlResult == null) return null;

            var dbEntity = Query(CrmDbContext.TaskTemplates)
                      .FirstOrDefault(x => x.ContainerId == sqlResult.ContainerId &&
                                      x.SortOrder > sqlResult.SortOrder && !x.DeadLineIsFixed);

            var result = _mapper.Map<TaskTemplate>(dbEntity);

            var dbEntityToDelete = CrmDbContext.TaskTemplateTask.Find(taskID);

            if (dbEntityToDelete.TenantId != TenantID) return null;

            CrmDbContext.Remove(dbEntityToDelete);
            CrmDbContext.SaveChanges();

            return result;
        }

        public List<TaskTemplate> GetAll()
        {
            var dbEntities = Query(CrmDbContext.TaskTemplates)
                    .AsNoTracking()
                    .OrderBy(x => x.SortOrder)
                    .ToList();

            return _mapper.Map<List<DbTaskTemplate>, List<TaskTemplate>>(dbEntities);
        }

        public List<TaskTemplate> GetList(int containerID)
        {
            var dbEntities = Query(CrmDbContext.TaskTemplates)
                                .AsNoTracking()
                                .Where(x => x.ContainerId == containerID)
                                .OrderBy(x => x.SortOrder)
                                .ToList();

            return _mapper.Map<List<DbTaskTemplate>, List<TaskTemplate>>(dbEntities);
        }

        public TaskTemplate GetByID(int id)
        {
            var dbEntity = CrmDbContext.TaskTemplates.Find(id);

            if (dbEntity.TenantId != TenantID) return null;

            return _mapper.Map<TaskTemplate>(dbEntity);
        }

        public virtual void Delete(int id)
        {
            var dbEntity = CrmDbContext.TaskTemplateContainer.Find(id);

            if (dbEntity.TenantId != TenantID) return;

            CrmDbContext.Remove(dbEntity);

            CrmDbContext.SaveChanges();
        }
    }
}