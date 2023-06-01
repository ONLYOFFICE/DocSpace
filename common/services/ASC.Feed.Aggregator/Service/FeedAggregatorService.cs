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

namespace ASC.Feed.Aggregator.Service;

[Singletone]
public class FeedAggregatorService : FeedBaseService
{
    protected override string LoggerName { get; set; } = "ASC.Feed.Aggregator";

    public FeedAggregatorService(
        FeedSettings feedSettings,
        IServiceScopeFactory serviceScopeFactory,
        ILoggerProvider optionsMonitor)
        : base(feedSettings, serviceScopeFactory, optionsMonitor)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.InformationAggregatorServiceRunning();

        var cfg = _feedSettings;

        while (!stoppingToken.IsCancellationRequested)
        {
            await AggregateFeedsAsync(cfg.AggregateInterval);

            await Task.Delay(cfg.AggregatePeriod, stoppingToken);
        }

        _logger.InformationAggregatorServiceStopping();
    }

    private static async Task<T> Attempt<T>(int count, Func<Task<T>> action)
    {
        var counter = 0;
        while (true)
        {
            try
            {
                return await action();
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

    private static async Task<bool> TryAuthenticateAsync(SecurityContext securityContext, AuthManager authManager, int tenantId, Guid userid)
    {
        try
        {
            await securityContext.AuthenticateMeWithoutCookieAsync(await authManager.GetAccountByIDAsync(tenantId, userid));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task AggregateFeedsAsync(object interval)
    {
        try
        {
            var cfg = _feedSettings;
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var cache = scope.ServiceProvider.GetService<ICache>();
            var baseCommonLinkUtility = scope.ServiceProvider.GetService<BaseCommonLinkUtility>();
            baseCommonLinkUtility.Initialize(cfg.ServerRoot);

            var start = DateTime.UtcNow;
            _logger.DebugStartCollectiongFeeds();

            var unreadUsers = new Dictionary<int, Dictionary<Guid, int>>();
            var modules = scope.ServiceProvider.GetService<IEnumerable<IFeedModule>>();

            var feedAggregateDataProvider = scope.ServiceProvider.GetService<FeedAggregateDataProvider>();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var userManager = scope.ServiceProvider.GetService<UserManager>();
            var authManager = scope.ServiceProvider.GetService<AuthManager>();
            var securityContext = scope.ServiceProvider.GetService<SecurityContext>();
            var socketServiceClient = scope.ServiceProvider.GetRequiredService<SocketServiceClient>();

            foreach (var module in modules)
            {
                var result = new List<FeedRow>();
                var fromTime = await feedAggregateDataProvider.GetLastTimeAggregateAsync(module.GetType().Name);
                if (fromTime == default)
                {
                    fromTime = DateTime.UtcNow.Subtract((TimeSpan)interval);
                }

                var toTime = DateTime.UtcNow;

                var tenants = (await Attempt(10, async () => await module.GetTenantsWithFeeds(fromTime))).ToList();
                _logger.DebugFindCountTenants(tenants.Count, module.GetType().Name);

                foreach (var tenant in tenants)
                {
                    // Warning! There is hack here!
                    // clearing the cache to get the correct acl
                    cache.Remove("acl" + tenant);
                    cache.Remove("/webitemsecurity/" + tenant);
                    //cache.Remove(string.Format("sub/{0}/{1}/{2}", tenant, "6045b68c-2c2e-42db-9e53-c272e814c4ad", NotifyConstants.Event_NewCommentForMessage.ID));

                    try
                    {
                        if (await tenantManager.GetTenantAsync(tenant) == null)
                        {
                            continue;
                        }

                        await tenantManager.SetCurrentTenantAsync(tenant);
                        var users = await userManager.GetUsersAsync();

                        var feeds = await Attempt(10, async () => (await module.GetFeeds(new FeedFilter(fromTime, toTime) { Tenant = tenant })).Where(r => r.Item1 != null).ToList());
                        _logger.DebugCountFeeds(feeds.Count, tenant);

                        var tenant1 = tenant;
                        var module1 = module;
                        var feedsRow = feeds
                            .Select(tuple => new Tuple<FeedRow, object>(new FeedRow(tuple.Item1)
                            {
                                Tenant = tenant1,
                                Product = module1.Product
                            }, tuple.Item2))
                            .ToList();

                        foreach (var u in users)
                        {
                            if (!await TryAuthenticateAsync(securityContext, authManager, tenant1, u.Id))
                            {
                                continue;
                            }

                            await module.VisibleForAsync(feedsRow, u.Id);
                        }

                        result.AddRange(feedsRow.Select(r => r.Item1));
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorTenant(tenant, ex);
                    }
                }

                await feedAggregateDataProvider.SaveFeedsAsync(result, module.GetType().Name, toTime, cfg.PortionSize);

                foreach (var res in result)
                {
                    foreach (var userGuid in res.Users.Where(userGuid => !userGuid.Equals(res.ModifiedBy)))
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

            await socketServiceClient.MakeRequest("sendUnreadUsers", unreadUsers);

            _logger.DebugTimeCollectingNews(DateTime.UtcNow - start);
        }
        catch (Exception ex)
        {
            _logger.ErrorAggregateFeeds(ex);
        }
    }
}