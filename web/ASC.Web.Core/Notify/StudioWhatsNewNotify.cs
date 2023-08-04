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

namespace ASC.Web.Studio.Core.Notify;

[Scope]
public class StudioWhatsNewNotify
{
    private readonly ILogger _log;
    private readonly WebItemManager _webItemManager;
    private readonly TenantManager _tenantManager;
    private readonly ITariffService _tariffService;
    private readonly TenantUtil _tenantUtil;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly AuthManager _authManager;
    private readonly AuditEventsRepository _auditEventsRepository;
    private readonly CoreSettings _coreSettings;
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly IConfiguration _confuguration;
    private readonly WorkContext _workContext;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;

    public static readonly List<MessageAction?> DailyActions = new List<MessageAction?>()
    {
        MessageAction.FileCreated,
        MessageAction.FileUpdatedRevisionComment,
        MessageAction.RoomCreated,
        MessageAction.RoomRemoveUser,
        MessageAction.RoomRenamed,
        MessageAction.RoomArchived,
        MessageAction.UserCreated,
        MessageAction.UserUpdated
    };

    public static readonly List<MessageAction?> RoomsActivityActions = new List<MessageAction?>()
    {
         MessageAction.FileUploaded,
         MessageAction.UserFileUpdated,
         MessageAction.RoomCreateUser,
         MessageAction.RoomUpdateAccessForUser,
         MessageAction.UsersUpdatedType
    };

    public StudioWhatsNewNotify(
        TenantManager tenantManager,
        ITariffService tariffService,
        TenantUtil tenantUtil,
        StudioNotifyHelper studioNotifyHelper,
        UserManager userManager,
        SecurityContext securityContext,
        AuthManager authManager,
        CoreSettings coreSettings,
        NotifyEngineQueue notifyEngineQueue,
        IConfiguration confuguration,
        WorkContext workContext,
        ILoggerProvider optionsMonitor,
        AuditEventsRepository auditEventsRepository,
        WebItemManager webItemManager,
        DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        _webItemManager = webItemManager;
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _tenantUtil = tenantUtil;
        _studioNotifyHelper = studioNotifyHelper;
        _userManager = userManager;
        _securityContext = securityContext;
        _authManager = authManager;
        _coreSettings = coreSettings;
        _notifyEngineQueue = notifyEngineQueue;
        _confuguration = confuguration;
        _workContext = workContext;
        _auditEventsRepository = auditEventsRepository;
        _log = optionsMonitor.CreateLogger("ASC.Notify");
        _displayUserSettingsHelper = displayUserSettingsHelper;

    }

    public async Task SendMsgWhatsNewAsync(DateTime scheduleDate, WhatsNewType whatsNewType)
    {
        var products = _webItemManager.GetItemsAll<IProduct>();

        if (_webItemManager.GetItemsAll<IProduct>().Count == 0)
        {
            _log.InformationNoProducts();
            return;
        }

        _log.InformationStartSendWhatsNew();

        var tenants = await GetChangedTenantsAsync(scheduleDate, whatsNewType);

        foreach (var tenantid in tenants)
        {
            await SendMsgWhatsNewAsync(tenantid, scheduleDate, whatsNewType, products);
        }
    }

    private async Task<IEnumerable<int>> GetChangedTenantsAsync(DateTime date, WhatsNewType whatsNewType)
    {
        switch (whatsNewType)
        {
            case WhatsNewType.DailyFeed:
                return await _auditEventsRepository.GetTenantsAsync(date.Date.AddDays(-1), date.Date.AddSeconds(-1));
            case WhatsNewType.RoomsActivity:
                return await _auditEventsRepository.GetTenantsAsync(date.AddHours(-1), date.AddSeconds(-1));
            default:
                return Enumerable.Empty<int>();
        }
    }

