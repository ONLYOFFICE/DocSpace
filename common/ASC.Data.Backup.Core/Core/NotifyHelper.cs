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

    public async Task SendAboutTransferStartAsync(Tenant tenant, string targetRegion, bool notifyUsers)
    {
        await MigrationNotifyAsync(tenant, Actions.MigrationPortalStart, targetRegion, string.Empty, notifyUsers);
    }

    public async Task SendAboutTransferCompleteAsync(Tenant tenant, string targetRegion, string targetAddress, bool notifyOnlyOwner, int toTenantId)
    {
        await MigrationNotifyAsync(tenant, Actions.MigrationPortalSuccessV115, targetRegion, targetAddress, !notifyOnlyOwner, toTenantId);
    }

    public async Task SendAboutTransferErrorAsync(Tenant tenant, string targetRegion, string resultAddress, bool notifyOnlyOwner)
    {
        await MigrationNotifyAsync(tenant, !string.IsNullOrEmpty(targetRegion) ? Actions.MigrationPortalError : Actions.MigrationPortalServerFailure, targetRegion, resultAddress, !notifyOnlyOwner);
    }

    public async Task SendAboutBackupCompletedAsync(int tenantId, Guid userId)
    {
        await _tenantManager.SetCurrentTenantAsync(tenantId);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        await client.SendNoticeToAsync(
            Actions.BackupCreated,
            new[] { await _studioNotifyHelper.ToRecipientAsync(userId) },
            new[] { StudioNotifyService.EMailSenderName },
            new TagValue(Tags.OwnerName, (await _userManager.GetUsersAsync(userId)).DisplayUserName(_displayUserSettingsHelper)));
    }

    public async Task SendAboutRestoreStartedAsync(Tenant tenant, bool notifyAllUsers)
    {
        await _tenantManager.SetCurrentTenantAsync(tenant.Id);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        var owner = await _userManager.GetUsersAsync(tenant.OwnerId);
        var users =
            notifyAllUsers
                ? await _studioNotifyHelper.RecipientFromEmailAsync((await _userManager.GetUsersAsync(EmployeeStatus.Active)).Where(r => r.ActivationStatus == EmployeeActivationStatus.Activated).Select(u => u.Email).ToList(), false)
                : owner.ActivationStatus == EmployeeActivationStatus.Activated ? await _studioNotifyHelper.RecipientFromEmailAsync(owner.Email, false) : new IDirectRecipient[0];

        await client.SendNoticeToAsync(
            Actions.RestoreStarted,
            users,
            new[] { StudioNotifyService.EMailSenderName });
    }

    public async Task SendAboutRestoreCompletedAsync(Tenant tenant, bool notifyAllUsers)
    {
        _tenantManager.SetCurrentTenant(tenant);
        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        var users = notifyAllUsers
            ? await _userManager.GetUsersAsync(EmployeeStatus.Active)
            : new[] { await _userManager.GetUsersAsync((await _tenantManager.GetCurrentTenantAsync()).OwnerId) };

        foreach (var user in users)
        {
            var hash = (await _authManager.GetUserPasswordStampAsync(user.Id)).ToString("s");
            var confirmationUrl = await _commonLinkUtility.GetConfirmationEmailUrlAsync(user.Email, ConfirmType.PasswordChange, hash, user.Id);

            var greenButtonText = BackupResource.ResourceManager.GetString("ButtonSetPassword", GetCulture(user));

            await client.SendNoticeToAsync(
                Actions.RestoreCompletedV115,
                new IRecipient[] { user },
                new[] { StudioNotifyService.EMailSenderName },
                TagValues.GreenButton(greenButtonText, confirmationUrl));
        }
    }

    private async Task MigrationNotifyAsync(Tenant tenant, INotifyAction action, string region, string url, bool notify, int? toTenantId = null)
    {
        _tenantManager.SetCurrentTenant(tenant);

        var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifySource);

        var users = (await _userManager.GetUsersAsync())
            .Where(u => notify ? u.ActivationStatus.HasFlag(EmployeeActivationStatus.Activated) : u.IsOwner(tenant))
            .ToArray();

        if (users.Length > 0)
        {
            var args = await CreateArgsAsync(region, url);
            if (action == Actions.MigrationPortalSuccessV115)
            {
                foreach (var user in users)
                {
                    var currentArgs = new List<ITagValue>(args);

                    var newTenantId = toTenantId.HasValue ? toTenantId.Value : tenant.Id;
                    var hash = (await _authManager.GetUserPasswordStampAsync(user.Id)).ToString("s");
                    var confirmationUrl = url + "/" + _commonLinkUtility.GetConfirmationUrlRelative(newTenantId, user.Email, ConfirmType.PasswordChange, hash, user.Id);

                    var greenButtonText = BackupResource.ResourceManager.GetString("ButtonSetPassword", GetCulture(user));
                    currentArgs.Add(TagValues.GreenButton(greenButtonText, confirmationUrl));

                    await client.SendNoticeToAsync(
                        action,
                        null,
                        new IRecipient[] { user },
                        new[] { StudioNotifyService.EMailSenderName },
                        currentArgs.ToArray());
                }
            }
            else
            {
                await client.SendNoticeToAsync(
                    action,
                    null,
                    await users.ToAsyncEnumerable().SelectAwait(async u => await _studioNotifyHelper.ToRecipientAsync(u.Id)).ToArrayAsync(),
                    new[] { StudioNotifyService.EMailSenderName },
                    args.ToArray());
            }
        }
    }

    private async Task<List<ITagValue>> CreateArgsAsync(string region, string url)
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

            var attachment = await _tenantLogoManager.GetMailLogoAsAttacmentAsync();

            if (attachment != null)
            {
                args.Add(new TagValue(CommonTags.LetterLogo, "cid:" + attachment.ContentId));
                args.Add(new TagValue(CommonTags.EmbeddedAttachments, new[] { attachment }));
            }
        }

        return args;
    }

    private CultureInfo GetCulture(UserInfo user)
    {
        CultureInfo culture = null;

        if (!string.IsNullOrEmpty(user.CultureName))
        {
            culture = user.GetCulture();
        }

        if (culture == null)
        {
            culture = _tenantManager.GetCurrentTenant(false)?.GetCulture();
        }

        return culture;
    }
}
