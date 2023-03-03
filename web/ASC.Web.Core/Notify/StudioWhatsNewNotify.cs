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
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly AuditEventsRepository _auditEventsRepository;
    private readonly CoreSettings _coreSettings;
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly IConfiguration _confuguration;
    private readonly WorkContext _workContext;

    public readonly static List<MessageAction?> DailyActions = new List<MessageAction?>()
    {
        MessageAction.FileCreated,
        MessageAction.FileUpdatedRevisionComment,
        MessageAction.RoomCreated,
        MessageAction.RoomRenamed,
        MessageAction.RoomArchived,
        MessageAction.UserCreated,
        MessageAction.UserUpdated
    };

    public readonly static List<MessageAction?> RoomsActivityActions = new List<MessageAction?>()
    {
         MessageAction.FileUploaded,
         MessageAction.FileUpdated,
         MessageAction.RoomCreateUser,
         MessageAction.RoomRemoveUser,
         MessageAction.RoomUpdateAccessForUser,
         MessageAction.RoomDeleted,
         MessageAction.UsersUpdatedType,
    };

    public StudioWhatsNewNotify(
        TenantManager tenantManager,
        ITariffService tariffService,
        TenantUtil tenantUtil,
        StudioNotifyHelper studioNotifyHelper,
        UserManager userManager,
        SecurityContext securityContext,
        AuthManager authManager,
        CommonLinkUtility commonLinkUtility,
        CoreSettings coreSettings,
        NotifyEngineQueue notifyEngineQueue,
        IConfiguration confuguration,
        WorkContext workContext,
        ILoggerProvider optionsMonitor,
        AuditEventsRepository auditEventsRepository,
        WebItemManager webItemManager)
    {
        _webItemManager = webItemManager;
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _tenantUtil = tenantUtil;
        _studioNotifyHelper = studioNotifyHelper;
        _userManager = userManager;
        _securityContext = securityContext;
        _authManager = authManager;
        _commonLinkUtility = commonLinkUtility;
        _coreSettings = coreSettings;
        _notifyEngineQueue = notifyEngineQueue;
        _confuguration = confuguration;
        _workContext = workContext;
        _auditEventsRepository = auditEventsRepository;
        _log = optionsMonitor.CreateLogger("ASC.Notify");
    }

    public void SendMsgWhatsNew(DateTime scheduleDate, WhatsNewType whatsNewType)
    {
        var products = _webItemManager.GetItemsAll<IProduct>();

        if (_webItemManager.GetItemsAll<IProduct>().Count == 0)
        {
            _log.InformationNoProducts();
            return;
        }

        _log.InformationStartSendWhatsNew();

        var tenants = GetChangedTenants(scheduleDate, whatsNewType);

        foreach (var tenantid in tenants)
        {
            SendMsgWhatsNew(tenantid, scheduleDate, whatsNewType, products);
        }
    }

    private IEnumerable<int> GetChangedTenants(DateTime date, WhatsNewType whatsNewType)
    {
        switch (whatsNewType)
        {
            case WhatsNewType.DailyFeed:
                return _auditEventsRepository.GetTenants(date.Date.AddDays(-1), date.Date.AddSeconds(-1));
            case WhatsNewType.RoomsActivity:
                return _auditEventsRepository.GetTenants(date.AddHours(-1), date.AddSeconds(-1));
            default:
                return Enumerable.Empty<int>();
        }
    }

    private void SendMsgWhatsNew(int tenantid, DateTime scheduleDate, WhatsNewType whatsNewType, List<IProduct> products)
    {
        try
        {
            var tenant = _tenantManager.GetTenant(tenantid);
            if (tenant == null ||
                tenant.Status != TenantStatus.Active ||
                !TimeToSendWhatsNew(_tenantUtil.DateTimeFromUtc(tenant.TimeZone, scheduleDate), whatsNewType) || //ToDo
                TariffState.NotPaid <= _tariffService.GetTariff(tenantid).State)
            {
                return;
            }

            _tenantManager.SetCurrentTenant(tenant);
            var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

            _log.InformationStartSendWhatsNewIn(tenant.GetTenantDomain(_coreSettings), tenantid);
            foreach (var user in _userManager.GetUsers())
            {
                if (!CheckSubscription(user, whatsNewType))
                {
                    continue;
                }

                _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(tenant.Id, user.Id));

                var culture = string.IsNullOrEmpty(user.CultureName) ? tenant.GetCulture() : user.GetCulture();

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                var auditEvents = new List<ActivityInfo>();

                foreach (var p in products)
                {
                    auditEvents.AddRange(p.GetAuditEventsAsync(scheduleDate, user.Id, tenant, whatsNewType).Result);
                }

                var userActivities = new List<WhatsNewUserActivity>();

                foreach (var e in auditEvents)
                {
                    if (TryConvertToUserActivity(e, out var activity))
                    {
                        userActivities.Add(activity);
                    }
                }

                if (userActivities.Any())
                {
                    _log.InformationSendWhatsNewTo(user.Email);
                    client.SendNoticeAsync(
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

    private bool TryConvertToUserActivity(ActivityInfo activityInfo, out WhatsNewUserActivity whatsNewUserActivity)
    {
        whatsNewUserActivity = null;
        var action = activityInfo.Action;

        var user = _userManager.GetUsers(activityInfo.UserId);

        var date = activityInfo.Data;
        var userName = $"{user.FirstName} {user.LastName}";
        var userEmail = user.Email;
        var userRole = activityInfo.UserRole;
        var fileUrl = activityInfo.FileUrl;
        var fileTitle = HtmlUtil.GetText(activityInfo.FileTitle, 512);
        var roomsUrl = activityInfo.RoomUri;
        var roomsTitle = activityInfo.RoomTitle;
        var oldRoomTitle = activityInfo.RoomOldTitle;

        var targetUserName = "";
        if (activityInfo.TargetUsers != null)
        {
            var targetUser = _userManager.GetUsers(activityInfo.TargetUsers.FirstOrDefault());
            targetUserName = $"{targetUser.FirstName} {targetUser.LastName}";
        }


        if (action == MessageAction.FileCreated)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionFileCreated,
                userName, fileUrl, fileTitle, roomsUrl, roomsTitle, date)
            };

        }
        else if (action == MessageAction.FileUpdatedRevisionComment)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionNewComment,
                   userName, fileUrl, fileTitle, roomsUrl, roomsTitle, date)
            };
        }
        else if (action == MessageAction.RoomCreated)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionRoomCreated,
                   userName, roomsUrl, roomsTitle, date)
            };
        }
        else if (action == MessageAction.RoomRenamed)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionRoomRenamed,
                   userName, oldRoomTitle, roomsUrl, roomsTitle, date)
            };
        }
        else if (action == MessageAction.RoomArchived)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionRoomArchived,
                   userName, roomsUrl, roomsTitle, date)
            };
        }
        else if (action == MessageAction.UserCreated)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionUserCreated,
                   userName, userEmail, date)
            };
        }
        else if (action == MessageAction.UserUpdated)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionUserUpdated,
                   userName, userEmail, date)
            };
        }
        else if (action == MessageAction.FileUploaded)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionFileUploaded,
                    userName, fileUrl, fileTitle, roomsUrl, roomsTitle, date)
            };
        }
        else if (action == MessageAction.FileUpdated)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionFileEdited,
                   userName, fileUrl, fileTitle, roomsUrl, roomsTitle)
            };
        }
        else if (action == MessageAction.RoomCreateUser)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionUserAddedToRoom,
                   targetUserName, roomsUrl, roomsTitle)
            };
        }
        else if (action == MessageAction.RoomRemoveUser)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionUserRemovedFromRoom,
                   targetUserName, roomsUrl, roomsTitle, date)
            };
        }
        else if (action == MessageAction.RoomUpdateAccessForUser)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionRoomUpdateAccessForUser,
                   targetUserName, userRole, roomsUrl, roomsTitle)
            };
        }
        else if (action == MessageAction.RoomDeleted)
        {
            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionRoomRemoved,
                    userName, oldRoomTitle)
            };
        }
        else if(action == MessageAction.UsersUpdatedType)
        {
            var targetUserNames = activityInfo.TargetUsers.Select(r => _userManager.GetUsers(r).UserName);
            var separatedUserNames = string.Join(",", targetUserNames);

            whatsNewUserActivity = new WhatsNewUserActivity()
            {
                Action = string.Format(WebstudioNotifyPatternResource.ActionUsersUpdatedType,
                   userName, separatedUserNames, userRole)
            };
        }
        else
        {
            return false;
        }

        return true;
    }

    private bool CheckSubscription(UserInfo user, WhatsNewType whatsNewType)
    {
        if (whatsNewType == WhatsNewType.DailyFeed &&
            _studioNotifyHelper.IsSubscribedToNotify(user, Actions.SendWhatsNew))
        {
            return true;
        }
        else if (whatsNewType == WhatsNewType.RoomsActivity &&
            _studioNotifyHelper.IsSubscribedToNotify(user, Actions.RoomsActivity))
        {
            return true;
        }

        return false;
    }

    private bool TimeToSendWhatsNew(DateTime currentTime, WhatsNewType type)
    {
        if(type == WhatsNewType.RoomsActivity)
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
        if(type == WhatsNewType.DailyFeed)
        {
            d = d.AddDays(-1);
        }
        else
        {
            d = d.AddHours(-1);
        }

        return d.ToString(c.TwoLetterISOLanguageName == "ru" ? "d MMMM" : "M", c);
    }


    class WhatsNewUserActivity
    {
        public string Action { get; set; }
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