    private async Task SendMsgWhatsNewAsync(int tenantid, DateTime scheduleDate, WhatsNewType whatsNewType, List<IProduct> products)
    {
        try
        {
            var tenant = await _tenantManager.GetTenantAsync(tenantid);
            if (tenant == null ||
                tenant.Status != TenantStatus.Active ||
                !TimeToSendWhatsNew(_tenantUtil.DateTimeFromUtc(tenant.TimeZone, scheduleDate), whatsNewType) || //ToDo
                TariffState.NotPaid <= (await _tariffService.GetTariffAsync(tenantid)).State)
            {
                return;
            }

            _tenantManager.SetCurrentTenant(tenant);
            var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

            _log.InformationStartSendWhatsNewIn(tenant.GetTenantDomain(_coreSettings), tenantid);
            foreach (var user in await _userManager.GetUsersAsync())
            {
                _log.Debug($"SendMsgWhatsNew start checking subscription: {user.Email}");//temp

                if (!await CheckSubscriptionAsync(user, whatsNewType))
                {
                    continue;
                }

                _log.Debug($"SendMsgWhatsNew checking subscription complete: {user.Email}");//temp

                await _securityContext.AuthenticateMeWithoutCookieAsync(await _authManager.GetAccountByIDAsync(tenant.Id, user.Id));

                var culture = string.IsNullOrEmpty(user.CultureName) ? tenant.GetCulture() : user.GetCulture();

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                var auditEvents = new List<ActivityInfo>();

                foreach (var p in products)
                {
                    auditEvents.AddRange(await p.GetAuditEventsAsync(scheduleDate, user.Id, tenant, whatsNewType));
                }

                _log.Debug($"SendMsgWhatsNew auditEvents count : {auditEvents.Count}");//temp

                var userActivities = new List<string>();

                foreach (var e in auditEvents)
                {
                    if (TryGetUserActivityText(e, out var activityText))
                    {
                        userActivities.Add(activityText);
                    }
                }

                _log.Debug($"SendMsgWhatsNew userActivities count : {userActivities.Count}");//temp

                if (userActivities.Any())
                {
                    _log.InformationSendWhatsNewTo(user.Email);
                    await client.SendNoticeAsync(
                        Actions.SendWhatsNew, null, user,
                        new TagValue(Tags.Activities, userActivities),
                        new TagValue(Tags.Date, DateToString(scheduleDate, whatsNewType, culture)),
                        new TagValue(CommonTags.Priority, 1)
                    );
                }
            }
        }
        catch (Exception error)
        {
            _log.ErrorSendMsgWhatsNew(error);
        }
    }

