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

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;

namespace ASC.Projects.Data
{
    public abstract class BaseDao
    {
        protected Guid CurrentUserID { get; private set; }
        protected WebProjectsContext WebProjectsContext { get; private set; }
        protected int Tenant { get; private set; }

        protected BaseDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantManager tenantManager)
        {
            CurrentUserID = securityContext.CurrentAccount.ID;
            WebProjectsContext = dbContextManager.Value;
            Tenant = tenantManager.GetCurrentTenant().TenantId;
        }

        protected static Guid ToGuid(object guid)
        {
            try
            {
                var str = guid as string;
                return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }

        }
    }
}
