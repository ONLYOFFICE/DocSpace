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
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly FeedAggregateDataProvider _feedAggregateDataProvider;
    private readonly CoreSettings _coreSettings;
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly IConfiguration _confuguration;
    private readonly WorkContext _workContext;
    private readonly IMapper _mapper;

    public StudioWhatsNewNotify(
        TenantManager tenantManager,
        ITariffService tariffService,
        TenantUtil tenantUtil,
        StudioNotifyHelper studioNotifyHelper,
        UserManager userManager,
        SecurityContext securityContext,
        AuthManager authManager,
        CommonLinkUtility commonLinkUtility,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        FeedAggregateDataProvider feedAggregateDataProvider,
        CoreSettings coreSettings,
        NotifyEngineQueue notifyEngineQueue,
        IConfiguration confuguration,
        WorkContext workContext,
        ILoggerProvider optionsMonitor,
        IMapper mapper,
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
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _feedAggregateDataProvider = feedAggregateDataProvider;
        _coreSettings = coreSettings;
        _notifyEngineQueue = notifyEngineQueue;
        _confuguration = confuguration;
        _workContext = workContext;
        _mapper = mapper;
        _log = optionsMonitor.CreateLogger("ASC.Notify");
    }

    public void SendMsgWhatsNew(DateTime scheduleDate)
    {
        if (_webItemManager.GetItemsAll<IProduct>().Count == 0)
        {
            _log.InformationNoProducts();
            return;
        }

        _log.InformationStartSendWhatsNew();

        var products = _webItemManager.GetItemsAll().ToDictionary(p => p.GetSysName());
        var tenants = GetChangedTenants(scheduleDate);

        foreach (var tenantid in tenants)
        {
            SendMsgWhatsNew(tenantid, scheduleDate, products);
        }
    }

    private IEnumerable<int> GetChangedTenants(DateTime date)
    {
        return _feedAggregateDataProvider.GetTenants(new TimeInterval(date.Date.AddDays(-1), date.Date.AddSeconds(-1)));
    }

    private void SendMsgWhatsNew(int tenantid, DateTime scheduleDate, Dictionary<string, IWebItem> products)
    {
        try
        {
            var tenant = _tenantManager.GetTenant(tenantid);
            if (tenant == null ||
                tenant.Status != TenantStatus.Active ||
                !TimeToSendWhatsNew(_tenantUtil.DateTimeFromUtc(tenant.TimeZone, scheduleDate)) ||
                TariffState.NotPaid <= _tariffService.GetTariff(tenantid).State)
            {
                return;
            }

            _tenantManager.SetCurrentTenant(tenant);
            var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

            _log.InformationStartSendWhatsNewIn(tenant.GetTenantDomain(_coreSettings), tenantid);
            foreach (var user in _userManager.GetUsers())
            {
                if (!_studioNotifyHelper.IsSubscribedToNotify(user, Actions.SendWhatsNew))
                {
                    continue;
                }

                _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(tenant.Id, user.Id));

                var culture = string.IsNullOrEmpty(user.CultureName) ? tenant.GetCulture() : user.GetCulture();

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                var feeds = _feedAggregateDataProvider.GetFeeds(new FeedApiFilter
                {
                    From = scheduleDate.Date.AddDays(-1),
                    To = scheduleDate.Date.AddSeconds(-1),
                    Max = 100,
                });

                var feedMinWrappers = _mapper.Map<List<FeedResultItem>, List<FeedMin>>(feeds);

                var feedMinGroupedWrappers = feedMinWrappers
                    .Where(f =>
                        (f.CreatedDate == DateTime.MaxValue || f.CreatedDate >= scheduleDate.Date.AddDays(-1)) && //'cause here may be old posts with new comments
                        products.ContainsKey(f.Product) &&
                        !f.Id.StartsWith("participant")
                    )
                    .GroupBy(f => products[f.Product]);

                var ProjectsProductName = products["projects"]?.Name; //from ASC.Feed.Aggregator.Modules.ModulesHelper.ProjectsProductName

                var activities = feedMinGroupedWrappers
                    .Where(f => f.Key.Name != ProjectsProductName) //not for project product
                    .ToDictionary(
                    g => g.Key.Name,
                    g => g.Select(f => new WhatsNewUserActivity
                    {
                        Date = f.CreatedDate,
                        UserName = f.Author != null && f.Author.UserInfo != null ? f.Author.UserInfo.DisplayUserName(_displayUserSettingsHelper) : string.Empty,
                        UserAbsoluteURL = f.Author != null && f.Author.UserInfo != null ? _commonLinkUtility.GetFullAbsolutePath(f.Author.UserInfo.GetUserProfilePageURL(_commonLinkUtility)) : string.Empty,
                        Title = HtmlUtil.GetText(f.Title, 512),
                        URL = _commonLinkUtility.GetFullAbsolutePath(f.ItemUrl),
                        BreadCrumbs = Array.Empty<string>(),
                        Action = GetWhatsNewActionText(f)
                    }).ToList());


                var projectActivities = feedMinGroupedWrappers
                    .Where(f => f.Key.Name == ProjectsProductName) // for project product
                    .SelectMany(f => f);

                var projectActivitiesWithoutBreadCrumbs = projectActivities.Where(p => string.IsNullOrEmpty(p.ExtraLocation));

                var whatsNewUserActivityGroupByPrjs = new List<WhatsNewUserActivity>();

                foreach (var prawbc in projectActivitiesWithoutBreadCrumbs)
                {
                    whatsNewUserActivityGroupByPrjs.Add(
                                new WhatsNewUserActivity
                                {
                                    Date = prawbc.CreatedDate,
                                    UserName = prawbc.Author != null && prawbc.Author.UserInfo != null ? prawbc.Author.UserInfo.DisplayUserName(_displayUserSettingsHelper) : string.Empty,
                                    UserAbsoluteURL = prawbc.Author != null && prawbc.Author.UserInfo != null ? _commonLinkUtility.GetFullAbsolutePath(prawbc.Author.UserInfo.GetUserProfilePageURL(_commonLinkUtility)) : string.Empty,
                                    Title = HtmlUtil.GetText(prawbc.Title, 512),
                                    URL = _commonLinkUtility.GetFullAbsolutePath(prawbc.ItemUrl),
                                    BreadCrumbs = Array.Empty<string>(),
                                    Action = GetWhatsNewActionText(prawbc)
                                });
                }

                var groupByPrjs = projectActivities.Where(p => !string.IsNullOrEmpty(p.ExtraLocation)).GroupBy(f => f.ExtraLocation);
                foreach (var gr in groupByPrjs)
                {
                    var grlist = gr.ToList();
                    for (var i = 0; i < grlist.Count; i++)
                    {
                        var ls = grlist[i];
                        whatsNewUserActivityGroupByPrjs.Add(
                            new WhatsNewUserActivity
                            {
                                Date = ls.CreatedDate,
                                UserName = ls.Author != null && ls.Author.UserInfo != null ? ls.Author.UserInfo.DisplayUserName(_displayUserSettingsHelper) : string.Empty,
                                UserAbsoluteURL = ls.Author != null && ls.Author.UserInfo != null ? _commonLinkUtility.GetFullAbsolutePath(ls.Author.UserInfo.GetUserProfilePageURL(_commonLinkUtility)) : string.Empty,
                                Title = HtmlUtil.GetText(ls.Title, 512),
                                URL = _commonLinkUtility.GetFullAbsolutePath(ls.ItemUrl),
                                BreadCrumbs = i == 0 ? new string[1] { gr.Key } : Array.Empty<string>(),
                                Action = GetWhatsNewActionText(ls)
                            });
                    }
                }

                if (whatsNewUserActivityGroupByPrjs.Count > 0)
                {
                    activities.Add(ProjectsProductName, whatsNewUserActivityGroupByPrjs);
                }

                if (activities.Count > 0)
                {
                    _log.InformationSendWhatsNewTo(user.Email);
                    client.SendNoticeAsync(
                        Actions.SendWhatsNew, null, user,
                        new TagValue(Tags.Activities, activities),
                        new TagValue(Tags.Date, DateToString(scheduleDate.AddDays(-1), culture)),
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

    private static string GetWhatsNewActionText(FeedMin feed)
    {

        if (feed.Module == Feed.Constants.BookmarksModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateBookmark;
        }
        else if (feed.Module == Feed.Constants.BlogsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateBlog;
        }
        else if (feed.Module == Feed.Constants.ForumsModule)
        {
            if (feed.Item == "forumTopic")
            {
                return WebstudioNotifyPatternResource.ActionCreateForum;
            }

            if (feed.Item == "forumPost")
            {
                return WebstudioNotifyPatternResource.ActionCreateForumPost;
            }

            if (feed.Item == "forumPoll")
            {
                return WebstudioNotifyPatternResource.ActionCreateForumPoll;
            }
        }
        else if (feed.Module == Feed.Constants.EventsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateEvent;
        }
        else if (feed.Module == Feed.Constants.ProjectsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateProject;
        }
        else if (feed.Module == Feed.Constants.MilestonesModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateMilestone;
        }
        else if (feed.Module == Feed.Constants.DiscussionsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateDiscussion;
        }
        else if (feed.Module == Feed.Constants.TasksModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateTask;
        }
        else if (feed.Module == Feed.Constants.CommentsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateComment;
        }
        else if (feed.Module == Feed.Constants.CrmTasksModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateTask;
        }
        else if (feed.Module == Feed.Constants.ContactsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateContact;
        }
        else if (feed.Module == Feed.Constants.DealsModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateDeal;
        }
        else if (feed.Module == Feed.Constants.CasesModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateCase;
        }
        else if (feed.Module == Feed.Constants.FilesModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateFile;
        }
        else if (feed.Module == Feed.Constants.FoldersModule)
        {
            return WebstudioNotifyPatternResource.ActionCreateFolder;
        }

        return "";
    }

    private bool TimeToSendWhatsNew(DateTime currentTime)
    {
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

    private static string DateToString(DateTime d, CultureInfo c)
    {
        return d.ToString(c.TwoLetterISOLanguageName == "ru" ? "d MMMM" : "M", c);
    }


    class WhatsNewUserActivity
    {
        public IList<string> BreadCrumbs { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string UserName { get; set; }
        public string UserAbsoluteURL { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
    }
}
