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
using ASC.Projects.Model.Settings;
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
        public ProjectsCommonSettings UpdateSettings(ModelUpdateSettings model)
        {
            if (model.EverebodyCanCreate.HasValue || model.HideEntitiesInPausedProjects.HasValue)
            {
                if (!ProjectSecurity.CurrentUserAdministrator) ProjectSecurity.CreateSecurityException();

                var settings = SettingsManager.Load<ProjectsCommonSettings>();

                if (model.EverebodyCanCreate.HasValue)
                {
                    settings.EverebodyCanCreate = model.EverebodyCanCreate.Value;
                }

                if (model.HideEntitiesInPausedProjects.HasValue)
                {
                    settings.HideEntitiesInPausedProjects = model.HideEntitiesInPausedProjects.Value;
                }

                SettingsManager.Save(settings);
                return settings;
            }

            if (model.StartModule.HasValue || model.FolderId != null)
            {
                if (!ProjectSecurity.IsProjectsEnabled(SecurityContext.CurrentAccount.ID)) ProjectSecurity.CreateSecurityException();
                var settings = SettingsManager.LoadForCurrentUser<ProjectsCommonSettings>();
                if (model.StartModule.HasValue)
                {
                    settings.StartModuleType = model.StartModule.Value;
                }

                if (model.FolderId != null)
                {
                    settings.FolderId = model.FolderId;
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
        public CustomTaskStatus CreateStatus(ModelCreateStatus model)
        {
            return EngineFactory.GetStatusEngine().Create(model.Status);
        }

        [Update(@"status")]
        public CustomTaskStatus UpdateStatus(ModelUpdateStatus model)
        {
            if (model.NewStatus.IsDefault && !EngineFactory.GetStatusEngine().Get().Any(r => r.IsDefault && r.StatusType == model.NewStatus.StatusType))
            {
                return EngineFactory.GetStatusEngine().Create(model.NewStatus);
            }

            var status = EngineFactory.GetStatusEngine().Get().FirstOrDefault(r => r.Id == model.NewStatus.Id).NotFoundIfNull();

            status.Title = Update.IfNotEmptyAndNotEquals(status.Title, model.NewStatus.Title);
            status.Description = Update.IfNotEmptyAndNotEquals(status.Description, model.NewStatus.Description);
            status.Color = Update.IfNotEmptyAndNotEquals(status.Color, model.NewStatus.Color);
            status.Image = Update.IfNotEmptyAndNotEquals(status.Image, model.NewStatus.Image);
            status.ImageType = Update.IfNotEmptyAndNotEquals(status.ImageType, model.NewStatus.ImageType);
            status.Order = Update.IfNotEmptyAndNotEquals(status.Order, model.NewStatus.Order);
            status.StatusType = Update.IfNotEmptyAndNotEquals(status.StatusType, model.NewStatus.StatusType);
            status.Available = Update.IfNotEmptyAndNotEquals(status.Available, model.NewStatus.Available);

            EngineFactory.GetStatusEngine().Update(status);

            return status;
        }

        [Update(@"statuses")]
        public List<CustomTaskStatus> UpdateStatuses(ModelUpdateStatuses model)
        {
            foreach (var status in model.Statuses)
            {
                UpdateStatus(new ModelUpdateStatus() { NewStatus = status});
            }

            return model.Statuses;
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