// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using ASC.Notify.Engine;

namespace ASC.Data.Backup;

[Singletone(Additional = typeof(NotifyHelperExtension))]
public class NotifyHelper
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly NotifyEngine _notifyEngine;
    private readonly WorkContext _workContext;

    public NotifyHelper(IServiceScopeFactory serviceScopeFactory, NotifyEngine notifyEngine, WorkContext workContext)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _notifyEngine = notifyEngine;
        _workContext = workContext;
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
        using var scope = _serviceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
        var (userManager, studioNotifyHelper, studioNotifySource, displayUserSettingsHelper, tenantManager, _) = scopeClass;
        tenantManager.SetCurrentTenant(tenantId);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngine, studioNotifySource, scope);

        client.SendNoticeToAsync(
            Actions.BackupCreated,
            new[] { studioNotifyHelper.ToRecipient(userId) },
            new[] { StudioNotifyService.EMailSenderName },
            new TagValue(Tags.OwnerName, userManager.GetUsers(userId).DisplayUserName(displayUserSettingsHelper)));
    }

    public void SendAboutRestoreStarted(Tenant tenant, bool notifyAllUsers)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
        var (userManager, studioNotifyHelper, studioNotifySource, displayUserSettingsHelper, tenantManager, _) = scopeClass;
        tenantManager.SetCurrentTenant(tenant.Id);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngine, studioNotifySource, scope);

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
        using var scope = _serviceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();
        var (userManager, _, studioNotifySource, _, _, authManager) = scopeClass;
        var client = _workContext.NotifyContext.RegisterClient(_notifyEngine, studioNotifySource, scope);

        var users = notifyAllUsers
            ? userManager.GetUsers(EmployeeStatus.Active)
            : new[] { userManager.GetUsers(tenantManager.GetCurrentTenant().OwnerId) };

        foreach (var user in users)
        {
            var hash = authManager.GetUserPasswordStamp(user.Id).ToString("s");
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
        using var scope = _serviceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyHelperScope>();
        var (userManager, studioNotifyHelper, studioNotifySource, _, _, authManager) = scopeClass;
        var client = _workContext.NotifyContext.RegisterClient(_notifyEngine, studioNotifySource, scope);
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

                    var newTenantId = toTenantId.HasValue ? toTenantId.Value : tenant.Id;
                    var hash = authManager.GetUserPasswordStamp(user.Id).ToString("s");
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
                    users.Select(u => studioNotifyHelper.ToRecipient(u.Id)).ToArray(),
                    new[] { StudioNotifyService.EMailSenderName },
                    args.ToArray());
            }
        }
    }

    private List<ITagValue> CreateArgs(IServiceScope scope, string region, string url)
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
    private readonly AuthManager _authManager;
    private readonly UserManager _userManager;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly StudioNotifySource _studioNotifySource;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private TenantManager TenantManager { get; }

    public NotifyHelperScope(
        UserManager userManager,
        StudioNotifyHelper studioNotifyHelper,
        StudioNotifySource studioNotifySource,
        DisplayUserSettingsHelper displayUserSettingsHelper,
            TenantManager tenantManager,
        AuthManager authManager)
    {
        _userManager = userManager;
        _studioNotifyHelper = studioNotifyHelper;
        _studioNotifySource = studioNotifySource;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        TenantManager = tenantManager;
        _authManager = authManager;
    }

    public void Deconstruct(
        out UserManager userManager,
        out StudioNotifyHelper studioNotifyHelper,
        out StudioNotifySource studioNotifySource,
        out DisplayUserSettingsHelper displayUserSettingsHelper,
            out TenantManager tenantManager,
            out AuthManager authManager)
    {
        userManager = _userManager;
        studioNotifyHelper = _studioNotifyHelper;
        studioNotifySource = _studioNotifySource;
        displayUserSettingsHelper = _displayUserSettingsHelper;
        tenantManager = TenantManager;
        authManager = _authManager;
    }
}

public static class NotifyHelperExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<NotifyHelperScope>();
    }
}
