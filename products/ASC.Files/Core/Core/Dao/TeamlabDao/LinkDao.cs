/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Linq;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core.Data
{
    [Scope]
    internal class LinkDao : AbstractDao, ILinkDao
    {
        public DbContextManager<EF.FilesDbContext> DbContextManager { get; }

        public LinkDao(
            UserManager userManager,
            DbContextManager<EF.FilesDbContext> dbContextManager,
            TenantManager tenantManager,
            TenantUtil tenantUtil,
            SetupInfo setupInfo,
            TenantExtra tenantExtra,
            TenantStatisticsProvider tenantStatisticProvider,
            CoreBaseSettings coreBaseSettings,
            CoreConfiguration coreConfiguration,
            SettingsManager settingsManager,
            AuthContext authContext,
            IServiceProvider serviceProvider,
            ICache cache)
            : base(
                  dbContextManager,
                  userManager,
                  tenantManager,
                  tenantUtil,
                  setupInfo,
                  tenantExtra,
                  tenantStatisticProvider,
                  coreBaseSettings,
                  coreConfiguration,
                  settingsManager,
                  authContext,
                  serviceProvider,
                  cache)
        { }

        public void AddLink(string sourceId, string linkedId)
        {
            FilesDbContext.AddOrUpdate(r => r.FilesLink, new DbFilesLink()
            {
                TenantId = TenantID,
                SourceId = sourceId,
                LinkedId = linkedId,
                LinkedFor = AuthContext.CurrentAccount.ID
            });

            FilesDbContext.SaveChanges();
        }

        public string GetSource(string linkedId)
        {
            return FilesDbContext.FilesLink
                .Where(r => r.TenantId == TenantID && r.LinkedId == linkedId && r.LinkedFor == AuthContext.CurrentAccount.ID)
                .Select(r => r.SourceId)
                .SingleOrDefault();
        }

        public string GetLinked(string sourceId)
        {
            return FilesDbContext.FilesLink
                .Where(r => r.TenantId == TenantID && r.SourceId == sourceId && r.LinkedFor == AuthContext.CurrentAccount.ID)
                .Select(r => r.LinkedId)
                .SingleOrDefault();
        }

        public void DeleteLink(string sourceId)
        {
            var link = FilesDbContext.FilesLink
                .Where(r => r.TenantId == TenantID && r.SourceId == sourceId && r.LinkedFor == AuthContext.CurrentAccount.ID)
                .SingleOrDefault();

            FilesDbContext.FilesLink.Remove(link);
            FilesDbContext.SaveChanges();
        }

        public void DeleteAllLink(string fileId)
        {
            var link = FilesDbContext.FilesLink.Where(r => r.TenantId == TenantID && (r.SourceId == fileId || r.LinkedId == fileId));

            FilesDbContext.FilesLink.RemoveRange(link);
            FilesDbContext.SaveChanges();
        }
    }
}