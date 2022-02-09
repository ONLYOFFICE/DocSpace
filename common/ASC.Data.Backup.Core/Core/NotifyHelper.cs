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
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Backup.Core;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Backup
{
    [Singletone(Additional = typeof(NotifyHelperExtension))]
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

        public void SendAboutTransferComplete(Tenant tenant, string targetRegion, string targetAddress, bool notifyOnlyOwner, int toTenantId)
        {
            MigrationNotify(tenant, Actions.MigrationPortalSuccessV115, targetRegion, targetAddress, !notifyOnlyOwner, toTenantId);
        }

        public void SendAboutTransferError(Tenant tenant, string targetRegion, string resultAddress, bool notifyOnlyOwner)
        {
            MigrationNotify(tenant, !string.IsNullOrEmpty(targetRegion) ? Actions.MigrationPortalError : Actions.MigrationPortalServerFailure, targetRegion, resultAddress, !notifyOnlyOwner);
        }

        public void SendAboutBackupCompleted(int tenantId, Guid userId)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var (userManager, studioNotifyHelper, studioNotifySource, displayUserSettingsHelper, tenantManager, _) = scopeClass;
            tenantManager.SetCurrentTenant(tenantId);

            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifySource, scope);

            client.SendNoticeToAsync(
                Actions.BackupCreated,
                new[] { studioNotifyHelper.ToRecipient(userId) },
                new[] { StudioNotifyService.EMailSenderName },
                new TagValue(Tags.OwnerName, userManager.GetUsers(userId).DisplayUserName(displayUserSettingsHelper)));
        }

        public void SendAboutRestoreStarted(Tenant tenant, bool notifyAllUsers)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var (userManager, studioNotifyHelper, studioNotifySource, displayUserSettingsHelper, tenantManager, _) = scopeClass;
            tenantManager.SetCurrentTenant(tenant.TenantId);

            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifySource, scope);

            var owner = userManager.GetUsers(tenant.OwnerId);
            var users =
                notifyAllUsers
                    ? studioNotifyHelper.RecipientFromEmail(userManager.GetUsers(EmployeeStatus.Active).Where(r => r.ActivationStatus == EmployeeActivationStatus.Activated).Select(u => u.Email).ToList(), false)
                    : owner.ActivationStatus == EmployeeActivationStatus.Activated ? studioNotifyHelper.RecipientFromEmail(owner.Email, false) : new IDirectRecipient[0];

            client.SendNoticeToAsync(
                Actions.RestoreStarted,
                users,
                new[] { StudioNotifyService.EMailSenderName });
        }

        public void SendAboutRestoreCompleted(Tenant tenant, bool notifyAllUsers)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();
            var (userManager, _, studioNotifySource, _, _, authManager) = scopeClass;
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifySource, scope);

            var users = notifyAllUsers
                ? userManager.GetUsers(EmployeeStatus.Active)
                : new[] { userManager.GetUsers(tenantManager.GetCurrentTenant().OwnerId) };

            foreach (var user in users)
            {
                var hash = authManager.GetUserPasswordStamp(user.ID).ToString("s");
                var confirmationUrl = commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PasswordChange, hash);

                Func<string> greenButtonText = () => BackupResource.ButtonSetPassword;

                client.SendNoticeToAsync(
                    Actions.RestoreCompletedV115,
                    new IRecipient[] { user },
                    new[] { StudioNotifyService.EMailSenderName },
                    null,
                    TagValues.GreenButton(greenButtonText, confirmationUrl));
            }
        }

        private void MigrationNotify(Tenant tenant, INotifyAction action, string region, string url, bool notify, int? toTenantId = null)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
            var (userManager, studioNotifyHelper, studioNotifySource, _, _, authManager) = scopeClass;
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifySource, scope);
            var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();

            var users = userManager.GetUsers()
                .Where(u => notify ? u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) : u.IsOwner(tenant))
                .ToArray();

            if (users.Length > 0)
            {
                var args = CreateArgs(scope, region, url);
                if (action == Actions.MigrationPortalSuccessV115)
                {
                    foreach (var user in users)
                    {
                        var currentArgs = new List<ITagValue>(args);

                        var newTenantId = toTenantId.HasValue ? toTenantId.Value : tenant.TenantId;
                        var hash = authManager.GetUserPasswordStamp(user.ID).ToString("s");
                        var confirmationUrl = url + "/" + commonLinkUtility.GetConfirmationUrlRelative(newTenantId, user.Email, ConfirmType.PasswordChange, hash);

                        Func<string> greenButtonText = () => BackupResource.ButtonSetPassword;
                        currentArgs.Add(TagValues.GreenButton(greenButtonText, confirmationUrl));

                        client.SendNoticeToAsync(
                            action,
                            null,
                            new IRecipient[] { user },
                            new[] { StudioNotifyService.EMailSenderName },
                            currentArgs.ToArray());
                    }
                }
                else
                {
                    client.SendNoticeToAsync(
                        action,
                        null,
                        users.Select(u => studioNotifyHelper.ToRecipient(u.ID)).ToArray(),
                        new[] { StudioNotifyService.EMailSenderName },
                        args.ToArray());
                }
            }
        }

        private List<ITagValue> CreateArgs(IServiceScope scope,string region, string url)
        {
            var args = new List<ITagValue>()
                    {
                        new TagValue(Tags.RegionName, TransferResourceHelper.GetRegionDescription(region)),
                        new TagValue(Tags.PortalUrl, url)
                    };

            if (!string.IsNullOrEmpty(url))
            {
                args.Add(new TagValue(CommonTags.VirtualRootPath, url));
                args.Add(new TagValue(CommonTags.ProfileUrl, url + scope.ServiceProvider.GetService<CommonLinkUtility>().GetMyStaff()));
                args.Add(new TagValue(CommonTags.LetterLogo, scope.ServiceProvider.GetService<TenantLogoManager>().GetLogoDark(true)));
            }
            return args;
        }
    }

    [Scope]
    public class NotifyHelperScope
    {
        private AuthManager AuthManager { get; }
        private UserManager UserManager { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private StudioNotifySource StudioNotifySource { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private TenantManager TenantManager { get; }

        public NotifyHelperScope(
            UserManager userManager,
            StudioNotifyHelper studioNotifyHelper,
            StudioNotifySource studioNotifySource,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantManager tenantManager, 
            AuthManager authManager)
        {
            UserManager = userManager;
            StudioNotifyHelper = studioNotifyHelper;
            StudioNotifySource = studioNotifySource;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            TenantManager = tenantManager;
            AuthManager = authManager;
        }

        public void Deconstruct(
            out UserManager userManager,
            out StudioNotifyHelper studioNotifyHelper,
            out StudioNotifySource studioNotifySource,
            out DisplayUserSettingsHelper displayUserSettingsHelper,
            out TenantManager tenantManager,
            out AuthManager authManager)
        {
            userManager = UserManager;
            studioNotifyHelper = StudioNotifyHelper;
            studioNotifySource = StudioNotifySource;
            displayUserSettingsHelper = DisplayUserSettingsHelper;
            tenantManager = TenantManager;
            authManager = AuthManager;
        }
    }

    public static class NotifyHelperExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<NotifyHelperScope>();
        }
    }
}
