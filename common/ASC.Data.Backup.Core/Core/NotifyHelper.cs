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

[Scope]
public class NotifyHelper
{
    private readonly AuthManager _authManager;
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly WorkContext _workContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly TenantLogoManager _tenantLogoManager;
    private readonly UserManager _userManager;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly StudioNotifySource _studioNotifySource;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly TenantManager _tenantManager;

    public NotifyHelper(
        UserManager userManager,
        StudioNotifyHelper studioNotifyHelper,
        StudioNotifySource studioNotifySource,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        TenantManager tenantManager,
        AuthManager authManager,
        NotifyEngineQueue notifyEngineQueue,
        WorkContext workContext,
        CommonLinkUtility commonLinkUtility,
        TenantLogoManager tenantLogoManager)
    {
        _userManager = userManager;
        _studioNotifyHelper = studioNotifyHelper;
        _studioNotifySource = studioNotifySource;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _tenantManager = tenantManager;
        _authManager = authManager;
        _notifyEngineQueue = notifyEngineQueue;
        _workContext = workContext;
        _commonLinkUtility = commonLinkUtility;
        _tenantLogoManager = tenantLogoManager;
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
        _tenantManager.SetCurrentTenant(tenantId);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        client.SendNoticeToAsync(
            Actions.BackupCreated,
            new[] { _studioNotifyHelper.ToRecipient(userId) },
            new[] { StudioNotifyService.EMailSenderName },
            new TagValue(Tags.OwnerName, _userManager.GetUsers(userId).DisplayUserName(_displayUserSettingsHelper)));
    }

    public void SendAboutRestoreStarted(Tenant tenant, bool notifyAllUsers)
    {
        _tenantManager.SetCurrentTenant(tenant.Id);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        var owner = _userManager.GetUsers(tenant.OwnerId);
        var users =
            notifyAllUsers
                ? _studioNotifyHelper.RecipientFromEmail(_userManager.GetUsers(EmployeeStatus.Active).Where(r => r.ActivationStatus == EmployeeActivationStatus.Activated).Select(u => u.Email).ToList(), false)
                : owner.ActivationStatus == EmployeeActivationStatus.Activated ? _studioNotifyHelper.RecipientFromEmail(owner.Email, false) : new IDirectRecipient[0];

        client.SendNoticeToAsync(
            Actions.RestoreStarted,
            users,
            new[] { StudioNotifyService.EMailSenderName });
    }

    public void SendAboutRestoreCompleted(Tenant tenant, bool notifyAllUsers)
    {
        _tenantManager.SetCurrentTenant(tenant);
        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        var users = notifyAllUsers
            ? _userManager.GetUsers(EmployeeStatus.Active)
            : new[] { _userManager.GetUsers(_tenantManager.GetCurrentTenant().OwnerId) };

        foreach (var user in users)
        {
            var hash = _authManager.GetUserPasswordStamp(user.Id).ToString("s");
            var confirmationUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.PasswordChange, hash);

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
        _tenantManager.SetCurrentTenant(tenant);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        var users = _userManager.GetUsers()
            .Where(u => notify ? u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) : u.IsOwner(tenant))
            .ToArray();

        if (users.Length > 0)
        {
            var args = CreateArgs(region, url);
            if (action == Actions.MigrationPortalSuccessV115)
            {
                foreach (var user in users)
                {
                    var currentArgs = new List<ITagValue>(args);

                    var newTenantId = toTenantId.HasValue ? toTenantId.Value : tenant.Id;
                    var hash = _authManager.GetUserPasswordStamp(user.Id).ToString("s");
                    var confirmationUrl = url + "/" + _commonLinkUtility.GetConfirmationUrlRelative(newTenantId, user.Email, ConfirmType.PasswordChange, hash);

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
                    users.Select(u => _studioNotifyHelper.ToRecipient(u.Id)).ToArray(),
                    new[] { StudioNotifyService.EMailSenderName },
                    args.ToArray());
            }
        }
    }

    private List<ITagValue> CreateArgs(string region, string url)
    {
        var args = new List<ITagValue>()
                    {
                        new TagValue(Tags.RegionName, TransferResourceHelper.GetRegionDescription(region)),
                        new TagValue(Tags.PortalUrl, url)
                    };

        if (!string.IsNullOrEmpty(url))
        {
            args.Add(new TagValue(CommonTags.VirtualRootPath, url));
            args.Add(new TagValue(CommonTags.ProfileUrl, url + _commonLinkUtility.GetMyStaff()));
            args.Add(new TagValue(CommonTags.LetterLogo, _tenantLogoManager.GetLogoDark(true)));
        }

        return args;
    }
}
