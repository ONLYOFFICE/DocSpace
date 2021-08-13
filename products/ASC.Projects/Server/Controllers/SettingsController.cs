/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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
using ASC.Web.Core.Utility;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Api.Projects
{
    public class SettingsController : BaseProjectController
    {
        public SettingsController(SecurityContext securityContext, ProjectSecurity projectSecurity, ApiContext context, EngineFactory engineFactory, TenantUtil tenantUtil, DisplayUserSettingsHelper displayUserSettingsHelper, CommonLinkUtility commonLinkUtility, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, ModelHelper modelHelper, FileWrapperHelper fileWrapperHelper, IOptionsMonitor<ILog> options, DaoFactory factory, ReportHelper reportHelper, DocbuilderReportsUtility docbuilderReportsUtility, IHttpContextAccessor httpContextAccessor, TenantManager tenantManager, ReportTemplateHelper reportTemplateHelper, FilesLinkUtility filesLinkUtility, CustomStatusHelper customStatusHelper, IServiceProvider serviceProvider, SettingsManager settingsManager, HtmlUtility htmlUtility, NotifyConfiguration notifyConfiguration) : base(securityContext, projectSecurity, context, engineFactory, tenantUtil, displayUserSettingsHelper, commonLinkUtility, userPhotoManager, messageService, messageTarget, modelHelper, fileWrapperHelper, options, factory, reportHelper, docbuilderReportsUtility, httpContextAccessor, tenantManager, reportTemplateHelper, filesLinkUtility, customStatusHelper, serviceProvider, settingsManager, htmlUtility, notifyConfiguration)
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