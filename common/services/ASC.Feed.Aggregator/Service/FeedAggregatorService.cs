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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Notify.Signalr;
using ASC.Feed.Aggregator.Modules;
using ASC.Feed.Configuration;
using ASC.Feed.Data;

using Autofac;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Feed.Aggregator
{
    [Singletone(Additional = typeof(FeedAggregatorServiceExtension))]
    public class FeedAggregatorService : IHostedService, IDisposable
    {
        private ILog Log { get; set; }
        private SignalrServiceClient SignalrServiceClient { get; }

        private Timer aggregateTimer;
        private Timer removeTimer;

        private volatile bool isStopped;
        private readonly object aggregateLock = new object();
        private readonly object removeLock = new object();

        private FeedSettings FeedSettings { get; }
        private IServiceProvider ServiceProvider { get; }

        public FeedAggregatorService(
            FeedSettings feedSettings,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> optionsMonitor,
            IOptionsSnapshot<SignalrServiceClient> optionsSnapshot)
        {
            FeedSettings = feedSettings;
            ServiceProvider = serviceProvider;
            Log = optionsMonitor.Get("ASC.Feed.Agregator");
            SignalrServiceClient = optionsSnapshot.Get("counters");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var cfg = FeedSettings;
            isStopped = false;

            aggregateTimer = new Timer(AggregateFeeds, cfg.AggregateInterval, TimeSpan.Zero, cfg.AggregatePeriod);
            removeTimer = new Timer(RemoveFeeds, cfg.AggregateInterval, cfg.RemovePeriod, cfg.RemovePeriod);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            isStopped = true;

            if (aggregateTimer != null)
            {
                aggregateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                aggregateTimer.Dispose();
                aggregateTimer = null;
            }

            if (removeTimer != null)
            {
                removeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                removeTimer.Dispose();
                removeTimer = null;
            }

            return Task.CompletedTask;
        }

        private void AggregateFeeds(object interval)
        {
            if (!Monitor.TryEnter(aggregateLock)) return;

            try
            {
                var cfg = FeedSettings;
                using var scope = ServiceProvider.CreateScope();
                var scopeClass = scope.ServiceProvider.GetService<FeedAggregatorServiceScope>();
                var cache = scope.ServiceProvider.GetService<ICache>();
                var (baseCommonLinkUtility, tenantManager, feedAggregateDataProvider, userManager, securityContext, authManager) = scopeClass;
                baseCommonLinkUtility.Initialize(cfg.ServerRoot);

                var start = DateTime.UtcNow;
                Log.DebugFormat("Start of collecting feeds...");

                var unreadUsers = new Dictionary<int, Dictionary<Guid, int>>();
                var modules = scope.ServiceProvider.GetService<IEnumerable<IFeedModule>>();

                foreach (var module in modules)
                {
                    var result = new List<FeedRow>();
                    var fromTime = feedAggregateDataProvider.GetLastTimeAggregate(module.GetType().Name);
                    if (fromTime == default) fromTime = DateTime.UtcNow.Subtract((TimeSpan)interval);
                    var toTime = DateTime.UtcNow;

                    var tenants = Attempt(10, () => module.GetTenantsWithFeeds(fromTime)).ToList();
                    Log.DebugFormat("Find {1} tenants for module {0}.", module.GetType().Name, tenants.Count);

                    foreach (var tenant in tenants)
                    {
                        // Warning! There is hack here!
                        // clearing the cache to get the correct acl
                        cache.Remove("acl" + tenant);
                        cache.Remove("/webitemsecurity/" + tenant);
                        //cache.Remove(string.Format("sub/{0}/{1}/{2}", tenant, "6045b68c-2c2e-42db-9e53-c272e814c4ad", NotifyConstants.Event_NewCommentForMessage.ID));

                        try
                        {
                            if (tenantManager.GetTenant(tenant) == null)
                            {
                                continue;
                            }

                            tenantManager.SetCurrentTenant(tenant);
                            var users = userManager.GetUsers();

                            var feeds = Attempt(10, () => module.GetFeeds(new FeedFilter(fromTime, toTime) { Tenant = tenant }).Where(r => r.Item1 != null).ToList());
                            Log.DebugFormat("{0} feeds in {1} tenant.", feeds.Count, tenant);

                            var tenant1 = tenant;
                            var module1 = module;
                            var feedsRow = feeds
                                .Select(tuple => new Tuple<FeedRow, object>(new FeedRow(tuple.Item1)
                                {
                                    Tenant = tenant1,
                                    ProductId = module1.Product
                                }, tuple.Item2))
                                .ToList();

                            foreach (var u in users)
                            {
                                if (isStopped)
                                {
                                    return;
                                }
                                if (!TryAuthenticate(securityContext, authManager, tenant1, u.ID))
                                {
                                    continue;
                                }

                                module.VisibleFor(feedsRow, u.ID);
                            }

                            result.AddRange(feedsRow.Select(r => r.Item1));
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorFormat("Tenant: {0}, {1}", tenant, ex);
                        }
                    }

                    feedAggregateDataProvider.SaveFeeds(result, module.GetType().Name, toTime);

                    foreach (var res in result)
                    {
                        foreach (var userGuid in res.Users.Where(userGuid => !userGuid.Equals(res.ModifiedById)))
                        {
                            if (!unreadUsers.TryGetValue(res.Tenant, out var dictionary))
                            {
                                dictionary = new Dictionary<Guid, int>();
                            }
                            if (dictionary.ContainsKey(userGuid))
                            {
                                ++dictionary[userGuid];
                            }
                            else
                            {
                                dictionary.Add(userGuid, 1);
                            }

                            unreadUsers[res.Tenant] = dictionary;
                        }
                    }
                }

                SignalrServiceClient.SendUnreadUsers(unreadUsers);

                Log.DebugFormat("Time of collecting news: {0}", DateTime.UtcNow - start);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                Monitor.Exit(aggregateLock);
            }
        }

        private void RemoveFeeds(object interval)
        {
            if (!Monitor.TryEnter(removeLock)) return;

            try
            {
                using var scope = ServiceProvider.CreateScope();
                var feedAggregateDataProvider = scope.ServiceProvider.GetService<FeedAggregateDataProvider>();
                Log.DebugFormat("Start of removing old news");
                feedAggregateDataProvider.RemoveFeedAggregate(DateTime.UtcNow.Subtract((TimeSpan)interval));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                Monitor.Exit(removeLock);
            }
        }

        private static T Attempt<T>(int count, Func<T> action)
        {
            var counter = 0;
            while (true)
            {
                try
                {
                    return action();
                }
                catch
                {
                    if (count < ++counter)
                    {
                        throw;
                    }
                }
            }
        }

        private static bool TryAuthenticate(SecurityContext securityContext, AuthManager authManager, int tenantId, Guid userid)
        {
            try
            {
                securityContext.AuthenticateMeWithoutCookie(authManager.GetAccountByID(tenantId, userid));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (aggregateTimer != null)
            {
                aggregateTimer.Dispose();
            }

            if (removeTimer != null)
            {
                removeTimer.Dispose();
            }
        }
    }

    [Scope]
    public class FeedAggregatorServiceScope
    {
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private TenantManager TenantManager { get; }
        private FeedAggregateDataProvider FeedAggregateDataProvider { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private AuthManager AuthManager { get; }

        public FeedAggregatorServiceScope(BaseCommonLinkUtility baseCommonLinkUtility,
            TenantManager tenantManager,
            FeedAggregateDataProvider feedAggregateDataProvider,
            UserManager userManager,
            SecurityContext securityContext,
            AuthManager authManager)
        {
            BaseCommonLinkUtility = baseCommonLinkUtility;
            TenantManager = tenantManager;
            FeedAggregateDataProvider = feedAggregateDataProvider;
            UserManager = userManager;
            SecurityContext = securityContext;
            AuthManager = authManager;
        }

        public void Deconstruct(out BaseCommonLinkUtility baseCommonLinkUtility,
            out TenantManager tenantManager,
            out FeedAggregateDataProvider feedAggregateDataProvider,
            out UserManager userManager,
            out SecurityContext securityContext,
            out AuthManager authManager)
        {
            baseCommonLinkUtility = BaseCommonLinkUtility;
            tenantManager = TenantManager;
            feedAggregateDataProvider = FeedAggregateDataProvider;
            userManager = UserManager;
            securityContext = SecurityContext;
            authManager = AuthManager;
        }
    }

    public static class FeedAggregatorServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FeedAggregatorServiceScope>();
        }
    }
}