    private bool TryGetUserActivityText(ActivityInfo activityInfo, out string userActivityText)
    {
        userActivityText = null;
        var action = activityInfo.Action;

        var user = _userManager.GetUsers(activityInfo.UserId);

        var date = activityInfo.Data;
        var userName = user.DisplayUserName(_displayUserSettingsHelper);
        var userEmail = user.Email;
        var userRole = activityInfo.UserRole;
        var fileUrl = activityInfo.FileUrl;
        var fileTitle = HtmlUtil.GetText(activityInfo.FileTitle, 512);
        var roomsUrl = activityInfo.RoomUri;
        var roomsTitle = activityInfo.RoomTitle;
        var oldRoomTitle = activityInfo.RoomOldTitle;

        var targetUserNames = "";
        var targetUserEmail = "";

        if (action == MessageAction.UsersUpdatedType)
        {
            var targetUsers = activityInfo.TargetUsers.Select(_userManager.GetUsers);
            var rawTargetUserNames = targetUsers.Select(r => r.DisplayUserName(_displayUserSettingsHelper));
            targetUserNames = string.Join(",", rawTargetUserNames);
        }
        else if (activityInfo.TargetUsers != null)
        {
            var targetUser = _userManager.GetUsers(activityInfo.TargetUsers.FirstOrDefault());
            targetUserEmail = targetUser.Email;
            targetUserNames = targetUser.DisplayUserName(_displayUserSettingsHelper);
        }


        if (action == MessageAction.FileCreated)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionFileCreated,
                userName, fileUrl, fileTitle, roomsUrl, roomsTitle, date);

        }
        else if (action == MessageAction.FileUpdatedRevisionComment)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionNewComment,
                userName, fileUrl, fileTitle, roomsUrl, roomsTitle, date);
        }
        else if (action == MessageAction.RoomCreated)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionRoomCreated,
                userName, roomsUrl, roomsTitle, date);
        }
        else if (action == MessageAction.RoomRenamed)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionRoomRenamed,
                userName, oldRoomTitle, roomsUrl, roomsTitle, date);
        }
        else if (action == MessageAction.RoomArchived)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionRoomArchived,
                userName, roomsUrl, roomsTitle, date);
        }
        else if (action == MessageAction.UserCreated)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionUserCreated,
                targetUserNames, targetUserEmail, date);
        }
        else if (action == MessageAction.UserUpdated)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionUserUpdated,
                targetUserNames, targetUserEmail, date);
        }
        else if (action == MessageAction.FileUploaded)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionFileUploaded,
                userName, fileUrl, fileTitle, roomsUrl, roomsTitle, date);
        }
        else if (action == MessageAction.UserFileUpdated)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionFileEdited,
                userName, fileUrl, fileTitle, roomsUrl, roomsTitle);
        }
        else if (action == MessageAction.RoomCreateUser)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionUserAddedToRoom,
                targetUserNames, roomsUrl, roomsTitle);
        }
        else if (action == MessageAction.RoomRemoveUser)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionUserRemovedFromRoom,
                targetUserNames, roomsUrl, roomsTitle, date);
        }
        else if (action == MessageAction.RoomUpdateAccessForUser)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionRoomUpdateAccessForUser,
                targetUserNames, userRole, roomsUrl, roomsTitle);
        }
        else if (action == MessageAction.RoomDeleted)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionRoomRemoved,
                userName, oldRoomTitle);
        }
        else if (action == MessageAction.UsersUpdatedType)
        {
            userActivityText = string.Format(WebstudioNotifyPatternResource.ActionUsersUpdatedType,
                userName, targetUserNames, userRole);
        }
        else
        {
            return false;
        }

        return true;
    }

    private async Task<bool> CheckSubscriptionAsync(UserInfo user, WhatsNewType whatsNewType)
    {
        if (whatsNewType == WhatsNewType.DailyFeed &&
            await _studioNotifyHelper.IsSubscribedToNotifyAsync(user, Actions.SendWhatsNew))
        {
            return true;
        }
        else if (whatsNewType == WhatsNewType.RoomsActivity &&
            await _studioNotifyHelper.IsSubscribedToNotifyAsync(user, Actions.RoomsActivity))
        {
            return true;
        }

        return false;
    }

    private bool TimeToSendWhatsNew(DateTime currentTime, WhatsNewType type)
    {
        if (type == WhatsNewType.RoomsActivity)
        {
            return true;
        }

        var hourToSend = 7;
        if (!string.IsNullOrEmpty(_confuguration["web:whatsnew-time"]))
        {
            if (int.TryParse(_confuguration["web:whatsnew-time"], out var hour))
            {
                hourToSend = hour;
            }
        }
        return currentTime.Hour == hourToSend;
    }

    private static string DateToString(DateTime d, WhatsNewType type, CultureInfo c)
    {
        if (type == WhatsNewType.DailyFeed)
        {
            d = d.AddDays(-1);
        }
        else
        {
            d = d.AddHours(-1);
        }

        return d.ToString(c.TwoLetterISOLanguageName == "ru" ? "d MMMM" : "M", c);
    }
}

public class ActivityInfo
{
    public Guid UserId { get; set; }
    public MessageAction Action { get; set; }
    public DateTime Data { get; set; }
    public string FileTitle { get; set; }
    public string FileUrl { get; set; }
    public string RoomUri { get; set; }
    public string RoomTitle { get; set; }
    public string RoomOldTitle { get; set; }
    public List<Guid> TargetUsers { get; set; }
    public string UserRole { get; set; }
}

public enum WhatsNewType
{
    DailyFeed = 0,
    RoomsActivity = 1
}
