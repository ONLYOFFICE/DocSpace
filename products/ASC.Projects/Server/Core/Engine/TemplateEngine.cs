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

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    [Scope]
    public class TemplateEngine
    {
        public ProjectSecurity ProjectSecurity { get; set; }
        public SecurityContext SecurityContext { get; set; }
        public TenantUtil TenantUtil { get; set; }
        public IDaoFactory DaoFactory { get; set; }

        public TemplateEngine(SecurityContext securityContext, IDaoFactory daoFactory, ProjectSecurity projectSecurity, TenantUtil tenantUtil)
        {
            SecurityContext = securityContext;
            ProjectSecurity = projectSecurity;
            TenantUtil = tenantUtil;
            DaoFactory = daoFactory;
        }

        public List<Template> GetAll()
        {
            return DaoFactory.GetTemplateDao().GetAll();
        }

        public int GetCount()
        {
            return DaoFactory.GetTemplateDao().GetCount();
        }

        public Template GetByID(int id)
        {
            return DaoFactory.GetTemplateDao().GetByID(id);
        }

        public Template SaveOrUpdate(Template template)
        {
            if (template.Id == default(int))
            {
                if (template.CreateBy == default(Guid)) template.CreateBy = SecurityContext.CurrentAccount.ID;
                if (template.CreateOn == default(DateTime)) template.CreateOn = TenantUtil.DateTimeNow();
            }
            else
            {
                if (!ProjectSecurity.CanEditTemplate(template))
                {
                    ProjectSecurity.CreateSecurityException();
                }
            }

            template.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            template.LastModifiedOn = TenantUtil.DateTimeNow();

            return DaoFactory.GetTemplateDao().SaveOrUpdate(template);
        }

        public void Delete(int id)
        {
            DaoFactory.GetTemplateDao().Delete(id);
        }
    }
}
