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

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Api.Utils;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.Dao;
using ASC.MessagingSystem;
using ASC.Projects;
using ASC.Projects.Classes;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Engine;
using ASC.Projects.Model;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class SettingsController : BaseProjectController
    {
        public SettingsController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager)
        {
        }

        [Update(@"settings")]
        public ProjectsCommonSettings UpdateSettings(bool? everebodyCanCreate,
            bool? hideEntitiesInPausedProjects,
            StartModuleType? startModule,
            object folderId)
        {
            if (everebodyCanCreate.HasValue || hideEntitiesInPausedProjects.HasValue)
            {
                if (!ProjectSecurity.CurrentUserAdministrator) ProjectSecurity.CreateSecurityException();

                var settings = SettingsManager.Load<ProjectsCommonSettings>();

                if (everebodyCanCreate.HasValue)
                {
                    settings.EverebodyCanCreate = everebodyCanCreate.Value;
                }

                if (hideEntitiesInPausedProjects.HasValue)
                {
                    settings.HideEntitiesInPausedProjects = hideEntitiesInPausedProjects.Value;
                }

                SettingsManager.Save(settings);
                return settings;
            }

            if (startModule.HasValue || folderId != null)
            {
                if (!ProjectSecurity.IsProjectsEnabled(SecurityContext.CurrentAccount.ID)) ProjectSecurity.CreateSecurityException();
                var settings = SettingsManager.LoadForCurrentUser<ProjectsCommonSettings>();
                if (startModule.HasValue)
                {
                    settings.StartModuleType = startModule.Value;
                }

                if (folderId != null)
                {
                    settings.FolderId = folderId;
                }
                SettingsManager.SaveForCurrentUser(settings);
                return settings;
            }

            return null;
        }

        [Read(@"settings")]
        public ProjectsCommonSettings GetSettings()
        {
            var commonSettings = SettingsManager.Load<ProjectsCommonSettings>();
            var userSettings = SettingsManager.LoadForCurrentUser<ProjectsCommonSettings>();

            return new ProjectsCommonSettings
            {
                EverebodyCanCreate = commonSettings.EverebodyCanCreate,
                HideEntitiesInPausedProjects = commonSettings.HideEntitiesInPausedProjects,
                StartModuleType = userSettings.StartModuleType,
                FolderId = userSettings.FolderId,
            };
        }


        [Create(@"status")]
        public CustomTaskStatus CreateStatus(CustomTaskStatus status)
        {
            return EngineFactory.GetStatusEngine().Create(status);
        }

        [Update(@"status")]
        public CustomTaskStatus UpdateStatus(CustomTaskStatus newStatus)
        {
            if (newStatus.IsDefault && !EngineFactory.GetStatusEngine().Get().Any(r => r.IsDefault && r.StatusType == newStatus.StatusType))
            {
                return CreateStatus(newStatus);
            }

            var status = EngineFactory.GetStatusEngine().Get().FirstOrDefault(r => r.Id == newStatus.Id).NotFoundIfNull();

            status.Title = Update.IfNotEmptyAndNotEquals(status.Title, newStatus.Title);
            status.Description = Update.IfNotEmptyAndNotEquals(status.Description, newStatus.Description);
            status.Color = Update.IfNotEmptyAndNotEquals(status.Color, newStatus.Color);
            status.Image = Update.IfNotEmptyAndNotEquals(status.Image, newStatus.Image);
            status.ImageType = Update.IfNotEmptyAndNotEquals(status.ImageType, newStatus.ImageType);
            status.Order = Update.IfNotEmptyAndNotEquals(status.Order, newStatus.Order);
            status.StatusType = Update.IfNotEmptyAndNotEquals(status.StatusType, newStatus.StatusType);
            status.Available = Update.IfNotEmptyAndNotEquals(status.Available, newStatus.Available);

            EngineFactory.GetStatusEngine().Update(status);

            return status;
        }

        [Update(@"statuses")]
        public List<CustomTaskStatus> UpdateStatuses(List<CustomTaskStatus> statuses)
        {
            foreach (var status in statuses)
            {
                UpdateStatus(status);
            }

            return statuses;
        }

        [Read(@"status")]
        public List<CustomTaskStatus> GetStatuses()
        {
            return EngineFactory.GetStatusEngine().GetWithDefaults();
        }

        [Delete(@"status/{id}")]
        public CustomTaskStatus DeleteStatus(int id)
        {
            var status = EngineFactory.GetStatusEngine().Get().FirstOrDefault(r => r.Id == id).NotFoundIfNull();
            EngineFactory.GetStatusEngine().Delete(status.Id);
            return status;
        }
    }
}