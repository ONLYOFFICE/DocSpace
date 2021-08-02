/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    /* internal class CachedStatusDao : StatusDao
     {
         private readonly HttpRequestDictionary<List<CustomTaskStatus>> statusCache = new HttpRequestDictionary<List<CustomTaskStatus>>("status");

         public CachedStatusDao(int tenant) : base(tenant)
         {
         }

         public override void Delete(int id)
         {
             base.Delete(id);
             ResetCache();
         }

         public override CustomTaskStatus Create(CustomTaskStatus status)
         {
             var result = base.Create(status);
             ResetCache();
             return result;
         }

         public override void Update(CustomTaskStatus status)
         {
             base.Update(status);
             ResetCache();
         }

         public override List<CustomTaskStatus> Get()
         {
             return statusCache.Get(Tenant.ToString(), BaseGet);
         }

         private List<CustomTaskStatus> BaseGet()
         {
             return base.Get();
         }

         private void ResetCache()
         {
             statusCache.Reset(Tenant.ToString());
         }
     }
    */
    [Scope]
    public class StatusDao : BaseDao, IStatusDao
    {
        public StatusDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
        }

        public virtual CustomTaskStatus Create(CustomTaskStatus status)
        {
            var dbStatus = new DbStatus()
            {
                Id = status.Id,
                Title = status.Title,
                Description = status.Description,
                StatusType = (int)status.StatusType,
                Image = status.Image,
                ImageType = status.ImageType,
                Color = status.Color,
                Order = status.Order,
                IsDefault = Convert.ToInt32(status.IsDefault),
                Available = Convert.ToInt32(status.Available.GetValueOrDefault())
            };
            WebProjectsContext.Status.Add(dbStatus);
            WebProjectsContext.SaveChanges();

            return status;
        }

        public virtual void Update(CustomTaskStatus status)
        {
            var dbStatus = new DbStatus()
            {
                Id = status.Id,
                Title = status.Title,
                Description = status.Description,
                StatusType = (int)status.StatusType,
                Image = status.Image,
                ImageType = status.ImageType,
                Color = status.Color,
                Order = status.Order,
                IsDefault = Convert.ToInt32(status.IsDefault),
                Available = Convert.ToInt32(status.Available.GetValueOrDefault())
            };
            WebProjectsContext.Status.Update(dbStatus);
            WebProjectsContext.SaveChanges();
        }

        public virtual List<CustomTaskStatus> Get()
        {
            return WebProjectsContext.Status
                .Select(s => new CustomTaskStatus
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Image = s.Image,
                    ImageType = s.ImageType,
                    Color = s.Color,
                    Order = s.Order,
                    StatusType = (TaskStatus)s.StatusType,
                    IsDefault = Convert.ToBoolean(s.IsDefault),
                    Available = Convert.ToBoolean(s.Available)
                })
                .ToList();
        }

        public virtual void Delete(int id)
        {
            var status = WebProjectsContext.Status.Where(s => s.Id == id).SingleOrDefault();
            WebProjectsContext.Status.Remove(status);

            var tasks = WebProjectsContext.Task.Where(t => t.StatusId == id).ToList();
            WebProjectsContext.Task.RemoveRange(tasks);

            WebProjectsContext.SaveChanges();
        }
    }
}