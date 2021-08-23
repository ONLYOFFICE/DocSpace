/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
using System.Globalization;
using System.Linq;
using System.Threading;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Feed;
using ASC.Feed.Data;
using ASC.Notify.Patterns;
using ASC.Web.Core;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.Core.Notify
{
    [Singletone(Additional = typeof(StudioWhatsNewNotifyExtension))]
    public class StudioWhatsNewNotify
    {
        private IServiceProvider ServiceProvider { get; }
        public IConfiguration Confuguration { get; }

        public StudioWhatsNewNotify(IServiceProvider serviceProvider, IConfiguration confuguration)
        {
            ServiceProvider = serviceProvider;
            Confuguration = confuguration;
        }

        public void SendMsgWhatsNew(DateTime scheduleDate)
        {
            var log = ServiceProvider.GetService<IOptionsMonitor<ILog>>().Get("ASC.Notify");
            var WebItemManager = ServiceProvider.GetService<WebItemManager>();

            if (WebItemManager.GetItemsAll<IProduct>().Count == 0)
            {
                log.Info("No products. Return from function");
                return;
            }

            log.Info("Start send whats new.");

            var products = WebItemManager.GetItemsAll().ToDictionary(p => p.GetSysName());

            foreach (var tenantid in GetChangedTenants(ServiceProvider.GetService<FeedAggregateDataProvider>(), scheduleDate))
            {
                try
                {
                    using var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<StudioWhatsNewNotifyScope>();
                    var (tenantManager, paymentManager, tenantUtil, studioNotifyHelper, userManager, securityContext, authContext, authManager, commonLinkUtility, displayUserSettingsHelper, feedAggregateDataProvider, coreSettings) = scopeClass;
                    var tenant = tenantManager.GetTenant(tenantid);
                    if (tenant == null ||
                        tenant.Status != TenantStatus.Active ||
                        !TimeToSendWhatsNew(tenantUtil.DateTimeFromUtc(tenant.TimeZone, scheduleDate)) ||
                        TariffState.NotPaid <= paymentManager.GetTariff(tenantid).State)
                    {
                        continue;
                    }

                    tenantManager.SetCurrentTenant(tenant);
                    var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifyHelper.NotifySource, scope);

                    log.InfoFormat("Start send whats new in {0} ({1}).", tenant.GetTenantDomain(coreSettings), tenantid);
                    foreach (var user in userManager.GetUsers())
                    {
                        if (!studioNotifyHelper.IsSubscribedToNotify(user, Actions.SendWhatsNew))
                        {
                            continue;
                        }

                        securityContext.AuthenticateMeWithoutCookie(authManager.GetAccountByID(tenant.TenantId, user.ID));

                        var culture = string.IsNullOrEmpty(user.CultureName) ? tenant.GetCulture() : user.GetCulture();

                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        var feeds = feedAggregateDataProvider.GetFeeds(new FeedApiFilter
                        {
                            From = scheduleDate.Date.AddDays(-1),
                            To = scheduleDate.Date.AddSeconds(-1),
                            Max = 100,
                        });

                        var feedMinWrappers = feeds.ConvertAll(f => f.ToFeedMin(userManager));

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
                                UserName = f.Author != null && f.Author.UserInfo != null ? f.Author.UserInfo.DisplayUserName(displayUserSettingsHelper) : string.Empty,
                                UserAbsoluteURL = f.Author != null && f.Author.UserInfo != null ? commonLinkUtility.GetFullAbsolutePath(f.Author.UserInfo.GetUserProfilePageURL(commonLinkUtility)) : string.Empty,
                                Title = HtmlUtil.GetText(f.Title, 512),
                                URL = commonLinkUtility.GetFullAbsolutePath(f.ItemUrl),
                                BreadCrumbs = new string[0],
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
                                            UserName = prawbc.Author != null && prawbc.Author.UserInfo != null ? prawbc.Author.UserInfo.DisplayUserName(displayUserSettingsHelper) : string.Empty,
                                            UserAbsoluteURL = prawbc.Author != null && prawbc.Author.UserInfo != null ? commonLinkUtility.GetFullAbsolutePath(prawbc.Author.UserInfo.GetUserProfilePageURL(commonLinkUtility)) : string.Empty,
                                            Title = HtmlUtil.GetText(prawbc.Title, 512),
                                            URL = commonLinkUtility.GetFullAbsolutePath(prawbc.ItemUrl),
                                            BreadCrumbs = new string[0],
                                            Action = GetWhatsNewActionText(prawbc)
                                        });
                        }

                        var groupByPrjs = projectActivities.Where(p => !string.IsNullOrEmpty(p.ExtraLocation)).GroupBy(f => f.ExtraLocation);
                        foreach (var gr in groupByPrjs)
                        {
                            var grlist = gr.ToList();
                            for (var i = 0; i < grlist.Count(); i++)
                            {
                                var ls = grlist[i];
                                whatsNewUserActivityGroupByPrjs.Add(
                                    new WhatsNewUserActivity
                                    {
                                        Date = ls.CreatedDate,
                                        UserName = ls.Author != null && ls.Author.UserInfo != null ? ls.Author.UserInfo.DisplayUserName(displayUserSettingsHelper) : string.Empty,
                                        UserAbsoluteURL = ls.Author != null && ls.Author.UserInfo != null ? commonLinkUtility.GetFullAbsolutePath(ls.Author.UserInfo.GetUserProfilePageURL(commonLinkUtility)) : string.Empty,
                                        Title = HtmlUtil.GetText(ls.Title, 512),
                                        URL = commonLinkUtility.GetFullAbsolutePath(ls.ItemUrl),
                                        BreadCrumbs = i == 0 ? new string[1] { gr.Key } : new string[0],
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
                            log.InfoFormat("Send whats new to {0}", user.Email);
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
                    log.Error(error);
                }
            }
        }

        private static string GetWhatsNewActionText(FeedMin feed)
        {

            if (feed.Module == ASC.Feed.Constants.BookmarksModule)
                return WebstudioNotifyPatternResource.ActionCreateBookmark;
            else if (feed.Module == ASC.Feed.Constants.BlogsModule)
                return WebstudioNotifyPatternResource.ActionCreateBlog;
            else if (feed.Module == ASC.Feed.Constants.ForumsModule)
            {
                if (feed.Item == "forumTopic")
                    return WebstudioNotifyPatternResource.ActionCreateForum;
                if (feed.Item == "forumPost")
                    return WebstudioNotifyPatternResource.ActionCreateForumPost;
                if (feed.Item == "forumPoll")
                    return WebstudioNotifyPatternResource.ActionCreateForumPoll;
            }
            else if (feed.Module == ASC.Feed.Constants.EventsModule)
                return WebstudioNotifyPatternResource.ActionCreateEvent;
            else if (feed.Module == ASC.Feed.Constants.ProjectsModule)
                return WebstudioNotifyPatternResource.ActionCreateProject;
            else if (feed.Module == ASC.Feed.Constants.MilestonesModule)
                return WebstudioNotifyPatternResource.ActionCreateMilestone;
            else if (feed.Module == ASC.Feed.Constants.DiscussionsModule)
                return WebstudioNotifyPatternResource.ActionCreateDiscussion;
            else if (feed.Module == ASC.Feed.Constants.TasksModule)
                return WebstudioNotifyPatternResource.ActionCreateTask;
            else if (feed.Module == ASC.Feed.Constants.CommentsModule)
                return WebstudioNotifyPatternResource.ActionCreateComment;
            else if (feed.Module == ASC.Feed.Constants.CrmTasksModule)
                return WebstudioNotifyPatternResource.ActionCreateTask;
            else if (feed.Module == ASC.Feed.Constants.ContactsModule)
                return WebstudioNotifyPatternResource.ActionCreateContact;
            else if (feed.Module == ASC.Feed.Constants.DealsModule)
                return WebstudioNotifyPatternResource.ActionCreateDeal;
            else if (feed.Module == ASC.Feed.Constants.CasesModule)
                return WebstudioNotifyPatternResource.ActionCreateCase;
            else if (feed.Module == ASC.Feed.Constants.FilesModule)
                return WebstudioNotifyPatternResource.ActionCreateFile;
            else if (feed.Module == ASC.Feed.Constants.FoldersModule)
                return WebstudioNotifyPatternResource.ActionCreateFolder;

            return "";
        }

        private static IEnumerable<int> GetChangedTenants(FeedAggregateDataProvider feedAggregateDataProvider, DateTime date)
        {
            return feedAggregateDataProvider.GetTenants(new TimeInterval(date.Date.AddDays(-1), date.Date.AddSeconds(-1)));
        }

        private bool TimeToSendWhatsNew(DateTime currentTime)
        {
            var hourToSend = 7;
            if (!string.IsNullOrEmpty(Confuguration["web:whatsnew-time"]))
            {
                if (int.TryParse(Confuguration["web:whatsnew-time"], out var hour))
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

    [Scope]
    public class StudioWhatsNewNotifyScope
    {
        private TenantManager TenantManager { get; }
        private PaymentManager PaymentManager { get; }
        private TenantUtil TenantUtil { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private AuthContext AuthContext { get; }
        private AuthManager AuthManager { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private FeedAggregateDataProvider FeedAggregateDataProvider { get; }
        private CoreSettings CoreSettings { get; }

        public StudioWhatsNewNotifyScope(TenantManager tenantManager,
            PaymentManager paymentManager,
            TenantUtil tenantUtil,
            StudioNotifyHelper studioNotifyHelper,
            UserManager userManager,
            SecurityContext securityContext,
            AuthContext authContext,
            AuthManager authManager,
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            FeedAggregateDataProvider feedAggregateDataProvider,
            CoreSettings coreSettings)
        {
            TenantManager = tenantManager;
            PaymentManager = paymentManager;
            TenantUtil = tenantUtil;
            StudioNotifyHelper = studioNotifyHelper;
            UserManager = userManager;
            SecurityContext = securityContext;
            AuthContext = authContext;
            AuthManager = authManager;
            CommonLinkUtility = commonLinkUtility;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            FeedAggregateDataProvider = feedAggregateDataProvider;
            CoreSettings = coreSettings;
        }

        public void Deconstruct(out TenantManager tenantManager,
            out PaymentManager paymentManager,
            out TenantUtil tenantUtil,
            out StudioNotifyHelper studioNotifyHelper,
            out UserManager userManager,
            out SecurityContext securityContext,
            out AuthContext authContext,
            out AuthManager authManager,
            out CommonLinkUtility commonLinkUtility,
            out DisplayUserSettingsHelper displayUserSettingsHelper,
            out FeedAggregateDataProvider feedAggregateDataProvider,
            out CoreSettings coreSettings)
        {
            tenantManager = TenantManager;
            paymentManager = PaymentManager;
            tenantUtil = TenantUtil;
            studioNotifyHelper = StudioNotifyHelper;
            userManager = UserManager;
            securityContext = SecurityContext;
            authContext = AuthContext;
            authManager = AuthManager;
            commonLinkUtility = CommonLinkUtility;
            displayUserSettingsHelper = DisplayUserSettingsHelper;
            feedAggregateDataProvider = FeedAggregateDataProvider;
            coreSettings = CoreSettings;
        }
    }

    public static class StudioWhatsNewNotifyExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<WebItemManager>();
            services.TryAdd<FeedAggregateDataProvider>();
            services.TryAdd<StudioWhatsNewNotifyScope>();
        }
    }
}