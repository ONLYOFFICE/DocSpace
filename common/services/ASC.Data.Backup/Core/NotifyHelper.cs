/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Backup
{
    public class NotifyHelper
    {
        private IServiceProvider ServiceProvider { get; }

        public NotifyHelper(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void SendAboutTransferStart(Tenant tenant, string targetRegion, bool notifyUsers)
        {
            MigrationNotify(tenant, Actions.MigrationPortalStart, targetRegion, string.Empty, notifyUsers);
        }

        public void SendAboutTransferComplete(Tenant tenant, string targetRegion, string targetAddress, bool notifyOnlyOwner)
        {
            MigrationNotify(tenant, Actions.MigrationPortalSuccess, targetRegion, targetAddress, !notifyOnlyOwner);
        }

        public void SendAboutTransferError(Tenant tenant, string targetRegion, string resultAddress, bool notifyOnlyOwner)
        {
            MigrationNotify(tenant, !string.IsNullOrEmpty(targetRegion) ? Actions.MigrationPortalError : Actions.MigrationPortalServerFailure, targetRegion, resultAddress, !notifyOnlyOwner);
        }

        public void SendAboutBackupCompleted(Guid userId)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(scopeClass.StudioNotifySource, scope);

            client.SendNoticeToAsync(
                Actions.BackupCreated,
                new[] { scopeClass.StudioNotifyHelper.ToRecipient(userId) },
                new[] { StudioNotifyService.EMailSenderName },
                new TagValue(Tags.OwnerName, scopeClass.UserManager.GetUsers(userId).DisplayUserName(scopeClass.DisplayUserSettingsHelper)));
        }

        public void SendAboutRestoreStarted(Tenant tenant, bool notifyAllUsers)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(scopeClass.StudioNotifySource, scope);

            var owner = scopeClass.UserManager.GetUsers(tenant.OwnerId);
            var users =
                notifyAllUsers
                    ? scopeClass.StudioNotifyHelper.RecipientFromEmail(scopeClass.UserManager.GetUsers(EmployeeStatus.Active).Where(r => r.ActivationStatus == EmployeeActivationStatus.Activated).Select(u => u.Email).ToList(), false)
                    : owner.ActivationStatus == EmployeeActivationStatus.Activated ? scopeClass.StudioNotifyHelper.RecipientFromEmail(owner.Email, false) : new IDirectRecipient[0];

            client.SendNoticeToAsync(
                Actions.RestoreStarted,
                users,
                new[] { StudioNotifyService.EMailSenderName });
        }

        public void SendAboutRestoreCompleted(Tenant tenant, bool notifyAllUsers)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(scopeClass.StudioNotifySource, scope);

            var owner = scopeClass.UserManager.GetUsers(tenant.OwnerId);

            var users =
                notifyAllUsers
                    ? scopeClass.UserManager.GetUsers(EmployeeStatus.Active).Select(u => scopeClass.StudioNotifyHelper.ToRecipient(u.ID)).ToArray()
                    : new[] { scopeClass.StudioNotifyHelper.ToRecipient(owner.ID) };

            client.SendNoticeToAsync(
                Actions.RestoreCompleted,
                users,
                new[] { StudioNotifyService.EMailSenderName },
                new TagValue(Tags.OwnerName, owner.DisplayUserName(scopeClass.DisplayUserSettingsHelper)));
        }

        private void MigrationNotify(Tenant tenant, INotifyAction action, string region, string url, bool notify)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(scopeClass.StudioNotifySource, scope);

            var users = scopeClass.UserManager.GetUsers()
                .Where(u => notify ? u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) : u.IsOwner(tenant))
                .Select(u => scopeClass.StudioNotifyHelper.ToRecipient(u.ID))
                .ToArray();

            if (users.Any())
            {
                client.SendNoticeToAsync(
                    action,
                    users,
                    new[] { StudioNotifyService.EMailSenderName },
                    new TagValue(Tags.RegionName, TransferResourceHelper.GetRegionDescription(region)),
                    new TagValue(Tags.PortalUrl, url));
            }
        }
    }

    public class NotifyHelperScope
    {
        internal UserManager UserManager { get; }
        internal StudioNotifyHelper StudioNotifyHelper { get; }
        internal StudioNotifySource StudioNotifySource { get; }
        internal DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }

        public NotifyHelperScope(UserManager userManager, StudioNotifyHelper studioNotifyHelper, StudioNotifySource studioNotifySource, DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            UserManager = userManager;
            StudioNotifyHelper = studioNotifyHelper;
            StudioNotifySource = studioNotifySource;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
        }
    }

    public static class NotifyHelperExtension
    {
        public static DIHelper AddNotifyHelperService(this DIHelper services)
        {
            services.TryAddSingleton<NotifyHelper>();
            services.TryAddScoped<NotifyHelperScope>();

            return services
                .AddNotifyConfiguration()
                .AddStudioNotifySourceService()
                .AddUserManagerService()
                .AddStudioNotifyHelperService()
                .AddDisplayUserSettingsService();
        }
    }
}
