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
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    [Scope]
    public class TemplateDao : BaseDao, ITemplateDao
    {
        private TenantUtil TenantUtil { get; set; }

        public TemplateDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
        }

        public List<Template> GetAll()
        {
            return WebProjectsContext.Template
                .OrderBy(t => t.CreateOn)
                .ToList()
                .ConvertAll(t => ToTemplate(t));
        }

        public int GetCount()
        {
            return WebProjectsContext.Template.Count();
        }

        public Template GetByID(int id)
        {
            var query = WebProjectsContext.Template
                .Where(t=> t.Id == id)
                .SingleOrDefault();
            return query == null ? null : ToTemplate(query);
        }

        public Template SaveOrUpdate(Template template)
        {
            if(WebProjectsContext.Template.Where(t=> t.Id == template.Id).Any())
            {
                var db = WebProjectsContext.Template.Where(t => t.Id == template.Id).SingleOrDefault();
                db.Title = template.Title;
                db.Description = template.Description;
                db.CreateBy = template.CreateBy.ToString();
                db.TenantId = Tenant;
                db.CreateOn = TenantUtil.DateTimeToUtc(db.CreateOn);
                db.LastModifiedOn = TenantUtil.DateTimeToUtc(db.LastModifiedOn);
                db.LastModifiedBy = template.LastModifiedBy.ToString();
                WebProjectsContext.Template.Update(db);
                WebProjectsContext.SaveChanges();
                return ToTemplate(db);
            }
            else
            {
                var db = ToDbTemplate(template);
                db.CreateOn = TenantUtil.DateTimeToUtc(db.CreateOn);
                db.LastModifiedOn = TenantUtil.DateTimeToUtc(db.LastModifiedOn);
                WebProjectsContext.Template.Add(db);
                WebProjectsContext.SaveChanges();
                return ToTemplate(db);
            }
        }

        public void Delete(int id)
        {
            var template = WebProjectsContext.Template.Where(t => t.Id == id).SingleOrDefault();
            WebProjectsContext.Template.Remove(template);
            WebProjectsContext.SaveChanges();
        }

        private Template ToTemplate(DbTemplate template)
        {
            return new Template
            {
                Id = template.Id,
                Title = template.Title,
                Description = template.Description,
                CreateBy = new Guid(template.CreateBy),
                CreateOn = TenantUtil.DateTimeFromUtc(template.CreateOn)
            };
        }

        private DbTemplate ToDbTemplate(Template template)
        {
            return new DbTemplate
            {
                Id = template.Id,
                Title = template.Title,
                Description = template.Description,
                CreateBy = template.CreateBy.ToString(),
                CreateOn = TenantUtil.DateTimeFromUtc(template.CreateOn),
                TenantId = Tenant
            };
        }
    }
}
